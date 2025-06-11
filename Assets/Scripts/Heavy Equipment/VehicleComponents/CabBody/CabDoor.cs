/*
 * SUMMARY: This file contains the CabDoor class. The purpose this class is to open and close the CabDoor
 *          through an animation done with coroutines.
 */

using System.Collections;
using System.Collections.Generic;
using RemoteEducation.Interactions;
using RemoteEducation.Scenarios.Inspectable;
using UnityEngine;
using RemoteEducation.Scenarios;
using RemoteEducation.Audio;

namespace RemoteEducation.Modules.HeavyEquipment
{
    /// <summary>This class defines the basic functionality of the cab door. </summary>
    /// <remarks>A cab door can be open and closed, which is done through animations.
    /// This class defines those animations.</remarks>
    [DisallowMultipleComponent]
    public sealed class CabDoor : SemiSelectable, IBreakable, IInitializable, ITaskable
    {
        public enum DoorPositions
        {
            Right,
            Left
        }

        [Tooltip("Bad/Damaged door decals")]
        [SerializeField] private GameObject DamagedDecals;

        [Tooltip("The handle that opens and closes the door from the inside")]
        [SerializeField] private DoorHandle innerDoorHandle;

        [Tooltip("The handle that opens and closes the door from the outside")]
        [SerializeField] private ExternalDoorHandle outerDoorHandle;

        [Tooltip("The position of the door when it is opened")]
        [SerializeField] private Transform OpenTransform;

        [Tooltip("Determines the length and speed of the animation")]
        [SerializeField] private float animationTime = 2f;

        [SerializeField] private DoorPositions position;

        /// <summary>The rotation of the door when it is not closed</summary>
        private Quaternion ClosedRotation;

        /// <summary>The rotation of the door when it is opened</summary>
        private Quaternion OpenRotation;

        /// <summary>Stores the animation which is being executed</summary>
        private Coroutine activeAnimation;

        public TaskableObject Taskable { get; private set; }

        [Tooltip("Close Door Sound.")]
        [SerializeField] private SoundEffect closeSoundEffect;

        [Tooltip("Open Door Sound.")]
        [SerializeField] private SoundEffect openSoundEffect;

        private BackhoeController backhoe;

        public DynamicInspectableElement InspectableElement { get; private set; }

        public enum ScenarioPass
        {
            None,
            Stage3,
            Stage5,
            Stage21,
            Stage21End,
            Stage23,
            StageFuelTank
        }

        public ScenarioPass currentIteration = ScenarioPass.None;
        public bool CanClear = false;
        private int isOpen = 0;

        public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int breakMode)
        {
            InspectableElement = inspectableElement;

            SetDoorMesh(broken);

            inspectableElement.OnCurrentlyInspectableStateChanged += (active) =>
            {
                //When interactable, don't show the highlight for this script. Also
                //make it so that it can be interacted with at any time.
                ToggleFlags(!active, Flags.Highlightable | Flags.ExclusiveInteraction);
            };
        }

        public void SetDoorInteractable(bool canClick) 
        {
            innerDoorHandle.GetComponent<BoxCollider>().enabled = canClick;
            outerDoorHandle.GetComponent<BoxCollider>().enabled = canClick;
        }

        public void Initialize(object input = null)
        {
            //Assumes that door starts off in a closed position
            ClosedRotation = transform.localRotation;
            OpenRotation = OpenTransform.localRotation;
            Taskable = new TaskableObject(this, position.ToString());
            backhoe = input as BackhoeController;
        }

        private void SetDoorMesh(bool broken)
        {
            if (broken)
            {
                DamagedDecals.SetActive(true);
            }
        }

        /// <summary>Calls coroutines to animate.</summary>
        /// <remarks>Door can only be opened or closed, in between interaction is ignored</remarks>
        public void ToggleDoorOpen()
        {

            var open = transform.localRotation == ClosedRotation;

            if (activeAnimation != null)
            {
                return;
            }

            activeAnimation = StartCoroutine(OpenDoor(open));

        }

        public void CloseDoorAfterSeconds(float seconds)
        {
            if (transform.localRotation == ClosedRotation) return;
            StartCoroutine(IECloseDoorAfterSeconds(seconds));
        }

        private IEnumerator IECloseDoorAfterSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            StartCoroutine(OpenDoor(false));
        }

        public bool CheckIfOpen()
        {
            return transform.localRotation != ClosedRotation;
        }

        /// <summary>Calls coroutines to animate.</summary>
        /// <remarks>Door can only be opened or closed, in between interaction is ignored</remarks>
        /// <param name="open">Choses which rotation the target should change to </param>
        /// <returns>A notice to the coroutine to wait until the frame has loaded completely before continuing the loop.</returns>
        private IEnumerator OpenDoor(bool open)
        {
            float startTime = Time.time;
            float endTime = startTime + animationTime;
            float totalTime = endTime - startTime;

            Quaternion targetRotation = open ? OpenRotation : ClosedRotation;

            if (open)
            {
                openSoundEffect.Play();
            }
            else
            {
                closeSoundEffect.Play();
            }

            while (Time.time <= endTime)
            {
                float timeRatio = (Time.time - startTime) / totalTime;

                transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, timeRatio);

                //Leave as soon as target is reached
                if (transform.localRotation == targetRotation)
                {
                    break;
                }

                yield return null;
            }

            transform.localRotation = targetRotation; //Ensures that target is reached after time ends
            activeAnimation = null;

            CanClear = true;
            isOpen = open ? 1 : 0;
            Taskable.PokeTaskManager();
        }

        public object CheckTask(int checkType, object inputData)
        {
            switch (checkType)
            {
                case (int)TaskVertexManager.CheckTypes.Int:
                    return isOpen;

                case (int)TaskVertexManager.CheckTypes.Bool:
                    if (currentIteration == ScenarioPass.Stage3 ||
                        currentIteration == ScenarioPass.Stage5 ||
                        currentIteration == ScenarioPass.Stage21 ||
                        currentIteration == ScenarioPass.Stage21End ||
                        currentIteration == ScenarioPass.Stage23)
                    {
                        return CanClear;
                    }

                    if (currentIteration == ScenarioPass.StageFuelTank)
                    {
                        if (backhoe.FuelTank.FuelGaugeCheck())
                        {
                            return true;
                        }
                        else
                        {
                            return CanClear;
                        }
                    }
                    break;

                default:
                    Debug.LogError($"Invalid check type passed into {GetType().Name}");
                    break;
            }

            return null;
        }
    }
}