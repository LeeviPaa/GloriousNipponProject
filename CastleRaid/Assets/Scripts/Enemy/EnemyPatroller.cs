using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody))]
public class EnemyPatroller : MonoBehaviour
{
    #region References & variables
    enum EAlertnessState
    {
        PATROLLING,
        SUSPICIOUS,
        ALERTED,

    }
    enum EMovementState
    {
        WALKING,
        RUNNING,
        LOOKINGAROUND,

    }

    EAlertnessState alertnessState;
    EMovementState movementState;

    NavMeshAgent navAgent;
    Rigidbody rb;
    EnemyAnimationController animationController;

    PatrolPoint[] patrolPoints;
    int currentPatrolPointIndex = -1;
    float patrolPointFinishRange = 0.1f;

    float suspiciousLocationCheckRange = 3f;
    float suspiciousStateExitDuration = 6f;

    GameObject currentTarget;

    float meleeRange = 1f;
    float meleeAttackCooldownDuration = 2f;
    float meleeAttackCooldownTimer = -1f;

    float navAgentDestinationHeightDifferenceIgnoreLimit = 3f;

    float lookAroundStartTime = -1f;

    [SerializeField]
    float walkingSpeed = 1f;
    [SerializeField]
    float runningSpeed = 3f;

    Vector3 currentDestination;
    Vector3 lastDestination;

    float navTickInterval = 0.25f;
    float navTickCounter = -1;

    [SerializeField]
    float defaultVisionAngle = 60f;
    [SerializeField]
    float suspiciousVisionAngleMultiplier = 1.5f;
    [SerializeField]
    float alertedVisionAngleMultiplier = 2f;
    float currentVisionAngle = -1;

    [SerializeField]
    float defaultVisionRange = 6f;
    [SerializeField]
    float suspiciousVisionRangeMultiplier = 1.5f;
    [SerializeField]
    float alertedVisionRangeMultiplier = 2f;
    float currentVisionRange = -1f;

    [SerializeField]
    LayerMask visionMask; //This mask should include all the layers that the vision raycasts should hit (the targets as well as the obstacles)
    float visionCastResolution = 0.25f;

    [SerializeField]
    Transform visionOrigin;
    [SerializeField]
    Light visionLight;

    [SerializeField]
    ColliderController alertColliderController;
    [SerializeField]
    float alertRange = 10f;
    float alertNearbyEnemiesDuration = 1f;
    float alertNearbyEnemiesTimer = -1f;
    bool alertingNearbyEnemies = false;

    List<GameObject> targets = new List<GameObject>();

    bool ragdolled = false;
    [SerializeField]
    Transform ragdollColliderParent;

    [SerializeField]
    ColliderController meleeColliderHolder;

    bool pauseBehaviorForAnimations = false;

    bool targetInSightTrigger = false;

    bool clearPath = false;

    AudioItem walkingSound;
    AudioItem lookAroundSound;

    [SerializeField]
    GameObject suspiciousIndicator;
    [SerializeField]
    GameObject alertedIndicator;

    #endregion

    #region Initialization
    public void InitializeEnemy(PatrolPoint[] _patrolPoints)
    {
        patrolPoints = _patrolPoints;
        //if (patrolPoints == null || patrolPoints.Length == 0)
        //{
        //    Debug.LogError("PatrolPoints not initialized properly! Disabling EnemyPatroller.");
        //    gameObject.SetActive(false);
        //}

        navAgent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        animationController = GetComponent<EnemyAnimationController>();

        //Initialize looping sound effects
        walkingSound = GameManager.audioManager.GetAudio("FootStep", false, false, transform.position, transform);
        lookAroundSound = GameManager.audioManager.GetAudio("GuardLookAround", false, false, transform.position, transform);

        ResetEnemy();

        currentDestination = transform.position;
        lastDestination = currentDestination;
    }

