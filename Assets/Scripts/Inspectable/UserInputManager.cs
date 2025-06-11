/*
*  FILE          :	UserInputManager.cs
*  PROGRAMMER    :	Leon Vong
*  FIRST VERSION :	2021-19-08
*  DESCRIPTION   :  This file is used for managing the user input window
*/
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Lean.Gui;

namespace RemoteEducation.Scenarios.Inspectable
{
    public class UserInputManager : MonoBehaviour, IInspectableUI
    {
        /// <summary> Ensures exactly one UserInputManager exists in a Unity Scene </summary>
        /// <remarks> This is an implementation of the Singleton design pattern in the Unity environment </remarks>
        public static UserInputManager Instance;

        /// <summary> The Canvas Element To Spawn A Panel If One Does Not Exist </summary>
        public Transform TBICanvas;

        [Tooltip("The UserInputPanel to Manage.")]
        public UserInputPanel currentPanel;

        [Tooltip("UserPanel Prefab.")]
        public GameObject UserPanelPrefab;

        void Awake()
        {
            // Get reference to Instance
            if (Instance != null)
            {
                Debug.LogError("UserInputManager already exists. Deleting this instance's gameobject.");
                Destroy(Instance.gameObject);
            }

            // Assign this object to the static Instance property
            Instance = this;
        }

        //UI Initialization, Create/set panel if it does not exist
        public void InitializeUI(InspectableManager inspectableManager)
        {
            //Check if we already have a panel
            if(currentPanel)
                return;

            //Make sure we can make a panel
            if (!UserPanelPrefab)
            {
                Debug.LogWarning("UserPanelPrefab : No UserPanelPrefab resource is found for UserInputManager");
                return;
            }

            //Create and set a panel
            currentPanel = CreatePanel();
        }


        /// <summary> Function that shows the content </summary>
        public void ShowPanel(TextBasedElement element)
        {
            if(currentPanel == null)
                return;
            currentPanel.ShowPanel();
        }

        /// <summary> Function that hides the content via unselected </summary>
        public void HidePanel(TextBasedElement element)
        {
            if(currentPanel == null)
                return;
            currentPanel.HidePanel();
        }

        /// <summary> Function that updates the content on mouse selected </summary>
        public void UpdateContents(TextBasedElement element)
        {
            if (currentPanel != null)
            {
                currentPanel.UpdateContent(element);
            }
        }

        /// <summary> Function that creates a panel if one does not exist on scene and set it to the canvas </summary>
        public UserInputPanel CreatePanel()
        {
            GameObject newDetailPanel = Instantiate(UserPanelPrefab);
            newDetailPanel.transform.SetParent(TBICanvas.transform);

            if (!newDetailPanel)
            {
                Debug.LogError("UserInputPanel : Instantiated UserInputPanel is null");
                return null;
            }

            return newDetailPanel.GetComponent<UserInputPanel>();
        }
    }
}