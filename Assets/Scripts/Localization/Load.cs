using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using RemoteEducation.Extensions;

namespace RemoteEducation.Localization
{
    public static partial class Localizer
    {
        // This function is like 99% error handling and warnings. It would be like 10 lines otherwise.
        /// <summary>
        /// Loads all localized strings for a given module and language into memory for use with Localize.
        /// </summary>
        private static bool LoadLocalization(string category, string language, bool clearDictionary = false)
        {
            category = category.Capitalize();

            if (!clearDictionary && loadedCategories.Contains(category))
            {
                Debug.LogWarning("<color=#0099ff>Localizer</color> : Category <b>" + category + "</b> already loaded!");
                return false;
            }

            if (string.IsNullOrEmpty(language))
            {
                Debug.LogWarning("<color=#0099ff>Localizer</color> : No language specified in LoadLocalization!");
                return false;
            }

            if (string.IsNullOrEmpty(category))
            {
                Debug.LogWarning("<color=#0099ff>Localizer</color> : No category specified in LoadLocalization!");
                return false;
            }

            string subPath = "Strings/" + category + "/" + language;

            string[] lines = languageLoader.LoadLanguage(subPath);

            if (lines == null || lines.Length < 1)
                return false;

            if(clearDictionary)
                ResetLocalizationDictionary();

            int i = 0;

            foreach (string line in lines)
            {
                i++;

                // Comments and lines that don't have any double quotes.
                if (line.StartsWith("//") || !line.Contains("\""))
                    continue;

                // Match everything within quotes, does not respect nested quotes that use escape characters, use single quotes instead.
                MatchCollection kvp = Regex.Matches(line, "\".*?\"");

                if (kvp.Count != 2)
                {
                    Debug.LogWarning("<color=#0099ff>.." + subPath + "</color> : Line " + i + ", does not contain two sets of double quotes!");
                    continue;
                }

                // Remove Quotes from Token
                string key = kvp[0].ToString().TrimStart('\"').TrimEnd('\"');

                if (string.IsNullOrEmpty(key))
                {
                    Debug.LogWarning("<color=#0099ff>.." + subPath + "</color> : Line " + i + ", token string is empty!");
                    continue;
                }

                // Remove Quotes from Localized text
                string value = kvp[1].ToString().TrimStart('\"').TrimEnd('\"');

                if (string.IsNullOrEmpty(value))
                    Debug.LogWarning("<color=#0099ff>.." + subPath + "</color> : Line " + i + ", localized string for token <b>\"" + key + "\"</b> is empty!");

                // Parse newlines that are in our sentence.
                if (value.Contains("\\n"))
                    value = value.Replace("\\n", Environment.NewLine);

                // Add our category to the key, e.g. "Engine.Quit" is a valid key, not just "Quit."
                key = category + "." + key;

                // Prevent case sensitive localization tokens. Still nice to write them as Engine.Quit rather than engine.quit but both will work.
                key = key.ToLower();

                // Warn devs if the localization token has already been used earlier in the file.
                // If not, add the token and localized text to the Dictionary.
                if (localization.ContainsKey(key))
                {
                    Debug.LogWarning("<color=#0099ff>.." + subPath + "</color> : Line " + i + ", localized string for token <b>\"" + key + "\"</b> already exists! <b>Ignoring this line.</b>");
                    continue;
                }
                else
                {
                    localization.Add(key, value);
                }
            }

            // Lets us track if we've loaded this module into the Dictionary already so we can prevent the function from running again later if it has.
            loadedCategories.Add(category);

            return true;
        }

        private static void ResetLocalizationDictionary()
        {
            localization.Clear();
            loadedCategories.Clear();
        }
    }
}
