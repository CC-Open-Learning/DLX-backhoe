using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace RemoteEducation.Scenarios.Inspectable
{

    public class StateLogger : MonoBehaviour
    {
        public static string CurrentStateLog { get; private set; }

        [Serializable] public class DebugData {
            public string Module;
            public string Branch;
            public string Commit;
            public List<DebugElement> DebugElements;
        }
        [Serializable] public class DebugElement
        {
            public string Name;
            public string ElementName;
            public string StateMessage;
            public int StateIdentifier;
        }

        private void Awake()
        {
            InspectableManager.OnElementSetup += (inspectableElements) => CreateLogFile(inspectableElements);
        }

        public static DebugData GetDebugData(string loadPath) {
            try
            {
                using (StreamReader file = new StreamReader(loadPath))
                {
                    string contents = file.ReadToEnd();
                    DebugData debugData = JsonUtility.FromJson<DebugData>(contents);
                    return debugData;
                }
            }
            catch (Exception e) {
                Debug.LogError(e.Message);
                return null;
            }
        }

        private async void CreateLogFile(List<InspectableElement> inspectableElements)
        {
            string moduleName = ScenarioManager.Instance.CurrentScenario.Module;
            int unixTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            string directoryPath = Application.dataPath + $"/Logs/{moduleName}";
            string path = $"{directoryPath}/{DateTime.Now:M-d-yyyy}_{unixTimestamp % 100000}.json";
            CurrentStateLog = path;

            try
            {
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }

            var debugData = new DebugData();
            debugData.Module = moduleName;
            debugData.Branch = Git.GetBranch();
            debugData.Commit = Git.GetCommit();
            debugData.DebugElements = new List<DebugElement>();

            foreach (InspectableElement ie in inspectableElements)
            {
                var de = new DebugElement();
                de.Name = ie.FullName;
                de.ElementName = ie.GetComponent<ElementSpawningData>().ElementName;
                de.StateMessage = ie.StateMessage;
                de.StateIdentifier = ie.StateIdentifier;
                debugData.DebugElements.Add(de);
            }

            string json = JsonUtility.ToJson(debugData, true);

            using (StreamWriter file = new StreamWriter(path)) {
                await file.WriteLineAsync(json);
            }

            Debug.Log($"Debug File Created at {path}");
        }
    }
}