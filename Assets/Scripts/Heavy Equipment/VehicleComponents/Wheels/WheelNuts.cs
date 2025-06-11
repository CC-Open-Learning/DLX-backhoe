/*
* SUMMARY: Contains the WheelNuts class, used to attach the inspectable and set the wheel nuts of
*          the wheel to being either loose or tight.
*/

using System.Collections.Generic;
using UnityEngine;
using RemoteEducation.Scenarios.Inspectable;
using RemoteEducation.UI.Tooltip;
using RemoteEducation.Localization;
using RemoteEducation.Extensions;
using RemoteEducation.Scenarios;
using RemoteEducation.Interactions;

namespace RemoteEducation.Modules.HeavyEquipment
{
    /// <summary>This class allows the wheel nuts of the Heavy Equipment Backhoe to be inspected.</summary>
    /// <remarks>Sets the wheel nuts to be loose or tight. If they are set to be loose, a random amount of at least the value of <see cref="MinimumLooseNuts"/> will be set to loose.</remarks>
    public class WheelNuts : MonoBehaviour, ITaskable, IInitializable
    {
        private const string NUT_TOOLTIP_TEXT = "HeavyEquipment.WheelNutsTooltip";

        public enum NutLocation
        {
            Default,
            FrontLeft,
            FrontRight,
            BackLeft,
            BackRight
        }

        [Tooltip("List of all the 'wheel_nuts_#_octagon' objects in this wheel")]
        [SerializeField] private List<WheelNut> Nuts;
        // Created for UT purpose
        public List<WheelNut> AllNuts => Nuts;

        [Tooltip("Minimum amount of wheel nuts to be loose, if this wheel is in the \"Loose wheel nuts\" state.")]
        [SerializeField] private int MinimumLooseNuts = 3;

        [SerializeField, Tooltip("The location of this debris group.")]
        private NutLocation location;

        private bool taskCleared = false;

        public TaskableObject Taskable { get; private set; }

        /// <summary>Initialize each individual debris in the group (cluster).</summary>
        /// <param name="input"><see cref="BackhoeController"/> instance argument.</param>
        public void Initialize(object input = null)
        {
            Taskable = new TaskableObject(this, location.ToString());

            foreach (WheelNut temp in Nuts)
            {
                temp.SetupGroup(this);
            }
        }

        /// <summary>Sets the wheel nuts to be either tight or loose</summary>
        /// <remarks>Randomly sets the wheel nuts to be tight or loose. Ensures that at least the value of <see cref="MinimumLooseNuts"/> are set to loose.</remarks>
        public void SetLooseWheelNuts()
        {
            int i = 0;  // counter for setting default number of loose wheel nuts

            // shuffle list so the defaulted loose nuts are randomized
            Nuts.Shuffle();
            MinimumLooseNuts = (MinimumLooseNuts > 0) ? MinimumLooseNuts : 1;
            foreach (WheelNut wheelNut in Nuts)
            {
                // set at least MinimumLooseNuts to loose, the rest are random
                wheelNut.looseNut = (i++ < MinimumLooseNuts) ? true : Random.value > 0.5f;
            }
        }

        /// <summary>Set all the wheel nuts to tight. </summary>
        public void SetAllNutsTight()
        {
            Nuts.ForEach(wheelNut => wheelNut.looseNut = false);
        }

        /// <summary>Enables wheel nuts to still be interacted with while the inspectable element on the wheel is selected.</summary>
        /// <param name="wheelInteractable">The interactable of the wheel containing these nuts.</param>
        public void SetupInteractable(DynamicInspectableElement wheelInteractable)
        {
            foreach (WheelNut nut in Nuts)
            {
                if (!(nut.HasExceptions && nut.ExclusiveInteractionExceptions.Contains(wheelInteractable)))
                {
                    nut.AddExclusiveException(wheelInteractable);
                }

                nut.WheelInteractable = wheelInteractable;
            }
        }

        /// <summary>Display tooltips over each wheel nut.</summary>
        public void SetupToolTips()
        {
            foreach (WheelNut nut in Nuts)
            {
                if (nut.gameObject.GetComponent<SimpleTooltip>() == null)
                {
                    SimpleTooltip tooltip = nut.gameObject.AddComponent<SimpleTooltip>();

                    tooltip.tooltipText = NUT_TOOLTIP_TEXT.Localize();
                }
            }
        }

        public void Poke()
        {
            foreach (WheelNut temp in Nuts)
            {
                if (!temp.isInspected)
                    return;
            }


            taskCleared = true;
            Taskable.PokeTaskManager();
        }


        public object CheckTask(int checkType, object inputData)
        {
            switch (checkType)
            {
                case (int)TaskVertexManager.CheckTypes.Bool:
                    if (taskCleared)
                    {
                        taskCleared = false;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                default:
                    Debug.LogError($"Invalid check type passed into {GetType().Name}");
                    break;
            }

            return null;
        }
    }
}