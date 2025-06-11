using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RemoteEducation.Interactions;
using RemoteEducation.Scenarios;


namespace RemoteEducation.Modules.HeavyEquipment
{
    public class GaugePanel : SemiSelectable, IInitializable
    {
        [SerializeField, Tooltip("Animation Time For Clipboard Animation")]
        private float AnimationDuration = 1;

        [SerializeField, Tooltip("Is Open")]
        private bool IsClipboardOpen = false;

        [SerializeField, Tooltip("Is Animating")]
        private bool IsAnimating = false;

        [SerializeField, Tooltip("Open Position")]
        private Transform ClipOpenPosition;
        private Transform cameraPos => CoreCameraController.Instance.MainCameraTransform;

        [SerializeField, Tooltip("Clipboard to Animate")]
        private Transform ClipboardObj;

        /// <summary>Initialization of Gauge Panel</summary>
        public void Initialize(object input = null)
        {
            OnSelect += OpenClipboard;
            ClipboardObj.gameObject.SetActive(false);
        }

        public void OpenClipboard()
        {
            if (IsClipboardOpen && IsAnimating)
                return;

            IsClipboardOpen = true;
            IsAnimating = true;
            StartCoroutine(OpenClipboardAnimation());
        }

        public void CloseClipboard()
        {
            if (!IsClipboardOpen && IsAnimating)
                return;

            IsAnimating = true;
            IsClipboardOpen = false;
            StartCoroutine(CloseClipboardAnimation());
        }

        private IEnumerator OpenClipboardAnimation()
        {
            float startTimer = Time.time;
            float endTimer = startTimer + AnimationDuration;
            float timeRatio = 0;
            ClipboardObj.gameObject.SetActive(true);

            Vector3 OpenPosition = ClipOpenPosition.position;
            Vector3 ClosePosition = cameraPos.position + cameraPos.up * -1f;

            StartCoroutine(AlwaysLookAtCamera());

            while (Time.time <= endTimer)
            {
                timeRatio = (Time.time - startTimer) / AnimationDuration;
                ClipboardObj.position = Vector3.Lerp(ClosePosition, OpenPosition, timeRatio);
                yield return null;
            }

            ClipboardObj.position = OpenPosition;
            IsAnimating = false;
        }

        private IEnumerator CloseClipboardAnimation()
        {
            float startTimer = Time.time;
            float endTimer = startTimer + AnimationDuration;
            float timeRatio = 0;

            Vector3 OpenPosition = ClipOpenPosition.position;
            Vector3 ClosePosition = cameraPos.position + cameraPos.up * -1f;

            StopCoroutine(AlwaysLookAtCamera());

            while (Time.time <= endTimer)
            {
                timeRatio = (Time.time - startTimer) / AnimationDuration;
                ClipboardObj.position = Vector3.Lerp(OpenPosition, ClosePosition, timeRatio);
                yield return null;
            }

            ClipboardObj.position = ClosePosition;
            ClipboardObj.gameObject.SetActive(false);
            IsAnimating = false;
        }

        private IEnumerator AlwaysLookAtCamera()
        {

            while (IsClipboardOpen)
            {
                ClipboardObj.LookAt(cameraPos);
                yield return null;
            }
        }
    }
}