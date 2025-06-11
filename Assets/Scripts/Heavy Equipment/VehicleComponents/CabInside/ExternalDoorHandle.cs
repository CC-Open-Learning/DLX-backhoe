using RemoteEducation.Interactions;
using RemoteEducation.Localization;
using RemoteEducation.UI.Tooltip;
using UnityEngine;

namespace RemoteEducation.Modules.HeavyEquipment
{
    /// <summary>The handle that opens the right and left backhoe cab doors.</summary>
    [DisallowMultipleComponent]
    public sealed class ExternalDoorHandle : SemiSelectable
    {
        [SerializeField, Tooltip("The cab door opened with this handle")]
        private CabDoor cabDoor;

        private SimpleTooltip tooltip;

        void Start()
        {
            tooltip = gameObject.AddComponent<SimpleTooltip>();
            tooltip.tooltipText = Localizer.Localize("HeavyEquipment.DoorHandleToolTip");
            OnClick += ToggleDoor;
        }

        void ToggleDoor()
        {
            cabDoor.ToggleDoorOpen();
        }
    }
}
