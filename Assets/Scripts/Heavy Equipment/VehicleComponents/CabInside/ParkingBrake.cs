/*
 * SUMMARY: Defines Parking Brake Mechanics
 */
using RemoteEducation.Interactions;
using RemoteEducation.Scenarios.Inspectable;
using System.Collections;
using UnityEngine;
using RemoteEducation.UI;
using RemoteEducation.Localization;

namespace RemoteEducation.Modules.HeavyEquipment
{
    /// <summary> This class handles how the parking brake works and moves </summary>
    public class ParkingBrake : SemiSelectable, IInitializable, IBreakable
    {

        /// <summary>This variable sets the physical state of the object. </summary>
        private bool isBroken = false;

        /// <summary> This variable determines if the parking brake is engaged or disengaged. </summary>
        private bool isEngaged = false;

        /// <summary> This variable determines if the animation is on or not. </summary>
        private bool isAnimating = false;

        [SerializeField, Tooltip("Duration of the animation in seconds.")]
        private float animationDuration = 1;

        [SerializeField, Tooltip("Position of where parking brake is off.")]
        private Transform OffPosition;

        [SerializeField, Tooltip("Position of where parking brake is on.")]
        private Transform OnPosition;

        [SerializeField] private Light indicatorLight;

        /// <summary> Set object to broken or not, make it inspectable</summary>
        public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int breakMode)
        {
            //Set the state if broken or not
            isBroken = broken;

            inspectableElement.OnCurrentlyInspectableStateChanged += (active) =>
            {
                ToggleFlags(!active, Flags.Highlightable | Flags.ExclusiveInteraction);
            };
        }

        public void Initialize(object input = null)
        {
            OnSelect += ToggleBrake;
        }

        /// <summary> Brake State Toggle </summary>
        private void ToggleBrake()
        {
            //Temporary Broken Message
            if (isBroken)
                HEPrompts.CreateToast(Localizer.Localize("HeavyEquipment.ParkingBrakeToastBroken"), HEPrompts.LongToastDuration);

            //Exit if broken or animating
            if (isBroken || isAnimating)
                return;

            //Set state and start animation

            isEngaged = !isEngaged;
            StartCoroutine(ToggleBrakeAnimation());
        }

        /// <summary>Toggle Brake Animation</summary>
        private IEnumerator ToggleBrakeAnimation()
        {
            isAnimating = true;
            float startTime = Time.time;
            float endTime = startTime + animationDuration;
            float timeRatio;

            if (!isEngaged) indicatorLight.enabled = false;

            //Move object from one spot to another
            while (Time.time <= endTime)
            {
                timeRatio = (Time.time - startTime) / animationDuration;

                if(isEngaged)
                {
                    //OffPosition to OnPosition
                    transform.position = Vector3.Lerp(OffPosition.position, OnPosition.position, timeRatio);
                    transform.rotation = Quaternion.Lerp(OffPosition.rotation, OnPosition.rotation, timeRatio);
                }
                else
                {
                    //OnPosition to OffPosition
                    transform.position = Vector3.Lerp(OnPosition.position, OffPosition.position, timeRatio);
                    transform.rotation = Quaternion.Lerp(OnPosition.rotation, OffPosition.rotation, timeRatio);
                }

                yield return null;
            }

            if (isEngaged) indicatorLight.enabled = true;
            isAnimating = false;
        }

        public bool CheckIfEngaged()
        {
            return isEngaged;
        }
    }
}