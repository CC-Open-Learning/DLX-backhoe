/*
 * SUMMARY: This file contains the Air Filter Service Indicator class. The purpose of this class is to check
 *          if the air filter needs to be replaced.
 */

using System.Collections;
using System.Collections.Generic;
using RemoteEducation.Interactions;
using RemoteEducation.Scenarios.Inspectable;
using UnityEngine;
using RemoteEducation.UI;
using RemoteEducation.UI.Tooltip;
using RemoteEducation.Localization;

namespace RemoteEducation.Modules.HeavyEquipment
{
    public class AirFilterServiceIndicator : SemiSelectable, IBreakable
    {

        [SerializeField, Tooltip("Light in object to indicate status")]
        private GameObject airFilterServiceIndicatorLight;

        public DynamicInspectableElement InspectableElement;

        private SimpleTooltip tooltip = null;

        public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int breakMode)
        {
            InspectableElement = inspectableElement;

            airFilterServiceIndicatorLight.GetComponent<MeshRenderer>().material.color = broken ? Color.red : Color.green;

            inspectableElement.OnCurrentlyInspectableStateChanged += (active) =>
            {
            //When interactable, don't show the highlight for this script. Also
            //make it so that it can be interacted with at any time.
            ToggleFlags(!active, Flags.Highlightable | Flags.ExclusiveInteraction);
            };
        }

    }
}
