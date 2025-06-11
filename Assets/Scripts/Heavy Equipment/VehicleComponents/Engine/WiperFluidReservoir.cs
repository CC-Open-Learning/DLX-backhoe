/*
 * SUMMARY: The purpose of this file is to inspect the wiper fluid reservoir but it has a "fake" bad state so
 *          it will always only be in a good state, and therefore only needs a toast message for the good state.
 */

using RemoteEducation.Interactions;
using RemoteEducation.Localization;
using RemoteEducation.Scenarios.Inspectable;
using RemoteEducation.UI;

namespace RemoteEducation.Modules.HeavyEquipment
{
    public sealed class WiperFluidReservoir : SemiSelectable, IInitializable, IBreakable
    {
        public DynamicInspectableElement InspectableElement { get; private set; }

        public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int breakMode)
        {
            InspectableElement = inspectableElement;
        }

        public void Initialize(object input = null)
        {
            OnSelect += DisplayWiperFluidState;
        }

        private void DisplayWiperFluidState()
        {
            string shifterStateToast = Localizer.Localize("HeavyEquipment.WiperFluidFull");

            HEPrompts.CreateToast(shifterStateToast, HEPrompts.LongToastDuration);
        }
    }
}
