/*
 * SUMMARY: Steering Column Lock System.
 */
using System.Collections;
using System.Collections.Generic;
using RemoteEducation.Interactions;
using RemoteEducation.Scenarios.Inspectable;
using RemoteEducation.Scenarios;
using UnityEngine;
using RemoteEducation.UI;
using RemoteEducation.Localization;
using RemoteEducation.Audio;

namespace RemoteEducation.Modules.HeavyEquipment
{
    /// <summary>
    /// This class checks and uses the column lock
    /// </summary>
    public class SteerColumnLock : SemiSelectable, IInitializable
    {
        /// <summary>Value to determine if it is locked or not</summary>
        private bool isLocked = true;

        /// <summary>UI objects to show/hide/interact with</summary>
        [SerializeField]
        private List<GameObject> UIObjects;

        /// <summary>Steering column to move</summary>
        [SerializeField]
        private Transform steerColumn;

        /// <summary>Steering column position to move</summary>
        [SerializeField]
        private Transform steerColumnPosition;

        /// <summary>Steer value to determine it's position</summary>
        [SerializeField]
        private Steering steeringWheel;

        /// <summary>Value to determine if object is in animation</summary>
        private bool isAnimating = false;

        /// <summary>Steering column rotational value/speed</summary>
        [SerializeField, Tooltip("Adjust In/Out Speeds")]
        private float InOutSpeed = 1.0f;

        /// <summary>Lock animation time</summary>
        private float lockAnimationTime = .5f;

        /// <summary>In Position found on the Column Positions</summary>
        [SerializeField]
        private Transform InPosition;

        /// <summary>Out Position found on the Column Positions</summary>
        [SerializeField]
        private Transform OutPosition;

        /// <summary>Down Position found on the Column Positions</summary>
        [SerializeField]
        private Transform DownPosition;

        /// <summary>Up Position found on the Column Positions</summary>
        [SerializeField]
        private Transform UpPosition;

        /// <summary>Rotation Direction, either up or down</summary>
        private int RotationDirection = 0;

        /// <summary>Position Direction, either in or out</summary>
        private int PositionDirection = 0;

        [SerializeField] private Vector3 pressedRotation;
        private Vector3 depressedRotation;

        [SerializeField] public SteerLockInspectable steerLock;

        [Header("Broken Lock Animation Elements")]
        [SerializeField, Tooltip("Curve to dictate how the lock should animate.")]
        private AnimationCurve lockCurve;

        [Tooltip("Squeak sound plays when the wheel nut is loose.")]
        [SerializeField] private SoundEffect squeakSoundEffect;

        private void Start()
        {
            depressedRotation = transform.localRotation.eulerAngles;
        }

        public bool GetIsLocked()
        {
            return isLocked;
        }

        public void Initialize(object input = null)
        {
        }

        /// <summary>Locking Toggle for the Column</summary>
        public void ToggleLock()
        {
            if (isAnimating) return;

            if(steerLock.GetBrokenStatus())
            {
                StartCoroutine(BrokenLockAnimation());
                return;
            }

            //isLocked = !isLocked;
            //StartCoroutine(AnimateLock(isLocked));
            //Start the testing animation
            StartCoroutine(TestColumnAnimation());
        }

