/*
 * SUMMARY: The purpose of this file is to inspect the gear shifter but it has a "fake" bad state so it
 *          will always only be in a good state, and therefore only needs a toast message for the good state.
 */



using RemoteEducation.Scenarios.Inspectable;
using RemoteEducation.Interactions;
using RemoteEducation.Localization;
using RemoteEducation.UI;
using RemoteEducation.Modules.HeavyEquipment;
using UnityEngine;

public sealed class GearShifter : SemiSelectable, IInitializable, IBreakable
{
    public DynamicInspectableElement InspectableElement { get; private set; }

    public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int breakMode)
    {
        InspectableElement = inspectableElement;

    }
    
    public void Initialize(object input = null)
    {
        OnSelect += DisplayShifterState;
    }

    private void DisplayShifterState()
    {
        string shifterStateToast = Localizer.Localize("HeavyEquipment.GearShifterFunctional");
        HEPrompts.CreateToast(shifterStateToast, HEPrompts.LongToastDuration);
    }
}
