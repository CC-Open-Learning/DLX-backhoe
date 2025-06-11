/*
 *  FILE          :	InspectionChecklist.cs
 *  PROJECT       :	CORE Engine (Inspection Module)
 *  PROGRAMMER    :	David Inglis
 *  FIRST VERSION :	2020-11-23
 *  DESCRIPTION   :
 *      The InspectionChecklist and associated prefab are used to display information about
 *      and provide user interaction to the InspectableManager. Using the information provided
 *      by the InspectableManager, corresponding headings and elements are created in the UI panel
 *      
 *      The InspectionChecklist corresponds directly to the InspectableManager component
 *      of the Inspectable hierarchy
 */

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Lean.Gui;
using RemoteEducation.Scenarios;
using RemoteEducation.Scenarios.Inspectable;
using RemoteEducation.UI;
using RemoteEducation.Localization;
using UnityEngine.Serialization;

namespace RemoteEducation.Modules.Inspection.Scenarios
{
    /// <summary>The InspectionChecklist corresponds directly to the InspectableManager component of the Inspectable hierarchy.</summary>
    public class InspectionChecklist : MonoBehaviour, IInspectableUI
    {
        [SerializeField] private InspectableManager InspectableManager;
        [SerializeField] private TextMeshProUGUI ChecklistTitle;
        [SerializeField] private GameObject ChecklistContent;

        [Tooltip("A LeanToggle object used to submit the checklist and finish inspection. " +
            "Disabled on start and enabled after evaluating at least once.")]
        [SerializeField] private LeanToggle SubmitButton;
        [SerializeField] private LeanButton EvaluateButton;


        [Tooltip("If enabled, the Checklist will be opened when selecting an InspectableElement that has not yet been inspected")]
        [FormerlySerializedAs("TogglePanelOnSelect")]
        public bool OpenOnSelectionNone = true;

        [Tooltip("If enabled, the Checklist will be opened when selecting an InspectableElement that has been inspected, but not yet evaluated")]
        public bool OpenOnSelectionInspected = true;

        [Tooltip("If enabled, the Checklist will be opened when selecting an InspectableElement that has been evaluated")]
        public bool OpenOnSelectionEvaluated = true;

        [Tooltip("If enabled, user must finish inspecting every object before they are able to submit their results")]
        public bool RequireAllInspectionsForSubmission = false;

        /// <summary>Tracks all ChecklistItems in the checklist, so that they can be dynamically manipulated.</summary>
        private List<ChecklistItem> Items;

        /// <summary>Tracks the different sections within the checklist.</summary>
        private List<ChecklistHeading> Headings;

        private LeanWindow window;

        public void InitializeUI(InspectableManager inspectableManager)
        {
            WindowLoader loader = GetComponent<WindowLoader>();
            window = loader.Window.GetComponent<LeanWindow>();
            ScenarioManager.Instance.WindowManager.Add(loader);

            InspectableManager = inspectableManager;

            Items = new List<ChecklistItem>();

            if (ChecklistTitle)
            {
                ChecklistTitle.text = "Engine.InspectionChecklist".Localize();
            }

            SubmitButton?.TurnOff();

            GenerateChecklistItems(inspectableManager.InspectableControllers);

            EvaluateButton?.OnClick.AddListener(() => inspectableManager.EvaluateElements());

            inspectableManager.OnElementStateChanged += RefreshElement;

            inspectableManager.OnEvaluationCompleted += RefreshActiveElements;
            inspectableManager.OnEvaluationCompleted += UpdateSubmitButtonVisability;

            inspectableManager.OnActiveElementsChanged += ShowElementSubset;

            inspectableManager.OnElementSelected += ContractAll;

            inspectableManager.OnElementSelected += TryOpenChecklist;

            inspectableManager.OnElementDeselected += ContractOne;
        }

        /// <summary>Check if the submit button should become clickable.</summary>
        /// <remarks>This should be called after an evaluation.</remarks>
        private void UpdateSubmitButtonVisability()
        {
            if (UserMaySubmitResults())
            {
                SubmitButton?.TurnOn();
            }
        }

        /// <summary>Checks if the user may submit their results.</summary>
        /// <returns><see langword="true"/> if the button is active and either the user does not need to inspect every object or they have inspected every object.</returns>
        private bool UserMaySubmitResults()
        {
            return SubmitButton.gameObject.activeSelf && (!RequireAllInspectionsForSubmission || InspectableManager.ElementsInspected(InspectableManager.InspectableElements));
        }

        /// <summary>Update the status icons for all active elements.</summary>
        public void RefreshActiveElements()
        {
            Items.Where(x => x.InspectableElement.CurrentlyInspectable).ToList().ForEach(x => x.UpdateStatusIcon());
        }

        /// <summary>Update the status icon for an Item based on a <see cref="InspectableElement"/></summary>
        /// <param name="inspectableElement">The element that was changed</param>
        public void RefreshElement(InspectableElement inspectableElement)
        {
            ChecklistItem item = Items.Find(x => x.InspectableElement == inspectableElement);

            item.UpdateStatusIcon();
            item.UpdateDropDownItem();
        }

        /// <summary>Toggle if the submit button is visible. This is different from  when it is disabled.</summary>
        /// <param name="visible">If the button should be visible</param>
        public void SetSubmitButtonVisible(bool visible)
        {
            SubmitButton?.gameObject.SetActive(visible);
        }

