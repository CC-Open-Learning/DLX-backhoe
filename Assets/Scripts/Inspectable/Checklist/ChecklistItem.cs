/*
 *  FILE          :	ChecklistItem.cs
 *  PROJECT       :	CORE Engine (Inspection Module)
 *  PROGRAMMER    :	David Inglis
 *  FIRST VERSION :	2020-11-23
 *  DESCRIPTION   :
 *      The ChecklistItem component and associated prefab are used to represent
 *      a single InspectableElement object in the InspectionChecklist UI. 
 *      
 *      The ChecklistItem corresponds directly to the InspectableElement component
 *      of the Inspectable hierarchy
 */

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using RemoteEducation.Interactions;
using Lean.Gui;
using RemoteEducation.Scenarios;
using RemoteEducation.Scenarios.Inspectable;
using System;
using OptionData = TMPro.TMP_Dropdown.OptionData;
using System.Linq;
using Status = RemoteEducation.Scenarios.Inspectable.InspectableElement.Status;

namespace RemoteEducation.Modules.Inspection.Scenarios
{
    public class ChecklistItem : ChecklistHeading
    {
        /// <summary>The path to the "ChecklistItem" prefab resource.</summary>
        new public static readonly string ResourcePath = "UI/Checklist/Checklist Item";

        /// <summary>The <see cref="InspectableElement"/> that this UI element corresponds to.</summary>
        private InspectableElement inspectableElement;

        private LeanToggle toggle;

        [SerializeField] private TMP_Dropdown ConditionDropdown;

        /// <summary>The correlation between the index of the value in the drop down menu and the state it refers to.</summary>
        /// <remarks>Tuple&lt;Drop down menu index, The element state it refers to&gt; </remarks>
        private List<Tuple<int, int>> dropDownStates; 

        [Header("Background Colors")]
        public Image Background;
        public Color NormalColor = new Color(255, 255, 255, 32);
        public Color SelectedColor = new Color(9, 91, 166, 200);

        
        [Header("Status Icons")]
        [SerializeField] private GameObject CheckIcon;
        [SerializeField] private GameObject CrossIcon;
        [SerializeField] private GameObject WarningIcon;
        [SerializeField] private GameObject GoodIcon;
        [SerializeField] private GameObject BadIcon;

        public InspectableElement InspectableElement
        {
            get { return inspectableElement; }
            set
            {
                inspectableElement = value;
                BuildDropDown();
            }
        }

        protected new void Awake()
        {
            base.Awake(); // Configures the 'heading' portion of the Checklist Item.

            SetStatus(Status.None);

            toggle = GetComponent<LeanToggle>();
        }

        public void Expand()
        {
            if (toggle)
            {
                toggle.TurnOn();
            }
        }

        public void Contract()
        {
            if (toggle)
            {
                toggle.TurnOff();
            }
        }

        /// <summary>
        /// Toggle selection on the element that corresponds to this item.
        /// This should be called from the <see cref="LeanButton"/> on the item.
        /// </summary>
        public void Select()
        {
            bool wasSelected = inspectableElement.IsSelected;

            Interactable.UnselectCurrentSelections();

            if(!wasSelected)
            {
                inspectableElement.Select(true);
            }
        }

        /// <summary>
        /// Create all the options for the drop down list.
        /// The states index of the state is saved along with the state identifier it corresponds to.
        /// </summary>
        public void BuildDropDown()
        {
            if (!ConditionDropdown) { return; }

            dropDownStates = new List<Tuple<int, int>>() { new Tuple<int, int>(0, InspectableElement.NO_STATE_SELECTION) };

            List<OptionData> options = new List<OptionData>()
            {
                new OptionData(string.Empty)     // The empty default option
            };

            for(int i = 0; i < InspectableController.ElementStates.Count; i++)
            {
                InspectableElement currentState = InspectableController.ElementStates[i];

                options.Add(new OptionData(currentState.StateMessage));

                dropDownStates.Add(new Tuple<int, int>(i + 1, InspectableController.IdentifiersByElement[currentState]));
            }

            ConditionDropdown.options = options;
        }

        /// <summary>
        /// Called when a new drop down item is selected.
        /// An inspection is made on the <see cref="InspectableElement"/> 
        /// with the state the corresponds to the drop down element.
        /// </summary>
        /// <param name="value">The value selected in the drop down</param>
        public void MakeInspection(int dropDownIndex)
        {
            inspectableElement.MakeUserSelection(dropDownStates.Find(x => x.Item1 == dropDownIndex).Item2);
        }

        /// <summary>Update the selected item in the drop down to the current state of the element.</summary>
        /// <remarks>This should be called when the state of the element is changed by another UI</remarks>
        public void UpdateDropDownItem()
        {
            SetDropDownItem(inspectableElement.UserSelectedIdentifier);
        }

        /// <summary>Set the selected item in the drop down.</summary>
        /// <remarks>This should be called when the state of the element is changed by another UI</remarks>
        public void SetDropDownItem(int stateIndex)
        {
            ConditionDropdown.SetValueWithoutNotify(dropDownStates.Find(x => x.Item2 == stateIndex).Item1);
        }

        /// <summary>Set the icon based on the corresponding elements state.</summary>
        public void UpdateStatusIcon()
        {
            SetStatus(inspectableElement.CurrentStatus);
        }

        /// <summary>Set the status icon on the item.</summary>
        /// <param name="status">The icon to use.</param>
        public void SetStatus(Status status = Status.None)
        {
            GoodIcon.SetActive(status == Status.InspectedPositive);
            BadIcon.SetActive(status == Status.InspectedNegitive);
            WarningIcon.SetActive(status == Status.Warning);
            CheckIcon.SetActive(status == Status.EvaluatedCorrect);
            CrossIcon.SetActive(status == Status.EvaluatedIncorrect);
        }
    }
}