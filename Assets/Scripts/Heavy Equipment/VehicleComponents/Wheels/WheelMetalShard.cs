/*
* SUMMARY: Contains the WheelMetalShard class, used to prevent highlighting if the wheel is already selected.
*          Allowing the highlighting of both simultaneously results in a pink colour highlight applied to
*          the metal shard.
*/

using RemoteEducation.Interactions;
using UnityEngine;

namespace RemoteEducation.Modules.HeavyEquipment
{
    /// <summary>This class defines the individual metal shard and prevents them being highlighted while the wheel is.</summary>
    public sealed class WheelMetalShard : SemiSelectable
    {
        [HideInInspector] public Interactable WheelInteractable;

        /// <summary>Checks if the wheel is selected before it will try to highlight.</summary>
        protected override void MouseEnter()
        {
            if(WheelInteractable != null)
            {
                ToggleFlags(!WheelInteractable.IsSelected, Flags.Highlightable);
            }

            base.MouseEnter();
        }
    }
}