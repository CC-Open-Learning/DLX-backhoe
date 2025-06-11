using System.IO;
using TMPro;
using UnityEngine;

namespace RemoteEducation.Localization
{
    public class LanguageLoaderFromResources : ILanguageLoader
    {
        /// <summary>Reads all localized string keyvalue pairs from the given relative resources folder path.</summary>
        /// <param name="subPath">Relative path to text file within the Resources folder.</param>
        /// <returns>All localized string keyvalue pairs from the provided file.</returns>
        public string[] LoadLanguage(string subPath)
        {
            var load = Resources.Load<TextAsset>(subPath);

            if (load == null)
            {
                Debug.LogWarning("<color=#0099ff>.." + subPath + "</color> : Localization File Not Found in Resources!");
                return null;
            }

            return load.text.Split('\n');
        }
    }
}
