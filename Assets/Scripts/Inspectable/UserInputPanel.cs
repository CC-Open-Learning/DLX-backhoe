/*
*  FILE          :	UserInputPanel.cs
*  PROGRAMMER    :	Leon Vong
*  FIRST VERSION :	2021-19-08
*  DESCRIPTION   :  This file is the UI display of our UserInput Window, it handles its functionality
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using RemoteEducation.Interactions;

namespace RemoteEducation.Scenarios.Inspectable
{
    public class UserInputPanel : MonoBehaviour
    {
        [Tooltip("Titlebox of the Panel")]
        public TextMeshProUGUI TitleField;

        [Tooltip("Prompt of the Panel")]
        public TextMeshProUGUI PromptField;

        [Tooltip("Prompt of the Panel")]
        public TMP_InputField UserInputField;

        [Tooltip("Submit Button of the Panel")]
        public Button Button;
        
        /// <summary> Inspectable Element that Panel is reading </summary>
        private TextBasedElement context;

        private IInspectable<int> inspectableScript;

        void Awake()
        {
            Button.onClick.AddListener(() => SubmitButton());
        }

        /// <summary> Function that shows the content </summary>
        public void ShowPanel()
        {
            gameObject.SetActive(true);

            TitleField.text = context.ElementName;
            PromptField.text = context.UserPrompt;
        }

        /// <summary> Function that hides the content via unselected </summary>
        public void HidePanel()
        {
            gameObject.SetActive(false);
        }

        
        /// <summary> Function that updates the content on mouse selected </summary>
        public void UpdateContent(TextBasedElement element)
        {
            context = element;
            inspectableScript = context.GetComponent<IInspectable<int>>();
        }

        /// <summary> Function that handles what the submit button does </summary>
        private void SubmitButton()
        {
            if(context != null && inspectableScript != null)
            {
                inspectableScript.CheckIfComplete(int.Parse(UserInputField.text));
            }
        }
    }
}