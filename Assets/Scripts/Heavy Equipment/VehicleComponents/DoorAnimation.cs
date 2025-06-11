/*
 * SUMMARY: This file contains the Door Animation class. When the latch is pressed,
 *          the "StartAnimation()" method is invoked and plays through this animation.
 */


using System.Collections;
using System.Collections.Generic;
using RemoteEducation.Interactions;
using RemoteEducation.Scenarios.Inspectable;
using UnityEngine;

namespace RemoteEducation.Modules.HeavyEquipment
{
    public class DoorAnimation : MonoBehaviour, IInitializable
    {
        [SerializeField, Tooltip("Length of animation")] private float animationLength;

        [SerializeField, Tooltip("Window associated with the latch")] private Transform Door;
        [SerializeField, Tooltip("The cab door script associated with the latch")] private CabDoor cabDoor;

        [SerializeField, Tooltip("Opened transform of the door")] private Transform DoorOpened;
        [SerializeField, Tooltip("Closed transform of the door")] private Transform DoorClosed;

        /// <summary>Rotation of the window when closed</summary>
        private Quaternion doorClosedRotation;

        /// <summary>Rotation of the window when opened</summary>
        private Quaternion doorOpenedRotation;

        /// <summary>Current animation being played</summary>
        private Coroutine currentAnimation;

        bool doorOpen = false;

        public void Initialize(object input = null)
        {
            doorClosedRotation = DoorClosed.localRotation;
            doorOpenedRotation = DoorOpened.localRotation;
        }

        /// <summary>Starts the door animation</summary>
        public void StartAnimation()
        {
            if (currentAnimation != null)
            {
                StopCoroutine(currentAnimation);
            }

            currentAnimation = StartCoroutine(doorOpen ? DoorClose() : DoorOpen());
            doorOpen = !doorOpen;

            //cabDoor.CanClear = true;
            //cabDoor.Taskable.PokeTaskManager();
        }

        /// <summary>Animation for unlocking opening door</summary>
        private IEnumerator DoorOpen()
        {
            float startTime = Time.time;
            float openTime = startTime + animationLength;
            float timeRatio = 0;

            while (Time.time <= openTime)
            {
                timeRatio = (Time.time - startTime) / animationLength;
                Door.transform.localRotation = Quaternion.Lerp(Door.transform.localRotation, doorOpenedRotation, timeRatio);
                yield return null;
            }       
        }

        /// <summary>Animation for closing the door</summary>
        private IEnumerator DoorClose()
        {
            float startTime = Time.time;
            float closeTime = startTime + animationLength;
            float timeRatio = 0;

            while (Time.time <= closeTime)
            {
                timeRatio = (Time.time - startTime) / animationLength;
                Door.transform.localRotation = Quaternion.Lerp(Door.transform.localRotation, doorClosedRotation, timeRatio);
                yield return null;
            }
        }
    }


}
