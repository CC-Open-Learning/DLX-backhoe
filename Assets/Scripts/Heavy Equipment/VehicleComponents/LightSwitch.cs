using RemoteEducation.Interactions;
using RemoteEducation.Localization;
using RemoteEducation.UI.Tooltip;
using UnityEngine;

namespace RemoteEducation.Modules.HeavyEquipment 
{
    public class LightSwitch : SemiSelectable, IInitializable
    {
        /// <summary>The lightgroup turned on by this switch</summary>
        private BackhoeLightController backhoeLightController;
        [SerializeField] private string toolTipToken;
        [SerializeField] private BackhoeLightController.Light targetLight;

        void IInitializable.Initialize(object input)
        {
            SetToolTip();
            backhoeLightController = FindObjectOfType<BackhoeLightController>();
            OnSelect += ClickSwitch;
        }

        void ClickSwitch() 
        {
            var lightOnStr = Localizer.Localize("HeavyEquipment.BackhoeLightOn");
            var lightOffStr = Localizer.Localize("HeavyEquipment.BackhoeLightOff");
            var switchToast = "";

            var on = backhoeLightController.ToggleLights(targetLight);
            switchToast = on ? lightOnStr : lightOffStr;

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
