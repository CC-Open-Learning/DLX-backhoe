using System.Collections;
using System.Collections.Generic;
using RemoteEducation.Interactions;
using RemoteEducation.Scenarios.Inspectable;
using RemoteEducation.Scenarios;
using UnityEngine;
using RemoteEducation.UI;
using RemoteEducation.Localization;

namespace RemoteEducation.Modules.HeavyEquipment
{
    public class SteerLockInspectable : SemiSelectable, IBreakable
    {
        /// <summary>Value to determine if object is broken</summary>
        [SerializeField, Tooltip("If the lock is broken.")]
        private bool isBroken = false;

        public DynamicInspectableElement InspectableElement;

        [SerializeField, Tooltip("The Steering Column Lock")]
        private SteerColumnLock steerLock;

        /// <summary>Make object inspectable for damages</summary>
        public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int breakMode)
        {
            //Setting it broken
            SetBrokenStatus(broken);
            InspectableElement = inspectableElement;
            inspectableElement.OnCurrentlyInspectableStateChanged += (active) =>
            {
                // When interactable, don't show the highlight for this script. 
                // Also make it so that it can be interacted with at any time.
                ToggleFlags(!active, Flags.Highlightable | Flags.ExclusiveInteraction);
            };

            OnSelect += ToggleLockAnimation;
        }

        public void ToggleLockAnimation()
        {
            steerLock.ToggleLock();
        }

        /// <summary>Sets the object as broken and thus not toggleable</summary>
        public void SetBrokenStatus(bool setState)
        {
            isBroken = setState;
        }

        public bool GetBrokenStatus()
        {
            return isBroken;
        }
    }
}