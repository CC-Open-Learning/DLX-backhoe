using RemoteEducation.Interactions;
using RemoteEducation.Scenarios;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RemoteEducation.Modules.HeavyEquipment
{
    public class ClipboardController : MonoBehaviour, IInitializable, ITaskable
    {
        [SerializeField, Tooltip("Clipboard items")]
        private List<ClipboardItem> clipboardItems;

        [SerializeField, Tooltip("Clipboard items")]
        private GaugePanelController GaugePanel;

        [Tooltip("Prompt of the Panel")]
        public TMP_InputField UserInputField;

        private int correct = 0;
        private int incorrect = 0;
        private bool evaluated = false;

        public TaskableObject Taskable { get; private set; }

        public void Initialize(object input = null)
        {
            Taskable = new TaskableObject(this);
            gameObject.SetActive(false);
            UserInputField.characterLimit = 6;
        }

        public void Submit()
        {
            correct = 0;
            incorrect = 0;
            foreach (ClipboardItem item in clipboardItems)
            {
                GaugePanelLight iterLight = GaugePanel.GetPanelLight(item.GetClipItemType());
                if (iterLight == null)
                {
                    continue;
                }
                else
                {
                    if(iterLight.GetIndicatorStatus() == item.CheckYes() || iterLight.GetIndicatorStatus() == item.CheckNo())
                    {
                        correct++;
                    }
                    else
                    {
                        incorrect++;
                    }
                }
            }

            int hours = 0;
            if(UserInputField.text != "")
            {
                hours = int.Parse(UserInputField.text);
            }

            if(hours == GaugePanel.GetHoursRunning())
            {
                Debug.Log("Checklist task complete~!");
            }

            if(clipboardItems.Count == correct + incorrect && hours != 0)
                evaluated = true;
            Taskable.PokeTaskManager();
        }

        public object CheckTask(int checkType, object inputData)
        {
            switch (checkType)
            {
                case (int)TaskVertexManager.CheckTypes.Bool:
                    return evaluated;
                default:
                    Debug.LogError("Invalid check type passed into FrontHood");
                    break;
            }

            return null;
        }
    }
}