using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace VARConsole
{
    [DisallowMultipleComponent]
    public class VARConsole : MonoBehaviour
    {
        public static OnCommandEvent OnCommandSubmitted;
        public static OnConsoleOpenEvent OnConsoleOpen;

        [SerializeField] private GameObject logLinePrefab;
        [SerializeField] private Transform logLineContainer;
        [SerializeField] private GameObject consoleParent;
        [SerializeField] private TMP_InputField input;
        [SerializeField] private ScrollRect scrollRect;

        private static bool initialized = false;

        private static Dictionary<string, string> commandDescriptions;

        private static VARConsole singleton;

        private static List<string> commandHistory = new List<string>();
        private int currentHistoryIndex;

        private void Awake()
        {
            if(initialized)
            {
                Destroy(gameObject);
                return;
            }
            singleton = this;
            consoleParent.SetActive(false); // Hide console visuals if they aren't already disabled in the scene or prefab.

            DontDestroyOnLoad(gameObject);
            initialized = true;

            commandDescriptions = new Dictionary<string, string>();

            OnCommandSubmitted += LogCommand;
            input.onValidateInput += ValidateInput;

            DefaultCommands.AddDefaultCommands();
        }

        public static bool AddCommand(ICommand command)
        {
            if(commandDescriptions.ContainsKey(command.Prefix))
            {
                Debug.LogError($"VARConsole: Duplicate console command for {command.Prefix}.");
                return false;
            }

            commandDescriptions.Add(command.Prefix, command.Description);
            OnCommandSubmitted += command.RunCommand;
            return true;
        }

        public static bool RemoveCommand(ICommand command)
        {
            OnCommandSubmitted -= command.RunCommand;
            return commandDescriptions.Remove(command.Prefix);
        }

        private char ValidateInput(string text, int charIndex, char addedChar)
        {
            // If we press the ` or ~ key we want to exit the console rather than add it to the input.
            if(addedChar == '`' || addedChar == '~')
                return '\0';

            return addedChar;
        }

        private void LogCommand(OnCommandEventArgs e)
        {
            if (e.Command.Equals("clear"))
            {
                ClearConsole();
                e.HideMessage = true;
            }
            else
                CreateLogMessage(e.Command);

            // Resetting the text field and scroll position.
            input.text = "";
            input.ActivateInputField();

            StartCoroutine(WaitOneFrameAndResetScrollPosition());
        }

        private void ClearConsole()
        {
            for (int i = 0; i < logLineContainer.childCount; i++)
            {
                Destroy(logLineContainer.GetChild(i).gameObject);
            }
        }

        private void CreateLogMessage(string message)
        {
            var lines = message.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                var log = Instantiate(logLinePrefab, logLineContainer);
                log.transform.localScale = Vector3.one; // Parenting UI Elements can cause odd scaling issues so we reset it.

                var commandText = log.GetComponent<TextMeshProUGUI>();
                commandText.text = lines[i];

                // Resize the log's layout element to always match the font size of the text component on the message.
                // Then if someone down the line changes the font size, the layout won't break.
                var le = log.GetComponent<LayoutElement>();
                le.preferredHeight = commandText.fontSize;
                le.minHeight = commandText.fontSize;
            }    
        }

        private IEnumerator WaitOneFrameAndResetScrollPosition()
        {
            yield return null;
            scrollRect.verticalNormalizedPosition = 0f;
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.BackQuote) || Input.GetKeyUp(KeyCode.Tilde)) // Tilde key doesn't work for everyone, also added BackQuote as they usually are the same key.
                ToggleConsole();

            if (!consoleParent.activeSelf)
                return;

            HandleHistory();

            HandleMessageSubmission();
        }

        private void HandleMessageSubmission()
        {
            if (Input.GetKeyUp(KeyCode.Return))
            {
                var command = input.text;

                if (string.IsNullOrEmpty(command))
                    return;

                command = command.TrimEnd('\n', ' ').ToLower();

                var e = new OnCommandEventArgs(command);
                OnCommandSubmitted?.Invoke(e);

                if (!e.HideMessage)
                {
                    var msg = string.IsNullOrEmpty(e.Message) ? "Command not found." : e.Message;

                    CreateLogMessage(msg);
                }

                // Only record history if the command is different than the last command.
                if (commandHistory.Count > 0 && command.Equals(commandHistory[commandHistory.Count - 1]))
                    return;

                commandHistory.Add(command);
                currentHistoryIndex = commandHistory.Count;
            }
        }

        private void HandleHistory()
        {
            if (commandHistory.Count <= 0)
                return;

            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                currentHistoryIndex--;

                if (currentHistoryIndex < 0)
                    currentHistoryIndex = commandHistory.Count - 1;
            }
            else if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                currentHistoryIndex++;

                if (currentHistoryIndex >= commandHistory.Count)
                    currentHistoryIndex = 0;
            }
            else
            {
                return;
            }

            input.text = commandHistory[currentHistoryIndex];
            input.caretPosition = input.text.Length;
        }

        private void ToggleConsole()
        {
            consoleParent.SetActive(!consoleParent.activeSelf);

            if (consoleParent.activeSelf)
                input.ActivateInputField();

            OnConsoleOpen?.Invoke(consoleParent.activeSelf);
        }

        public static void ListOptions()
        {
            foreach (var entry in commandDescriptions)
            {
                singleton.CreateLogMessage($"{entry.Key} - {entry.Value}");
            }
        }

        private void OnDestroy()
        {
            OnCommandSubmitted -= LogCommand;
        }
    }
}