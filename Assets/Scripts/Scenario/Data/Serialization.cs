/*
 *  Responsible for file I/O and seralization necessary to import Scenarios from content modules
 */

#region Resources

using UnityEngine;
using System;
using System.Xml.Serialization;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

#endregion

namespace RemoteEducation.Scenarios
{

    /// <summary>
    ///     Handles seralization necessary to import Scenarios from content modules
    /// </summary>
    public static class Serialization
    {

        /// <summary>Indicates file format used for serialization</summary>
        public enum FileType
        {
            XML,
            JSON
        }

        private static readonly string ResourcePath = "Scenarios";

        #region Public Methods

        /// <summary>
        ///     Loads all resources in any Resources/Scenarios/ directories as TextAssets and attempts to parse
        ///     each TextAsset as a properly configured XML file containing <see cref="Scenario"/> data
        /// </summary>
        /// <param name="fileType">The markup type to use for deserialization</param>
        /// <returns>List of all Scenarios retrieved through the resource loading and deserialization process</returns>
        public static List<Scenario> GetScenarios(FileType fileType)
        {
            Debug.Log(string.Format("Loading all Scenarios found in Resources/{0}", ResourcePath));
            TextAsset[] assets = Resources.LoadAll<TextAsset>(ResourcePath);

            List<Scenario> data = new List<Scenario>();
            
            foreach (TextAsset file in assets)
            {
                if (TryImportScenarioXML(file, out Scenario scenario))
                {
                    scenario.OriginalFileName = file.name;
                    data.Add(scenario);
                }
            }

            return data;
        }

        
        /// <summary>
        ///     Attempts to deserialize the contents of <paramref name="file"/> into a <see cref="Scenario"/>,
        ///     which is then passed out of the method in the <paramref name="scenario"/> <c>out</c> parameter
        /// </summary>
        /// <param name="file">The TextAsset which contains Scenario data as XML</param>
        /// <param name="scenario">Contains the contents of <paramref name="file"/> as a Scenario, if <paramref name="file" /> is valid</param>
        /// <returns>Whether the deserialization of <paramref name="file"/> into a Scenario was successful</returns>
        private static bool TryImportScenarioXML(TextAsset file, out Scenario scenario)
        {
            scenario = null;

            if (file == null || file.text.Length == 0)
            {
                Debug.LogWarning(string.Format("Serialization: The file '{0}' is not a valid Scenario", file.name));
                return false;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(Scenario));

#pragma warning disable IDE0063 // Unity does not support the "simple 'using' statement" language feature
            using (StringReader reader = new StringReader(file.text))
#pragma warning restore IDE0063
            {
                try
                {
                    scenario = serializer.Deserialize(reader) as Scenario;
                    return true;
                }
                catch (InvalidOperationException e)
                {
                    Debug.LogWarning(string.Format("Serialization: XML syntax/type error detected in file '{0}' : {1}", file.name, e.Message));
                    return false;
                }
                catch (Exception e)
                {
                    Debug.LogWarning(string.Format("Serialization: There was an error reading Scenario data from file '{0}' : {1}", file.name, e.Message));
                    return false;
                }
            }
        }


        /// <summary>
        ///     Removes "(Clone)" or "(#)" from the name of an interactable object when it is being serialized,
        ///     so that when it is imported from file again, the correct prefab can be found in the Resources 
        ///     folder by the name held in the given string.
        /// </summary>
        /// <remarks>
        ///     This method may be better suited in the <see cref="Helpers.Unity"/> library, as it has applications
        ///     in other areas of the project
        ///     <para />
        ///     It should also be expanded to remove the "(Instance)" trailing string
        /// </remarks>
        /// <param name="name">The string to be cleaned</param>
        public static void CleanName(ref string name)
        {
            if (name.Contains("(Clone)"))
            {
                name = name.Remove(name.Length - 7);
            }
            else if (Regex.IsMatch(name, "[\\s][(]\\d[)]$"))
            {
                name = name.Remove(name.Length - 4);
            }
        }

    #endregion
    }
}