        private IEnumerator BrokenLockAnimation()
        {
            isAnimating = true;

            Quaternion targetRotation = Quaternion.Euler(pressedRotation);
            Quaternion currentRotation = transform.localRotation;

            /*//Animate Rotation
            for (float time = 0; time <= lockAnimationTime; time += Time.deltaTime)
            {
                float pos = lockCurve.Evaluate(time / lockAnimationTime);
                transform.localRotation = Quaternion.Lerp(currentRotation, targetRotation, pos);
                yield return null;
            }*/

            //Animate Rotation
            for (float time = 0; transform.localRotation != targetRotation; time += Time.deltaTime)
            {
                transform.localRotation = Quaternion.Lerp(currentRotation, targetRotation, time / lockAnimationTime);
                yield return null;
            }

            //Time Variables
            float startTime = Time.time;
            float upInTime = startTime + 1f;
            float downOutTime = upInTime + 1f;
            float endTime = downOutTime;
            float timeRatio = 0;

            //Rotation Variables

            Vector3 originPos = steerColumnPosition.position;
            Quaternion originRot = steerColumnPosition.rotation;

            while (Time.time <= endTime)
            {
                if (Time.time <= upInTime)
                {
                    //Up
                    timeRatio = (Time.time - startTime) / 1f;
                    steerColumn.rotation = Quaternion.Lerp(originRot, DownPosition.rotation, timeRatio);
                    steerColumnPosition.rotation = Quaternion.Lerp(originRot, DownPosition.rotation, timeRatio);
                    steerColumn.position = Vector3.Lerp(originPos, OutPosition.position, timeRatio);
                }
                else if (Time.time <= downOutTime)
                {
                    //Down
                    timeRatio = (Time.time - upInTime) / 1f;
                    steerColumn.rotation = Quaternion.Lerp(DownPosition.rotation, UpPosition.rotation, timeRatio);
                    steerColumnPosition.rotation = Quaternion.Lerp(DownPosition.rotation, UpPosition.rotation, timeRatio);
                    steerColumn.position = Vector3.Lerp(OutPosition.position, InPosition.position, timeRatio);
                }
                yield return null;
            }

            targetRotation = Quaternion.Euler(depressedRotation);
            currentRotation = transform.localRotation;

            //Animate Rotation
            for (float time = 0; transform.localRotation != targetRotation; time += Time.deltaTime)
            {
                transform.localRotation = Quaternion.Lerp(currentRotation, targetRotation, time / lockAnimationTime);
                yield return null;
            }

            squeakSoundEffect.Play();

            //Animate Rotation
            for (float time = 0; time <= 0.2f; time += Time.deltaTime)
            {
                steerColumn.rotation = Quaternion.Lerp(UpPosition.rotation, DownPosition.rotation, time * 5f);
                steerColumnPosition.rotation = Quaternion.Lerp(UpPosition.rotation, DownPosition.rotation, time * 5f);
                steerColumn.position = Vector3.Lerp(InPosition.position, OutPosition.position, time * 5f);
                yield return null;
            }

            isAnimating = false;

            yield return null;
        }

        private IEnumerator TestColumnAnimation()
        {
            isAnimating = true;

            Quaternion targetRotation = Quaternion.Euler(pressedRotation);
            Quaternion currentRotation = transform.localRotation;

            //Animate Rotation
            for (float time = 0; transform.localRotation != targetRotation; time += Time.deltaTime)
            {
                transform.localRotation = Quaternion.Lerp(currentRotation, targetRotation, time / lockAnimationTime);
                yield return null;
            }

            steeringWheel.SetWheelLock(true);

            //Time Variables
            float startTime = Time.time;
            float upInTime = startTime + 1f;
            float downOutTime = upInTime + 1f;
            float endTime = downOutTime;
            float timeRatio = 0;

            //Rotation Variables

            Vector3 originPos = steerColumnPosition.position;
            Quaternion originRot = steerColumnPosition.rotation;

            while (Time.time <= endTime)
            {
                if(Time.time <= upInTime)
                {
                    //Up
                    timeRatio = (Time.time - startTime) / 1f;
                    steerColumn.rotation = Quaternion.Lerp(UpPosition.rotation, DownPosition.rotation, timeRatio);
                    steerColumnPosition.rotation = Quaternion.Lerp(UpPosition.rotation, DownPosition.rotation, timeRatio);
                    steerColumn.position = Vector3.Lerp(InPosition.position, OutPosition.position, timeRatio);
                }
                else if(Time.time <= downOutTime)
                {
                    //Down
                    timeRatio = (Time.time - upInTime) / 1f;
                    steerColumn.rotation = Quaternion.Lerp(DownPosition.rotation, UpPosition.rotation, timeRatio);
                    steerColumnPosition.rotation = Quaternion.Lerp(DownPosition.rotation, UpPosition.rotation, timeRatio);
                    steerColumn.position = Vector3.Lerp(OutPosition.position, InPosition.position, timeRatio);
                }
                yield return null;
            }

            targetRotation = Quaternion.Euler(depressedRotation);
            currentRotation = transform.localRotation;

            //Animate Rotation
            for (float time = 0; transform.localRotation != targetRotation; time += Time.deltaTime)
            {
                transform.localRotation = Quaternion.Lerp(currentRotation, targetRotation, time / lockAnimationTime);
                yield return null;
            }

            isAnimating = false;

            yield return null;
        }