    public void ResetEnemy()
    {
        navAgent.enabled = true;
        currentPatrolPointIndex = 0;
        navAgent.Warp(patrolPoints[currentPatrolPointIndex].transform.position);
        transform.rotation = patrolPoints[currentPatrolPointIndex].transform.rotation;

        rb.isKinematic = true;
        rb.useGravity = false;

        SetRagdollPhysicsState(false);
        GetComponent<Collider>().enabled = true;
        ragdolled = false;

        SphereCollider sCollider = alertColliderController.GetComponent<SphereCollider>();
        if (sCollider != null)
        {
            sCollider.radius = alertRange;
        }

        alertColliderController.gameObject.SetActive(false);
        alertingNearbyEnemies = false;

        SetAlertnessState(EAlertnessState.PATROLLING);
        animationController.SetActiveState(true);

        pauseBehaviorForAnimations = false;
        targetInSightTrigger = false;
        clearPath = false;

        Collider[] meleeColliders = meleeColliderHolder.GetComponents<Collider>();
        foreach (Collider c in meleeColliders)
        {
            c.enabled = false;
        }

        SetAlertnessIndicatorState(0);
    }
    #endregion

    #region Event subscribers
    private void OnColliderControllerTriggerEnter(ColliderController colliderController, Collider collider)
    {
        if (colliderController == alertColliderController)
        {
            EnemyPatroller foundEnemy = colliderController.GetComponentInParent<EnemyPatroller>();

            if (foundEnemy != null)
            {
                Alert(currentTarget);
            }
        }
    }
    #endregion

    #region MonoBehaviour
    private void Update()
    {
        if (!ragdolled)
        {
            UpdateEnemyState(Time.deltaTime);
            UpdateNavigation(Time.deltaTime);

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            if (meleeAttackCooldownTimer > 0)
            {
                meleeAttackCooldownTimer -= Time.deltaTime;

                if (meleeAttackCooldownTimer <= 0)
                {
                    meleeAttackCooldownTimer = 0;
                }
            }

            if (alertingNearbyEnemies)
            {
                alertNearbyEnemiesTimer -= Time.deltaTime;

                if (alertNearbyEnemiesTimer <= 0)
                {
                    alertingNearbyEnemies = false;
                    alertColliderController._OnTriggerEnter -= OnColliderControllerTriggerEnter;
                    alertColliderController.gameObject.SetActive(false);
                }
            }
        }
    }
    #endregion

    #region Navigation update
    private void UpdateNavigation(float updateLoopDeltaTime)
    {

        if (!clearPath)
        {

            if (!walkingSound.source.isPlaying)
            {
                walkingSound.source.Play();
            }
            if (lookAroundSound.source.isPlaying)
            {
                lookAroundSound.source.Stop();
            }

            navTickCounter += updateLoopDeltaTime;

            if (navTickCounter >= navTickInterval)
            {
                navTickCounter = 0;

                if (currentDestination != lastDestination)
                {
                    navAgent.SetDestination(currentDestination);
                    lastDestination = currentDestination;
                    animationController.StartMovementAnimation();
                }
            }
        }
    }
    #endregion

