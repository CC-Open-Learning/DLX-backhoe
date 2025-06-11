/*
 *  FILE          :	TableCanvas.cs
 *  PROJECT       :	CORE Engine
 *  PROGRAMMER    :	Kieron Higgs
 *  FIRST VERSION :	2020-07-17
 *  DESCRIPTION   : Holds the "spreadsheet" of rows and columns that must be filled out by the user to complete a lab.
 */

#region Resources

using Lean.Gui;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System;

#endregion

namespace RemoteEducation.Scenarios
{
    /*
     * CLASS      : TableCanvas
     * DESCRIPTION: Holds labels for rows and columns and creates a "spreadsheet" dynamically.
     */
    public class TableCanvas : MonoBehaviour
    {
        public string[] rowLabels = new string[] {};
        public string[] columnLabels = new string[] {};
        private List<Text> inputFields = new List<Text>();
        private LeanButton submitButton;

        /*
         * METHOD      : Init()
         * DESCRIPTION : Sets up the "spreadsheet" to have the corrent number of input fields and adjust sizing.
         */
        public void Init()
        {
            List<TextMeshProUGUI> rowLabels_TMPro = new List<TextMeshProUGUI>();
            List<TextMeshProUGUI> columnLabels_TMPro = new List<TextMeshProUGUI>();

            Transform tableT = transform.Find("Panel/ScrollView/Viewport/Table");
            submitButton = transform.Find("Panel/TableFooter/SubmitButton").GetComponent<LeanButton>();
            submitButton.interactable = false;
            
            // Assign the first row's label (ex. cell [0,0]; "P1 Pressure (BAR)" in Hydraulics Lab 1; "DCV Position" in Hydraulics Lab 2)
            GameObject headerRow = Instantiate(Resources.Load("TableRow"), tableT) as GameObject;
            TextMeshProUGUI headerRowLabel_TMPro = headerRow.transform.Find("Label").GetComponent<TextMeshProUGUI>();
            headerRowLabel_TMPro.text = rowLabels[0];

            rowLabels_TMPro.Add(headerRowLabel_TMPro);

            // Assign the column labels; their width is dynamic (unlike the row labels, though row label width is anchored):
            Transform cellsT = headerRow.transform.Find("Cells");
            float cellWidth = cellsT.GetComponent<RectTransform>().rect.width / columnLabels.Length;
            float cellHeight = cellsT.parent.GetComponent<RectTransform>().rect.height;
            Vector2 newCellSize = new Vector2(cellWidth, cellHeight);

            foreach (string columnLabel in columnLabels)
            {
                GameObject newColumnLabel = Instantiate(Resources.Load("TableLabel"), cellsT.transform) as GameObject;
                TextMeshProUGUI newColumnLabel_TMPro = newColumnLabel.GetComponent<TextMeshProUGUI>();
                newColumnLabel_TMPro.text = columnLabel;
                newColumnLabel.GetComponent<RectTransform>().sizeDelta = newCellSize;

                columnLabels_TMPro.Add(newColumnLabel_TMPro);
            }

            // Create rows of input fields (using the TableInput prefab) for the rest of the table
            int numInputRows = rowLabels.Length;
            int numColumns = columnLabels.Length;
            int i = 1; // start at 1 to skip the header row just created
            int j = 0; // for iterating through columns
            for (i = 1; i < numInputRows; i++)
            {
                GameObject newRow = Instantiate(Resources.Load("TableRow"), tableT) as GameObject;
                TextMeshProUGUI newRowLabel_TMPro = newRow.transform.Find("Label").GetComponent<TextMeshProUGUI>();
                newRowLabel_TMPro.text = rowLabels[i];

                rowLabels_TMPro.Add(newRowLabel_TMPro);

                for (j = 0; j < numColumns; j++)
                {
                    GameObject newInputField = Instantiate(Resources.Load("TableInput"), newRow.transform.Find("Cells")) as GameObject;
                    newInputField.GetComponent<RectTransform>().sizeDelta = newCellSize;
                    inputFields.Add(newInputField.transform.Find("InputField/Text").GetComponent<Text>());
                }
            }

            Transform scrollViewT = transform.Find("Panel/ScrollView");
            StartCoroutine(SetScrollbar(scrollViewT));
            submitButton.GetComponent<LeanButton>().OnClick.AddListener(SubmitResults);
            NormalizeFontSize(rowLabels_TMPro);
            NormalizeFontSize(columnLabels_TMPro);
        }


        /*
         * METHOD      : SetScrollbar()
         * DESCRIPTION : 
         *      Sets the TableCanvas scrollbar to the top of the ScrollView when it is done being filled with TableRows.
         * PARAMETERS  :
         *      Transform scrollViewT - the transform of the ScrollView for accessing the ScrollRect within
         * RETURNS     :
         *      IENumerator - coroutine
         */
        private IEnumerator SetScrollbar(Transform scrollViewT)
        {
            yield return new WaitForEndOfFrame(); // Wait a frame so the scrollbar can be set to the top
            scrollViewT.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
            yield return null;
        }

        /*
         * METHOD      : NormalizeFontSize()
         * DESCRIPTION : 
         *      Iterates over the row labels of the TableCanvas and assigns the lowest common font amongst them to all of them.
         */
        public void NormalizeFontSize(List<TextMeshProUGUI> labels_TMPro)
        {
            // Assign the row labels (using TableLabel prefab) on the left-hand side of the table:
            float lowestFontSize = 100f;

            foreach (TextMeshProUGUI label in labels_TMPro)
            {
                if (label.fontSize < lowestFontSize)
                {
                    lowestFontSize = label.fontSize;
                }
            }

            foreach (TextMeshProUGUI label in labels_TMPro)
            {
                if (label.fontSize > lowestFontSize)
                {
                    label.enableAutoSizing = false;
                    label.fontSize = lowestFontSize;
                }
            }
        }

        /*
         * METHOD      : CheckInputFields()
         * DESCRIPTION : 
         *      Iterates over the list of input fields and determines whether all of them have been filled by the user.
         *      If so, the method enables the Submit button.
         */
        public void CheckInputFields()
        {
            int numInputsGiven = 0;
            foreach (Text inputField in inputFields)
            {
                if (!String.IsNullOrWhiteSpace(inputField.text))
                {
                    numInputsGiven++;
                }
            }
            if (numInputsGiven == inputFields.Count)
            {
                submitButton.interactable = true;
            }
        }

        /*
         * METHOD      : SubmitResults()
         * DESCRIPTION : 
         *      Concludes the scene by marking the last Interactor is finished. This method can only be fired once the Submit button
         *      has been activated (and as such, the user has entered all fields into the table).
         */
        private void SubmitResults()
        {
            //Scenario scenario = FindObjectOfType(typeof(Scenario));
            //ScenarioManager.Instance.ApplyScore(ScenarioManager.Instance.GetScenarioTasks(scenario.Identifier)[scenario.GetInteractors(Interactor.Type.Task).Count - 1]);
            // stub for handing lab results to database (perhaps by calling upon scenariomanager/builder/etc)
        }
    }
}