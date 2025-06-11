/*
 * SUMMARY: This file contains the FrontHood class. 
 *          The purpose of this file is to animate the front hood when ToggleHoodOpen is triggered.
 */

using RemoteEducation.Interactions;
using RemoteEducation.Scenarios;
using System.Collections;
using UnityEngine;

namespace RemoteEducation.Modules.HeavyEquipment
{
    /// <summary>Defines the behaviour of how the hood of a 416F2 should open and close.</summary>
    /// <remarks>The speed of which it opens and close is dependent on a upwards curve starting from 0 to 1</remarks>
    public sealed class FrontHood : SemiSelectable, IInitializable, ITaskable
    {
        /// <value>Two transformations to hold our destination and source locations used to dictate movement.</value>
        private Transform FromRotation;
        private Transform ToRotation;

        [Header("Transformations")]
        [SerializeField, Tooltip("Transform representing the open position of the hood.")]
        private Transform OpenTransform;

        [SerializeField, Tooltip("Transform representing the closed position of the hood.")]
        private Transform CloseTransform;

        [Header("Hood Animation Elements")]
        [SerializeField, Tooltip("Curve to dictate how the hood should open.")]
        private AnimationCurve HoodCurve;

        /// <summary>If the hood is open or closed.</summary>
        private bool isOpen = false;

        public bool IsOpen { get { return isOpen; } }

        [SerializeField, Tooltip("Time it should take to finish the animation created in the HoodCurve.")]
        private float AnimationDuration = 2;

        /// <summary>If the hood currently opening or closing.</summary>
        private bool IsAnimating;


        public TaskableObject Taskable { get; private set; }

        public EngineCoolant Coolant;

        public void Initialize(object input = null)
        {
            transform.localRotation = CloseTransform.localRotation;

            Taskable = new TaskableObject(this);
        }

        /// <summary>This method toggles the hoods state and sets animation properties as well as trigger animation.</summary>
        /// <remarks>The hood can only be open or closed and this moment</remarks>
        public void ToggleHoodOpen(float delay)
        {
            //Checks if the hood is already animating and if so, then exit and don't change anything
            if (IsAnimating)
                return;

            if (isOpen)
            {
                //Checks if hood is currently open and if so, set animation variables to start closing
                FromRotation = OpenTransform;
                ToRotation = CloseTransform;
            }
            else
            {
                //Checks if hood is currently close and if so, set animation variables to start opening
                FromRotation = CloseTransform;
                ToRotation = OpenTransform;
                GetComponent<AudioSource>().Play();
            }

            //Start animating the hood
            StartCoroutine(HoodAnimation(delay));
            IsAnimating = true;
        }

        /// <summary>This method does the hood animation.</summary>
        /// <remarks>The hood currently handles opening and closing</remarks>
        private IEnumerator HoodAnimation(float delay)
        {
            yield return new WaitForSeconds(delay);
            //Start timer and end timer for our animation
            float StartTimer = Time.time;
            float EndTimer = StartTimer + AnimationDuration;

            //While we are animating, animate the object
            while (Time.time <= EndTimer)
            {
                //Get a ratio from current time and total duration
                float TimeRatio = (Time.time - StartTimer) / AnimationDuration;

                //Get the object position in span of 0 to 1 and set the lerp to that position
                float ObjPosition = HoodCurve.Evaluate(TimeRatio);

                if (!isOpen)
                {
                    //If we are opening the hood then set the position based on the curve
                    transform.localRotation = Quaternion.Lerp(FromRotation.localRotation, ToRotation.localRotation, ObjPosition);
                }
                else
                {
                    //If we are closing the hood then set the position based on the duration of the animation
                    transform.localRotation = Quaternion.Lerp(FromRotation.localRotation, ToRotation.localRotation, TimeRatio);
                }

                yield return null;
            }

            //Set the final position and set animating to false
            transform.localRotation = ToRotation.localRotation;
            IsAnimating = false;

            if (!isOpen)
            {
                Coolant.ShowHeatEffectIfAny();
            }
            else
            {
                Coolant.HideHeatEffect();
            }
            isOpen = !isOpen;
            Taskable.PokeTaskManager();
        }

        public object CheckTask(int checkType, object inputData)
        {
            switch (checkType) 
            {
                case (int)TaskVertexManager.CheckTypes.Bool:
                    return IsOpen;

                default:
                    Debug.LogError("Invalid check type passed into FrontHood");
                    break;
            }

            return null;
        }
    }
}