    #region Main behaviour
    void UpdateEnemyState(float deltaTime)
    {
        switch (alertnessState)
        {
            case EAlertnessState.PATROLLING:
                if (IsAnyTargetInSight())
                {
                    SetAlertnessState(EAlertnessState.ALERTED);
                }
                else if (IsWithinGivenRangeOfDestination(patrolPointFinishRange))
                {
                    if (IsGuardablePatrolPoint(currentPatrolPointIndex))
                    {
                        if (movementState != EMovementState.LOOKINGAROUND)
                        {
                            GuardPoint();
                        }

                        if (!IsFacingInGivenDirection(patrolPoints[currentPatrolPointIndex].transform.forward))
                        {
                            TurnTowardsGivenDirection(patrolPoints[currentPatrolPointIndex].transform.forward, deltaTime);
                        }

                        if (IsDoneLookingAround(lookAroundStartTime, patrolPoints[currentPatrolPointIndex].GetGuardingDuration()))
                        {
                            SetNextPatrolPoint();
                        }
                    }
                    else
                    {
                        SetNextPatrolPoint();
                    }
                }
                else
                {
                    //Continue walking towards current destination (and update position if necessary)

                    //TODO: If deemed necessary, implement collision detection:
                    //If the enemy gets stuck on something while moving towards a destination, unstuck it somehow?
                }
                break;

            case EAlertnessState.SUSPICIOUS:
                if (IsAnyTargetInSight())
                {
                    SetAlertnessState(EAlertnessState.ALERTED);
                }
                else if (IsWithinGivenRangeOfDestination(suspiciousLocationCheckRange))
                {
                    if (movementState != EMovementState.LOOKINGAROUND)
                    {
                        CheckPerimeter();
                    }

                    if (IsDoneLookingAround(lookAroundStartTime, suspiciousStateExitDuration))
                    {
                        SetAlertnessState(EAlertnessState.PATROLLING);
                    }
                }
                else
                {
                    //Continue walking towards current destination (and update position if necessary)

                    //TODO: If deemed necessary, implement collision detection
                }
                break;

            case EAlertnessState.ALERTED:
                if (!pauseBehaviorForAnimations)
                {
                    if (IsGivenTargetInSight(currentTarget))
                    {
                        SetNewNavDestination(currentTarget.transform.position);

                        if (IsWithinGivenRangeOfDestination(meleeRange))
                        {
                            clearPath = true;
                            MeleeAttack(deltaTime);
                        }
                        else
                        {
                            //Continue walking towards current destination (and update position if necessary)

                            //TODO: If deemed necessary, implement collision detection
                        }
                    }
                    else if (IsAnyTargetInSight())
                    {
                        SetNewNavDestination(currentTarget.transform.position);

                        if (IsWithinGivenRangeOfDestination(meleeRange))
                        {
                            clearPath = true;
                            MeleeAttack(deltaTime);
                        }
                        else
                        {
                            //Continue walking towards current destination (and update position if necessary)

                            //TODO: If deemed necessary, implement collision detection
                        }
                    }
                    else
                    {
                        SetAlertnessState(EAlertnessState.SUSPICIOUS);
                    }
                }
                break;

            default:
                break;
        }
    }
    #endregion

    #region Status checks
    bool IsWithinGivenRangeOfDestination(float range)
    {
        Vector3 currentPosition = transform.position;
        Vector3 destination = currentDestination;

        //currentDestination == lastDestination && navAgent.remainingDistance <= range
        //if (Mathf.Abs(transform.position.y - currentDestination.y) <= navAgentDestinationHeightDifferenceIgnoreLimit)
        //{
        //}

        currentPosition.y = 0;
        destination.y = 0;

        if ((destination - currentPosition).sqrMagnitude <= range * range)
        {
            return true;
        }

        return false;
    }

    private bool IsAnyTargetInSight()
    {
        if (targetInSightTrigger)
        {
            return true;
        }

        GameObject foundTarget = RaycastForAnyTargetWithinVision();
        if (foundTarget != null)
        {
            currentTarget = foundTarget;
            return true;
        }

        return false;
    }

    private bool IsGivenTargetInSight(GameObject target)
    {
        //Should the visibility check here be modified (at least in case of ALERTED state)
        //so that if a raycast directly to the target is within the vision cone, this returns true?
        //Instead of checking only the vision cone raycasts. This would remove the height limitation
        //on the enemy's visibility while alerted.
        //DONE ^

        if (RaycastForGivenTargetWithinVision(target))
        {
            return true;
        }

        return false;
    }

    private bool IsDoneLookingAround(float _lookAroundStartTime, float lookAroundDuration)
    {
        if ((Time.time - _lookAroundStartTime) >= lookAroundDuration)
        {
            return true;
        }

        return false;
    }

