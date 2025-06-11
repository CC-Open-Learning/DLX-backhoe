using UnityEngine;
using Lean.Gui;
using System.Collections.Generic;
using System.Linq;

namespace RemoteEducation.Scenarios.Inspectable
{
    /// <summary>
    ///     Provides multiple <see cref="InspectionPanel"/> resources 
    ///     for an Inspection Scenario. 
    /// </summary>
    /// <remarks>
    ///     Multiple <see cref="InspectionPanel"/> objects are created and
    ///     held in a buffer. An object in an Inspection Scenario can obtain 
    ///     a reference to a single <see cref="InspectionPanel"/> by calling 
    ///     <see cref="GetAvailable"/> to display information and receive user input. 
    ///     The <see cref="InspectionPanel"/> is then no longer "available" and can only be 
    ///     used by that Inspection Scenario object. <para />
    ///     
    ///     When the Inspection Scenario object is finished displaying UI, 
    ///     it should call <see cref="Release(InspectionPanel)"/> to indicate 
    ///     that the <see cref="InspectionPanel"/> may be used by another object.
    /// </remarks>
    public class InspectionPanelManager : MonoBehaviour, IInspectableUI
    {
        #region Fields

        [Tooltip("Window object which contains element controls.")]
        public GameObject InspectionPanelResource;

        [Tooltip("Number of panels to be created in the circular buffer")]
        public byte PanelBufferSize = 2;

        /// <summary>
        /// The <see cref="InspectionPanel"/>s that are used by this class.
        /// The <see cref="InspectableElement"/> is the 
        /// </summary>
        private Dictionary<InspectionPanel, InspectableElement> panels;

        private List<InspectionPanel> ActivePanels => (from pair in panels where pair.Value != null select pair.Key).ToList();

        #endregion

        public void InitializeUI(InspectableManager inspectableManager)
        {
            if (!InspectionPanelResource)
            {
                Debug.LogWarning("InspectionPanelManager : No InspectionPanel resource provided for InspectionPanelManager");
                return;
            }

            inspectableManager.OnElementMouseEnter += AttachPanelToElement;
            inspectableManager.OnElementSelected += AttachPanelToElement;
            inspectableManager.OnElementSelected += ExpandPanel;

            inspectableManager.OnElementClicked += ExpandPanel;

            inspectableManager.OnElementMouseExit += DetachedPanelFromElement;
            inspectableManager.OnElementDeselected += DetachedPanelFromElement;

            inspectableManager.OnElementStateChanged += UpdatePanelContent;

            inspectableManager.OnEvaluationCompleted += UpdatePanelStateIcons;

            inspectableManager.OnActiveElementsChanged += (_) => CheckIfPanelsAreOnActiveElements();


            // Instantiate 'Panels' buffer and create resources
            panels = new Dictionary<InspectionPanel, InspectableElement>();

            for (byte i = 0; i < PanelBufferSize; i++)
            {
                panels.Add(CreatePanel(), null);
            }
        }

        #region InspectableManager Event Handlers 

        /// <summary>
        /// Assign a panel to an <see cref="InspectableElement"/>.
        /// The panel will start closed. 
        /// </summary>
        /// <remarks>This method should handle the <see cref="inspectableManager.OnElementMouseEnter"/> event.</remarks>
        /// <param name="element">The element to attach the panel to</param>
        private void AttachPanelToElement(InspectableElement element)
        {
            if(ElementHasPanel(element))
            {
                return;
            }

            InspectionPanel panel = GetAvailable(element);

            if(panel == null)
            {
                Debug.LogError("Unable to get a panel for " + element.gameObject.name);
                return;
            }

            panel.ShowPanel(element);
        }

        /// <summary>
        /// Try to expand a panel that is the given element.
        /// If there is no panel for that element, nothing will happen.
        /// </summary>
        /// <remarks>This method should handle the <see cref="inspectableManager.OnElementSelected"/> event.</remarks>
        /// <param name="element">The element that want to expand its panel.</param>
        private void ExpandPanel(InspectableElement element)
        {
            InspectionPanel panel = GetPanelByElement(element);

            if (panel == null)
            {
                Debug.Log("no panel");

                //There was no panel attached to this element. This could be because the element was selected 
                // by something that wasn't the mouse.
                return;
            }

            panel.ExpandPanel();
        }

