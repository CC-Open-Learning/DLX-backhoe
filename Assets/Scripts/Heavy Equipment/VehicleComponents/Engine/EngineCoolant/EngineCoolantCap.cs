/*
 * SUMMARY: This file contains the Engine Coolant Cap class which keeps track of the selection
 *          and contains animations for opening and closing.
 */

using System.Collections;
using System.Collections.Generic;
using RemoteEducation.Interactions;
using RemoteEducation.Scenarios.Inspectable;
using UnityEngine;
using RemoteEducation.UI;
using RemoteEducation.Scenarios;

namespace RemoteEducation.Modules.HeavyEquipment
{
    /// <summary>This class defines the basic functionality of the engine coolant cap. </summary>
    /// <remarks>A cap can be removed or closed, which is done through animations.
    /// This class defines those animations.</remarks>
    public sealed class EngineCoolantCap : SemiSelectable, IInitializable, ITaskable
    {

        [SerializeField] private AnimationCurve animationCurve;

        [Tooltip("The cap's starting/closed position")]
        [SerializeField] private Transform startingTransform;

        [Tooltip("The cap's ending/opened position")]
        [SerializeField] private Transform endTransform;

        [Tooltip("The length of the animation")]
        [SerializeField] private float animationDuration;

        [Tooltip("Is Taskable")]
        [SerializeField] private bool isTaskable;

        private Vector3 startPosition;
        private Vector3 endPosition;
        private Quaternion startRotation;
        private Quaternion endRotation;

        /// <summary>Boolean that tracks selection </summary>
        public bool Removed { get; private set; } = false;

        public TaskableObject Taskable { get; private set; }

        /// <summary>
        /// The current animation that is opening or closing the cap.</summary>
        private Coroutine currentAnimation;

        public void Initialize(object input = null)
        {
            if (isTaskable)
                Taskable = new TaskableObject(this);

            AddFlags(Flags.InteractionsDisabled);
            OnSelect += ToggleCapState;

            startPosition = startingTransform.position;
            startRotation = startingTransform.rotation;
            endPosition = endTransform.position;
            endRotation = endTransform.rotation;
        }

        public void RemoveCapCall()
        {
            StartCoroutine(ToggleCapPosition(true));
        }

        public void CloseCapCall()
        {
            StartCoroutine(ToggleCapPosition(true));
        }

        /// <summary>
        /// If the cap is closed, open it. If it's open, close it.
        /// This should be called when the cap is clicked on.
        /// </summary>
        private void ToggleCapState()
        {
            Removed = !Removed; 

            if(currentAnimation != null)
            {
                StopCoroutine(currentAnimation);
            }

            currentAnimation = StartCoroutine(ToggleCap(Removed));
        }

        private IEnumerator ToggleCap(bool removed)
        {
            // if cap is being removed
            if (removed)
            {
                StartCoroutine(ToggleCapRotation(removed));
            }
            // if cap is being put back on
            else
            {
                StartCoroutine(ToggleCapPosition(removed));
            }

            Taskable.PokeTaskManager();

            yield return null;
        }

        /// <summary>Toggles the cap rotation between closed and opened</summary>
        private IEnumerator ToggleCapRotation(bool removed)
        {
            float startTime = Time.time;
            float endTime = startTime + animationDuration;

            Quaternion target = (transform.rotation == startRotation) ? endRotation : startRotation;

            while (Time.time <= endTime)
            {
                float timeRatio = (Time.time - startTime) / animationDuration;

                transform.rotation = Quaternion.Lerp(transform.rotation, target, timeRatio);

                yield return null;
            }

            transform.rotation = target;

            // move cap after if being removed from reservoir
            if (removed)
                StartCoroutine(ToggleCapPosition(removed));
        }

        /// <summary>Toggles the cap position between closed and opened</summary>
        private IEnumerator ToggleCapPosition(bool removed)
        {
            float startTime = Time.time;
            float endTime = startTime + animationDuration;
            Vector3 animationCurvePosition = Vector3.zero;

            Vector3 target = (transform.position == startPosition) ? endPosition : startPosition;

            while (Time.time <= endTime)
            {
                float timeRatio = (Time.time - startTime) / animationDuration;

                animationCurvePosition.y = animationCurve.Evaluate(timeRatio);

                transform.position = Vector3.Lerp(transform.position, target + animationCurvePosition, timeRatio);

                yield return null;
            }

            transform.position = target;

            // rotate cap after if being put back on reservoir
            if(!removed)
                StartCoroutine(ToggleCapRotation(removed));
        }

        public object CheckTask(int checkType, object inputData)
        {
            switch (checkType)
            {
                case (int)TaskVertexManager.CheckTypes.Bool:
                    return Removed;

                default:
                    Debug.LogError("EngineCoolantCap was passed a check type it couldn't handle");
                    return null;
            }
        }
    }
}