    private bool IsFacingInGivenDirection(Vector3 direction, float errorMargin = 0, bool excludeYAxis = true)
    {
        Vector3 horizontalForwardDirection = transform.forward;

        if (excludeYAxis)
        {
            direction.y = 0;
            horizontalForwardDirection.y = 0;
        }
        if (Vector3.Angle(horizontalForwardDirection, direction) <= errorMargin)
        {
            return true;
        }

        //Debug.DrawRay(transform.position, transform.forward * 3, Color.green);
        //Debug.DrawRay(transform.position, direction.normalized * 3, Color.blue);
        //Debug.Log("IsFacingInGivenDirection: " + Vector3.Angle(horizontalForwardDirection, direction) + ", returning false");
        return false;
    }

    private bool IsGuardablePatrolPoint(int patrolPointIndex)
    {
        if (patrolPoints[patrolPointIndex].GetGuardingDuration() <= 0)
        {
            return false;
        }

        return true;
    }
    #endregion

    #region Setters
    void SetAlertnessState(EAlertnessState newState)
    {
        if (alertnessState == EAlertnessState.PATROLLING
            && (newState == EAlertnessState.SUSPICIOUS
            || newState == EAlertnessState.ALERTED))
        {
            animationController.PlayAlertedAnimation();
            StartCoroutine(PauseAIAndSetTargetPositionAsDestinationAfterDuration(0.8f));

            AudioItem alertedSoundEffect = GameManager.audioManager.GetAudio("GuardNotice", true, true, transform.position, transform);
        }

        alertnessState = newState;

        switch (alertnessState)
        {
            case EAlertnessState.PATROLLING:
                currentVisionAngle = defaultVisionAngle;
                currentVisionRange = defaultVisionRange;

                if (visionLight != null)
                {
                    visionLight.range = currentVisionRange;
                    visionLight.spotAngle = currentVisionAngle;
                }

                SetNextPatrolPoint();
                SetAlertnessIndicatorState(0);
                break;

            case EAlertnessState.SUSPICIOUS:
                currentVisionAngle = defaultVisionAngle * suspiciousVisionAngleMultiplier;
                currentVisionRange = defaultVisionRange * suspiciousVisionRangeMultiplier;

                if (visionLight != null)
                {
                    visionLight.range = currentVisionRange;
                    visionLight.spotAngle = currentVisionAngle;
                }

                SetAlertnessIndicatorState(1);
                break;

            case EAlertnessState.ALERTED:
                currentVisionAngle = defaultVisionAngle * alertedVisionAngleMultiplier;
                currentVisionRange = defaultVisionRange * alertedVisionRangeMultiplier;

                if (visionLight != null)
                {
                    visionLight.range = currentVisionRange;
                    visionLight.spotAngle = currentVisionAngle;
                }

                if (currentTarget == null)
                {
                    Debug.LogError("CurrentTarget is null when changing to ALERTED state!");
                }

                SetAlertnessIndicatorState(2);
                break;

            default:
                break;
        }
    }

    void SetMovementState(EMovementState newState)
    {
        movementState = newState;

        switch (movementState)
        {
            case EMovementState.WALKING:
                navAgent.speed = walkingSpeed;
                //TODO: Set animator movementSpeed to walking
                animationController.SetMovementSpeed(0);
                break;
            case EMovementState.RUNNING:
                navAgent.speed = runningSpeed;
                //TODO: Set animator movementSpeed to running
                animationController.SetMovementSpeed(1);
                break;
            case EMovementState.LOOKINGAROUND:
                navAgent.speed = runningSpeed;
                //TODO: Set animator movementSpeed to running
                animationController.SetMovementSpeed(1);
                break;
            default:
                break;
        }
    }

