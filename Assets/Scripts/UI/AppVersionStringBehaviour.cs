
using TMPro;
using UnityEngine;

namespace RemoteEducation.UI
{
    /// <summary>
    ///     Updates a TextMeshProUGUI component with the application version number
    /// </summary>
    public class AppVersionStringBehaviour : MonoBehaviour
    {
        [SerializeField]
        private string Prefix = string.Empty;

        [SerializeField]
        private TextMeshProUGUI versionText;

        // Start is called before the first frame update
        private void Awake()
        {
            if (versionText)
            {
                versionText.text = Prefix + Application.version;
            }
        }
    }
}