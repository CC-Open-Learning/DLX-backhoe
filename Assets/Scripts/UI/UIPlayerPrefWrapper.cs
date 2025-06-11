using TMPro;
using UnityEngine;
using UnityEngine.UI;
using RemoteEducation.Networking.Telemetry;
using System;

namespace RemoteEducation.UI
{
    /// <summary>Gets/Sets the PlayerPref value for the given UI element and key.</summary>
    public class UIPlayerPrefWrapper : MonoBehaviour
    {
        [SerializeField] private UnityEngine.Object element;
        [SerializeField] private PreferenceKeys.Key key;

        private string keyStr;

        private void Start()
        {
            keyStr = PreferenceKeys.GetPreferenceName(key);

            if (string.IsNullOrEmpty(keyStr))
                return;

            SetElementValue();
        }

        private void SetElementValue()
        {
            switch(element)
            {
                case Slider slider:
                    slider.value = PlayerPrefs.GetFloat(keyStr, slider.maxValue); // Sliders default to maxvalue if the pref is not found.
                    slider.onValueChanged.AddListener(SetPrefsValue);
                    break;

                case Toggle toggle:
                    toggle.isOn = PlayerPrefs.GetInt(keyStr) == 1;
                    toggle.onValueChanged.AddListener(SetPrefsValue);
                    break;

                case Dropdown dropdown:
                    dropdown.value = PlayerPrefs.GetInt(keyStr);
                    dropdown.onValueChanged.AddListener(SetPrefsValue);
                    break;
            }
        }

        private void SetPrefsValue(bool value)  { PlayerPrefs.SetInt(keyStr, value ? 1 : 0); }
        private void SetPrefsValue(float value) { PlayerPrefs.SetFloat(keyStr, value); }
        private void SetPrefsValue(int value)   { PlayerPrefs.SetInt(keyStr, value); }
    }
}
