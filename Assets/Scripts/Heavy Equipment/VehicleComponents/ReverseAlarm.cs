/*
 * SUMMARY: This file contains the ReverseAlarm class. The purpose this class is to allow the control
 *          lever to be moved into reverse to allow the reverse alarm to play. An animation also plays
 *          when clicking the lever, pulling it back.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RemoteEducation.Audio;
using RemoteEducation.Scenarios;
using RemoteEducation.Interactions;
using RemoteEducation.Scenarios.Inspectable;

namespace RemoteEducation.Modules.HeavyEquipment
{
    public class ReverseAlarm : SemiSelectable, IInitializable, IBreakable, ITaskable
    {
        [Tooltip("Reverse sound effect played when the backhoe is put into reverse.")]
        [SerializeField] private SoundEffect ReverseSoundEffect;

        [Tooltip("Rotation object of the control lever when pulled into reverse.")]
        [SerializeField] private Transform ReverseTransform;

        /// <summary> Flag to prevent the alarm playing if it is broken </summary>
        private bool reverseAlarmBroken;

        /// <summary> Allows the reverse sound to be toggled on and off </summary>
        private bool reverseAlarmPlaying = false;

        /// <summary> Prevents the reverse alarm playing if it has been set to broken </summary>
        private Quaternion neutralRotation;

        /// <summary> Prevents the reverse alarm playing if it has been set to broken </summary>
        private Quaternion reverseRotation;

        /// <summary>Is the lever on</summary>
        public bool IsLeverOn = false;

        public TaskableObject Taskable { get; private set; }

        /// <summary>Sets up input for the control lever</summary>
        public void Initialize(object input = null)
        {
            Taskable = new TaskableObject(this);
            neutralRotation = transform.localRotation;
            reverseRotation = ReverseTransform.localRotation;

            OnSelect += ToggleLever;
        }

        public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int breakMode)
        {
            reverseAlarmBroken = broken;

            inspectableElement.OnCurrentlyInspectableStateChanged += (active) =>
            {
                //When interactable, don't show the highlight for this script. Also
                //make it so that it can be interacted with at any time.
                ToggleFlags(!active, Flags.Highlightable | Flags.ExclusiveInteraction);
            };
        }

        /// <summary>Toggles the control lever between positions and the reverse alarm.</summary>
        public void ToggleLever()
        {
            StartCoroutine(RotateControlLever());
            if(!reverseAlarmBroken)
            {
                ToggleAlarm();
            }
        }

        /// <summary>Rotates the control lever from neutral to reverse or reverse to neutral.</summary>
        private IEnumerator RotateControlLever()
        {
            float startTime = Time.time;
            float endTime = startTime + 0.5f;
            float totalTime = endTime - startTime;

            Quaternion targetRotation = (transform.localRotation == neutralRotation) ? reverseRotation : neutralRotation;

            while (Time.time <= endTime && transform.localRotation != targetRotation)
            {
                float timeRatio = (Time.time - startTime) / totalTime;

                transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, timeRatio);

                yield return null;
            }
            IsLeverOn = !IsLeverOn;
            Taskable.PokeTaskManager();
        }

        /// <summary>Toggles the reverse alarm playing.</summary>
        private void ToggleAlarm()
        {
            if(!reverseAlarmPlaying)
                ReverseSoundEffect.Play(true);
            else
                ReverseSoundEffect.FadeOut(0f);
            
            reverseAlarmPlaying = !reverseAlarmPlaying;
        }

        public object CheckTask(int checkType, object inputData)
        {
            switch (checkType)
            {
                case (int)TaskVertexManager.CheckTypes.Bool:
                    return IsLeverOn;

                default:
                    Debug.LogError($"Invalid check type passed into {GetType().Name}");
                    break;
            }

            return null;
        }
    }
}