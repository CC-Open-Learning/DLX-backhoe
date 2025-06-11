using RemoteEducation.Scenarios.Inspectable;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RemoteEducation.Interactions;

namespace RemoteEducation.Modules.HeavyEquipment
{
    /// <summary>
    /// Generates a group of elements that is needed for a specific part of the scenario.
    /// This class can be a component on a gameobject in the scene.
    /// The <see cref="componentLocations"/> should be filled with the components that 
    /// need to be part of this group.
    /// The <see cref="states"/> should be filled with the states that the user needs to check in 
    /// this group. 
    /// </summary>
    public class InspectableGroup : MonoBehaviour
    {
        [SerializeField, Tooltip("The location of the objects in this group")]
        private List<ElementSpawningData> componentLocations;

        [SerializeField, Tooltip("The states that you want to check for")]
        private List<InspectableElement> states;

        /// <summary>The <see cref="InspectableElement"/> that are in this group.</summary>
        public List<InspectableElement> GroupElements => (from location in componentLocations select location.GetComponent<InspectableElement>()).ToList();

        /// <summary>
        /// Generate a list of elements that are broken in a state from <see cref="states"/>
        /// </summary>
        public List<InspectableElement> GetElementsToCheck()
        {
            //get the elements that were added to the locations at startup
            List<InspectableElement> elements = GroupElements;

            //sort the elements by controllers
            Dictionary<InspectableController, List<InspectableElement>> elementsByController = new Dictionary<InspectableController, List<InspectableElement>>();

            foreach (InspectableElement element in elements)
            {
                if (!elementsByController.ContainsKey(element.ParentController))
                {
                    elementsByController.Add(element.ParentController, new List<InspectableElement>());
                }

                elementsByController[element.ParentController].Add(element);
            }

            //remove all the elements that are not needed by the group
            foreach (InspectableController controller in elementsByController.Keys)
            {
                //get the identifiers from this controller uses that are in the states specified for the group.
                List<int> stateIdentifiers = (from state in states where controller.ElementStates.Contains(state) select controller.IdentifiersByElement[state]).ToList();

                //get the elements for this controller that don't have a needed identifier
                List<InspectableElement> toRemove = (from element in elementsByController[controller] where !stateIdentifiers.Contains(element.StateIdentifier) select element).ToList();

                //remove them from the list of elements that need to be inspected
                elements.RemoveAll(x => toRemove.Contains(x));
            }

            return elements;
        }

        public void ToggleGroupInteraction(bool state)
        {
            foreach(InspectableElement temp in GroupElements)
            {
                temp.GetComponent<Interactable>().ToggleFlags(state, Interactable.Flags.InteractionsDisabled);
            }
        }
    }
}