        /// <summary>Creates headings and interactable elements in the InspectionChecklist UI panel to allow a user to provide feedback on the Inspection Scenario using their mouse.</summary>
        private void GenerateChecklistItems(List<InspectableController> controllers)
        {
            // Load template prefabs from Resources
            GameObject headingResource = Resources.Load(ChecklistHeading.ResourcePath) as GameObject;
            GameObject itemResource = Resources.Load(ChecklistItem.ResourcePath) as GameObject;

            Headings = new List<ChecklistHeading>();

            foreach (InspectableController controller in controllers)
            {
                ChecklistHeading currentHeading = null;

                if (controller.InspectableElements != null)
                {
                    // Create group heading
                    currentHeading = Instantiate(headingResource).GetComponent<ChecklistHeading>();
                    if (!currentHeading)
                    {
                        Debug.LogError("InspectionChecklist : Template 'Checklist Heading' does not have the appropriate component attached");
                        return;
                    }

                    // Set position and heading text
                    currentHeading.transform.SetParent(ChecklistContent.transform, false);
                    currentHeading.Text = controller.ElementGroupName;
                    currentHeading.InspectableController = controller;
                    Headings.Add(currentHeading);
                }

                // Create individual elements under each group heading
                foreach (InspectableElement element in controller.InspectableElements)
                {
                    ChecklistItem item = Instantiate(itemResource).GetComponent<ChecklistItem>();

                    if (!item)
                    {
                        Debug.LogError("InspectionChecklist : Template 'Checklist Item' does not have the appropriate component attached");
                        return;
                    }

                    item.transform.SetParent(ChecklistContent.transform, false);
                    item.Text = element.ChecklistName;
                    item.InspectableController = controller;
                    item.InspectableElement = element;

                    // Add reference to List tracked by InspectionChecklist
                    Items.Add(item);

                    if (currentHeading != null)
                    {
                        currentHeading.Items.Add(item);
                    }
                }
            }
        }

        /// <summary>Show only a sub set of elements in the check list.</summary>
        /// <param name="elementsToShow">The elements to show.</param>
        public void ShowElementSubset(List<InspectableElement> elementsToShow)
        {
            //for all the check list items, only show it if its element is in the list.
            Items.ForEach(x => x.gameObject.SetActive(elementsToShow.Contains(x.InspectableElement)));

            //update which headings are visible
            Headings.ForEach(x => x.UpdateVisability());
        }

        /// <summary>A Coroutine used to contract all ChecklistItems in the InspectionChecklist, and expand only one single ChecklistItem instance.
        /// This provides the behaviour of having at most one expanded ChecklistItem at any given time,
        /// which allows the user to focus their inspection on only the component they have just selected,
        /// either through the UI panel or by clicking on the 'Inspectable' object directly.</summary>
        /// <remarks>This behaviour must be run as a coroutine with a short delay between the contraction and expansion of ChecklistItems.
        /// It seems that expanding and contracting ChecklistItems simultaneously causes the VerticalLayoutGroup component to become confused and
        /// set the width of any transitioning ChecklistItem to be set to 0.</remarks>
        /// <param name="focus">The single <see cref="ChecklistItem"/> which should be expanded. Defaults to null, in which case all ChecklistItems are contracted.</param>
        private IEnumerator ContractAndSwitchFocus(ChecklistItem focus = null)
        {
            foreach (ChecklistItem item in Items.Except(new List<ChecklistItem>() { focus }))
            {
                item.Contract();
                item.Background.color = item.NormalColor;
            }

            yield return new WaitForSeconds(.05f);

            if (focus)
            {
                focus.Expand();
                focus.Background.color = focus.SelectedColor;
            }
        }

        /// <summary>Contracts all <see cref="ChecklistItem"/> objects except for the given <paramref name="exception"/>.</summary>
        /// <param name="exception">The element to leave expanded.</param>
        public void ContractAll(ChecklistItem exception = null)
        {
            StartCoroutine(ContractAndSwitchFocus(exception));
        }

        /// <summary>Contracts all <see cref="ChecklistItem"/> objects except for the one attached to the given <paramref name="exception"/>.</summary>
        /// <param name="exception">The element to leave expanded.</param>
        public void ContractAll(InspectableElement exception = null)
        {
            ContractAll(Items.Find(item => item.InspectableElement == exception));
        }

        /// <summary>Contracts the given <paramref name="element"/> within the list.</summary>
        /// <param name="element">The element to contract.</param>
        public void ContractOne(InspectableElement element)
        {
            ChecklistItem item = Items.Find(tmp => tmp.InspectableElement == element);

            if (item)
            {
                item.Contract();
                item.Background.color = item.NormalColor;
            }
        }

        /// <summary>
        ///     Reads the <see cref="OpenOnSelectionNone"/>, <see cref="OpenOnSelectionInspected"/>, 
        ///     and <see cref="OpenOnSelectionEvaluated"/> to determine if the Checklist window
        ///     should be opened.
        /// </summary>
        /// <param name="element">
        ///     The currently selected <see cref="InspectableElement"/>. If null, the checklist will 
        ///     not be opened. <see cref="OpenChecklist"/> should be used instead if this parameter is
        ///     not needed.
        /// </param>
        public void TryOpenChecklist(InspectableElement element)
        {
            if (element != null
                && ((OpenOnSelectionNone && !element.HasBeenInspected)
                    || (OpenOnSelectionInspected && element.HasBeenInspected && !element.HasBeenEvaluated)
                    || (OpenOnSelectionEvaluated && element.HasBeenEvaluated)))
            {
                OpenChecklist();
            }
        }

        /// <summary>
        ///     Turns on the window that contains the checklist
        /// </summary>
        /// <remarks>
        ///     This method is a simple abstraction of the underlying LeanWindow. Since it
        ///     is public, it allows another class to bypass any selection restrictions and
        ///     open the Checklist window directly
        /// </remarks>
        public void OpenChecklist() { window.TurnOn(); }
    }
}