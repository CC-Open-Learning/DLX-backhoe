using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Lean.Gui;

namespace RemoteEducation.Scenarios.Inspectable
{
    public class DetailsPanelManager : MonoBehaviour, IInspectableUI
    {
        /// <summary> Ensures exactly one DetailsPanelManager exists in a Unity Scene </summary>
        /// <remarks> This is an implementation of the Singleton design pattern in the Unity environment </remarks>
        public static DetailsPanelManager Instance;

        [Tooltip("Window object which contains element controls.")]
        public GameObject DetailPanelResource;

        public Transform DPCanvas;

        [Tooltip("Details Panel, can be dynamically created or referenced from scene.")]
        public DetailsPanel currentPanel;
      
        void Awake()
        {
            // Get reference to Instance
            if (Instance != null)
            {
                Debug.LogError("DetailsPanelManager already exists. Deleting this instance's gameobject.");
                Destroy(Instance.gameObject);
            }

            // Assign this object to the static Instance property
            Instance = this;
        }

        //UI Initialization, set mouse selects and create/set panel if it does not exist
        public void InitializeUI(InspectableManager inspectableManager)
        {
            inspectableManager.OnElementSelected += UpdateContents;

            inspectableManager.OnElementDeselected += HidePanel;

            //Check if we already have a panel
            if(currentPanel)
                return;

            //Make sure we can make a panel
            if (!DetailPanelResource)
            {
                Debug.LogWarning("DetailPanelResource : No DetailPanel resource is found for DetailPanelManager");
                return;
            }

            //Create and set a panel
            currentPanel = CreatePanel();
        }

        
        /// <summary> Function that shows the content </summary>
        public void ShowPanel()
        {
            if(currentPanel == null)
                return;
            currentPanel.ShowPanel();
        }

        /// <summary> Function that hides the content via unselected </summary>
        public void HidePanel(InspectableElement element)
        {
            if(currentPanel == null)
                return;
            currentPanel.HidePanel();
            currentPanel.RemoveContext();
        }

        /// <summary> Function that updates the content on mouse selected </summary>
        public void UpdateContents(InspectableElement element)
        {
            if (currentPanel != null)
            {
                currentPanel.UpdateContent(element);
            }
        }

        
        /// <summary> Function that creates a panel if one does not exist on scene and set it to the canvas </summary>
        public DetailsPanel CreatePanel()
        {
            GameObject newDetailPanel = Instantiate(DetailPanelResource);
            newDetailPanel.transform.SetParent(DPCanvas.transform);

            if (!newDetailPanel)
            {
                Debug.LogError("InspectionPanelManager : Instantiated InspectionPanel is null");
                return null;
            }

            return newDetailPanel.GetComponent<DetailsPanel>();
        }
    }
}