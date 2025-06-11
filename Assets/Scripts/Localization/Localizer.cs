using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using RemoteEducation.Extensions;

namespace RemoteEducation.Localization
{
    public static partial class Localizer
    {
        public const string ERRORSTR = "STRING MISSING!";
        private const string REGISTRY_TOKEN = "language";
        private const string ENGINE_CATEGORY = "engine";

        private static string currentLanguage;

        private static Dictionary<string, string> localization = new Dictionary<string, string>();

        private static ILanguageLoader languageLoader = new LanguageLoaderFromResources();

        // Searching a List is faster than this HashSet until we have more than a handful of categories/modules loaded. Does it matter for this? Not really!
        private static HashSet<string> loadedCategories = new HashSet<string>();

        /// <summary>Loads our initial language from the system registry.</summary>
        static Localizer()
        {
            if (!string.IsNullOrEmpty(currentLanguage))
            {
                Debug.LogWarning("<color=#0099ff>Localizer</color> : Init() has already been called previously.");
                return;
            }

            string language;

            if (!PlayerPrefs.HasKey(REGISTRY_TOKEN))
            {
                language = "english";
            }
            else
            {
                language = PlayerPrefs.GetString(REGISTRY_TOKEN);
            }

            SetLanguage(language, false);
        }

        /// <summary>Looks up a string token to find the word or sentence it refers to in the current language.</summary>
        /// <returns>Localized text if the token is found and an error string if not.</returns>
        public static string Localize(string text)
        {
            // Keys are all converted to lower case for the dictionary, this prevents case sensitive keys.
            text = text.ToLower();
            string val;
            localization.TryGetValue(text, out val);
            if (val == null)
            {
                if (CheckForValidUnloadedModule(text))
                {
                    return Localize(text);
                }

                Debug.LogWarning("<color=#0099ff>Localizer</color> : Cannot find string for token <b>" + text + "</b>!");
                return ERRORSTR;
            }
            return val;
        }

        /// <summary>Looks up a string token to find the word or sentence it refers to in the current language.</summary>
        /// <returns>Localized text if the token is found and the original token if not.</returns>
        public static string LocalizePassthrough(string text)
        {
            var temp = text;

            // Keys are all converted to lower case for the dictionary, this prevents case sensitive keys.
            text = text.ToLower();
            string val;
            localization.TryGetValue(text, out val);
            if (val == null)
            {
                if (CheckForValidUnloadedModule(text))
                {
                    return LocalizePassthrough(temp);
                }

                return temp;
            }
            return val;
        }

        /// <summary>Checks if a string token's module prefix is valid and loads it into memory if it is.</summary>
        /// <returns><see langword="true"/> if the module name is valid.</returns>
        private static bool CheckForValidUnloadedModule(string text)
        {
            if (text.Contains(".") && LoadLocalization(text.Split('.')[0], currentLanguage))
            {
                Debug.Log("<color=#0099ff>Localizer</color> : Category not precached for <b>" + text + "</b>, loading.");
                return true;
            }

            return false;
        }

        /// <summary>Sets the current language strings to be loaded into memory.</summary>
        /// <remarks>Replaces old language strings if necessary.</remarks>
        public static void SetLanguage(string language, bool saveToPlayerPrefs)
        {
            // Safety in case someone loads english as "English" somewhere and "ENGLISH" somewhere else.
            language = language.ToLower();

            // We are already on this language.
            if (language == currentLanguage)
            {
                Debug.LogWarning("<color=#0099ff>Localizer</color> : SetLanguage called for " + language.Capitalize() + " but we are already on that language!");
                return;
            }

            if (!LoadLocalization(ENGINE_CATEGORY, language, true))
                return;

            currentLanguage = language;

            if(saveToPlayerPrefs)
                PlayerPrefs.SetString(REGISTRY_TOKEN, language);
        }

        /// <summary>Resets all localization collections and current language. Sets <see cref="languageLoader"/>.</summary>
        public static void Reset(ILanguageLoader languageLoader)
        {
            currentLanguage = null;

            localization = new Dictionary<string, string>();

            Localizer.languageLoader = languageLoader;

            loadedCategories = new HashSet<string>();
        }
    }
}
