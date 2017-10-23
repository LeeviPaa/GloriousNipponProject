namespace VRTK.Examples
{
    using UnityEngine;

    public class Zipline : VRTK_InteractableObject
    {
        [Header("Zipline Options", order = 4)]
        public float downStartSpeed = 0.2f;
        public float acceleration = 1.0f;
        public float moveSpeed = 1.0f;
        public Transform handleEndPosition;
        public Transform handleStartPosition;
        public GameObject handle;

        public bool isMoving = false;
        private bool isMovingDown = true;

        public override void OnInteractableObjectGrabbed(InteractableObjectEventArgs e)
        {
            base.OnInteractableObjectGrabbed(e);
            isMoving = true;
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Update()
        {
            base.Update();

            if (isMoving)
            {
                Vector3 moveAmount = (handleEndPosition.position - handleStartPosition.position).normalized *moveSpeed * Time.deltaTime;
                handle.transform.position += moveAmount;

                float myDistanceToStart = (handleStartPosition.position - transform.position).magnitude;
                float endDistanceToStart = (handleStartPosition.position - handleEndPosition.position).magnitude;
                if(myDistanceToStart > endDistanceToStart)
                {
                    isMoving = false;
                    isMovingDown = !isMovingDown;

                    Transform T = handleEndPosition;
                    handleEndPosition = handleStartPosition;
                    handleStartPosition = T;
                }
            }
        }
    }
}