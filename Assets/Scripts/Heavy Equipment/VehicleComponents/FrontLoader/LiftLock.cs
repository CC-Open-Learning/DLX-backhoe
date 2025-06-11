/*
* SUMMARY: Contains the LiftLock class, initializes the lift lock and used to start the animation of the lift arms locking.
*/

using RemoteEducation.Interactions;
using RemoteEducation.Localization;
using RemoteEducation.Scenarios;
using RemoteEducation.UI;
using RemoteEducation.UI.Tooltip;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RemoteEducation.Modules.HeavyEquipment
{
    public sealed class LiftLock : SemiSelectable, IInitializable, ITaskable
    {
        [SerializeField, Tooltip("The front loader.")]
        private FrontLoader frontLoader;

        [SerializeField, Tooltip("The loader shifter (control) of the front loader.")]
        private LoaderControl loaderControl;

        [SerializeField, Tooltip("The side engine panels.")]
        private List<SidePanel> sidePanels;

        /// <summary>If the loader arm lift lock is in unlocked (default) position.</summary>
        private bool isUnlocked = true;
        public bool IsUnlocked => isUnlocked;

        public TaskableObject Taskable { get; private set; }

        private bool isAnimating = false;

        /// <summary>Initializes OnSelect to UseLiftLock.</summary>
        public void Initialize(object input = null)
        {
            OnSelect += UseLiftLock;
            SetupToolTips();

            Taskable = new TaskableObject(this);
        }

        /// <summary>
        /// Calls the front loader to play the lift lock animations. 
        /// Clicking the lift lock for each step of locking.
        /// </summary>
        private void UseLiftLock()
        {
            if (isAnimating)
                return;

            // If the front loader is in lifted position or either of the side engine panels is opened,
            // can lock/ unlock the lift lock, display toast message and return
            if (loaderControl.InDefaultPos
                || sidePanels[0].IsOpen
                || sidePanels[1].IsOpen)
            {
                string warningMessage = Localizer.Localize("HeavyEquipment.ArmLiftLockCantMove");
                HEPrompts.CreateToast(warningMessage, HEPrompts.LongToastDuration);
                return;
            }

            frontLoader.StartLoader(isUnlocked ?
                FrontLoader.State.LiftLockProcedure :
                FrontLoader.State.Lift);

            isUnlocked = !isUnlocked;

            isAnimating = true;
            StartCoroutine(PokeWhenAnimaitonFinished());
        }

        IEnumerator PokeWhenAnimaitonFinished()
        {

            float lockAnimationLength = frontLoader.GetComponent<Animator>().runtimeAnimatorController.animationClips[8].length * 0.9f;
            yield return new WaitForSeconds(lockAnimationLength);

            isAnimating = false;
            Taskable.PokeTaskManager();
        }

        /// <summary>Display tooltips over the lift lock.</summary>
        public void SetupToolTips()
        {
            if (this.gameObject.GetComponent<SimpleTooltip>() == null)
            {
                SimpleTooltip tooltip = this.gameObject.AddComponent<SimpleTooltip>();
                tooltip.tooltipText = Localizer.Localize("HeavyEquipment.LiftLockTooltip");
            }
        }

        public object CheckTask(int checkType, object inputData)
        {
            switch (checkType)
            {
                case (int)TaskVertexManager.CheckTypes.Bool:
                    return IsUnlocked;

                default:
                    Debug.LogError("Invalid check type passed into LiftLock");
                    break;
            }

            return null;
        }
    }
}