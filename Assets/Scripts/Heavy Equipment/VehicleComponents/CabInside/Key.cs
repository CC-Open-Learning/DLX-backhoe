using RemoteEducation.Interactions;
using RemoteEducation.Localization;
using RemoteEducation.Scenarios;
using RemoteEducation.UI.Tooltip;
using UnityEngine;

namespace RemoteEducation.Modules.HeavyEquipment
{
    [DisallowMultipleComponent]
    public sealed class Key : SemiSelectable, IInitializable, ITaskable
    {
        [SerializeField, Tooltip("Backhoe")]
        private BackhoeController backhoe;
        public TaskableObject Taskable { get; private set; }
        public bool Clicked = false;
        public bool CanTurnOnEngine = false;

        [SerializeField] private string toolTipToken;

        [SerializeField, Tooltip("Gauge Panel")]
        private GaugePanelController gaugePanel;

        public enum States
        {
            Off,
            PartiallyOn,
            On
        }

        public Key.States currentState = Key.States.Off;

        public void Initialize(object input = null)
        {
            SetToolTip();

            Taskable = new TaskableObject(this);
            OnSelect += ToggleClick;
            if (!HeavyEquipmentModule.ScenarioAttatched)
                CanTurnOnEngine = true;
        }

        public void ToggleClick()
        {
            if (gaugePanel.IsChanging || !CanTurnOnEngine)
                return;

            switch (currentState) {
                case States.Off:
                    backhoe.EngineSound.PlayBeep();
                    gaugePanel.StartEngine();
                    currentState = States.PartiallyOn;
                    break;
                case States.PartiallyOn:
                    backhoe.EngineSound.StartEngine();
                    currentState = States.On;
                    break;
                case States.On:
                    backhoe.EngineSound.StopEngine();
                    gaugePanel.StopEngine();
                    currentState = States.Off;
                    break;
                default:
                    Debug.LogError("Invalid State");
                    break;
            }

            Taskable.PokeTaskManager();
        }

        public object CheckTask(int checkType, object inputData)
        {
            switch (checkType)
            {
                case (int)TaskVertexManager.CheckTypes.Int:
                    return (int)currentState;
                default:
                    Debug.LogError("Invalid check type passed into FrontHood");
                    break;
            }

            return null;
        }

        private void SetToolTip()
        {
            if (gameObject.GetComponent<SimpleTooltip>() == null)
            {
                SimpleTooltip tooltip = gameObject.AddComponent<SimpleTooltip>();
                tooltip.tooltipText = Localizer.Localize(toolTipToken);
            }
        }
    }
}