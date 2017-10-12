namespace VRTK
{
    using UnityEngine;
    using System.Collections;
#if UNITY_5_5_OR_NEWER
    using UnityEngine.AI;
#endif

    public class VRTK_BlinkDash : MonoBehaviour
    {
        VRTK_ControllerEvents CE;
        GameObject RightController;

        protected Transform headset;
        protected Transform playArea;

        public GameObject testMove;
        public float dashAmount = 3;
        public float adjustSpeed = 1;
        public float blinkTime = 0.25f;
        private bool blinking = false;

        public float blinkFadeBack = 0.03f;
        public float blinkFade = 0.02f;
        protected float fadeInTime = 0f;

        public VRTK_HeadsetFade headsetFade;
        void OnEnable()
        {
            headsetFade = transform.GetComponent<VRTK_HeadsetFade>();

            headset = VRTK_SharedMethods.AddCameraFade();
            playArea = VRTK_DeviceFinder.PlayAreaTransform();

            bool found = false;
            foreach (Transform child in transform.parent)
            {
                if(child.name == "RightController" && child.GetComponent<VRTK_ControllerEvents>())
                {
                    CE = child.GetComponent<VRTK_ControllerEvents>();
                    RightController = child.gameObject;
                    found = true;
                }
            }
            if(!found)
            {
                Debug.LogError("Right controller script not found :(");
            }

            CE.TouchpadPressed += TouchPadPressedListener;
        }
        void OnDisable()
        {
            CE.TouchpadPressed -= TouchPadPressedListener;
        }

        public void TouchPadPressedListener(object sender, ControllerInteractionEventArgs e)
        {
            Vector3 direction = new Vector3(e.touchpadAxis.x, 0, e.touchpadAxis.y) * dashAmount;
            direction = RightController.transform.TransformDirection(direction);
            direction.y = 0;

            direction += testMove.transform.position;

            StartCoroutine(blinkDash(testMove.transform.position, direction));
            Blink(0.5f);
        }
        protected virtual void Blink(float transitionSpeed)
        {
            Debug.Log("blink start");
            //VRTK_SDK_Bridge.HeadsetFade(Color.black, 0);
            headsetFade.Fade(new Color(0,0,0,0.75f), 0);
            Invoke("ReleaseBlink", blinkFadeBack);
        }
        protected virtual void ReleaseBlink()
        {
            Debug.Log("blink stop");
            //VRTK_SDK_Bridge.HeadsetFade(Color.clear, fadeInTime);
            headsetFade.Unfade(blinkFade);
            fadeInTime = 0f;
        }

        IEnumerator blinkDash( Vector3 startPos, Vector3 destination)
        {
            if (!blinking)
            {
                float elapsedTime = 0;
                float t = 0;

                while (t < 1)
                {
                    testMove.transform.position = Vector3.Lerp(startPos, destination, t);
                    elapsedTime += Time.deltaTime;
                    t = elapsedTime / blinkTime;
                    if (t > 1)
                    {
                        if (testMove.transform.position != destination)
                        {
                            testMove.transform.position = destination;
                        }
                        t = 1;
                    }
                    yield return new WaitForEndOfFrame();
                }
            }
            blinking = false;
            yield return null;
        }


        void Update()
        {
            //Vector2 touchValue = CE.GetTouchpadAxis();
            //if (touchValue.magnitude > 0.05f)
            //{
            //    Vector3 MoveVec = new Vector3(-touchValue.y, 0, touchValue.x)* adjustSpeed *Time.deltaTime;
            //    testMove.transform.Translate(MoveVec);
            //}
        }
    }
}