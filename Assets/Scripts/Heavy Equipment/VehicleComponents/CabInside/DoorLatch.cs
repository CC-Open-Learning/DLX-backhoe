using RemoteEducation.Interactions;
using RemoteEducation.Scenarios.Inspectable;
using RemoteEducation.UI;
using UnityEngine;

namespace RemoteEducation.Modules.HeavyEquipment
{
    /// <summary>The latch that locks/unlocks the cab doors from the inside.</summary>
    [DisallowMultipleComponent]
    public sealed class DoorLatch : SemiSelectable, IBreakable, IInitializable
    {
        /// <summary>Holds locked state.</summary>
        private bool isLocked;

        /// <summary>Used to check if the lock is broken</summary>
        private bool isBroken;

        /// <summary>If this <see cref="DoorLatch"/> is locked or not.</summary>
        public bool IsLocked
        {
            get
            {
                if (isBroken)
                {
                    return false;
                }

                return isLocked;
            }
            set
            {
                isLocked = value;
            }
        }

        public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int breakMode)
        {
            isBroken = broken;
        }

        public void Initialize(object input = null)
        {
            isLocked = false;
            OnSelect += ToggleLock;
        }

        private void ToggleLock()
        {
            isLocked = !isLocked;
            string toastString = Localization.Localizer.Localize(isLocked ? "HeavyEquipment.LatchToastLocked" : "HeavyEquipment.LatchToastUnlocked");
            HEPrompts.CreateToast(toastString, HEPrompts.ShortToastDuration);
        }
    }
}
