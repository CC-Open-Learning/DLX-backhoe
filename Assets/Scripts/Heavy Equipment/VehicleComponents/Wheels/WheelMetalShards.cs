/*
* SUMMARY: Contains the WheelMetalShards class, used to setup the interactions and tooltips of the
*          metal shards displayed on the inside of the tire if that break mode is used.
*/

using System.Collections.Generic;
using UnityEngine;
using RemoteEducation.Scenarios.Inspectable;
using RemoteEducation.UI.Tooltip;
using RemoteEducation.Localization;

namespace RemoteEducation.Modules.HeavyEquipment
{
    /// <summary>This class allows the metal shards on the inside of the backhoe tires to be interacted with and display tooltips.</summary>
    public class WheelMetalShards : MonoBehaviour
    {
        [Tooltip("List of all the 'Metal_Shard_#' objects inside the wheel")]
        [SerializeField] private List<WheelMetalShard> MetalShards;
        // Created for UT purpose
        public List<WheelMetalShard> ListMetalShards => MetalShards;

        private const string MET_TOOLTIP_TEXT = "HeavyEquipment.MetalShardsTooltip";

        /// <summary>Allows the metal shards to be interacted with while the wheel is currently selected.</summary>
        /// <param name="wheelInteractable">The interactable of the wheel.</param>
        private void SetupInteractable(DynamicInspectableElement wheelInteractable)
        {
            foreach (WheelMetalShard shard in MetalShards)
            {
                if (!(shard.HasExceptions && shard.ExclusiveInteractionExceptions.Contains(wheelInteractable)))
                {
                    shard.AddExclusiveException(wheelInteractable);
                }
                shard.WheelInteractable = wheelInteractable;
            }
        }

        /// <summary>Display tooltips over each metal shard.</summary>
        private void SetupToolTips()
        {
            foreach (WheelMetalShard shard in MetalShards)
            {
                if (shard.gameObject.GetComponent<SimpleTooltip>() == null)
                {
                    var tooltip = shard.gameObject.AddComponent<SimpleTooltip>();

                    tooltip.tooltipText = MET_TOOLTIP_TEXT.Localize();
                }
            }
        }

        /// <summary>Enables the metal shards and sets them up to allow for inspection.</summary>
        public void SetupForInspection(DynamicInspectableElement wheelInteractable)
        {
            gameObject.SetActive(true);
            SetupToolTips();
            SetupInteractable(wheelInteractable);
        }
    }
}
