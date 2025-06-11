/*
* SUMMARY: File contains all logic related to hydraulic line component.
* Contains methods for toggling the leaking/ok states.
*/
using UnityEngine;
using RemoteEducation.Scenarios.Inspectable;
using RemoteEducation.Interactions;
using RemoteEducation.UI;
using RemoteEducation.Localization;

namespace RemoteEducation.Modules.HeavyEquipment
{
    /// <summary>HydraulicLine class allows for determining if hydraulic lines shown will be leaking or not.</summary>
    public sealed class HydraulicLine : SemiSelectable, IBreakable
    {
        public enum LineLocations
        {
            FrontLeft,
            FrontRight,
            FrontMain,
            RearLower,
            RearUpper
        }

        private bool isBroken = false;

        [SerializeField, Tooltip("The leaking decals for the hydraulic line.")]
        private GameObject LeakingDecals;

        [SerializeField, Tooltip("This location of this hydraulic line on the Backhoe.")]
        private LineLocations LineLocation;


        /// <summary>Allows the student to inspect a hydraulic line. </summary>
        /// <param name="inspectableElement">The element being inspected.</param>
        /// <param name="broken">If the hydraulic line is leaking.</param>
        /// <param name="badMode">what is broken, doesn't matter for this inspectable.</param>
        public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int badMode)
        {
            isBroken = broken;
            OnSelect += LineStatus;

            if (broken)
            {
                LeakingDecals.SetActive(true);
            }
        }

        public void LineStatus()
        {
            string lineInfo = "";
            if(isBroken)
            {
                lineInfo = Localizer.Localize("HeavyEquipment.HydraulicLinesToastLeaking");
            }
            else
            {
                lineInfo = Localizer.Localize("HeavyEquipment.HydraulicLinesToastGood");
            }

            //HEPrompts.CreateToast(lineInfo, HEPrompts.ShortToastDuration);
        }
    }
}