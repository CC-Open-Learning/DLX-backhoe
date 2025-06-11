/*
 * SUMMARY: This file contains the SidePanelclass. 
 *          The purpose of this file is to animate the side engine panel when TogglePanelOpen is triggered.
 */

using RemoteEducation.Interactions;
using RemoteEducation.Localization;
using RemoteEducation.Scenarios;
using RemoteEducation.UI;
using System.Collections;
using UnityEngine;

namespace RemoteEducation.Modules.HeavyEquipment
{
    /// <summary>Defines the behaviour of how the engine side panel of a 416F2 should open and close.</summary>
    /// <remarks>The speed of which it opens and close is dependent on a upwards curve starting from 0 to 1</remarks>
    public sealed class SidePanel : SemiSelectable, IInitializable, ITaskable
    {
        public enum PanelSides
        {
            Left,
            Right
        }

        [SerializeField, Tooltip("The front arm lift lock.")]
        private LiftLock liftLock;

        /// <value>Two transformations to hold our destination and source locations used to dictate movement.</value>
        private Transform FromPosition;
        private Transform ToPosition;

        [Header("Transformations")]
        [SerializeField, Tooltip("Transform representing the open position of the panel.")]
        private Transform OpenTransform;

        [SerializeField, Tooltip("Transform representing the mid animation position of the panel.")]
        private Transform MidTransform;

        [SerializeField, Tooltip("Transform representing the closed position of the panel.")]
        private Transform CloseTransform;

        [Header("panel Animation Elements")]
        [SerializeField, Tooltip("Curve to dictate how the panel should open.")]
        private AnimationCurve PanelCurve;

        [SerializeField, Tooltip("Panel side.")]
        private PanelSides panelSide;

        /// <summary>If the panel is open or closed.</summary>
        private bool isOpen = false;
        public bool IsOpen => isOpen;

        [SerializeField, Tooltip("Time it should take to finish the animation created in the PanelCurve.")]
        private float AnimationDuration = 2;

        /// <summary>If the panel currently opening or closing.</summary>
        private bool IsAnimating;

        public TaskableObject Taskable { get; private set; }

        public void Initialize(object input = null)
        {
            transform.position = CloseTransform.position;
            transform.localRotation = CloseTransform.localRotation;

            //this is just for testing
            OnSelect += TogglePanelOpen;

            Taskable = new TaskableObject(this, panelSide.ToString());
        }

        /// <summary>This method toggles the panel's state and sets animation properties as well as trigger animation.</summary>
        /// <remarks>The panel can only be open or closed and this moment</remarks>
        public void TogglePanelOpen()
        {
            //Checks if the panel is already animating and if so, then exit and don't change anything
            if (IsAnimating)
                return;

            if (isOpen)
            {
                //Checks if panel is currently open and if so, set animation variables to start closing
                FromPosition = OpenTransform;
                ToPosition = CloseTransform;
            }
            else
            {
                //Checks if panel is currently close and if so, set animation variables to start opening
                FromPosition = CloseTransform;
                ToPosition = OpenTransform;
            }

            //Start animating the panel
            StartCoroutine(PanelAnimation());
            IsAnimating = true;
        }

        /// <summary>This method does the panel animation.</summary>
        /// <remarks>The panel currently handles opening and closing</remarks>
        private IEnumerator PanelAnimation()
        {
            //Start timer and end timer for our animation
            float StartTimer = Time.time;
            float EndTimer = StartTimer + AnimationDuration;

            if (!isOpen)
            {
                while (Time.time <= EndTimer)
                {
                    //Get a ratio from current time and total duration
                    float TimeRatio = (Time.time - StartTimer) / AnimationDuration;

                    //Get the object position in span of 0 to 1 and set the lerp to that position
                    float ObjPosition = PanelCurve.Evaluate(TimeRatio);

                    transform.position = Vector3.Lerp(FromPosition.position, MidTransform.position, ObjPosition);
                    yield return null;
                }

                StartTimer = Time.time;
                EndTimer = StartTimer + AnimationDuration;

                while (Time.time <= EndTimer)
                {
                    //Get a ratio from current time and total duration
                    float TimeRatio = (Time.time - StartTimer) / AnimationDuration;

                    //Get the object position in span of 0 to 1 and set the lerp to that position
                    float ObjPosition = PanelCurve.Evaluate(TimeRatio);

                    transform.position = Vector3.Lerp(MidTransform.position, ToPosition.position, ObjPosition);
                    transform.localRotation = Quaternion.Lerp(MidTransform.localRotation, ToPosition.localRotation, ObjPosition);
                    yield return null;
                }
            }
            else
            {
                while (Time.time <= EndTimer)
                {
                    //Get a ratio from current time and total duration
                    float TimeRatio = (Time.time - StartTimer) / AnimationDuration;

                    transform.position = Vector3.Lerp(FromPosition.position, MidTransform.position, TimeRatio);
                    transform.localRotation = Quaternion.Lerp(FromPosition.localRotation, MidTransform.localRotation, TimeRatio);
                    yield return null;
                }

                StartTimer = Time.time;
                EndTimer = StartTimer + AnimationDuration;

                while (Time.time <= EndTimer)
                {
                    //Get a ratio from current time and total duration
                    float TimeRatio = (Time.time - StartTimer) / AnimationDuration;

                    transform.position = Vector3.Lerp(MidTransform.position, ToPosition.position, TimeRatio);
                    yield return null;
                }
            }

            //Set the final position and set animating to false
            transform.localRotation = ToPosition.localRotation;
            IsAnimating = false;
            isOpen = !isOpen;
            Taskable.PokeTaskManager();
        }

        public object CheckTask(int checkType, object inputData)
        {
            switch (checkType)
            {
                case (int)TaskVertexManager.CheckTypes.Bool:
                    return isOpen;

                default:
                    Debug.LogError("Invalid check type passed into FrontPanel");
                    break;
            }

            return null;
        }
    }
}