    private void SetNextPatrolPoint()
    {
        //If the current patrol point is not the last one of the list
        if (patrolPoints.Length > currentPatrolPointIndex + 1)
        {
            //Increment the currentPatrolPointIndex by one
            currentPatrolPointIndex++;
        }
        else
        {
            //Else, set the currentPatrolPointIndex to the first index
            currentPatrolPointIndex = 0;
        }

        SetNewNavDestination(patrolPoints[currentPatrolPointIndex].transform.position);
    }

    private void SetNewNavDestination(Vector3 newDestination)
    {
        currentDestination = newDestination;

        switch (alertnessState)
        {
            case EAlertnessState.PATROLLING:
                SetMovementState(EMovementState.WALKING);
                break;
            case EAlertnessState.SUSPICIOUS:
                SetMovementState(EMovementState.RUNNING);
                break;
            case EAlertnessState.ALERTED:
                SetMovementState(EMovementState.RUNNING);
                break;
            default:
                break;
        }

        clearPath = false;
    }

    IEnumerator EnableMeleeColliderForDuration(float duration)
    {
        meleeColliderHolder._OnTriggerEnter -= OnMeleeColliderEnter;
        meleeColliderHolder._OnTriggerEnter += OnMeleeColliderEnter;

        Collider[] meleeColliders = meleeColliderHolder.GetComponents<Collider>();
        foreach (Collider c in meleeColliders)
        {
            c.enabled = true;
        }

        yield return new WaitForSeconds(duration);
        
        meleeColliders = meleeColliderHolder.GetComponents<Collider>();
        foreach (Collider c in meleeColliders)
        {
            c.enabled = false;
        }

        meleeColliderHolder._OnTriggerEnter -= OnMeleeColliderEnter;
    }

    IEnumerator PauseAIAndSetTargetPositionAsDestinationAfterDuration(float duration)
    {
        //Pause walking sound effect
        walkingSound.source.Stop();

        navAgent.ResetPath();
        clearPath = true;
        pauseBehaviorForAnimations = true;

        yield return new WaitForSeconds(duration);

        pauseBehaviorForAnimations = false;
        clearPath = false;
        //SetNewNavDestination(currentTarget.transform.position);
    }
    #endregion

    #region Actions
    private void MeleeAttack(float deltaTime)
    {
        TurnTowardsGivenDirection(currentTarget.transform.position - transform.position, deltaTime);

        if (meleeAttackCooldownTimer <= 0)
        {
            meleeAttackCooldownTimer = meleeAttackCooldownDuration;
            navAgent.ResetPath();
            clearPath = true;

            //Play melee attack animation
            animationController.PlayAttackAnimation();

            //Enable melee collider for the duration of the animation
            StartCoroutine(EnableMeleeColliderForDuration(1.835f));

            //Call melee sound effect
            AudioItem meleeSoundEffect = GameManager.audioManager.GetAudio("GuardSwing", true, true, transform.position, transform);
        }
    }

    private void GuardPoint()
    {
        //Play guardPoint animation
        //Rotate vision cone with the enemy model's vision (lantern's spotlight)

        lookAroundStartTime = Time.time;
        SetMovementState(EMovementState.LOOKINGAROUND);
        animationController.StartGuardAnimation();

        //Call lookAround sound effect
        lookAroundSound.source.Play();
    }

    private void CheckPerimeter()
    {
        //Play checkPerimeter animation
        //Rotate vision cone with the enemy model's vision (lantern's spotlight)

        lookAroundStartTime = Time.time;
        SetMovementState(EMovementState.LOOKINGAROUND);
        animationController.StartCheckPerimeterAnimation();

        //Call lookAround sound effect
        lookAroundSound.source.Play();
    }

