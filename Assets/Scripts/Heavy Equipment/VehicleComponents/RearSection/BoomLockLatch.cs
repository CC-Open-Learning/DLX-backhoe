/*
 * SUMMARY: The purpose of this class is to model the behaviour of the Boom Lock Latch itself by the swing
 *          linkage area. When the control inside the cab is toggled, the state of the lock changes, and 
 *          locks, or unlocks the boom arm on the backhoe. If the lock is in the broken state, then the lock
 *          will not move. Additionally, the lock will display it's state with a toast message OnSelect()
 */



using RemoteEducation.Interactions;
using RemoteEducation.Localization;
using RemoteEducation.Scenarios;
using RemoteEducation.Scenarios.Inspectable;
using RemoteEducation.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RemoteEducation.Modules.HeavyEquipment
{

    public class BoomLockLatch : SemiSelectable, IInitializable, IBreakable, ITaskable
    {
        [SerializeField, Tooltip("Pivot point which the object will pivot around")]
        private Transform pivotPoint;

        /// <summary>The duration of the animation</summary>
        private float animationDuration = 1f;

        /// <summary>Angle that the object rotates each iteration of the while loop during the animation</summary>
        private float angle = -60f;

        /// <summary>If the latch is locked. Defaulted to locked.</summary>
        private bool isLocked = true;

        private bool isAnimating = false;

        private bool isBroken;

        public DynamicInspectableElement InspectableElement { get; private set; }

        public TaskableObject Taskable { get; private set; }

        /// <summary>State of the boom lock.</summary>
        private enum lockState
        {
            unlockLatch = -1,
            lockLatch = 1
        }

        public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int breakMode)
        {
            InspectableElement = inspectableElement;
            isBroken = broken;

            //Make it so the student can see the toast message if they click ont he lock again without selecting something else
            inspectableElement.OnCurrentlyInspectableStateChanged += (active) =>
            {
                ToggleFlags(!active, Flags.Highlightable | Flags.ExclusiveInteraction);
            };
        }

        public void Initialize(object input = null)
        {
            Taskable = new TaskableObject(this);
        }

        /// <summary>Toggles the lock from locked to unlocked and unlocked to locked if it is not broken</summary>
        public void ToggleLock()
        {
            if (!isBroken && !isAnimating)
            {
                isAnimating = true;
                StartCoroutine(BoomLockLatchAnimation());
                isLocked = !isLocked;
            }
        }

        /// <summary>Creates an animation of a boom lock latch rotating around a pivot point back and forth on click depending on lock state</summary>
        private IEnumerator BoomLockLatchAnimation()
        {
            //Start timer and end timer for our animation
            float startTimer = Time.time;
            float endTimer = startTimer + animationDuration;

            //-1 if latch is locked (true), 1 if the latch is unlocked (false)
            int direction = isLocked ? (int)lockState.unlockLatch : (int)lockState.lockLatch;

            //While we are animating, animate the object
            while (Time.time <= endTimer)
            {
                transform.RotateAround(pivotPoint.position, Vector3.right, Time.deltaTime * direction * angle);
                yield return null;
            }

            isAnimating = false;
            Taskable.PokeTaskManager();
        }

        public object CheckTask(int checkType, object inputData)
        {
            switch (checkType)
            {
                case (int)TaskVertexManager.CheckTypes.Bool:
                    return isLocked;

                default:
                    Debug.LogError("Invalid check type passed into Boom Lock Latch");
                    break;
            }

            return null;
        }
    }
}