        /// <summary><see cref="Coroutine"/> the steering wheel lock animation</summary>
        private IEnumerator AnimateLock(bool isLocking) {
            isAnimating = true;

            Quaternion targetRotation = Quaternion.Euler(isLocking ? pressedRotation : depressedRotation);
            Quaternion currentRotation = transform.localRotation;

            //Animate Rotation
            for (float time = 0; transform.localRotation != targetRotation; time += Time.deltaTime)
            {
                transform.localRotation = Quaternion.Lerp(currentRotation, targetRotation, time / lockAnimationTime);
                yield return null;
            }

            //Hide UI elements and unlock the steering wheel
            foreach (GameObject uiElement in UIObjects)
            {
                uiElement.SetActive(isLocking);
                steeringWheel.SetWheelLock(isLocking);
            }

            isAnimating = false;
        }

        /// <summary> Public call to start animating the up movement on mouse down</summary>
        public void MoveUp()
        {
            if (!isAnimating)
            {
                RotationDirection = -1;
                StartCoroutine(Rotation());
            }
        }

        /// <summary> Public call to start animating the down movement on mouse down</summary>
        public void MoveDown()
        {
            if (!isAnimating)
            {
                RotationDirection = 1;
                StartCoroutine(Rotation());
            }
        }

        /// <summary><see cref="Coroutine"/> moves the steering column on rotation</summary>
        private IEnumerator Rotation()
        {
            isAnimating = true;
            while (isAnimating)
            {
                float maxRotationRange = DownPosition.rotation.z + UpPosition.rotation.z * -1;
                float currentRotation = steerColumn.rotation.z + UpPosition.rotation.z * -1 + RotationDirection * Time.deltaTime;
                float currentOGRotation = steerColumnPosition.rotation.z + UpPosition.rotation.z * -1 + RotationDirection * Time.deltaTime;

                if (steerColumn.rotation.z < DownPosition.rotation.z && RotationDirection == 1)
                {
                    steerColumn.rotation = Quaternion.Lerp(UpPosition.rotation, DownPosition.rotation,
                        currentRotation / maxRotationRange);
                    steerColumnPosition.rotation = Quaternion.Lerp(UpPosition.rotation, DownPosition.rotation,
                        currentOGRotation / maxRotationRange);
                }
                else if (steerColumn.rotation.z > UpPosition.rotation.z && RotationDirection == -1)
                {
                    steerColumn.rotation = Quaternion.Lerp(UpPosition.rotation, DownPosition.rotation,
                        currentRotation / maxRotationRange);
                    steerColumnPosition.rotation = Quaternion.Lerp(UpPosition.rotation, DownPosition.rotation,
                        currentOGRotation / maxRotationRange);
                }
                yield return null;
            }
        }

        /// <summary> Public call to start animating the in movement on mouse down</summary>
        public void MoveIn()
        {
            if (!isAnimating)
            {
                PositionDirection = -1;
                StartCoroutine(Position());
            }
        }

        /// <summary> Public call to start animating the out movement on mouse down</summary>
        public void MoveOut()
        {
            if (!isAnimating)
            {
                PositionDirection = 1;
                StartCoroutine(Position());
            }
        }

        /// <summary><see cref="Coroutine"/> moves the steering column in</summary>
        private IEnumerator Position()
        {
            isAnimating = true;
            while (isAnimating)
            {
                float maxPositionRange = InPosition.position.y * -1 + OutPosition.position.y;
                float currentPosition = steerColumn.position.y + InPosition.position.y * -1 + PositionDirection * Time.deltaTime * InOutSpeed;

                if (steerColumn.position.y > InPosition.position.y && PositionDirection == -1)
                {
                    steerColumn.position = Vector3.Lerp(InPosition.position, OutPosition.position,
                        currentPosition / maxPositionRange);
                }
                else if (steerColumn.position.y < OutPosition.position.y && PositionDirection == 1)
                {
                    steerColumn.position = Vector3.Lerp(InPosition.position, OutPosition.position,
                        currentPosition / maxPositionRange);
                }
                yield return null;
            }
        }

        /// <summary><see cref="Coroutine"/> to stop animations. Called for mouse up events</summary>
        public void StopAnimating()
        {
            isAnimating = false;
        }
    }
}