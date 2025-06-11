using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeavyEquipmentGaugeLights = RemoteEducation.Modules.HeavyEquipment.GaugePanelLight.TypeOfGaugeLight;

namespace RemoteEducation.Modules.HeavyEquipment
{
    public class ClipboardItem : MonoBehaviour
    {
        [SerializeField, Tooltip("Yes Checkmark Object")]
        private GameObject YesCheckmark;

        [SerializeField, Tooltip("No Checkmark Object")]
        private GameObject NoCheckmark;

        [SerializeField, Tooltip("Is Yes Checked")]
        private bool IsYes;

        [SerializeField, Tooltip("Is No Checked")]
        private bool IsNo;

        [SerializeField, Tooltip("Type of gauge item")]
        private HeavyEquipmentGaugeLights ClipboardItemType;

        public void SetYes()
        {
            IsNo = false;
            IsYes = true;
            YesCheckmark.SetActive(true);
            NoCheckmark.SetActive(false);
        }

        public void SetNo()
        {
            IsNo = true;
            IsYes = false;
            YesCheckmark.SetActive(false);
            NoCheckmark.SetActive(true);
        }

        public bool CheckYes()
        {
            return IsYes;
        }

        public bool CheckNo()
        {
            return IsNo;
        }

        public HeavyEquipmentGaugeLights GetClipItemType()
        {
            return ClipboardItemType;
        }
    }
}