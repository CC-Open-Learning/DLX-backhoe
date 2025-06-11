using System;
using UnityEngine;

namespace RemoteEducation.Scenarios.Inspectable
{
    public class DynamicInspectableElement : InspectableElement
    {
        [Tooltip("An identifier used by IBreakable to differentiate between bad states. " +
            "A negative BadMode indicates a 'fake' bad state. Elements cannot be placed into a " +
            "'fake' bad state, but these states are shown in UI")]
        public int BadMode;

        public IBreakable Breakable { get; private set; }

        public override void Initialize(InspectableController parentController, InspectableObject parentObject, string checkListName, string fullName, int stateIdentifier)
        {
            base.Initialize(parentController, parentObject, checkListName, fullName, stateIdentifier);

            Breakable = GetComponent<IBreakable>();

            if (Breakable == null)
            {
                Debug.LogError("DynamicInspectableElement : Unable to find a component that implements the " + typeof(IBreakable).Name + ". This inspectable will not work without it.");
            }

            Breakable.AttachInspectable(this, State != InspectableState.Good, BadMode);
            OnCurrentlyInspectableStateChanged?.Invoke(CurrentlyInspectable);
        }

        public override InspectableElement AddElementToGameObject(GameObject gameObject)
        {
            DynamicInspectableElement newElement = gameObject.AddComponent<DynamicInspectableElement>();

            newElement.StateMessage = StateMessage;
            newElement.State = State;
            newElement.BadMode = BadMode;
            newElement.StateIdentifier = StateIdentifier;

            return newElement;
        }

        [Serializable]
        public struct Data
        {
            public string guid;
            public string Name;
            public string ElementName;
            public string StateMessage;
            public int StateIdentifier;
            public int Flags;
            public int Status;
            public int UserSelectedIdentifier;
            public bool CurrentlyInspectable;
            public int State;
            public int BadMode;
        }

        public Data Serialize(ElementSpawningData esd, string guid)
        {
            Data data;

            data.guid = guid;
            data.Name = FullName;
            data.ElementName = esd.ElementName;
            data.StateMessage = StateMessage;
            data.StateIdentifier = StateIdentifier;
            data.Flags = (int)GetFlags();
            data.Status = (int)CurrentStatus;
            data.UserSelectedIdentifier = UserSelectedIdentifier;
            data.CurrentlyInspectable = currentlyInspectable;
            data.State = (int)State;
            data.BadMode = BadMode;

            return data;
        }

        public void Deserialize(Data data)
        {
            FullName = data.Name;
            StateMessage = data.StateMessage;
            StateIdentifier = data.StateIdentifier;
            SetFlags(data.Flags);
            CurrentStatus = (Status)data.Status;
            UserSelectedIdentifier = data.UserSelectedIdentifier;
            currentlyInspectable = data.CurrentlyInspectable;
            State = (InspectableState)data.State;
            BadMode = data.BadMode;

            Breakable.AttachInspectable(this, State != InspectableState.Good, BadMode);
        }
    }
}