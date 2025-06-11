/*
 * SUMMARY: This file contains the SecureOpenables class. The class can be used 
 *          whenever an openable object (door or window for exmaple) needs to have 
 *          a locking mechanism. 
 */

using System.Collections;
using System.Collections.Generic;
using RemoteEducation.Interactions;
using RemoteEducation.Scenarios.Inspectable;
using RemoteEducation.UI;
using UnityEngine;
using UnityEngine.Events;

namespace RemoteEducation.Modules.HeavyEquipment
{

    /// <summary>This class defines the basic functionality of a lock. </summary>
    /// <remarks>A public boolean is simply toggled when clicked unless broken</remarks>
    public sealed class SecureOpenables : SemiSelectable, IBreakable, IInitializable
    {
        [SerializeField] private UnityEvent lockEvent;

        /// <summary>Holds locked state</summary>
        private bool isLocked;

        /// <summary>Used to check if the lock is broken</summary>
        private bool isBroken;

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
            private set { }
        }

        public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int breakMode)
        {
            isBroken = broken;
            
            inspectableElement.OnCurrentlyInspectableStateChanged += (active) =>
            {
                //When interactable, don't show the highlight for this script. Also
                //make it so that it can be interacted with at any time.
                ToggleFlags(!active, Flags.Highlightable | Flags.ExclusiveInteraction);
            };
            
        }

        public void Initialize(object input = null)
        {

            isLocked = false;

            OnSelect += ToggleLock;
        }

        private void ToggleLock()
        {
            isLocked = !isLocked;
            lockEvent?.Invoke();
        }
    }
}
