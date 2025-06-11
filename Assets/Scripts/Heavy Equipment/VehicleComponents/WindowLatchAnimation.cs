/*
 * SUMMARY: This file contains the Window Latch Animation class. When the latch is pressed,
 *          the "StartAnimation()" method is invoked and plays through this animation.
 */

using System.Collections;
using System.Collections.Generic;
using RemoteEducation.Interactions;
using RemoteEducation.Scenarios.Inspectable;
using UnityEngine;


namespace RemoteEducation.Modules.HeavyEquipment
{
    public class WindowLatchAnimation : MonoBehaviour, IInitializable
    {
        [SerializeField, Tooltip("The lock script associated with the latch")] private SecureOpenables windowLock;
        [SerializeField, Tooltip("Length of animation")] private float animationLength;

        [SerializeField, Tooltip("Opened transform of the latch")] private Transform OpenTransform;
        [SerializeField, Tooltip("Closed transform of the latch")] private Transform ClosedTransform;

        /// <summary>Rotation of the latch when closed</summary>
        private Quaternion ClosedRotation;

        /// <summary>Rotation of the latch when opened</summary>
        private Quaternion OpenRotation;

        [SerializeField, Tooltip("Window associated with the latch")] private Transform Window;
    
        [SerializeField, Tooltip("Opened transform of the window")] private Transform WindowOpened;
        [SerializeField, Tooltip("Closed transform of the window")] private Transform WindowClosed;

        /// <summary>Rotation of the window when closed</summary>
        private Quaternion WindowClosedRotation;

        /// <summary>Rotation of the window when opened</summary>
        private Quaternion WindowOpenedRotation;
     
        /// <summary>Current animation being played</summary>
        private Coroutine currentAnimation;

        public void Initialize(object input = null)
        {
            ClosedRotation = ClosedTransform.localRotation;
            OpenRotation = OpenTransform.localRotation;
            WindowClosedRotation = WindowClosed.localRotation;
            WindowOpenedRotation = WindowOpened.localRotation;
        }

        /// <summary>Starts the latch and window animation</summary>
        public void StartAnimation()
        {
            if (currentAnimation != null)
            {
                StopCoroutine(currentAnimation);
            }

            currentAnimation = StartCoroutine(windowLock.IsLocked ? WindowAndLatchClose() : WindowAndLatchOpen());
        }
        
        /// <summary>Animation for unlocking latch and opening window</summary>
        private IEnumerator WindowAndLatchOpen()
        {
            float startTime = Time.time;
            float latchTime = startTime + animationLength;
            float windowTime = latchTime + animationLength;
            float timeRatio = 0;

            while (Time.time <= windowTime)
            {
                if(Time.time <= latchTime)
                {
                    timeRatio = (Time.time - startTime) / animationLength;
                    transform.localRotation = Quaternion.Lerp(transform.localRotation, OpenRotation, timeRatio);
                }
                else
                {
                    timeRatio = (Time.time - latchTime) / animationLength;
                    Window.transform.localRotation = Quaternion.Lerp(Window.transform.localRotation, WindowOpenedRotation, timeRatio);
                }
               
                yield return null;
            }
        }

        /// <summary>Animation for closing the window and locking the latch</summary>
        private IEnumerator WindowAndLatchClose()
        {
            float startTime = Time.time;
            float windowTime = startTime + animationLength;
            float latchTime = windowTime + animationLength;
            float timeRatio = 0;

            while (Time.time <= latchTime)
            {
                if(Time.time <= windowTime)
                {
                    timeRatio = (Time.time - startTime) / animationLength;
                    Window.transform.localRotation = Quaternion.Lerp(Window.transform.localRotation, WindowClosedRotation, timeRatio);
                }
                else
                {
                    timeRatio = (Time.time - windowTime) / animationLength;
                    transform.localRotation = Quaternion.Lerp(transform.localRotation, ClosedRotation, timeRatio);
                }

                yield return null;
            }
        }
    }
}


