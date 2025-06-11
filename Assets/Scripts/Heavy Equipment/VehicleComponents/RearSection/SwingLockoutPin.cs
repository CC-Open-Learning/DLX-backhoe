/*
 * SUMMARY: Defines boom swing lockout pin.
 */
using RemoteEducation.Interactions;
using RemoteEducation.Localization;
using RemoteEducation.Modules.HeavyEquipment;
using RemoteEducation.Scenarios.Inspectable;
using RemoteEducation.UI;
using System.Collections;
using UnityEngine;
using RemoteEducation.Scenarios;

namespace RemoteEducation.Modules.HeavyEquipment
{

    /// <summary>This class check if the boom swing lockout pin is engaged</summary>
    public class SwingLockoutPin : SemiSelectable, IInitializable, IBreakable, ITaskable
    {
        public enum LockoutPinState
        {
            Engaged,    // locked
            Disengaged, // unlocked
            Missing
        }

        /// <summary>To determine if the inspection menu/mode is on</summary>
        private bool inspectOpen = false;

        /// <summary>This variable is the state of the swing lockout pin</summary>
        private LockoutPinState pinState;

        /// <summary>This variable is the engaged (locked) position of the swing lockout pin</summary>
        private Vector3 engagedPosition;

        /// <summary>Boolean variable to check if we are animating or not</summary>
        private bool isAnimating;

        [Tooltip("Duration of pin lifting animation.")]
        [SerializeField] private float verticalAnimationTime = 2;

        [Tooltip("Duration of pin horizontal movement animation.")]
        [SerializeField] private float horizontalAnimationTime = 3;

        [SerializeField, Tooltip("Transform representing the unlocked position of the pin.")]
        private Transform UnlockedTransform;

        [SerializeField, Tooltip("Transform representing the lifted position of the pin (above engaged).")]
        private Transform EngagedLiftedTransform;

        [SerializeField, Tooltip("Transform representing the lifted position of the pin (above unlocked).")]
        private Transform UnlockedLiftedTransform;

        public DynamicInspectableElement InspectableElement { get; private set; }

        public TaskableObject Taskable { get; private set; }

        /// <summary>Make the lockout pin an inspectable and set it to broken or fixed</summary>
        public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int breakMode)
        {
            pinState = LockoutPinState.Engaged;

            InspectableElement = inspectableElement;

            // Make it so the student can see the toast message if they click on the lockout pin again
            // without selecting something else
            InspectableElement.OnCurrentlyInspectableStateChanged += (active) =>
            {
                ToggleFlags(!active, Flags.Highlightable | Flags.ExclusiveInteraction);
            };
        }

        public void Initialize(object input = null)
        {

            Taskable = new TaskableObject(this);

                OnSelect += DisengageLockoutPin;

            engagedPosition = transform.position;
        }

        private void DisplayPinState()
        {
            string pinStateMessage = (pinState == LockoutPinState.Engaged)
               ? Localizer.Localize("HeavyEquipment.SwingLockoutPinToastEngaged")
               : Localizer.Localize("HeavyEquipment.SwingLockoutPinToastDisengagedOrMissing");

            HEPrompts.CreateToast(pinStateMessage, HEPrompts.LongToastDuration);
        }

        /// <summary>Public function to call to disengage (unlock) the lockout pin and set the mode change.</summary>
        public void DisengageLockoutPin()
        {
            if (inspectOpen || isAnimating)
                return;

            inspectOpen = true;
            StartCoroutine(LockoutPinRemoval());
            
        }

        /// <summary>Public function to return to normal functionality (lock) and disable UI elements.</summary>
        public void EngageLockoutPin()
        {
            if (!inspectOpen || isAnimating)
                return;

            StartCoroutine(LockoutPinReturn());
        }

        /// <summary>Coroutine for lockout pin removal.</summary>
        private IEnumerator LockoutPinRemoval()
        {
            // Start animating
            isAnimating = true;

            yield return MovePin(engagedPosition, EngagedLiftedTransform.position, verticalAnimationTime);
            yield return MovePin(EngagedLiftedTransform.position, UnlockedLiftedTransform.position, horizontalAnimationTime);
            yield return MovePin(UnlockedLiftedTransform.position, UnlockedTransform.position, verticalAnimationTime);

            //Set Final Destination
            transform.position = UnlockedTransform.position;

            // Set to not animating
            isAnimating = false;

            // If no graph is being used, can click on it again to move it back (lock)
            if (!HeavyEquipmentModule.ScenarioAttatched)
            {
                OnSelect += EngageLockoutPin;
            }

            // Change to Engaged state
            pinState = LockoutPinState.Disengaged;
            Taskable.PokeTaskManager();
        }

        /// <summary>Coroutine for returning the lockout pin.</summary>
        private IEnumerator LockoutPinReturn()
        {
            // Start animating
            isAnimating = true;

            yield return MovePin(transform.position, UnlockedLiftedTransform.position, verticalAnimationTime);
            yield return MovePin(UnlockedLiftedTransform.position, EngagedLiftedTransform.position, horizontalAnimationTime);
            yield return MovePin(EngagedLiftedTransform.position, engagedPosition, verticalAnimationTime);

            //Set that we are not animating or inspecting
            isAnimating = false;
            inspectOpen = false;
        }

        /// <summary>Move the lockout pin.</summary>
        /// <param name="start">Starting point</param>
        /// <param name="finish">Ending point</param>
        /// <param name="duration">Time to move</param>
        /// <returns></returns>
        private IEnumerator MovePin(Vector3 start, Vector3 finish, float duration)
        {
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                transform.position = Vector3.Slerp(start, finish, t);
                yield return null;
            }
        }

        public object CheckTask(int checkType, object inputData)
        {
            switch (checkType)
            {
                case (int)TaskVertexManager.CheckTypes.Bool:
                    return (pinState == LockoutPinState.Engaged);

                default:
                    Debug.LogError($"Invalid check type passed into {GetType().Name}");
                    break;
            }

            return null;
        }
    }
}

