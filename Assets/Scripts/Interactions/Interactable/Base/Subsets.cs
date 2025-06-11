using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RemoteEducation.Interactions
{
    partial class Interactable
    {
        /// <summary>Resets the <see cref="subsetActiveObjects"/> collection to <see langword="null"/> and checks if any <see cref="Interactable"/> is under the pointer for mouse hover logic.</summary>
        public static void ResetSubsetActivatedObjects()
        {
            subsetActiveObjects = null;
            PointerUpCheck();
        }

        /// <summary>Adds one or multiple <see cref="Interactable"></see> objects to the list of active objects the user may interact with.</summary>
        /// <remarks> If there was no subset of activated objects set up, then it will be created here. This will deactivate all <see cref="Interactable"/> objects other than the ones passed in.</remarks>
        /// <param name="activeObjects">The <see cref="Interactable"/>(s) that should be part of the active subset.</param>
        public static void AddSubsetActivated(params Interactable[] activeObjects)
        {
            AddSubsetActivated(activeObjects.ToList());
        }

        /// <summary>Adds one or multiple <see cref="Interactable"></see> objects to the list of active objects the user may interact with.</summary>
        /// <remarks> If there was no subset of activated objects set up, then it will be created here. This will deactivate all <see cref="Interactable"/> objects other than the ones passed in.</remarks>
        /// <param name="activeObjects">The List of <see cref="Interactable"/> objects that should become part of the active subset.</param>
        public static void AddSubsetActivated(List<Interactable> activeObjects)
        {
            SetSubsetActivated(subsetActiveObjects == null ? activeObjects : subsetActiveObjects.Union(activeObjects).ToList());
        }

        /// <summary>Removes one or multiple <see cref="Interactable"></see> objects from the list of active objects the user may interact with. </summary>
        /// <remarks>This will deactivate the object. If all the objects in the list are removed, then everything will be deactivated. To enable everything, use <see cref="ResetSubsetActivatedObjects"/>.</remarks>
        /// <param name="inactiveObjects">The objects to remove from the subset of active objects.</param>
        public static void RemoveSubsetActivated(params Interactable[] inactiveObjects)
        {
            RemoveSubsetActivated(inactiveObjects.ToList());
        }

        /// <summary>Removes one or multiple <see cref="Interactable"></see> objects from the list of active objects the user may interact with. </summary>
        /// <remarks>This will deactivate the object. If all the objects in the list are removed, then everything will be deactivated. To enable everything, use <see cref="ResetSubsetActivatedObjects"/>.</remarks>
        /// <param name="inactiveObjects">The objects to remove from the subset of active objects.</param>
        public static void RemoveSubsetActivated(List<Interactable> inactiveObjects)
        {
            if (subsetActiveObjects == null)
            {
                return;
            }

            SetSubsetActivated(subsetActiveObjects.Except(inactiveObjects).ToList());
        }

        /// <summary>Set the <see cref="Interactable"/> objects that are in the subset of active objects.</summary>
        /// <remarks>This will override anything that was previously in the list.
        /// If the were no objects in the list, the list will be created here.
        /// This will deactivate all objects other than ones passed into this method.
        /// If no objects are passed in, all objects will be deactivated.</remarks>
        /// <param name="activeObjects">The only objects that should be active.</param>
        public static void SetSubsetActivated(params Interactable[] activeObjects)
        {
            SetSubsetActivated(activeObjects.ToList());
        }

        /// <summary>Set the <see cref="Interactable"/> objects that are in the subset of active objects.</summary>
        /// <remarks>This will override anything that was previously in the list.
        /// If the were no objects in the list, the list will be created here.
        /// This will deactivate all objects other than ones passed into this method.
        /// If no objects are passed in, all objects will be deactivated.</remarks>
        /// <param name="activeObjects">The only objects that should be active.</param>
        public static void SetSubsetActivated(List<Interactable> activeObjects)
        {
            if(activeObjects == null || activeObjects.Contains(null))
            {
                Debug.LogError("Interactable: The active objects can not be set to null.");
                return;
            }

            subsetActiveObjects = new List<Interactable>();
            subsetActiveObjects.AddRange(activeObjects);

            DeactivateActiveObjects(activeObjects);

            PointerUpCheck();
        }

        /// <summary>If there were any objects that were selected or highlighted, deactivate them.</summary>
        /// <remarks>This method is only intended to be used when updating the subset of active objects.
        /// It has not been tested in any other use cases.</remarks>
        /// <param name="activeExceptions">The list of objects that can remain activated.</param>
        private static void DeactivateActiveObjects(List<Interactable> activeExceptions)
        {
            List<Interactable> objectsToDeactivate = (from obj in CurrentSelections where !activeExceptions.Contains(obj) select obj).ToList();

            if(HoveringOverInteractable != null && !activeExceptions.Contains(HoveringOverInteractable))
            {
                objectsToDeactivate.Add(HoveringOverInteractable);
            }

            foreach (Interactable interactable in objectsToDeactivate)
            {
                interactable.OnDisable();
            }
        }
    }
}
