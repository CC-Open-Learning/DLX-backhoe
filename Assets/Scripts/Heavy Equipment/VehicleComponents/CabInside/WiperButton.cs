using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RemoteEducation.Scenarios;
using RemoteEducation.Scenarios.Inspectable;
using RemoteEducation.Interactions;

namespace RemoteEducation.Modules.HeavyEquipment
{
    public class WiperButton : SemiSelectable, IInitializable, ITaskable
    {
        [Tooltip("Windshield Wiper Object")]
        [SerializeField] private WindShieldWiperController controller;

        private bool checkedBoolean = false;

        public TaskableObject Taskable { get; private set; }

        public void Initialize(object input = null)
        {
            Taskable = new TaskableObject(this);
            OnSelect += WiperToggle;
        }

        private void WiperToggle()
        {
            controller.ToggleWiper();
            checkedBoolean = true;
            Taskable.PokeTaskManager();
        }

        public void SetBoolean(bool state)
        {
            checkedBoolean = state;
        }

        public object CheckTask(int checkType, object inputData)
        {
            switch (checkType)
            {
                case (int)TaskVertexManager.CheckTypes.Bool:
                    return checkedBoolean;

                default:
                    Debug.LogError("A check type was passed to EdgeTakenTracker that it couldn't handle");
                    return null;
            }
        }
    }
}