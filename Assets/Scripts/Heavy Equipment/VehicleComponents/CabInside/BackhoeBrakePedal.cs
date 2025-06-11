/*
* SUMMARY: Contains the BackhoeBrakePedal class, used to rotate the backhoe brake pedals between
*          depressed and unpressed.
*/

using System.Collections;
using UnityEngine;
using RemoteEducation.Interactions;
using RemoteEducation.UI.Tooltip;
using RemoteEducation.Scenarios;
using RemoteEducation.Localization;

namespace RemoteEducation.Modules.HeavyEquipment
{
    public class BackhoeBrakePedal : SemiSelectable, IInitializable
    {
        [Tooltip("Brake pedals manager.")]
        [SerializeField] BrakePedalsManager PedalsManager;

        [Tooltip("Brake pedal transform when depressed.")]
        [SerializeField] private Transform DepressedPedal;

        /// <summary>Depressed brake pedal rotation.</summary>
        private Quaternion depressedRotation;

        /// <summary>Released/Unpressed brake pedal rotation.</summary>
        private Quaternion releasedRotation;

        /// <summary>Used to start and stop the RotatePedals coroutine.</summary>
        private Coroutine pedalAnimation;

        private bool isAnimating = false;

        private float AnimationTime = 1f;

        /// <summary>Sets up input for the brake pedals.</summary>
        public void Initialize(object input = null)
        {
            releasedRotation = transform.localRotation;
            depressedRotation = DepressedPedal.localRotation;

            OnSelect += DepressPedal;
        }

        /// <summary>Called to depress the brake pedal.</summary>
        private void DepressPedal()
        {
            if (isAnimating)
                return;

            isAnimating = true;
            SetPedal(true);
        }

        /// <summary>Called to release the brake pedal.</summary>
        private void ReleasePedal()
        {
            SetPedal(false);
        }

        /// <summary>Sets the brake pedal to depressed or released.</summary>
        /// <param name="depress">true if the pedal is being depressed, false if released.</param>
        private void SetPedal(bool depress)
        {
            // if the rod is intact, let the manager handle the pedals
            if(PedalsManager.pedalsState != BrakePedalsManager.PedalsState.NoConnectingRod)
                PedalsManager.SetPedals(depress);
            else
            {
                if(pedalAnimation != null)
                    StopCoroutine(pedalAnimation);
                pedalAnimation = StartCoroutine(RotatePedals(depress, PedalsManager.rotationTime));
            }

            if(depress)
                StartCoroutine(AutomatedReverse());
        }

        public IEnumerator AutomatedReverse()
        {
            yield return new WaitForSeconds(AnimationTime);
            SetPedal(false);
            isAnimating = false;
        }

        /// <summary>Rotates the break pedal to either the depressed or unpressed rotation.</summary>
        /// <param name="pressPedal">True if pedal is being pressed, false if unpressed.</param>
        /// <param name="duration">Length of time for the animation.</param>
        public IEnumerator RotatePedals(bool pressPedal, float duration)
        {
            CoreCameraController.ToggleObjectIsDragging(true);
            Quaternion currentRotation = transform.localRotation;
            Quaternion targetRotation = (pressPedal) ? depressedRotation : releasedRotation;

            for(float time = 0; transform.localRotation != targetRotation; time += Time.deltaTime)
            {
                transform.localRotation = Quaternion.Lerp(currentRotation, targetRotation, time / duration);
                yield return null;
            }
            CoreCameraController.ToggleObjectIsDragging(false);
        }

        /// <summary>Rotates the break pedal to either the depressed or unpressed rotation.</summary>
        /// <param name="pressPedal">True if pedal is being pressed, false if unpressed.</param>
        /// <param name="duration">Length of time for the animation.</param>
        /// <param name="initialDelay">Length of time that the animation is delayed for.</param>
        public IEnumerator RotatePedals(bool pressPedal, float duration, float initialDelay)
        {
            yield return new WaitForSeconds(initialDelay);

            CoreCameraController.ToggleObjectIsDragging(true);
            Quaternion currentRotation = transform.localRotation;
            Quaternion targetRotation = (pressPedal) ? depressedRotation : releasedRotation;

            for (float time = 0; transform.localRotation != targetRotation; time += Time.deltaTime)
            {
                transform.localRotation = Quaternion.Lerp(currentRotation, targetRotation, time / duration);
                yield return null;
            }
            CoreCameraController.ToggleObjectIsDragging(false);
        }

        /// <summary>Display tooltips over the pedal.</summary>
        public void SetupToolTips()
        {
            if(this.gameObject.GetComponent<SimpleTooltip>() == null)
            {
                SimpleTooltip tooltip = this.gameObject.AddComponent<SimpleTooltip>();
                tooltip.tooltipText = Localizer.Localize("HeavyEquipment.BrakePedalsTooltip");
            }
        }    
    }
}