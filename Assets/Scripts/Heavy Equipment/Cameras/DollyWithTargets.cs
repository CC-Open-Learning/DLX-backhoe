using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using RemoteEducation.Scenarios;

namespace RemoteEducation.Modules.HeavyEquipment
{
    /// <summary>
    /// Component added to a Dolly POI that allows the user animate motion along the track of the dolly as well as the look target for the camera.
    /// SetupTarget() Sets the look target and current position along the track of the path
    /// MoveToTarget() Animates movement from one Position to another
    /// </returns>
    [RequireComponent(typeof(PointOfInterest))]
    [DisallowMultipleComponent]
    public class DollyWithTargets : MonoBehaviour, ITaskable
    {
        [SerializeField, Tooltip("Positions for the camera to look at, related to targetPathPositions")] private List<Transform> targetPositions;
        [SerializeField, Tooltip("PathPosition value for the current index, related to targetPositions")] private List<float> targetPathPostions;

        public enum Positions 
        { 
            None, 
            FrontLiftArm, 
            RearAxle,
            LightsDolly,
            SwingLinkageDolly,
            EngineLoaderDolly
        }

        [SerializeField, Tooltip("Used by the ScenarioGraph to identify this component")] private Positions position = Positions.None;

        float moveTime = 1f;
        
        /// <summary> the current position index of the Dolly Path, used by targetPositions and targetPathPositions</summary>
        private int currentPosition = -1;
        /// <summary>the new requested position of the camera, set in MoveToTarget() and used in the Update() function to animate</summary>
        private int newPosition = 0;
        /// <summary>cached reference to the number value of the new position, used in update</summary>
        private float newPositionValue = -1;
        /// <summary>cached reference to the number value of the old position, used in update</summary>
        private float oldPositionValue = -1;
        /// <summary>flag for determining whether or not we are animating, used in update</summary>
        private bool moving = false;
        /// <summary>accumulator for the total time moved, used in update</summary>
        private float accumulatedMoveTime = 0;
        /// <summary>Reference to the virtualCamera used by this POI</summary>
        private CinemachineVirtualCamera virtualCamera;
        /// <summary>Reference Dolly component used by the virtual camera attached to this POI</summary>
        private CinemachineTrackedDolly virtualCameraDolly;
        /// <summary>The POI attached to this gameObject</summary>
        private PointOfInterest pointOfInterest;
        /// <summary>the path of this POI</summary>
        private CinemachinePath path;

        public TaskableObject Taskable { get; private set; }

        public void Start()
        {
            pointOfInterest = GetComponent<PointOfInterest>();
            virtualCamera = pointOfInterest.VirtualCamera;
            virtualCameraDolly = virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
            path = GetComponentInChildren<CinemachinePath>();

            Taskable = new TaskableObject(this, position.ToString());
        }

        public void SetupTarget(int position) 
        {
            moving = false;

            virtualCamera.LookAt = targetPositions[position];
            virtualCameraDolly.m_PathPosition = targetPathPostions[position];
        }

        public void SetUserControl(bool canControl) {
            pointOfInterest.CanMove = canControl;
            CoreCameraController.Instance.UpdateMoveAndZoomPanel(pointOfInterest);
        }

        public void MoveToTarget(int newPos, float timeInSeconds)
        {
            if (newPos < 0 || newPos > targetPositions.Count - 1) return;

            SetUserControl(false);

            moveTime = timeInSeconds;
            accumulatedMoveTime = 0;
            virtualCamera.LookAt = targetPositions[newPos];

            newPosition = newPos;

            newPositionValue = targetPathPostions[newPos];
            oldPositionValue = virtualCameraDolly.m_PathPosition;

            moving = true;
        }

        private void Update()
        {
            if (!moving) return;

            accumulatedMoveTime += Time.deltaTime;
            virtualCameraDolly.m_PathPosition += (newPositionValue - GetClampedPosition(oldPositionValue, newPositionValue)) * (Time.deltaTime / moveTime);
            
            if (accumulatedMoveTime >= moveTime) 
            {
                moving = false;
                currentPosition = newPosition;
                Taskable.PokeTaskManager();
            }
        }

        ///<summary>If the path units are normalized, clamp the path value such that the camera takes the shortest path to the target</summary>
        float GetClampedPosition(float oldPosition, float newPosition) 
        {
            //all looped automated dolly cameras should use normalized units for their virtual cameras
            if (virtualCameraDolly.m_PositionUnits != CinemachinePathBase.PositionUnits.Normalized) 
                return oldPosition;

            if (!path.m_Looped)
                return oldPosition;

            //remove multiple turns to get to destination
            while (oldPosition < 0) 
                oldPosition++;

            if (oldPosition > 0) 
                oldPosition %= 1;

            //take the shortest path to destination
            if (newPosition - 0.5 > oldPosition)
            {
                oldPosition++;
            }
            else if (newPosition + 0.5 < oldPosition) 
            {
                oldPosition--;
            }

            return oldPosition;
        }

        public object CheckTask(int checkType, object inputData)
        {
            switch (checkType)
            {
                case (int)TaskVertexManager.CheckTypes.Int:
                    return moving ? -1 : currentPosition;

                default:
                    Debug.LogError("Invalid check type passed into DollyWithTargets");
                    break;
            }

            return null;
        }
    }
}