    private void TurnTowardsGivenDirection(Vector3 direction, float deltaTime)
    {
        //TODO: Find out why the rotation "snaps" to place at the end
        //TODO: Fix rotation direction (rotate towards the smaller angle)
        float rotationStepMax = 30f;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), rotationStepMax * deltaTime);
    }

    private void Ragdoll()
    {
        //TODO: Implement properly
        //Enable ragdoll, disable enemy behaviour. Add / enable interactable_Grab script on the enemy?
        navAgent.enabled = false;
        animationController.SetActiveState(false);
        GetComponent<Collider>().enabled = false;
        SetRagdollPhysicsState(true);

        ragdolled = true;
    }

    private void SetRagdollPhysicsState(bool newState)
    {
        Collider[] ragdollColliders = ragdollColliderParent.GetComponentsInChildren<Collider>(true);
        foreach (Collider c in ragdollColliders)
        {
            c.enabled = newState;
        }


        Rigidbody[] ragdollRigidbodies = ragdollColliderParent.GetComponentsInChildren<Rigidbody>(true);
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = !newState;
            rb.useGravity = newState;
        }

        //Disable melee colliders
        Collider[] meleeColliders = meleeColliderHolder.GetComponents<Collider>();

        foreach (Collider c in meleeColliders)
        {
            c.enabled = false;
        }
    }

    private void AlertNearbyEnemies()
    {
        alertColliderController.gameObject.SetActive(true);
        alertColliderController._OnTriggerEnter -= OnColliderControllerTriggerEnter;
        alertColliderController._OnTriggerEnter += OnColliderControllerTriggerEnter;
        alertNearbyEnemiesTimer = alertNearbyEnemiesDuration;
        alertingNearbyEnemies = true;
    }

    private void SetAlertnessIndicatorState(int newState)
    {
        switch (newState)
        {
            case 0:
                suspiciousIndicator.SetActive(false);
                alertedIndicator.SetActive(false);
                break;
            case 1:
                suspiciousIndicator.SetActive(true);
                alertedIndicator.SetActive(false);
                break;
            case 2:
                suspiciousIndicator.SetActive(false);
                alertedIndicator.SetActive(true);
                break;
            default:
                break;
        }
    }
    #endregion

    #region Vision
    private bool RaycastForGivenTargetWithinVision(GameObject target)
    {
        Vector3 dir = target.transform.position - visionOrigin.position;
        Vector3 raycastOrigin = visionOrigin.position;
        RaycastHit hit;
        //Debug.DrawRay(raycastOrigin, dir * currentVisionRange, Color.red);
        Vector3 horizontalDir = dir;
        horizontalDir.y = transform.forward.y;

        if (Vector3.Angle(transform.forward, horizontalDir) <= (currentVisionAngle / 2))
        {
            if (Physics.Raycast(raycastOrigin, dir, out hit, currentVisionRange, visionMask))
            {
                TargetableByEnemy targetScript = hit.collider.GetComponent<TargetableByEnemy>();
                if (targetScript != null)
                {
                    if (targetScript.GetMainObject() == target)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private GameObject RaycastForAnyTargetWithinVision()
    {
        int stepCount = Mathf.RoundToInt(currentVisionAngle * visionCastResolution);
        float stepAngleSize = currentVisionAngle / stepCount;

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = visionOrigin.eulerAngles.y - currentVisionAngle / 2 + stepAngleSize * i;
            Vector3 visionCastOrigin = visionOrigin.position;
            visionCastOrigin.y -= 0.75f;
            VisionCastInfo visionCast = VisionCast(angle, visionCastOrigin);

            if (visionCast.hit && visionCast.col.GetComponent<TargetableByEnemy>())
            {
                //Debug.Log("visionCast.col.gameObject: " + visionCast.col.gameObject);
                return visionCast.col.GetComponent<TargetableByEnemy>().GetMainObject();
            }

            visionCastOrigin.y += 1f;
            visionCast = VisionCast(angle, visionCastOrigin);

            if (visionCast.hit && visionCast.col.GetComponent<TargetableByEnemy>())
            {
                //Debug.Log("visionCast.col.gameObject: " + visionCast.col.gameObject);
                return visionCast.col.GetComponent<TargetableByEnemy>().GetMainObject();
            }


        }

        return null;
    }

    private VisionCastInfo VisionCast(float globalAngle, Vector3 origin)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;
        Vector3 raycastOrigin = origin;
        //Debug.DrawRay(raycastOrigin, dir * currentVisionRange, Color.red);
        if (Physics.Raycast(raycastOrigin, dir, out hit, currentVisionRange, visionMask))
        {
            return new VisionCastInfo(true, hit.point, hit.distance, globalAngle, hit.collider);
        }
        else
        {
            return new VisionCastInfo(false, raycastOrigin + dir * currentVisionRange,
                currentVisionRange, globalAngle, null);
        }
    }

    private Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0,
            Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public struct VisionCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dist;
        public float angle;
        public Collider col;

        public VisionCastInfo(bool _hit, Vector3 _point, float _dist, float _angle, Collider _col)
        {
            hit = _hit;
            point = _point;
            dist = _dist;
            angle = _angle;
            col = _col;
        }
    }
    #endregion

    #region External events
    public void Distract(Vector3 distractorPosition)
    {
        //Debug.Log("Distract");
        switch (alertnessState)
        {
            case EAlertnessState.PATROLLING:
                SetNewNavDestination(distractorPosition);
                SetAlertnessState(EAlertnessState.SUSPICIOUS);
                break;
            case EAlertnessState.SUSPICIOUS:
                SetNewNavDestination(distractorPosition);
                SetAlertnessState(EAlertnessState.SUSPICIOUS);
                break;
            case EAlertnessState.ALERTED:
                //Don't do anything, aka continue chasing currentTarget
                break;
            default:
                break;
        }
    }

    public void Alert(GameObject newTarget)
    {
        if (alertnessState != EAlertnessState.ALERTED)
        {
            currentTarget = newTarget;
            SetAlertnessState(EAlertnessState.ALERTED);
        }
    }
    #endregion

    #region Collision detection
    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody collidingRigidbody = collision.rigidbody;
        if (collidingRigidbody != null)
        {
            float sqrVelocityThresholdToDamage = 5f;
            Debug.Log("EnemyPatroller: Collided with a rigidbody: " + collision.collider.gameObject + ". Colliding rigidbody velocity sqrMagnitude: "
                + collidingRigidbody.velocity.sqrMagnitude + " , sqrVelocityThresholdToDamage: "
                + sqrVelocityThresholdToDamage);
            if (collidingRigidbody.velocity.sqrMagnitude >= sqrVelocityThresholdToDamage)
            {
                Ragdoll();
            }
            else
            {
                Distract(collidingRigidbody.transform.position);
            }
        }
    }

    private void OnMeleeColliderEnter(ColliderController controller, Collider collider)
    {
        TargetableByEnemy targetScript = collider.GetComponent<TargetableByEnemy>();
        if (targetScript != null)
        {
            GameObject target = targetScript.GetMainObject();

            if (target != null)
            {
                Transform prisonTransform = GameObject.FindGameObjectWithTag("Prison").transform;

                if (prisonTransform != null)
                {
                    GameManager.levelInstance.GetPlayerRelocator().RelocatePlayer(prisonTransform.position, prisonTransform.rotation);
                    SetAlertnessState(EAlertnessState.PATROLLING);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        TargetableByEnemy targetScript = collider.GetComponent<TargetableByEnemy>();
        if (targetScript != null)
        {
            if (targetScript.GetMainObject() == currentTarget)
            {
                targetInSightTrigger = true;
            }
            else
            {
                GameObject target = targetScript.GetMainObject();

                if (target != null)
                {
                    currentTarget = target;
                    targetInSightTrigger = true;
                }
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        TargetableByEnemy targetScript = collider.GetComponent<TargetableByEnemy>();
        if (targetScript != null)
        {
            if (targetScript.GetMainObject() == currentTarget)
            {
                targetInSightTrigger = false;
            }
        }
    }
    #endregion

}