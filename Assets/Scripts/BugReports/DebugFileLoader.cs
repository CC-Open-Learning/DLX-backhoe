using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using RemoteEducation.Scenarios;
using RemoteEducation.Scenarios.Inspectable;
using System;

namespace RemoteEducation.Editor.BugReporting
{
    public class DebugFileLoader : MonoBehaviour
    {
        [SerializeField, Tooltip("Button to open and close the menu.")] private GameObject openButton;
        [SerializeField, Tooltip("Button to load the given path.")] private GameObject loadButton;
        [SerializeField, Tooltip("Menu textbox to disable / enable.")] private GameObject menu;
        [SerializeField, Tooltip("Field for showing the selected path.")] private TMP_InputField fileString;
        [SerializeField, Tooltip("Label telling the user that the file hasn't been picked.")] private TextMeshProUGUI feedback;

        public static string DebugFile { get; private set; } = "";

#if !UNITY_EDITOR
        private void Awake()
        {
            gameObject.SetActive(false);

        }
#endif

        public void SetMenuState(bool open)
        {
            menu.SetActive(open);
            openButton.SetActive(!open);
        }

        public void PickDebugFile()
        {
#if UNITY_EDITOR
            string path = EditorUtility.OpenFilePanel("Open Debug File", Application.dataPath, "JSON");

            if (string.IsNullOrEmpty(path)) 
                return;

            DebugFile = path;
            fileString.text = path;
            feedback.enabled = false;
            loadButton.SetActive(true);
#endif
        }

        public void LoadScene()
        {
            string moduleName = StateLogger.GetDebugData(DebugFile).Module;
            try
            {
                ScenarioBuilder.CreatePersistentScenarioData(ScenarioBuilder.LoadFromFile().Find(x => x.Module == moduleName));
                StartCoroutine(ScenarioManager.LazyLoad());
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
}