        /// <summary>
        /// Detached a panel from an element. The panel will only detach 
        /// if the element is not selected.
        /// The panel will be setup so that it is 
        /// ready to attach to a new <see cref="InspectableElement"/>.
        /// </summary>
        /// <remarks>
        /// This method should handle the events: inspectableManager.OnElementMouseExit and inspectableManager.OnElementDeselected.
        /// </remarks>
        /// <param name="element">The element that should be detached from.</param>
        private void DetachedPanelFromElement(InspectableElement element)
        {
            InspectionPanel panel = GetPanelByElement(element);

            if(panel == null || element.IsSelected)
            {
                return;
            }

            Release(panel);
        }

        /// <summary>
        /// Update the content on the panel attached to: <paramref name="element"/>
        /// </summary>
        /// <remarks>This method should handle the inspectableManager.OnElementStateChanged event</remarks>
        private void UpdatePanelContent(InspectableElement element)
        {
            InspectionPanel panel = GetPanelByElement(element);

            if(panel != null)
            {
                panel.UpdateContent();
            }
        }

        /// <summary>
        /// Update the status icons for <see cref="InspectionPanel"/>s that are attached to the <see cref="InspectableElement"/>s.
        /// </summary>
        /// <remarks>This method should handle the inspectableManager.OnEvaluationCompleted event.</remarks>
        private void UpdatePanelStateIcons()
        {
            ActivePanels.ForEach(x => x.UpdateStatusIcon());
        }

        /// <summary>
        /// Check if the active panels are on elements that are able to be inspected.
        /// </summary>
        /// <remarks>This method should handle the inspectableManager.OnActiveElementsChanged event.</remarks>
        private void CheckIfPanelsAreOnActiveElements()
        {
            List<InspectionPanel> activePanels = ActivePanels;

            for(int i = 0; i < activePanels.Count; i++)
            {
                if(!panels[activePanels[i]].CurrentlyInspectable)
                {
                    Release(activePanels[0]);
                }
            }
        }

        #endregion

        #region Panel Management

        /// <summary>
        /// Returns if there is a <see cref="InspectionPanel"/> for the given <see cref="InspectableElement"/>
        /// </summary>
        private bool ElementHasPanel(InspectableElement element)
        {
            return panels.Values.Contains(element);
        }

        /// <summary>
        ///     Retrieves an available <see cref="InspectionPanel"/> from the 
        ///     <see cref="panels"/> buffer and marks it as unavailable.
        /// </summary>
        /// <remarks>
        ///     The calling method should check for a <c>null</c> return value,
        ///     indicating that there are no available panels in the buffer.
        /// </remarks>
        /// <returns>
        ///     An <see cref="InspectionPanel"/> ready for use, or <c>null</c> 
        ///     if none are available.
        /// </returns>
        public InspectionPanel GetAvailable(InspectableElement element)
        {
            foreach (KeyValuePair<InspectionPanel, InspectableElement> pair in panels)
            {
                // If the 'Value' field is true, the panel is available for use
                if (pair.Value == null)
                {
                    // Update the dictionary with the same 'Key', but with the 'Value' set to 'false'
                    panels.Remove(pair.Key);
                    panels.Add(pair.Key, element);

                    return pair.Key;
                }
            }

            return null;
        }

        /// <summary>
        ///     Marks the specified <see cref="InspectionPanel"/> as available for use.
        /// </summary>
        /// <param name="panel">
        ///     An <see cref="InspectionPanel"/> which is a <c>Key</c> in the 
        ///     <see cref="panels"/> Dictionary
        /// </param>
        public void Release(InspectionPanel panel)
        {
            panel.HidePanel();

            panels.Remove(panel);
            panels.Add(panel, null);
        }

        /// <summary>
        ///     Instantiates an <see cref="InspectionPanel"/> GameObject and adds it
        ///     as a child window of the static <see cref="UI.WindowManager"/>.
        /// </summary>
        /// <returns>
        ///     The newly created <see cref="InspectionPanel"/>, or <c>null</c> if the
        ///     <see cref="InspectionPanelResource"/> field has not been assigned.
        /// </returns>
        public InspectionPanel CreatePanel()
        {
            GameObject resource = Instantiate(InspectionPanelResource);

            if (!resource)
            {
                Debug.LogError("InspectionPanelManager : Instantiated InspectionPanel is null");
                return null;
            }
            
            // Add the InspectionPanel, which has a LeanWindow component, to the static 
            // WindowManager upon creation. This ensures it uses the same canvas settings 
            // as all other LeanWindows in the application
            ScenarioManager.Instance.WindowManager.Add(resource.GetComponent<LeanWindow>());

            return resource.GetComponent<InspectionPanel>();
        }

        /// <summary>
        /// Gets a 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public InspectionPanel GetPanelByElement(InspectableElement element)
        {
            return panels.Keys.FirstOrDefault(x => panels[x] == element);
        }

        #endregion
    }
}