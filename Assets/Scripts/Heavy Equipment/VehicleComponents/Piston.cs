/*
* SUMMARY: File contains all logic related to piston component.
* Contains methods for toggling the broken/ok states.
*/
using UnityEngine;
using RemoteEducation.Scenarios.Inspectable;
using RemoteEducation.Interactions;
using RemoteEducation.UI;
using RemoteEducation.Localization;

namespace RemoteEducation.Modules.HeavyEquipment
{
    /// <summary>Piston class allows for determining if any piston shown will be broken or not.</summary>
    public sealed class Piston : SemiSelectable, IBreakable
    {
        public enum PistonLocations
        {
            BucketLeft,
            BucketRight,
            BucketMain,
            ArmLower,
            ArmUpper,
            LeftStab,
            RightStab,
            RearBucket,
            SwingLeft,
            SwingRight
        }
        
        private enum PistonStates
        {
            Damaged,
            Leaking
        }

        [SerializeField, Tooltip("The damaged piston decals.")]
        private GameObject DamagedDecals;

        [SerializeField, Tooltip("The leaking piston decals.")]
        private GameObject LeakingDecals;

        [SerializeField, Tooltip("This location of this piston on the Backhoe.")]
        private PistonLocations PistonLocation;

        public DynamicInspectableElement InspectableElement { get; private set; }

        private PistonStates damageState;

        private bool isBroken = false;

        /// <summary>
        /// Allows the student to inspect a piston. 
        /// </summary>
        /// <param name="inspectableElement">The element being inspected.</param>
        /// <param name="broken">If the piston is broken.</param>
        /// <param name="badMode">what is broken, 0 for damage, 1 for leaking.</param>
        public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int badMode)
        {
            InspectableElement = inspectableElement;
            isBroken = broken;
            OnSelect += PistonStatus;

            if (!broken)
            {
                return;
            }

            switch ((PistonStates)badMode)
            {
                case PistonStates.Damaged:
                    DamagedDecals.SetActive(true);
                    damageState = PistonStates.Damaged;
                    break;

                case PistonStates.Leaking:
                    LeakingDecals.SetActive(true);
                    damageState = PistonStates.Leaking;
                    break;
            }
        }

        public void PistonStatus()
        {
            string pistonInfo = "";

            if(!isBroken)
            {
                pistonInfo = Localizer.Localize("HeavyEquipment.PistonsToastGood");
            }
            else
            {
                switch ((PistonStates)damageState)
                {
                    case PistonStates.Damaged:
                        pistonInfo = Localizer.Localize("HeavyEquipment.PistonsToastDamaged");
                        break;

                    case PistonStates.Leaking:
                        pistonInfo = Localizer.Localize("HeavyEquipment.PistonsToastLeaking");
                        break;
                }
            }
            //HEPrompts.CreateToast(pistonInfo, HEPrompts.ShortToastDuration);
        }
    }
}