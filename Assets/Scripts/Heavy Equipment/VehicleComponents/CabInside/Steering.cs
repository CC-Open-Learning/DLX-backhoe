using System.Collections;
using System.Collections.Generic;
using RemoteEducation.Interactions;
using RemoteEducation.Scenarios.Inspectable;
using RemoteEducation.Scenarios;
using UnityEngine;

namespace RemoteEducation.Modules.HeavyEquipment
{
    /// <summary>Class to handle user input for the steering wheel.</summary>
    public class Steering : SemiSelectable, IInitializable, IBreakable, ITaskable
    {
        /// <summary>Steer value to determine it's position</summary>
        private float steerValue = 0;

        /// <summary>Determine if the steering wheel is held</summary>
        private bool userControllingSteeringWheel = false;

        /// <summary>Value to stop wheel based on physical constraints.</summary>
        private float maxSteerValue = 1.5f;

        /// <summary>Steering wheel rotational value/speed</summary>    
        private float steeringWheelRotationValue = 90.0f;

        [SerializeField, Tooltip("If the steering is currently locked.")]
        private bool steerLock = false;

        [SerializeField, Tooltip("Vehicle wheels for rotating.")]
        private List<Transform> steeringWheels;

        [SerializeField, Tooltip("Vehicle knuckles for rotating.")]
        private List<Transform> Knuckles;

        /// <summary>Steering wheel rotation in degrees per second.</summary>
        private float steeringWheelsRotation = 20f;

        /// <summary>Used by the scenario graph, check if the wheel has been turned all the way to the left</summary>
        private bool hasTurnedMaxLeft = false;

        /// <summary>Used by the scenario graph, check if the wheel has been turned all the way to the right</summary>
        private bool hasTurnedMaxRight = false;

        /// <summary>Used by the scenario graph, check if the wheel is locked</summary>
        public bool SteerLock { get { return steerLock; } }

        /// <summary>Initial Rotation of all rotating elements</summary>
        private List<Quaternion> wheelsInitialRotations;
        private List<Quaternion> knucklesInitialRotations;
        private Quaternion steeringWheelInitialRotation;

        public DynamicInspectableElement InspectableElement { get; private set; }

        public TaskableObject Taskable { get; private set; }

        /// <summary>Make object inspectable for damages</summary>
        public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int breakMode)
        {
            SetWheelLock(broken);

            InspectableElement = inspectableElement;

            inspectableElement.OnCurrentlyInspectableStateChanged += (active) =>
            {
                // When interactable, don't show the highlight for this script. 
                // Also make it so that it can be interacted with at any time.
                ToggleFlags(!active, Flags.Highlightable | Flags.ExclusiveInteraction);
            };
        }

        /// <summary>Initializes select and deselect functionality.</summary>
        public void Initialize(object input = null)
        {
            //get initial rotations of all rotating components
            steeringWheelInitialRotation = transform.rotation;
            wheelsInitialRotations = new List<Quaternion>();
            knucklesInitialRotations = new List<Quaternion>();
            foreach (Transform t in steeringWheels) wheelsInitialRotations.Add(t.rotation);
            foreach (Transform t in Knuckles) knucklesInitialRotations.Add(t.rotation);

            OnSelect += SteerWheel;
            OnDeselect += StopSteering;

            Taskable = new TaskableObject(this);
        }

        /// <summary>Set the wheel locking property on select.</summary>
        public void SetWheelLock(bool WheelLockStatus)
        {
            steerLock = WheelLockStatus;
        }

        /// <summary>Begins the <see cref="Steer"/> <see cref="Coroutine"/> if the steering is not locked.</summary>
        private void SteerWheel()
        {
            if (steerLock)
                return;
          
            StartCoroutine(Steer());
        }

        /// <summary>Sets <see cref="userControllingSteeringWheel"/> to <see langword="false"/> to exit the <see cref="Steer"/> <see cref="Coroutine"/>.</summary>
        private void StopSteering()
        {
            //reset transforms of all rotating components
            transform.rotation = steeringWheelInitialRotation;
            for (int i = 0; i < wheelsInitialRotations.Count; i++) steeringWheels[i].rotation = wheelsInitialRotations[i];
            for (int i = 0; i < knucklesInitialRotations.Count; i++) Knuckles[i].rotation = knucklesInitialRotations[i];
            steerValue = 0;

            userControllingSteeringWheel = false;
        }

        /// <summary><see cref="Coroutine"/> to animate and handle steering.</summary>
        private IEnumerator Steer()
        {
            CoreCameraController.ToggleObjectIsDragging(true);
            userControllingSteeringWheel = true;
            var startMousePos = Input.mousePosition;
            float xMouseValue;
            float directionMultiplier;
            Vector3 mousePos;

            while (userControllingSteeringWheel)
            {
                if (steerValue >= maxSteerValue) hasTurnedMaxRight = true;
                if (steerValue <= -maxSteerValue) hasTurnedMaxLeft = true;

                mousePos = Input.mousePosition; //Current Mouse Position

                xMouseValue = startMousePos.x - mousePos.x; //Mouse left or right difference

                if (xMouseValue > 0 && steerValue < maxSteerValue) //Steer Right
                {
                    directionMultiplier = 1f;
                }
                else if (xMouseValue < 0 && steerValue > -maxSteerValue) //Steer Left
                {
                    directionMultiplier = -1f;
                }
                else // Don't steer
                {
                    yield return null;
                    continue;
                }

                //Steering wheel rotation
                transform.RotateAround(transform.position, directionMultiplier * transform.right, Time.deltaTime * steeringWheelRotationValue);
                steerValue += directionMultiplier * Time.deltaTime;

                //Vehicle Wheels
                foreach (Transform transObject in steeringWheels)
                {
                    transObject.Rotate(0.0f, directionMultiplier * -steeringWheelsRotation * Time.deltaTime, 0.0f, Space.Self);
                }

                foreach (Transform transObject in Knuckles)
                {
                    transObject.Rotate(0.0f, 0.0f, directionMultiplier * -steeringWheelsRotation * Time.deltaTime, Space.Self);
                }

                yield return null;
            }

            CoreCameraController.ToggleObjectIsDragging(false);

            Taskable.PokeTaskManager();
        }

        public object CheckTask(int checkType, object inputData)
        {
            switch (checkType)
            {
                case (int)TaskVertexManager.CheckTypes.Bool:
                    return hasTurnedMaxLeft && hasTurnedMaxRight;

                default:
                    Debug.LogError("Invalid check type passed into Steering Wheel");
                    break;
            }

            return null;
        }
    }
}
