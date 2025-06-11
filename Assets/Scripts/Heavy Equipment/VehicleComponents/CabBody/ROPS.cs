/*
* SUMMARY: File contains all logic related to the ROPS component.
* Contains methods for toggling the broken/ok state.
*/
using UnityEngine;
using RemoteEducation.Scenarios.Inspectable;
using RemoteEducation.Interactions;

namespace RemoteEducation.Modules.HeavyEquipment
{

    /// <summary>ROPS class allows for determining if the ROPS shown will have damage or not.</summary>
    public class ROPS : MonoBehaviour, IBreakable
    {
        [SerializeField, Tooltip("The damaged decals for the ROPS.")]
        private GameObject DamagedDecals;

        /// <summary>
        /// Allows the student to inspect the ROPS. 
        /// </summary>
        /// <param name="inspectableElement">The element being inspected.</param>
        /// <param name="broken">If the ROPS is broken.</param>
        /// <param name="badMode">Irrelevant for this inspectable.</param>
        public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int badMode)
        {
            if (broken)
            {
                DamagedDecals.SetActive(true);
            }
        }
    }
}
