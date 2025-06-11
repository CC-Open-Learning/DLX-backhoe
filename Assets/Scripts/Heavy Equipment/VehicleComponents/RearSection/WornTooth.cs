using RemoteEducation.Interactions;
using System.Collections;
using UnityEngine;
using RemoteEducation.UI;
using RemoteEducation.Localization;

namespace RemoteEducation.Modules.HeavyEquipment
{
    public class WornTooth : SemiSelectable
    {
        public bool isInspected = false;

        BoomBucket ParentObject = null;

        public void Setup(BoomBucket parent)
        {
            OnSelect += WornPrompt;
            ParentObject = parent;
        }

        /// <summary>function to start the coroutine for loose tooth animation</summary>
        private void WornPrompt()
        {
            isInspected = true;
            HEPrompts.CreateToast(Localizer.Localize("HeavyEquipment.WornToothToast"), HEPrompts.ShortToastDuration);
            ParentObject.PokeTeeth();
        }
    }
}
