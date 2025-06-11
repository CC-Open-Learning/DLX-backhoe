using System.Collections;
using System.Collections.Generic;
using RemoteEducation.Interactions;
using RemoteEducation.Scenarios.Inspectable;
using RemoteEducation.Localization;
using RemoteEducation.Scenarios;
using RemoteEducation.UI;
using RemoteEducation.UI.Tooltip;
using UnityEngine;

namespace RemoteEducation.Modules.HeavyEquipment
{
    public class IndicatorLightSwitch : SemiSelectable, IBreakable, IInitializable
    {
        /// <summary>The backhoelightcontroller using this script</summary>
        private BackhoeLightController backhoeLightController;
        [SerializeField] private string toolTipToken;

        void IBreakable.AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int breakMode)
        {
            throw new System.NotImplementedException();
        }

        void IInitializable.Initialize(object input)
        {
            backhoeLightController = FindObjectOfType<BackhoeLightController>();
            SetToolTip();
            OnSelect += ClickSwitch;
        }

        void ClickSwitch() 
        {
            var on = backhoeLightController.ToggleLights(BackhoeLightController.Light.Indicator);

            string switchToast = on ? Localizer.Localize("HeavyEquipment.BackhoeLightOn") : Localizer.Localize("HeavyEquipment.BackhoeLightOff");
            HEPrompts.CreateToast(switchToast, HEPrompts.ShortToastDuration);
        }

        private void SetToolTip()
        {
            if (gameObject.GetComponent<SimpleTooltip>() == null)
            {
                SimpleTooltip tooltip = gameObject.AddComponent<SimpleTooltip>();
                tooltip.tooltipText = Localizer.Localize(toolTipToken);
            }
        }
    }
}
