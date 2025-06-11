/*
 *  FILE            : ScenarioPicker.cs
 *  PROJECT         : CORE (Config)
 *  PROGRAMMER      : David Inglis, adapted from ScenarioList.cs by Kieron Higgs
 *  DATE            : 2020-11-19
 */

using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Lean.Gui;

namespace RemoteEducation.Scenarios
{
    public class ScenarioPicker : MonoBehaviour
    {

        private readonly string NoScenariosLoaded = "No Scenarios found in configuration file";
        private readonly string NoScenariosSelected = "Please select at least one Scenario to proceed";

        #region Properties
        private ScenarioPickerItem selected;

        [Header("Resources")]
        [SerializeField] private GameObject pickerItemResource;

        [Header("Layout")]
        [SerializeField] private RectTransform content;

        [SerializeField] private TextMeshProUGUI errorText;

        [Tooltip("Button which will be enabled and disabled")]
        [SerializeField] private LeanToggle confirmButton;
        #endregion


        private void Awake()
        {
            SetErrorText(string.Empty);
        }

        void Start()
        {
            LoadContent();
        }

        #region Private Methods

        /// <summary>
        ///     Reads <see cref="Scenario"/> data from file and populates 
        ///     'content' with the available Scenarios.
        /// </summary>
        private void LoadContent()
        {

            // Use LINQ to sort the collection of 
            // Scenarios by Module name, then by Id
            IEnumerable<Scenario> scenarios = 
                ScenarioBuilder.LoadFromFile()
                .OrderBy(scenario => scenario.Module)
                .ThenBy(scenario => scenario.Identifier);

            if (scenarios.Count() > 0)
            {

                foreach (Scenario scenario in scenarios)
                {
                    CreateScenarioPickerItem(scenario);
                }
            }
            // If no file data is found or the file is empty, create a "default" Scenario and corresponding button (mostly for CORE testing):
            else
            {
                CreateScenarioPickerItem(CreateDefaultScenario());
                SetErrorText(NoScenariosLoaded);
            }

            UpdateConfirmButtonState();
        }

        
        /// <summary>
        ///     Creates a ScenarioPickerItem and adds it to the 
        ///     list view of available Scenarios
        /// </summary>
        /// <param name="scenario">List item is populated with the Module and Title of the Scenario</param>
        private void CreateScenarioPickerItem(Scenario scenario)
        {
            ScenarioPickerItem behaviour = Instantiate(pickerItemResource).GetComponent<ScenarioPickerItem>();

            if (behaviour)
            {
                behaviour.Scenario = scenario;
                behaviour.Parent = this;
                behaviour.transform.SetParent(content, false);
                behaviour.Selected = false;
            }
        }

        
        /// <summary>
        ///     Creates a Scenario to be used in the event that there 
        ///     are no Scenarios found on file, allowing the programmer 
        ///     to proceed to the Scenario scene in testing.
        /// </summary>
        /// <returns>A Scenario with a dummy module, title, and description</returns>
        private Scenario CreateDefaultScenario()
        {
            return new Scenario
            {
                Title = "No Scenarios",
                Description = "No Scenarios available",
                Module = "Error"
            };
        }


        /// <summary>
        ///     Enables or disables the "Confirm" button of the Scenario Picker
        ///     based on whether or not a Scenario is selected
        /// </summary>
        private void UpdateConfirmButtonState()
        {
            if (confirmButton)
            {
                confirmButton.On = selected;
            }
        }


        /// <summary>
        ///     Sets the error text field with the specified message text
        /// </summary>
        /// <param name="message"></param>
        private void SetErrorText(string message)
        {
            if (errorText)
            {
                errorText.text = message;
            }
        }

        #endregion


        #region Public Methods

        /// <summary>
        ///     Unselect the currently selected ScenarioPickerItem
        /// </summary>
        public void ClearSelections()
        {
            selected = null;
            SetErrorText(string.Empty);

            foreach (ScenarioPickerItem button in content.GetComponentsInChildren<ScenarioPickerItem>())
            {
                button.Reset();
            }

            UpdateConfirmButtonState();
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="item"></param>
        public void Select(ScenarioPickerItem item)
        {
            ClearSelections();

            item.Select();
            selected = item;

            UpdateConfirmButtonState();
        }

        /// <summary>
        ///     Loads the selected <see cref="Scenario"/> if it is not <c>null</c>
        /// </summary>
        public void ConfirmSelectedScenarios() 
        {
            if (selected != null && selected.Scenario != null)
            {

                ScenarioBuilder.CreatePersistentScenarioData(selected.Scenario);

                StartCoroutine(ScenarioManager.LazyLoad());
            }
            else
            {
                SetErrorText(NoScenariosSelected);
            }
        }

        #endregion

    }
}