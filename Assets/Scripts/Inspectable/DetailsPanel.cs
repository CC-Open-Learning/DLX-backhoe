using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using RemoteEducation.Interactions;

namespace RemoteEducation.Scenarios.Inspectable
{
    [RequireComponent(typeof(Animator))]
    public class DetailsPanel : MonoBehaviour, IInitializable
    {

        [Tooltip("Titlebox of the Panel")]
        public TextMeshProUGUI TitleField;

        [Tooltip("Single Format Context Panel")]
        public Transform SingleFormat;

        [Tooltip("Context of the Panel - Single Format")]
        public TextMeshProUGUI SingleContextField;

        [Tooltip("Image of the Panel - Single Format")]
        public Image SingleContextImage;

        [Tooltip("Double Format Context Panel")]
        public Transform DoubleFormat;

        [Tooltip("Context of the Panel - Double Format - Left Side")]
        public TextMeshProUGUI DoubleContextFieldLeft;

        [Tooltip("Image of the Panel - Double Format - Left Side")]
        public Image DoubleContextImageLeft;

        [Tooltip("Context of the Panel - Double Format - Right Side")]
        public TextMeshProUGUI DoubleContextFieldRight;

        [Tooltip("Image of the Panel - Double Format - Right Side")]
        public Image DoubleContextImageRight;

        [Tooltip("Footer TextBox")]
        public TextMeshProUGUI CommonText;

        /// <summary> Panel that surrounds the content </summary>
        public RectTransform PanelFrame;

        /// <summary> Extra Details Attachment that the panel can read information for </summary>
        private ExtraDetails contentDetails;

        /// <summary> Inspectable Element that Panel is reading </summary>
        private InspectableElement context;


        /// <summary> Flat values of window horizontal</summary>
        private float flatHorizontal = 550;

        /// <summary> Flat values of window vertical</summary>
        private float flatVertical = 650;

        /// <summary> Animator component on this gameObject that controls the open and close animations </summary>
        private Animator animator;

        /// <summary> String hashes for Animations to trigger </summary>
        private static readonly int openAnimHash = Animator.StringToHash("Open");
        private static readonly int closeAnimHash = Animator.StringToHash("Close");

        public void Initialize(object input = null)
        {
            //This needs to be put in here because this gameObject starts as not active
            animator = GetComponent<Animator>();
        }

        /// <summary> Function that shows the panel </summary>
        public void ShowPanel()
        {
            //Check if there is something to show
            if(contentDetails == null)
            {
                Debug.Log("DetailsPanel : Unable to find ExtraDetails of InspectableElement when showing panel");
                return;
            }

            //Show the panel
            gameObject.SetActive(true);

            //Set the title
            TitleField.text = context.ChecklistName;
            
            //Resize the panel
            ResizePanel();

            switch(contentDetails.objectFormat)
            {
                case ExtraDetails.Format.Single:
                    //On a single format read from the extra detail script, set it to the single format
                    SingleFormat.gameObject.SetActive(true);
                    DoubleFormat.gameObject.SetActive(false);

                    //Set Content Text
                    CommonText.text = contentDetails.GetContext(contentDetails.contentText);

                    //Set Image
                    SingleContextImage.sprite = contentDetails.contentImage[0];

                    //Preserve Aspect Ratio
                    SingleContextImage.preserveAspect = true;

                    //If a caption exist set it down
                    if(contentDetails.captionText.Count > 0)
                        SingleContextField.text = contentDetails.GetContext(contentDetails.captionText[0]);
                break;
                case ExtraDetails.Format.Double:
                    //Set position to be center since double format is large and set format to double format
                    contentDetails.objectPostion = ExtraDetails.Position.Center;
                    SingleFormat.gameObject.SetActive(false);
                    DoubleFormat.gameObject.SetActive(true);

                    //Set Content Text
                    CommonText.text = contentDetails.GetContext(contentDetails.contentText);

                    //Set Two Images
                    DoubleContextImageLeft.sprite = contentDetails.contentImage[0];
                    DoubleContextImageRight.sprite = contentDetails.contentImage[1];

                    //Preserve Aspect Ratio
                    DoubleContextImageLeft.preserveAspect = true;
                    DoubleContextImageRight.preserveAspect = true;

                    //If Left Caption Exist Set It
                    if (!string.IsNullOrEmpty(contentDetails.captionText[0]))
                        DoubleContextFieldLeft.text = contentDetails.GetContext(contentDetails.captionText[0]);
                    
                    //If Right Caption Exist Set It
                    if(!string.IsNullOrEmpty(contentDetails.captionText[1]))
                        DoubleContextFieldRight.text = contentDetails.GetContext(contentDetails.captionText[1]);
                break;
            }
            
            //Set panel position on screen
            RectTransform panelPosition = GetComponent<RectTransform>();
            switch(contentDetails.objectPostion)
            {
                case ExtraDetails.Position.Top:
                    panelPosition.localPosition = new Vector3(0, Screen.height / 2, 0);
                    break;
                case ExtraDetails.Position.Left:
                    panelPosition.localPosition  = new Vector3(-Screen.width / 2, 0, 0);
                    break;
                case ExtraDetails.Position.Center:
                    panelPosition.localPosition  = new Vector3(0, 0, 0);
                    break;
                case ExtraDetails.Position.Right:
                    panelPosition.localPosition  = new Vector3(Screen.width / 2, 0, 0);
                    break;
                case ExtraDetails.Position.Bottom:
                    panelPosition.localPosition  = new Vector3(0, -Screen.height / 2, 0);
                    break;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(PanelFrame);
            animator.SetTrigger(openAnimHash);

        }

        /// <summary> Function that gets the inspectable element from the manager</summary>
        public void UpdateContent(InspectableElement element)
        {
            context = element;

            //Get Extra Details
            contentDetails = element.gameObject.GetComponent<ExtraDetails>();
        }

        /// <summary> Function that resizes the panel based on fixed or special values </summary>
        public void ResizePanel()
        {
            float tempWidth = contentDetails.horizontalContextSize;
            float tempHeight = contentDetails.verticalContextSize;
            
            //Check if special values exist and set to flat if 0
            if (tempWidth == 0)
                tempWidth = flatHorizontal;

            if (tempHeight == 0)
                tempHeight = flatVertical;

            //Size the window
            switch(contentDetails.objectFormat)
            {
                case ExtraDetails.Format.Single:
                    PanelFrame.sizeDelta = new Vector2(tempWidth, tempHeight);
                break;
                case ExtraDetails.Format.Double:
                    PanelFrame.sizeDelta = new Vector2(tempWidth * 2, tempHeight);
                break;
            }
        }

        /// <summary> Function that would hide the panel </summary>
        public void HidePanel()
        {
            if(animator.gameObject.activeSelf)
                animator.SetTrigger(closeAnimHash);
        }

        /// <summary> Deactivate itself, called as an event from the animator after HidePanel() </summary>
        public void DeactivateSelf() {
            gameObject.SetActive(false);
        }

        /// <summary> Function that would remove the context </summary>
        public void RemoveContext()
        {
            context = null;
        }
    }
}