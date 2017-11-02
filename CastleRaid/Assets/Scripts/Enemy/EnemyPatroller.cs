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

	PatrolPoint[] patrolPoints;
	int currentPatrolPointIndex = -1;
	float patrolPointFinishRange = 0.1f;

	float suspiciousLocationCheckRange = 1f;
	float suspiciousStateExitDuration = 10f;

	GameObject currentTarget;

	float meleeRange = 1.5f;
	float meleeAttackCooldownDuration = 3f;
	float meleeAttackCooldownTimer = -1f;

	float navAgentDestinationHeightDifferenceIgnoreLimit = 3f;

	float lookAroundStartTime = -1f;

	[SerializeField]
	float walkingSpeed = 3f;
	[SerializeField]
	float runningSpeed = 6f;

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
	#endregion

	#region Initialization
	public void InitializeEnemy(PatrolPoint[] _patrolPoints)
	{
		patrolPoints = _patrolPoints;
		if (patrolPoints == null || patrolPoints.Length == 0)
		{
			Debug.LogError("PatrolPoints not initialized properly! Disabling EnemyPatroller.");
			gameObject.SetActive(false);
		}

		navAgent = GetComponent<NavMeshAgent>();
		rb = GetComponent<Rigidbody>();

		ResetEnemy();

		currentDestination = transform.position;
	}

	public void ResetEnemy()
	{
		navAgent.enabled = true;
		currentPatrolPointIndex = 0;
		navAgent.Warp(patrolPoints[currentPatrolPointIndex].transform.position);
		transform.rotation = patrolPoints[currentPatrolPointIndex].transform.rotation;

		rb.isKinematic = true;
		rb.useGravity = false;
		ragdolled = false;

		SphereCollider sCollider = alertColliderController.GetComponent<SphereCollider>();
		if (sCollider != null)
		{
			sCollider.radius = alertRange;
		}

		alertColliderController.gameObject.SetActive(false);
		alertingNearbyEnemies = false;

		SetAlertnessState(EAlertnessState.PATROLLING);
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
		UpdateEnemyState(Time.deltaTime);
		UpdateNavigation(Time.deltaTime);

		if (!ragdolled)
		{
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
		}

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

			if(alertNearbyEnemiesTimer <= 0)
			{
				alertingNearbyEnemies = false;
				alertColliderController._OnTriggerEnter -= OnColliderControllerTriggerEnter;
				alertColliderController.gameObject.SetActive(false);
			}
		}
	}
	#endregion

	#region Navigation update
	private void UpdateNavigation(float updateLoopDeltaTime)
	{
		navTickCounter += updateLoopDeltaTime;

		if (navTickCounter >= navTickInterval)
		{
			navTickCounter = 0;

			if (currentDestination != lastDestination)
			{
				navAgent.SetDestination(currentDestination);
				lastDestination = currentDestination;
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
						if (IsFacingInGivenDirection(patrolPoints[currentPatrolPointIndex].transform.forward))
						{
							if (movementState != EMovementState.LOOKINGAROUND)
							{
								GuardPoint();
							}

							if (IsDoneLookingAround(lookAroundStartTime, patrolPoints[currentPatrolPointIndex].GetGuardingDuration()))
							{
								SetNextPatrolPoint();
							}
						}
						else
						{
							TurnTowardsGivenDirection(patrolPoints[currentPatrolPointIndex].transform.forward, deltaTime);
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
				if (IsGivenTargetInSight(currentTarget))
				{
					SetNewNavDestination(currentTarget.transform.position);

					if (IsWithinGivenRangeOfDestination(meleeRange))
					{
						MeleeAttack();
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

						MeleeAttack();
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
		if (Mathf.Abs(transform.position.y - currentDestination.y) <= navAgentDestinationHeightDifferenceIgnoreLimit)
		{
			currentPosition.y = 0;
			destination.y = 0;
		}

		if ((destination - currentPosition).sqrMagnitude <= range * range)
		{
			return true;
		}

		return false;
	}

	private bool IsAnyTargetInSight()
	{
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
				break;

			case EAlertnessState.SUSPICIOUS:
				currentVisionAngle = defaultVisionAngle * suspiciousVisionAngleMultiplier;
				currentVisionRange = defaultVisionRange * suspiciousVisionRangeMultiplier;

				if (visionLight != null)
				{
					visionLight.range = currentVisionRange;
					visionLight.spotAngle = currentVisionAngle;
				}
				break;

			case EAlertnessState.ALERTED:
				currentVisionAngle = defaultVisionAngle * alertedVisionAngleMultiplier;
				currentVisionRange = defaultVisionRange * alertedVisionRangeMultiplier;

				if (visionLight != null)
				{
					visionLight.range = currentVisionRange;
					visionLight.spotAngle = currentVisionAngle;
				}

				if (currentTarget != null)
				{
					SetNewNavDestination(currentTarget.transform.position);
				}
				else
				{
					Debug.LogError("CurrentTarget is null when changing to ALERTED state!");
				}
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
				break;
			case EMovementState.RUNNING:
				navAgent.speed = runningSpeed;
				//TODO: Set animator movementSpeed to running
				break;
			case EMovementState.LOOKINGAROUND:
				navAgent.speed = runningSpeed;
				//TODO: Set animator movementSpeed to running
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
	}
	#endregion

	#region Actions
	private void MeleeAttack()
	{
		if (meleeAttackCooldownTimer <= 0)
		{
			meleeAttackCooldownTimer = meleeAttackCooldownDuration;
			//Implement melee attack cooldown?

			navAgent.ResetPath();
			//Does this require target transform or direction?
			TurnTowardsGivenDirection(currentTarget.transform.position - transform.position, -1f);

			//Play melee attack animation
		}
	}

	private void GuardPoint()
	{
		//Play guardPoint animation
		//Rotate vision cone with the enemy model's vision (lantern's spotlight)

		lookAroundStartTime = Time.time;
		SetMovementState(EMovementState.LOOKINGAROUND);
	}

	private void CheckPerimeter()
	{
		//Play checkPerimeter animation
		//Rotate vision cone with the enemy model's vision (lantern's spotlight)

		lookAroundStartTime = Time.time;
		SetMovementState(EMovementState.LOOKINGAROUND);
	}

	private void TurnTowardsGivenDirection(Vector3 direction, float deltaTime)
	{
		//TODO: Find out why the rotation "snaps" to place at the end
		//TODO: Fix rotation direction (rotate towards the smaller angle)
		float rotationStepMax = 30f;
		transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), rotationStepMax * deltaTime);
	}

	private void KnockOut()
	{
		//TODO: Implement properly
		//Enable ragdoll, disable enemy behaviour. Add / enable interactable_Grab script on the enemy?
		navAgent.enabled = false;
		Vector3 knockOutRotation = transform.eulerAngles;
		knockOutRotation.x = 90;
		knockOutRotation.z = 0;
		transform.eulerAngles = knockOutRotation;

		rb.isKinematic = false;
		rb.useGravity = true;
		ragdolled = true;
	}

	private void AlertNearbyEnemies()
	{
		alertColliderController.gameObject.SetActive(true);
		alertColliderController._OnTriggerEnter -= OnColliderControllerTriggerEnter;
		alertColliderController._OnTriggerEnter += OnColliderControllerTriggerEnter;
		alertNearbyEnemiesTimer = alertNearbyEnemiesDuration;
		alertingNearbyEnemies = true;
	}
	#endregion

	#region Vision
	private bool RaycastForGivenTargetWithinVision(GameObject target)
	{
		Vector3 dir = target.transform.position - visionOrigin.position;
		Vector3 raycastOrigin = visionOrigin.position;
		RaycastHit hit;
		//Debug.DrawRay(raycastOrigin, dir * currentVisionRange, Color.red);

		if (Vector3.Angle(transform.forward, dir) <= (currentVisionAngle / 2))
		{
			if (Physics.Raycast(raycastOrigin, dir, out hit, currentVisionRange, visionMask))
			{
				TargetableByEnemy targetScript = hit.collider.GetComponent<TargetableByEnemy>();
				if (targetScript.GetMainObject() == target)
				{
					return true;
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
			VisionCastInfo visionCast = VisionCast(angle);

			if (visionCast.hit && visionCast.col.GetComponent<TargetableByEnemy>())
			{
				return visionCast.col.GetComponent<TargetableByEnemy>().GetMainObject();
			}
		}

		return null;
	}

	private VisionCastInfo VisionCast(float globalAngle)
	{
		Vector3 dir = DirFromAngle(globalAngle, true);
		RaycastHit hit;
		Vector3 raycastOrigin = visionOrigin.position;
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
	public void Hit(GameObject hittingObject)
	{
		//TODO: Implement properly 
		//Check object mass / velocity or detect if proper knockouter and act accordingly
		KnockOut();
	}

	public void Distract(Vector3 soundPosition)
	{
		switch (alertnessState)
		{
			case EAlertnessState.PATROLLING:
				SetNewNavDestination(soundPosition);
				SetAlertnessState(EAlertnessState.SUSPICIOUS);
				break;
			case EAlertnessState.SUSPICIOUS:
				SetNewNavDestination(soundPosition);
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

}