using UnityEngine;
using TMPro;

namespace RemoteEducation.Localization
{
    /// <summary>Localizes the TMProUGUI text field of this object.</summary>
    /// <remarks>Component deletes itself upon completion.</remarks>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizedText : MonoBehaviour
    {
        [Tooltip("String token to be localized.")]
        [SerializeField] private string token = null;
        [Tooltip("String to be added at the start of the localized string.")]
        [SerializeField] private string prefix = null;
        [Tooltip("String to be added at the end of the localized string.")]
        [SerializeField] private string suffix = null;
        [Tooltip("Convert all characters to upper case.")]
        [SerializeField] private bool upperCase = false;

        void Start()
        {
            string str = prefix + Localizer.Localize(token) + suffix;

            if (upperCase)
                str = str.ToUpper();

            GetComponent<TextMeshProUGUI>().text = str;

            Destroy(this);
        }
    }
}
