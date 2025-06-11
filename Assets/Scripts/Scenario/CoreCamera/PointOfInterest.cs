using Cinemachine;
using System;
using UnityEngine;

namespace RemoteEducation.Scenarios
{

    /// <summary>
    /// This class is designed to be used with the CoreCameraController.
    /// Each PointOfIntrest(POI) defines a place in the scene where the 
    /// camera can move to.Each POI will have a Cinemachine Virtual Camera 
    /// which will become the main camera when the POI is selected.
    /// 
    /// Each POI can toggle if it can Pan, Zoom, or move. If the POI is able 
    /// to move, than a <see cref="CinemachinePath"/> must be attached. It can
    /// also define its own target group that will be used to position where the 
    /// camera is looking.
    /// </summary>
    public class PointOfInterest : MonoBehaviour
    {
        //The CoreCameraController in the Scene.
        [HideInInspector]
        public CoreCameraController CameraController { get; set; }

        public string POIName;

        public bool ShowInViewPanel;

        [Tooltip("Turn on if this POI should allow the user to zoom in")]
        public bool CanZoom;

        [Tooltip("Turn on if this POI should allow the user to pan the camera")]
        public bool CanPan;

        [Tooltip("If this POI should override the CoreCameraControllers defaults for the panning min and max")]
        public bool OverrideDefualtPanMinMax;

        [Tooltip("This POIs custom max panning angle in the X plane")]
        public float MaxCameraPan_X;
        [Tooltip("This POIs custom max panning angle in the Y plane")]
        public float MaxCameraPan_Y;

        [Tooltip("This POIs Virtual Camera. This shall be set in the Unity Inspector")]
        public CinemachineVirtualCamera VirtualCamera;
        [Tooltip("This POIs Recomposer. This shall be set in the Unity Inspector")]
        public CinemachineRecomposer Recomposer;

        [Header("On Dolly Components")]
        [Tooltip("If this Poi is on a dolly cam.")]
        public bool CanMove;
        [Tooltip("The path this poi can move on")]
        public CinemachinePath Path;
        [Tooltip("The target group that this camera will look at. If left null, the Camera default will be used.")]
        public CinemachineTargetGroup TargetGroup;
        [Tooltip("The tilt offset for the dolly camera")]
        public float DollyTiltOffset;
        [Tooltip("Swap the directions that the movement buttons move the camera")]
        public bool SwapLeftRight;
        [Tooltip("If false, on POI transitions the position on the path will update to be close to the cameras current position")]
        public bool MaintainPositionOnTransition;

        /// <summary>
        ///     Action invoked when entering the Point of Interest, providing the previous Point of Interest
        /// </summary>
        public Action<PointOfInterest> OnEnterPOI;

        /// <summary>
        ///     Action invoked when leaving the Point of Interest, providing the next Point of Interest
        /// </summary>
        public Action<PointOfInterest> OnExitPOI;

        public float DollySpeed { get; private set; }
        public CinemachineTrackedDolly TrackedDolly { get; private set; }

        private void Awake()
        {
            if(CanMove)
            {
                Recomposer.m_Tilt = DollyTiltOffset;
                DollySpeed = CoreCameraController.CalculateHorizontalSpeed(Path.PathLength);
                TrackedDolly = GetComponentInChildren<CinemachineTrackedDolly>();   
            }
        }
    }
}



