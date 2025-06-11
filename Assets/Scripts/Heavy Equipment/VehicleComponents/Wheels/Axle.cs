/*
* SUMMARY: Contains the Axle class, used to attach the inspectable and set the wheel axles
*          to either good or bad.
*/

using UnityEngine;
using RemoteEducation.Scenarios.Inspectable;

namespace RemoteEducation.Modules.HeavyEquipment
{
    /// <summary>This class sets the wheel axle to good or bad</summary>
    public class Axle : MonoBehaviour, IBreakable
    {
        [Tooltip("The damage decals.")]
        public GameObject BadAxle;

        public DynamicInspectableElement InspectableElement { get; private set; }

        /// <summary>
        /// Allows the student to inspect the axle. 
        /// </summary>
        /// <param name="inspectableElement">The element being inspected.</param>
        /// <param name="broken">If the axle is broken.</param>
        /// <param name="breakMode">Unused.</param>
        public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int breakMode)
        {
            SetAxleState(broken);
            InspectableElement = inspectableElement;
        }

        /// <summary>
        /// Sets the wheel axle on the backhoe. 
        /// </summary>
        /// <param name="broken">If the axle is broken or not.</param>
        public void SetAxleState(bool broken)
        {
            BadAxle.SetActive(broken);
        }
    }
}