using RemoteEducation.Scenarios;
using System.Collections;
using UnityEngine;

namespace RemoteEducation.Modules.Inspection.Scenarios
{
    public class Clipboard : MonoBehaviour
    {
        [SerializeField, Tooltip("Speed to move the clipboard into position")]
        private float AnimationSpeed;

        [SerializeField, Tooltip("Curve to dictate how the clipboard should show up.")]
        private AnimationCurve ClipboardAnimationCurve;

        private Transform cameraPos => CoreCameraController.Instance.MainCameraTransform;

        //Starting y position of the clipboard below the camera before it animates into view
        private float initialYPosOffset = -5.0f;

        //Floats to bring the clipboard in front of the camera in the center of the screen but slightly tilted back
        private float zPosOffset = 0.75f;
        private float xAngle = 85.0f;
        private float yAngle = 180.0f;
        private float zAngle = 0.0f;

        private State state = State.Inactive;

        private enum State
        {
            Inactive = 0,
            Activating = 1,
            Active = 2,
            Deactivating = 3
        }

        public void MoveClipboard()
        {
            //Camera position & rotation for right side of screen
            //gameObject.transform.position = cameraPos.position + (cameraPos.forward * 0.5f) + (cameraPos.right * 0.30f) + (cameraPos.up * -0.15f);
            //gameObject.transform.rotation = cameraPos.rotation * Quaternion.Euler(90.0f, 180.0f, -15.0f);

            switch (state)
            {
                case State.Inactive:
                case State.Deactivating:
                    state = State.Activating;
                    gameObject.SetActive(true);
                    break;

                case State.Active:
                case State.Activating:
                    state = State.Deactivating;
                    break;
            }
        }

        private void Awake()
        {
            transform.position = cameraPos.position + (cameraPos.forward * zPosOffset) + (cameraPos.up * initialYPosOffset);
            transform.rotation = cameraPos.rotation * Quaternion.Euler(xAngle, yAngle, zAngle);
        }

        void Update()
        {
            float animationCurve = ClipboardAnimationCurve.Evaluate(AnimationSpeed * Time.deltaTime);

            switch (state)
            {
                case State.Activating :
                    transform.position = Vector3.Lerp(transform.position, cameraPos.position + (cameraPos.forward * zPosOffset), animationCurve);

                    if (transform.position == cameraPos.position + (cameraPos.forward * zPosOffset))
                    {
                        state = State.Active;
                    }

                    break;

                case State.Deactivating :
                    transform.position = Vector3.Lerp(transform.position, cameraPos.position + (cameraPos.forward * zPosOffset) + (cameraPos.up * initialYPosOffset), animationCurve);

                    if (transform.position == cameraPos.position + (cameraPos.forward * zPosOffset) + (cameraPos.up * initialYPosOffset))
                    {
                        state = State.Inactive;
                        gameObject.SetActive(false);
                    }

                    break;
            }
        }
    }
}