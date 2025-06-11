/*
* SUMMARY: Contains the LoaderControl class, initializes the loader control lever and used to start the animation of the lever and loader arms.
*/

using RemoteEducation.Interactions;
using RemoteEducation.Localization;
using RemoteEducation.Scenarios;
using RemoteEducation.UI;
using RemoteEducation.UI.Tooltip;
using System.Collections;
using UnityEngine;

namespace RemoteEducation.Modules.HeavyEquipment
{
    public sealed class LoaderControl : SemiSelectable, IInitializable, ITaskable
    {
        /// <summary>Used to rotate the lever when clicked.</summary>
        private GenericRotationAnimation genericRotationAnimation;

        /// <summary>If the loader control lever is in the default position.</summary>
        private bool inDefaultPos = true;
        public bool InDefaultPos => inDefaultPos;

        [SerializeField, Tooltip("The front loader.")]
        private FrontLoader frontLoader;

        [SerializeField, Tooltip("The front arm lift lock.")]
        private LiftLock liftLock;

        private bool isAnimating = false;

        public TaskableObject Taskable { get; private set; }

        /// <summary>Initializes the rotation animation and OnSelect to ToggleLever.</summary>
        public void Initialize(object input = null)
        {
            genericRotationAnimation = GetComponent<GenericRotationAnimation>();
            genericRotationAnimation.SetAxisOfRotation();
            OnSelect += ToggleLever;
            SetupToolTips();

            Taskable = new TaskableObject(this);
        }

        /// <summary>Calls for the lift arm to raise and toggle the lever position.</summary>
        private void ToggleLever()
        {
            if (isAnimating)
                return;

            DisplayControlState();

            // If the loader arm lift lock is unlocked, can lift/ lower the loader
            if (liftLock.IsUnlocked)
            {
                frontLoader.StartLoader(inDefaultPos ? FrontLoader.State.Lift : FrontLoader.State.Idle);

                isAnimating = true;
                inDefaultPos = !inDefaultPos;
                StartCoroutine(genericRotationAnimation.RotationAnimation(inDefaultPos));

                StartCoroutine(PokeWhenAnimaitonFinished());
            }
        }

        IEnumerator PokeWhenAnimaitonFinished() {

            float bucketAnimationLength = frontLoader.GetComponent<Animator>().runtimeAnimatorController.animationClips[1].length * 0.9f;
            yield return new WaitForSeconds(bucketAnimationLength);

            isAnimating = false;

            Taskable.PokeTaskManager();
        }

        /// <summary>Display tooltips over the front loader control lever.</summary>
        public void SetupToolTips()
        {
            if (this.gameObject.GetComponent<SimpleTooltip>() == null)
            {
                SimpleTooltip tooltip = this.gameObject.AddComponent<SimpleTooltip>();
                tooltip.tooltipText = Localizer.Localize("HeavyEquipment.FrontLoaderControlLeverTooltip");
            }
        }

        private void DisplayControlState()
        {
            string controlStateMessage;

            if (!liftLock.IsUnlocked)
            {
                controlStateMessage = Localizer.Localize("HeavyEquipment.LoaderControlToastCantMove");
            }
            else
            {
                controlStateMessage = (inDefaultPos)
                   ? Localizer.Localize("HeavyEquipment.LoaderControlToastToLift")
                   : Localizer.Localize("HeavyEquipment.LoaderControlToastToLower");
            }

            HEPrompts.CreateToast(controlStateMessage, HEPrompts.LongToastDuration);
        }

        public object CheckTask(int checkType, object inputData)
        {
            switch (checkType)
            {
                case (int)TaskVertexManager.CheckTypes.Bool:
                    return InDefaultPos;

                default:
                    Debug.LogError("Invalid check type passed into LoaderControl");
                    break;
            }

            return null;
        }
    }
}