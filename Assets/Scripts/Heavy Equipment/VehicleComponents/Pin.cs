/*
 * SUMMARY: The purpose of this class is to model the behaviour and properties of the pins around the backhoe.
 *          Pins can be in 3 states, good (secure and appears to be accepting grease), not secure (retainer is
 *          missing), or appears to not be accepting grease. OnSelect the pins will display a toast message 
 *          of their state.
 */



using RemoteEducation.Interactions;
using RemoteEducation.Localization;
using RemoteEducation.Scenarios;
using RemoteEducation.UI;
using UnityEngine;



namespace RemoteEducation.Modules.HeavyEquipment
{
    public class Pin : SemiSelectable
    {
        /// <summary>Tracks what the current state of the pin is</summary>
        private State currentState;

        /// <summary>If pin is inspected.</summary>
        public bool IsInspected = false;

        public OtherInspectableObjects InspectorGroup;

        [SerializeField, Tooltip("Stage on scenariograph to poke")]
        private int pokeStage = 8;

        /// <summary>NotAcceptingGrease == Bad State 0. NotSecure == Bad State 1 for the Inspectable Element prefabs</summary>
        private enum State
        {
            Good = 0,
            NotAcceptingGrease = 1,
            NotSecure = 2
        }

        public void Setup()
        {
            OnSelect += CheckPin;
        }

        public void AttachToInspector(OtherInspectableObjects inspector)
        {
            InspectorGroup = inspector;
        }
    
    /// <summary>Method to set the private State variable to Good</summary>
        public void SetPinOkay()
        {
            currentState = State.Good;
        }

        /// <summary>Method to set the private State variable to NotSecure</summary>
        public void SetPinNotSecure()
        {
            currentState = State.NotSecure;
        }

        /// <summary>Method to set the private State variable to NotAcceptingGrease</summary>
        public void SetPinBlocked()
        {
            currentState = State.NotAcceptingGrease;
        }

        /// <summary>Displays a toast message indicating what state the pin is in</summary>
        private void CheckPin()
        {
            switch (currentState)
            {
                case State.NotSecure:
                    HEPrompts.CreateToast(Localizer.Localize("HeavyEquipment.PinsToastNotSecure"), HEPrompts.ShortToastDuration);
                    break;

                case State.NotAcceptingGrease:
                    HEPrompts.CreateToast(Localizer.Localize("HeavyEquipment.PinsToastNotAcceptingGrease"), HEPrompts.ShortToastDuration);
                    break;

                case State.Good:
                    HEPrompts.CreateToast(Localizer.Localize("HeavyEquipment.PinsToastGood"), HEPrompts.ShortToastDuration);
                    break;
            }

            IsInspected = true;
            InspectorGroup.Poke(pokeStage);
        }

        public bool CheckTask()
        {
            return IsInspected;
        }
    }
}