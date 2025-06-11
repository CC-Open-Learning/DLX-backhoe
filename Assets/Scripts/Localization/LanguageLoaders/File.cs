using System.IO;
using TMPro;
using UnityEngine;

namespace RemoteEducation.Localization
{
    public class LanguageLoaderFromFile : ILanguageLoader
    {
        /// <summary>Reads all localized string keyvalue pairs from the given relative file path.</summary>
        /// <param name="subPath">Relative path to text file within the StreamingAssets folder, without leading slash or file extension.</param>
        /// <returns>All localized string keyvalue pairs from the provided file.</returns>
        public string[] LoadLanguage(string subPath)
        {
            string path = Application.streamingAssetsPath + "/" + subPath + ".txt";

            if (!File.Exists(path))
            {
                Debug.LogWarning("<color=#0099ff>.." + subPath + "</color> : Localization File Not Found!");
                return null;
            }

            return File.ReadAllLines(path, System.Text.Encoding.UTF8);
        }
    }
}
