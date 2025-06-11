/*
 *  SUMMARY:   The purpose of this class is to model the behaviour of the Boom Lock Latch controls inside the cab. OnSelect() the
 *             lever is animated back and forth from the lock and unlocked positions and also toggles the boom lock's position/state if
 *             it is not in a broken state. Additionally a sound will play on state change and a message will appear to inform the user 
 *             the current state of the boom latch lock.
 */



using RemoteEducation.Interactions;
using RemoteEducation.UI;
using System.Collections;
using RemoteEducation.Audio;
using UnityEngine;
using RemoteEducation.Localization;
using RemoteEducation.UI.Tooltip;

namespace RemoteEducation.Modules.HeavyEquipment
{

    public sealed class BoomLockLever : SemiSelectable, IInitializable
    {
        [SerializeField, Tooltip("Sound effect played when the door begins to move.")]
        private SoundEffect boomLatchSoundEffect;

        [SerializeField, Tooltip("The lock that the lever will control.")]
        private BoomLockLatch boomLockLatch;

        /// <summary>Generic animation class for interior controls/buttons</summary>
        private GenericRotationAnimation genericRotationAnimation;

        private bool lockedPosition = false;

        public void Initialize(object input = null)
        {
            AddTooltip();

            //Set axis of rotation vector for the animation
            genericRotationAnimation = GetComponent<GenericRotationAnimation>();
            genericRotationAnimation.SetAxisOfRotation();
            OnSelect += ToggleLever;
        }

        /// <summary>Adds a tooltip to the game object if it not already present using a string token and the localization system</summary>
        private void AddTooltip()
        {
            //Add tooltip to the control inside the cabin so the user can distinguish between this control and the others
            if (gameObject.GetComponent<SimpleTooltip>() == null)
            {
                var tooltip = gameObject.AddComponent<SimpleTooltip>();
                tooltip.tooltipText = Localizer.Localize("HeavyEquipment.BoomLockLeverTooltip");
            }
        }

        /// <summary>Toggle boom latch lock states (locked and unlocked). Display a toast to inform the user of the state
        /// and play a sound effect on level pull to mimic the sound of a boom latch lock in real life.</summary>
        private void ToggleLever()
        {
            if (!genericRotationAnimation.IsAnimating)
            {
                boomLatchSoundEffect.Play();

                //Toggle isAnimating flag and start coroutine to animate the lever
                genericRotationAnimation.ToggleIsAnimating();
                StartCoroutine(genericRotationAnimation.RotationAnimation(lockedPosition));
                lockedPosition = !lockedPosition;

                //Toggle the lock flag and start the coroutine to animate the lock
                boomLockLatch.ToggleLock();
            }
        }
    }
}