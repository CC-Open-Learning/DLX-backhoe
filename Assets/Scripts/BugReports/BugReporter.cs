using UnityEngine;
using TMPro;
using RemoteEducation.Scenarios;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace RemoteEducation.Editor.BugReporting
{
    /// <summary>Handles the Unity-Integrated side of the Bug Reporting system.</summary>
    /// <remarks>Contains the UI integrations and Unity messages.</remarks>
    public class BugReporter : MonoBehaviour
    {
        public static OnBugReporterOpenEvent OnBugReporterOpen;

        private const string ERROR_EMPTY_FIELD = "One or more fields are empty.";
        private const string ERROR_BAD_REQUEST = "Login Info or Project Key not found.";
        private const string ERROR_DEFAULT = "Could not create ticket.";
        private const string TICKET_SUBMITTED = "Creating ticket...";
        private const string DEFAULT_PROJECT_KEY = "CE";

        /// <summary>Path to the temporary file generated to store the current run's console log.</summary>
        /// <remarks>This file is generated upon submitting a bug report.</remarks>
        public static string TempConsoleLogFilePath { get; private set; }

        [Tooltip("Text to display messages back to developers from the menu.")]
        [SerializeField] private TextMeshProUGUI feedback;

        [Tooltip("Button used to open the Bug Report Menu.")]
        [SerializeField] private GameObject bugReportButton;

        [Tooltip("Bug Report Menu GameObject.")]
        [SerializeField] private GameObject menu;

        [Tooltip("Field for the Atlassian email of the developer.")]
        [SerializeField] private TMP_InputField userNameField;

        [Tooltip("Field for the Jira API key of the developer.")]
        [SerializeField] private TMP_InputField APIKeyField;

        [Tooltip("Field for the Key associated with the project to submit to in Jira.")]
        [SerializeField] private TMP_InputField projectKey;

        [Tooltip("Field for the description of the ticket.")]
        [SerializeField] private TMP_InputField descriptionField;

        [Tooltip("Field for the summary of the ticket.")]
        [SerializeField] private TMP_InputField summaryField;

        /// <summary>Ticket reporting platform to use for submitting bug reports.</summary>
        private static ITicketPlatform ticketPlatform;

        /// <summary>Stores all console log messages, their severity, and callstack.</summary>
        private List<LogMessage> logMessages = new List<LogMessage>();

        void Awake()
        {
            TempConsoleLogFilePath = $"{Application.dataPath}/Logs/consoleLogTemp.txt";

            ticketPlatform = new Jira();

            SetMenuState(false);
            PopulateCredentialFields();
            APIKeyField.onEndEdit.AddListener(SaveCredentials);
            userNameField.onEndEdit.AddListener(SaveCredentials);
            feedback.text = "";

            Application.logMessageReceived += LogConsoleMessage;
        }

        /// <summary>Logs messages from Unity's console to a list allowing it to be saved to a file.</summary>
        /// <param name="description">The message as it appears in the Unity Editor console window.</param>
        /// <param name="stackTrace">Full stack trace with file names and lines.</param>
        /// <param name="type">Whether the message is a log, warning, error, or exception.</param>
        private void LogConsoleMessage(string description, string stackTrace, LogType type)
        {
            logMessages.Add(new LogMessage(description, stackTrace, type));
        }

        /// <summary>Hook for when either login information field is edited.</summary>
        private void SaveCredentials(string _)
        {
            ticketPlatform.SetCredentials(userNameField.text, APIKeyField.text);
        }

        /// <summary>Fills the login information fields with saved credentials if there are any.</summary>
        private void PopulateCredentialFields()
        {
            if (!ticketPlatform.HasCredentials())
                return;

            var (user, pass) = ticketPlatform.GetCredentials();

            userNameField.text = user;
            APIKeyField.text = pass;
        }

        /// <summary>Creates and uploads bug report to the given <see cref="ticketPlatform"/>.</summary>
        public async void CreateBugReport()
        {
            if (AnyFieldIsEmpty(userNameField, APIKeyField, projectKey, descriptionField, summaryField))
            {
                feedback.text = ERROR_EMPTY_FIELD;
                return;
            }

            ticketPlatform.SetCredentials(userNameField.text, APIKeyField.text);

            if (!ticketPlatform.HasCredentials())
            {
                Debug.LogError("No credentials supplied, cannot send bug report.");
                return;
            }

            feedback.text = TICKET_SUBMITTED;

            SaveConsoleLogToFile();

            var result = await ticketPlatform.SendBugReport(CollectBugReportData());

            switch(result.StatusCode)
            {
                case System.Net.HttpStatusCode.Created:
                    SetMenuState(false);
                    feedback.text = "";
                    break;

                case System.Net.HttpStatusCode.BadRequest:
                    feedback.text = ERROR_BAD_REQUEST;
                    break;

                default:
                    feedback.text = ERROR_DEFAULT;
                    break;
            }
        }

        /// <summary>Converts the <see cref="logMessages"/> list to a file.</summary>
        /// <remarks>Exceptions are set to provide a full stack trace.</remarks>
        private void SaveConsoleLogToFile()
        {
            using (var file = new StreamWriter(TempConsoleLogFilePath))
            {
                var line = "";

                foreach (LogMessage msg in logMessages)
                {
                    if (msg.logType == LogType.Log)
                        line = msg.condition;
                    else
                        line = $"{msg.logType}: {msg.condition}";

                    file.WriteLine(line);

                    if (msg.logType == LogType.Exception)
                        file.WriteLine(msg.stackTrace);

                    file.WriteLine();
                }
            }
        }

        /// <summary>Checks if any of the input fields in the bug reporter have been left empty.</summary>
        /// <returns><see langword="true"/> if any field is empty.</returns>
        private bool AnyFieldIsEmpty(params TMP_InputField[] fields)
        {
            return fields.Any(e => string.IsNullOrEmpty(e.text));
        }

        /// <summary>Opens or closes the menu and sets the visibility of the bug report button.</summary>
        public void SetMenuState(bool open)
        {
            menu.SetActive(open);
            bugReportButton.SetActive(!open);

            projectKey.text = DEFAULT_PROJECT_KEY;

            if (AnyScenarioIsActive())
                projectKey.text = ScenarioManager.Instance.CurrentScenario.ProjectKey;

            OnBugReporterOpen?.Invoke(open);
        }

        /// <summary>Checks if any scenario has been loaded.</summary>
        /// <returns><see langword="true"/> if any scenario has been loaded.</returns>
        private bool AnyScenarioIsActive()
        {
            return ScenarioManager.Instance != null &&
                ScenarioManager.Instance.CurrentScenario != null && 
                !string.IsNullOrEmpty(ScenarioManager.Instance.CurrentScenario.ProjectKey);
        }

        /// <summary>Assembles the bug report data structure from the input fields filled out by the developer.</summary>
        private BugReportData CollectBugReportData()
        {
            return new BugReportData { Description = descriptionField.text, ProjectKey = projectKey.text, Summary = summaryField.text };
        }

        /// <summary>Opens a link to the API Token generation page.</summary>
        /// <remarks>These tokens are required in place of passwords for security reasons.</remarks>
        public void OpenLoginLink()
        {
            Application.OpenURL("https://id.atlassian.com/manage-profile/security/api-tokens");
        }
    }
}
