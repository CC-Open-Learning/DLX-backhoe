/*
*  FILE          :	MagnifierTexture.cs
*  PROJECT       :	Core Engine 
*  PROGRAMMER    :	Vivek Savsaiya
*  FIRST VERSION :	2020-8-11
*  DESCRIPTION   :  This file contains the MagnifierTexture class. 
*  
*  Class Description: This class contains magnifierTexture, Camera that will parent/unparent on selection of Hydraulic Component.
*  
*/

using UnityEngine;
using UnityEngine.UI;

namespace RemoteEducation.Interactions
{
    public class MagnifierTexture : MonoBehaviour
    {
        public static MagnifierTexture Instance;

        [SerializeField]
        GameObject magnifierCamera;

        [SerializeField]
        GameObject renderParent;

        [SerializeField]
        RawImage renderRawImage;

        [SerializeField]
        Toggle toggleMagnifier;

        [SerializeField]
        Image highlightedRing;

        bool followMouse = false;

        [HideInInspector]
        public GameObject currentComponent;

        private bool isMagnifierActive = false;
        public bool isMagnifierLockActive = false;
        private RenderTexture renderTexture;

        public void Awake()
        {
            Instance = this;
        }
        // Start is called before the first frame update
        void Start()
        {
            //Create New Render texture and assign it to Camera and Render Raw Image
            renderTexture = CreateRenderTexture();
            magnifierCamera.GetComponent<Camera>().targetTexture = renderTexture;
            renderRawImage.texture = renderTexture;

            //Lock Toggle listener event on lock/unlock
            toggleMagnifier.onValueChanged.AddListener(delegate
            {
                MagnifierToggleValueChanged(toggleMagnifier);
            });

            //Disable Magnifier at start.
            DisableMagnifierTexture();
        }

        void Update()
        {
            if (isMagnifierActive && followMouse)
            {
                renderParent.transform.position = (Input.mousePosition);
            }

            // Once the magnifier is locked, call LockInMagnifier() to poke task manager
            // of the hydraulic component
            if (isMagnifierLockActive)
            {
                Interactable interactable = currentComponent.GetComponent<Interactable>();
                if (interactable is IMagnifierLockable)
                {
                    var lockable = interactable as IMagnifierLockable;
                    lockable.LockInMagnifier();
                }
            }
        }

        //Disable texture, unparent Camera from Component to This. When isForced TRUE, it will make sure that magnifier will be disabled on destroying hydraulics component
        public void DisableMagnifierTexture(bool isForced = false)
        {
            if (!this)
            {
                return;
            }

            if ((!isMagnifierLockActive || isForced))
            {
                currentComponent = null;
                isMagnifierActive = false;
                magnifierCamera.transform.SetParent(transform);
                toggleMagnifier.isOn = false;
                renderParent.SetActive(false);
                gameObject.SetActive(false);

                if (MagnifierManager.Instance)
                {
                    MagnifierManager.Instance.RefreshMagnifierCount();
                }
            }
        }

        public void SetMagnifierTexture(GameObject component, GameObject magnifierParent)
        {
            gameObject.SetActive(true);
            currentComponent = component;
            magnifierCamera.transform.SetParent(magnifierParent.transform);
            magnifierCamera.transform.localPosition = Vector3.zero;
            magnifierCamera.transform.localRotation = Quaternion.Euler(Vector3.zero);
            isMagnifierActive = true;
            renderParent.SetActive(true);
        }


        private void MagnifierToggleValueChanged(Toggle change)
        {
            isMagnifierLockActive = change.isOn;
            if (!change.isOn && currentComponent != null)
            {
                Interactable interactable = currentComponent.GetComponent<Interactable>();

                if (interactable != null && !interactable.IsSelected)
                {
                    DisableMagnifierTexture(true);
                }
            }
        }

        private RenderTexture CreateRenderTexture()
        {
            RenderTexture rt = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
            rt.Create();
            return rt;
        }

        public void SetHighlightedRing(bool isHighlighted)
        {
            highlightedRing.gameObject.SetActive(isHighlighted);
        }
    }
}