using RemoteEducation.Scenarios.Inspectable;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Status = RemoteEducation.Scenarios.Inspectable.InspectableElement.Status;

namespace RemoteEducation.Modules.Inspection.Scenarios
{
    ///<summary>Used within the <see cref="ReadOnlyInspectionChecklist"/> to display an individual inspectable and the state the student marked for it.</summary>
    public class ReadOnlyChecklistItem : MonoBehaviour
    {
        ///<summary>The <see cref="RemoteEducation.Scenarios.Inspectable.InspectableElement"/> that this UI element corresponds to.</summary>
        public InspectableElement InspectableElement { get; private set; }

        [Tooltip("Image component for displaying the checkmark or x.")]
        [SerializeField] private Image checkImage;

        [Tooltip("Checkmark sprite to display.")]
        [SerializeField] private Sprite checkSprite;

        [Tooltip("X-mark sprite to display.")]
        [SerializeField] private Sprite xSprite;

        [Tooltip("These are set active when a bad state has been chosen by the student for this item.")]
        [SerializeField] private GameObject[] badStateVisuals;

        [Tooltip("Text component to display the inspectable item's name in.")]
        [SerializeField] private TextMeshProUGUI itemName;

        [Tooltip("Text component to display the state of the inspectable as reported by the student.")]
        [SerializeField] private TextMeshProUGUI inspectionStatus;

        void Awake()
        {
            SetStatus(Status.None);
            ToggleBadStateVisuals(false);
        }

        /// <summary>Set the icon based on the corresponding elements state every time the user selects a new state.</summary>
        public void UpdateStatusIcon()
        {
            SetStatus(InspectableElement.CurrentStatus);
        }

        /// <summary>Set the status icon on the item.</summary>
        /// <param name="status">The icon to use.</param>
        public void SetStatus(Status status = Status.None)
        {
            var noStatus = status == Status.None;

            checkImage.enabled = !noStatus;

            if (noStatus)
                return;

            var goodState = status == Status.InspectedPositive;

            checkImage.sprite = goodState ? checkSprite : xSprite;

            ToggleBadStateVisuals(!goodState);

            if (goodState)
                return;

            var currentState = InspectableElement.GetSelectedState();
            inspectionStatus.text = currentState.StateMessage.ToString();
        }

        /// <summary>Shows or hides the visuals meant to be displayed when a bad state has been chosen by the student.</summary>
        private void ToggleBadStateVisuals(bool show)
        {
            for (int i = 0; i < badStateVisuals.Length; i++)
            {
                badStateVisuals[i].SetActive(show);
            }
        }

        /// <summary>Initializes this prefab to have all its correct values.</summary>
        internal void Setup(GameObject container, InspectableElement element)
        {
            transform.SetParent(container.transform, false);
            itemName.text = element.ChecklistName;
            InspectableElement = element;
        }
    }
}

