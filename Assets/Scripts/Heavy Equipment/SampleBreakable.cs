using UnityEngine;
using RemoteEducation.Scenarios.Inspectable;
using RemoteEducation.Interactions;

namespace RemoteEducation.Modules.HeavyEquipment
{

    /// <summary>
    ///     A prototype class for interfacing with the IBreakable interface 
    ///     and the DynamicInspectable set of classes.
    /// </summary>
    public class SampleBreakable : MonoBehaviour, IBreakable
    {
        InspectionPanel panel = null;
        DynamicInspectableElement context;

        #region Unity Callbacks


        // Inspection tooltip will be shown each frame that the mouse is over the Inspectable Element.
        public void OnMouseOver()
        {
            ShowInspectionTooltip();
        }

        // When the mouse leaves the Inspectable Element, the tooltip should be hidden, but only if the
        // Element hasn't been "selected"
        public void OnMouseExit()
        {
            if (context && panel != null && !context.IsSelected)
            {
                ReleaseControls();
            }
        }

        #endregion

        /// <summary>
        ///     Sets callback actions for showing and hiding the UI control panel
        ///     through the MouseInteractable component of the provided <paramref name="inspectableElement" />
        /// </summary>
        /// <param name="inspectableElement"></param>
        /// <param name="broken"></param>
        /// <param name="breakMode"></param>
        public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int breakMode)
        {
            context = inspectableElement;

            Debug.Log("The " + inspectableElement.FullName + " is " + (broken ? string.Empty : "not ") + "broken");


            // Set callbacks for Select and Unselect actions through the MouseInteractable class
            //
            // This will be further expanded in the update MouseInteractable implementation to better support
            // OnHover, OnSelect, OnUnselect actions, which will also replace the need for this class or other similarly
            // used classes to implement those checks in OnMouseEnter/OnMouseOver/OnMouseExit, since that can all be handled
            // and by the MouseInteractable
            inspectableElement.OnSelect += ShowInspectionControls;
            inspectableElement.OnDeselect += ReleaseControls;

            inspectableElement.OnInspect += () => { if (panel) { panel.UpdateContent(); } };
        }


        /// <summary>
        ///     Configures and displays a floating informational window with tooltip-like behaviour
        /// </summary>
        public void ShowInspectionTooltip()
        {
            // Attempt to get an available panel from the controller
            if (panel == null)
            {
                //panel = FindObjectOfType<InspectionPanelManager>().GetAvailable();

                // Display a warning and exit if there is no available panel for this element
                if (panel == null)
                {
                    Debug.LogWarning("ControlPanelController has no available panels");
                    return;
                }

                // Show info tooltip when not selected
                if (!context.IsSelected)
                {
                    panel.ShowPanel(context);
                }
            }
        }


        /// <summary>
        ///     Toggle the inspection UI from a tooltip to an interactive control panel.
        /// </summary>
        public void ShowInspectionControls()
        {
            if (panel)
            {
                panel.ExpandPanel();

                panel.ControlWindow.OnOff.AddListener(ReleaseSelection);
            }
        }

        /// <summary>
        ///     Hide the inspection UI and release references to GameObject resources
        /// </summary>
        public void ReleaseControls()
        {
            if (panel)
            {
                panel.ControlWindow.OnOff.RemoveListener(ReleaseSelection);

                // Free the currently held panel from the InspectionPanelManager for use with another element
                FindObjectOfType<InspectionPanelManager>().Release(panel);
            }

            // Set the panel context to null so that a new panel can be obtained when this
            // element is next focused
            panel = null;
        }

        /// <summary>
        ///     Indicate to the MouseInteractable system that the 
        ///     <see cref="context"/> should be unselected
        /// </summary>
        public void ReleaseSelection()
        {
            // Unselect the corresponding DynamicInspectable 
            if (context)
            {
                context.Select(false, Interactable.SelectionModes.Internal);
            }
        }
    }
}