using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using Cinemachine;
using RemoteEducation.Interactions;
using System.Collections.Generic;
using System.Linq;
using Lean.Gui;
using RemoteEducation.Extensions;
using UnityEngine.Serialization;

namespace RemoteEducation.Scenarios
{
    /// <summary>
    /// This class controls the camera in any scenario scene.
    /// This class should only be on the CoreCameraController prefab.
    /// The prefab should be loaded into the scene as part of the
    /// module that is being loaded. 
    /// 
    /// The positions of the camera are controlled by <see cref="PointOfInterest"/> 
    /// Gameobjects in the scene.
    /// </summary>
    public class CoreCameraController : MonoBehaviour, ITaskable
    {
        public delegate void OnPOIChangedEvent(PointOfInterest oldPOI, PointOfInterest newPOI);

        #region Attributes

        /// <summary>
        /// Limits for POIs that are on a NON-looped track </summary>
        public enum MovementLimits
        {
            LeftLimitReached,
            WithinBounds,
            RightLimitReached
        }

        public enum Directions
        {
            Left,
            Right
        }

        //constants 
        public const int BasePriority = 10;
        public const int HighPriority = 15;
        private const int NoDampening = 1;
        private const float DampeningScale = 5f;
        private const float RestorePanAngleX = 0.05f;
        private const float RestorePanAngleY = 0.01f;
        public const float PathLengthToSpeed = 4;
        public const float NonLoopedPathFullyLeft = 1;
        public const float NonLoopedPathFullyRight = 0;
        private const float BackIntoBoundsAnimationTime = 0.4f;
        public const float ZoomScrollSpeed = 0.04f;

        public static CoreCameraController Instance { get; set; }
        public static event OnPOIChangedEvent OnPOIChanged;

        //The buttons in the POI button panel
        private List<POIViewModesButton> ViewButtons;
        private POIViewModesButton CurrentViewButton;


        //If the movement/panning is locked.
        private bool PanningLocked;
        private bool UiIsMovingCamera;

        public CinemachineBrain Brain { get; private set; }

        //If when the user pressed the mouse to drag the camera, the press was down on a UI component.
        // if this is true, then the camera wont pan
        private bool PanningDownOnUI;

        //the camera panning dampening from the last frame. Used to determine if the dampening should still be applied. 
        private float LastPanningDampening;


        //properties that are based on the current poi
        public PointOfInterest CurrentPOI { get; private set; }
        private CinemachineVirtualCamera CurrentVirtualCamera => CurrentPOI.VirtualCamera;
        private CinemachineRecomposer CurrentRecomposer => CurrentPOI.CanPan ? CurrentPOI.Recomposer : null;
        private CinemachinePath CurrentPath => CurrentPOI.CanMove ? CurrentPOI.Path : null;
        private CinemachineTargetGroup CurrentTargetGroup => CurrentPOI.TargetGroup == null ? DefaultTargetGroup : CurrentPOI.TargetGroup;
        private CinemachineTrackedDolly CurrentTrackedDolly => CurrentPOI.CanMove ? CurrentPOI.TrackedDolly : null;
        private float CurrentDollyTiltOffset => CurrentPOI.DollyTiltOffset;
        private float CurrentHorizontalSpeed => CurrentPOI.CanMove ? CurrentPOI.DollySpeed : 0;
        private bool CurrentSwapLeftRight => CurrentPOI.SwapLeftRight;

        /// <summary>
        /// The current movement limit for the current poi. Only used if the is on a non-looped track </summary>
        private MovementLimits MovementLimit;

        /// <summary>
        /// The max panning angle in the X plane based on the current virtual camera
        /// </summary>
        private float maxCameraPan_X => CurrentPOI.OverrideDefualtPanMinMax ? CurrentPOI.MaxCameraPan_X : DefaultMaxPanAngle_X;

        /// <summary>
        /// The max panning angle in the X plane based on the current virtual camera
        /// </summary>
        private float maxCameraPan_Y => CurrentPOI.OverrideDefualtPanMinMax ? CurrentPOI.MaxCameraPan_Y : DefaultMaxPanAngle_Y;


        //actions for when the camera starts and stops the transition between Points of Interest
        private Action OnCameraBlendingStart;
        private Action OnCameraBlendingEnd;
        private Coroutine BlendingCheckCoroutine;

        private bool isInitialized = false;

        /// <summary>
        /// If the camera is currently moving between Points of Interest
        /// </summary>
        public bool IsCameraTransitioning => Brain.IsBlending;

        public Transform MainCameraTransform => mainCameraTransform;

        // Made the CoreCameraController Taskable to check related tasks in the Graph Scenario
        public TaskableObject Taskable { get; private set; }

        private bool cameraChanged = false;


        #region Serialized Fields

        [Header("General")]
        [SerializeField]
        private Transform mainCameraTransform;
        [SerializeField, Tooltip("The Maximum FOV the Zoom Slider can set a virtual camera to")]
        private float MaxCameraFOV;
        [SerializeField, Tooltip("The Minimum FOV the Zoom Slider can set a virtual camera to")]
        private float MinCameraFOV;
        [SerializeField]
        private CinemachineTargetGroup DefaultTargetGroup;

        [Header("UI")]

        [SerializeField, Tooltip("The gameobject for the cameras movement button and zoom controls")]
        private GameObject MoveAndZoomPanel;

        [SerializeField, Tooltip("The gameobject for the dolly movement buttons")]
        private GameObject MovementControlButtons;

        [SerializeField, Tooltip("The Left movement button")]
        private LeanToggle LeftMovementButton;

        [SerializeField, Tooltip("The Right movement button")]
        private LeanToggle RightMovementButton;

        [SerializeField, Tooltip("The gameobject for the zoom section of the movement panel")]
        private GameObject ZoomControlPanel;

        [SerializeField]
        private Slider ZoomSlider;

        [SerializeField]
        private ViewModesPanel ViewModeButtonPanel;

        [SerializeField]
        private GameObject ViewModeButtonPrefab;

        [Header("Dolly Settings")]
        [Tooltip("How fast the dolly camera moves around the path.")]
        [SerializeField]
        public float DollyMovementSpeed = 1;

        [SerializeField, Tooltip("How far the Dolly camera can pan right and left")]
        private float DefaultMaxPanAngle_X;

        [SerializeField, Tooltip("How far the Dolly camera can pan up and down")]
        private float DefaultMaxPanAngle_Y;

        [SerializeField, Tooltip("Left Arrow and 'A' by default")]
        private KeyCode[] LeftMovementKeys;

        [SerializeField, Tooltip("Right Arrow and 'D' by default")]
        private KeyCode[] RightMovementKeys;

        [Header("Points of Interest")]

        [SerializeField, Tooltip("The Points of interest throughout the scene")]
        [FormerlySerializedAs("PointsOfIntrest")]
        public List<PointOfInterest> PointsOfInterest;

        [Tooltip("The POI the Camera should start at.")]
        public PointOfInterest HomeLocation;

        #endregion
        #endregion

        private void Awake()
        {
            if (!isInitialized)
                enabled = false;
        }

        /// <summary>
        /// CreateElements all the setup needed for the Core Camera Controller.
        /// This method shall be called before <see cref="StartCameraController"/>
        /// </summary>
        public void Initialize()
        {
            Taskable = new TaskableObject(this);

            if (isInitialized)
            {
                Debug.LogError("CoreCameraController: Initialize called twice!");
                return;
            }

            Instance = this;

            DefaultTargetGroup = GetComponentInChildren<CinemachineTargetGroup>();
            Brain = GetComponentInChildren<CinemachineBrain>();

            //initialize the view modes button box
            InitalizeViewModesButton();

            LastPanningDampening = NoDampening;
            MovementLimit = MovementLimits.WithinBounds;

            if (LeftMovementKeys == null || LeftMovementKeys.Length == 0)
            {
                LeftMovementKeys = new KeyCode[] { KeyCode.A, KeyCode.LeftArrow };
            }
            if (RightMovementKeys == null || RightMovementKeys.Length == 0)
            {
                RightMovementKeys = new KeyCode[] { KeyCode.D, KeyCode.RightArrow };
            }

            //Set the default target group for any of the points of interest that
            // are on a path but don't have their own target group.
            foreach (PointOfInterest poi in PointsOfInterest)
            {
                if (poi.CanMove && poi.TargetGroup == null)
                {
                    poi.TargetGroup = DefaultTargetGroup;
                    poi.VirtualCamera.LookAt = DefaultTargetGroup.transform;
                }
            }

            HighlightObject.CreateHighlightCamera(GetComponentInChildren<Camera>());

            isInitialized = true;
            enabled = true;
        }

        /// <summary>
        /// Send the camera to the starting location and update the UI.
        /// This method must be called before the user can start using the scene
        /// but after <see cref="Initialize"/>.
        /// </summary>
        public void StartCameraController()
        {
            //get the right position for the camera
            SetCameraToStartingLocation();

            //set the visibility of the UI panels
            UpdateCurrentViewModeButton();
        }

        /// <summary>
        /// Make sure that the camera at the starting location.
        /// This method works similarly to the <see cref="SendCameraToPosition(PointOfInterest)"/>, but
        /// this method actually updates the transform of the main camera. 
        /// This ensures that the camera doesn't need to move at all once 
        /// the scene has started.
        /// </summary>
        private void SetCameraToStartingLocation()
        {
            CurrentPOI?.OnExitPOI?.Invoke(null);
            HomeLocation.VirtualCamera.Priority = HighPriority;
            CurrentPOI = HomeLocation;
            CurrentPOI.OnEnterPOI?.Invoke(null);

            if (CurrentPOI.CanMove)
            {
                mainCameraTransform.position = HomeLocation.Path.EvaluatePositionAtUnit(HomeLocation.TrackedDolly.m_PathPosition, CinemachinePathBase.PositionUnits.Normalized);
                mainCameraTransform.rotation = HomeLocation.VirtualCamera.gameObject.transform.rotation;

                UpdateLookAtAnchorWeights(mainCameraTransform.position);
            }
            else
            {
                mainCameraTransform.position = HomeLocation.transform.position;
                mainCameraTransform.rotation = HomeLocation.transform.rotation;
            }

            UpdateMoveAndZoomPanel(HomeLocation);
        }

        /// <summary>
        /// Set up all the buttons in the view modes box. 
        /// For each POI in the cameras list of POIs that is set up show in the
        /// view modes panel, a <see cref="POIViewModesButton"/> will be instantiated.
        /// The panel itself will then be initialized so that it can set its size.
        /// </summary>
        private void InitalizeViewModesButton()
        {
            if (!ViewModeButtonPanel.gameObject.activeSelf)
            {
                //if this panel is set inactive in the scene, leave it unactivated
                return;
            }

            ViewButtons = new List<POIViewModesButton>();
            List<PointOfInterest> visablePOIs = PointsOfInterest.Where(x => x.ShowInViewPanel).ToList();

            if (visablePOIs.Count > 0)
            {
                //initialize buttons for each of the POIs
                foreach (PointOfInterest poi in visablePOIs)
                {
                    POIViewModesButton button = Instantiate(ViewModeButtonPrefab, ViewModeButtonPanel.ButtonPanelParent.transform).GetComponent<POIViewModesButton>();
                    button.gameObject.name = poi.POIName + "_ViewButton";

                    button.Initialize(this, poi);
                    ViewButtons.Add(button);
                }
            }

            ViewModeButtonPanel.Initialize();
        }

        private void Update()
        {
            HandleMovementFromKeys();
            HandlePanningFromMouseDrag();
            HandleScroll();
        }

        /// <summary>
        /// This method is used by UI buttons to simulate a "OnHold" action.
        /// </summary>
        /// <param name="mode">The button being pressed. 0 = Move camera left, 1 = Move camera right </param>
        public void CameraControlsButtonDown(int mode)
        {
            StartCoroutine(LeanOnHoldOverrideCoRoutine(mode));
        }

        #region Camera Movement/Panning
        /// <summary>
        /// Manage the panning of the camera if panning isn't locked and the current POI can pan.
        /// The Recomposer on the <see cref="CurrentPOI"/> is used.
        /// </summary>
        private void HandlePanningFromMouseDrag()
        {
            //if on the dolly and the panning isn't locked by external classes
            if (CurrentRecomposer != null && !PanningLocked)
            {
                //ensure that when the mouse was pressed, that it was not on a UI element
                if (Input.GetMouseButtonDown(0))
                {
                    PanningDownOnUI = InputManager.MouseOverUI;
                }

                //if the button wasn't pressed on a UI element and the mouse is still held down
                if (!PanningDownOnUI && Input.GetMouseButton(0))
                {
                    float panningDampenning = CalculatePanningDampening(CurrentRecomposer.m_Pan, CurrentRecomposer.m_Tilt);

                    //get the input from the mouse and set rotations
                    CurrentRecomposer.m_Pan = CurrentRecomposer.m_Pan - Input.GetAxis("Mouse X") * panningDampenning;
                    CurrentRecomposer.m_Tilt = CurrentRecomposer.m_Tilt + Input.GetAxis("Mouse Y") * panningDampenning;
                }

                if (Input.GetMouseButtonUp(0))
                {
                    //ensure that transitions to other POIs won't spin around 
                    CurrentRecomposer.m_Pan = CurrentRecomposer.m_Pan.FixRotation();
                    CurrentRecomposer.m_Tilt = CurrentRecomposer.m_Tilt.FixRotation();

                    //when the mouse is released, if panning was being applied then push the camera back into pounds
                    if (CalculatePanningDampening(CurrentRecomposer.m_Pan, CurrentRecomposer.m_Tilt) != NoDampening)
                    {
                        StartCoroutine(AnimateCameraBackIntoBounds());
                    }

                    // Pokes task manager when pan task is complete
                    CameraTaskCheck();
                }
            }
        }

        /// <summary>
        /// This method is used to determiner if the camera panning speed should be slowed down. 
        /// This happens if the camera is dragged out of the max camera panning bounds. 
        /// The value that was calculated the last frame is used to determine if the camera is, being 
        /// dragged back into the bounds. If this is happening, then no dampening is a applied.
        /// </summary>
        /// <param name="x">Current camera pan in the x axis</param>
        /// <param name="y">Current camera pan in the y axis</param>
        /// <returns>A value between 0 - 1. This value can be multiplied with the current panning value to slow it down.
        /// If no dampening is needed the const <see cref="NoDampening"/> (1) is returned.</returns>
        private float CalculatePanningDampening(float x, float y)
        {
            //if the dolly is being used, apply the tilt offset
            float tiltOffset = CurrentDollyTiltOffset;

            //check if they are out of bounds
            if (Mathf.Abs(x) > maxCameraPan_X || y > (maxCameraPan_Y + tiltOffset) || y < (-maxCameraPan_Y + tiltOffset))
            {
                //calculate how far the passed the limit the camera is
                float totalPassedLimit = Mathf.Clamp(Mathf.Abs(x) - maxCameraPan_X, 0, float.PositiveInfinity);

                if (y > (maxCameraPan_Y + tiltOffset))
                {
                    totalPassedLimit += Mathf.Clamp(y - maxCameraPan_Y - tiltOffset, 0, float.PositiveInfinity);
                }
                else if (y < (-maxCameraPan_Y + tiltOffset))
                {
                    totalPassedLimit += Mathf.Clamp(-y - maxCameraPan_Y + tiltOffset, 0, float.PositiveInfinity);
                }

                //A simple equations the where the higher the totalPassedLimit, the lower the output is between 1 and 0
                float dampeningValue = 1 / (DampeningScale * totalPassedLimit + 1);

                //only use the new value if it is less than the old value
                // so only if they are panning further outside the view
                float outValue = dampeningValue <= LastPanningDampening ? dampeningValue : NoDampening;

                //save the dampening 
                LastPanningDampening = dampeningValue;

                return outValue;
            }

            return NoDampening;
        }

        /// <summary>
        /// Adjust the FOV of the current camera
        /// </summary>
        /// <param name="sliderValue">The slider value</param>
        public void OnZoomSliderChanged(float sliderValue)
        {
            CurrentVirtualCamera.m_Lens.FieldOfView = Mathf.Lerp(MaxCameraFOV, MinCameraFOV, sliderValue);
            CameraTaskCheck();
        }

        /// <summary>
        /// Allows the user to zoom the camera using the scroll wheel
        /// </summary>
        public void HandleScroll()
        {
            if (!InputManager.MouseOverUI && CurrentPOI.CanZoom && Input.mouseScrollDelta.y != 0)
            {
                //Get current zoom
                float currentZoom = ZoomSlider.value;

                //Add to current zoom the scroll offset reduced to the appropriate speed
                currentZoom += Input.mouseScrollDelta.y * ZoomScrollSpeed;

                //Eliminate chances that the zoom value could be out of range
                currentZoom = Mathf.Clamp(currentZoom, 0, 1);

                //Change the zoom
                OnZoomSliderChanged(currentZoom);

                //Have the zoom slider reflect the new zoom value
                ZoomSlider.value = currentZoom;
            }
        }

        /// <summary>
        /// Move the camera using the keyboard keys.
        /// If the camera is already being moved by the UI buttons, 
        /// the keyboard keys will be ignored.
        /// </summary>
        private void HandleMovementFromKeys()
        {
            if (!UiIsMovingCamera && CurrentPOI.CanMove)
            {
                bool movedLeft = false;

                foreach (KeyCode key in LeftMovementKeys)
                {
                    if (Hotkeys.GetKey(key))
                    {
                        MoveCamera(Directions.Left);
                        movedLeft = true;
                        break;
                    }
                }

                if (!movedLeft)
                {
                    foreach (KeyCode key in RightMovementKeys)
                    {
                        if (Hotkeys.GetKey(key))
                        {
                            MoveCamera(Directions.Right);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Move the dolly camera around the path. This method can be called every frame to get
        /// smooth movement.
        /// </summary>
        /// <param name="direction">The direction that the camera should move</param>
        public void MoveCamera(Directions direction)
        {
            //If we are supposed to flip the direction, flip it
            // This happens if the dolly path was drawn the wrong way
            if (CurrentSwapLeftRight)
            {
                direction = direction == Directions.Left ? Directions.Right : Directions.Left;
            }

            //this value can be changed when moving the between pois, so set it back here.
            CurrentTrackedDolly.m_ZDamping = 1;

            //calculate the speed
            float speed = Time.deltaTime * CurrentHorizontalSpeed * DollyMovementSpeed * (direction == Directions.Right ? -1 : 1);

            if (CurrentPath.Looped)
            {
                //move the path position
                CurrentTrackedDolly.m_PathPosition += speed;
            }
            else //the cameras path is not a loop
            {
                //if the camera is at the end of the path
                if (MovementLimit != MovementLimits.WithinBounds)
                {
                    //check if the direction coming in can move it back within limits
                    if (MovementLimit == MovementLimits.LeftLimitReached)
                    {
                        if (direction == Directions.Right)
                        {
                            MovementLimit = MovementLimits.WithinBounds;
                            ToggleMovementButtons(Directions.Left, true);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else if (MovementLimit == MovementLimits.RightLimitReached)
                    {
                        if (direction == Directions.Left)
                        {
                            MovementLimit = MovementLimits.WithinBounds;
                            ToggleMovementButtons(Directions.Right, true);
                        }
                        else
                        {
                            return;
                        }
                    }
                }

                float newPathPosition = CurrentTrackedDolly.m_PathPosition + speed;

                //check if the new position will push the camera to one of the limits of the non-looped path
                if (direction == Directions.Left && newPathPosition > NonLoopedPathFullyLeft)
                {
                    ToggleMovementButtons(Directions.Left, false);

                    MovementLimit = MovementLimits.LeftLimitReached;
                }
                else if (direction == Directions.Right && newPathPosition < NonLoopedPathFullyRight)
                {
                    ToggleMovementButtons(Directions.Right, false);

                    MovementLimit = MovementLimits.RightLimitReached;
                }

                //set the position of the camera
                CurrentTrackedDolly.m_PathPosition = Mathf.Clamp(newPathPosition, 0, 1);
            }

            UpdateLookAtAnchorWeights(mainCameraTransform.position);
            SlowlyDecreaseDollyPan();
        }

        /// <summary>
        /// This method will adjust the weight on the <see cref="CurrentTargetGroup"/> so that
        /// the camera is looking mostly at the closest one.
        /// </summary>
        /// <param name="position">The position that the weights should be updated for</param>
        private void UpdateLookAtAnchorWeights(Vector3 position)
        {
            if (CurrentTargetGroup.m_Targets.Length > 1)
            {
                float[] distances = new float[CurrentTargetGroup.m_Targets.Length];
                float totalDistance = 0;

                for (int i = 0; i < CurrentTargetGroup.m_Targets.Length; i++)
                {
                    distances[i] = SquDistance(CurrentTargetGroup.m_Targets[i].target.position, position);
                    totalDistance += distances[i];
                }

                for (int i = 0; i < CurrentTargetGroup.m_Targets.Length; i++)
                {
                    CurrentTargetGroup.m_Targets[i].weight = totalDistance / distances[i];
                }
            }
        }

        /// <summary>
        /// This method shall be called any time the camera moves to a POI
        /// that can move. It will make sure that the limits for non looped 
        /// paths are updated as well as making sure the left right movement
        /// buttons are enabled/disabled correctly.
        /// It will also make sure that the <see cref="MovementLimit"/> is updated 
        /// for non-looped paths
        /// </summary>
        private void UpdateMovementLimits()
        {
            if (CurrentPOI.Path.Looped)
            {
                //if the path is looped, always show both buttons
                ToggleMovementButtons(Directions.Left, true);
                ToggleMovementButtons(Directions.Right, true);
            }
            else
            {
                bool leftIsGood = true;
                bool rightIsGood = true;

                if (CurrentTrackedDolly.m_PathPosition == NonLoopedPathFullyLeft)
                {
                    leftIsGood = false;
                    MovementLimit = MovementLimits.LeftLimitReached;
                }
                else if (CurrentTrackedDolly.m_PathPosition == NonLoopedPathFullyRight)
                {
                    rightIsGood = false;
                    MovementLimit = MovementLimits.RightLimitReached;
                }

                if (leftIsGood && rightIsGood)
                {
                    MovementLimit = MovementLimits.WithinBounds;
                }

                ToggleMovementButtons(Directions.Left, leftIsGood);
                ToggleMovementButtons(Directions.Right, rightIsGood);
            }
        }

        /// <summary>
        /// This method should be called every time the camera is moved. 
        /// It will slowly rest the pan of the camera. This will make it 
        /// so that as if the user pans the camera to look at something at 
        /// one place, when they move to a new place, the camera will be more centered.
        /// </summary>
        private void SlowlyDecreaseDollyPan()
        {
            if (CurrentRecomposer == null)
                return;

            if (Mathf.Abs(CurrentRecomposer.m_Pan) > RestorePanAngleX * 5)
            {
                CurrentRecomposer.m_Pan += CurrentRecomposer.m_Pan > 0 ? -RestorePanAngleX : RestorePanAngleX;
            }

            //calculate the y value with the dolly tilt
            if (CurrentRecomposer.m_Tilt > RestorePanAngleY * 5 + CurrentDollyTiltOffset)
            {
                CurrentRecomposer.m_Tilt -= RestorePanAngleY;
            }
            else if (CurrentRecomposer.m_Tilt < -RestorePanAngleY * 5 + CurrentDollyTiltOffset)
            {
                CurrentRecomposer.m_Tilt += RestorePanAngleY;
            }

        }

        #endregion

        #region Points of Interest

        /// <summary>
        ///     Sends the camera to the specified <paramref name="newPOI"/> Point of Interest. 
        /// </summary>
        /// <param name="newPOI">The Point of Interest to go to.</param>
        public void SendCameraToPosition(PointOfInterest newPOI)
        {
            if (CurrentPOI == newPOI)
            {
                return;
            }

            // Before entering the "new" POI, the current POI has its OnExitPOI event invoked,
            // and the current POI is now considered to be "exited"
            CurrentPOI.OnExitPOI?.Invoke(newPOI);
            CurrentPOI.VirtualCamera.Priority = BasePriority;

            // Store the "old" POI locally for the remaining events.
            PointOfInterest previousPOI = CurrentPOI;

            // The "current" POI has now been updated
            CurrentPOI = newPOI;

            // Invoke OnEnterPOI for the "new" (now "current") POI, 
            // providing the "previous" POI (which was exited) as a parameter
            CurrentPOI.OnEnterPOI?.Invoke(previousPOI);
            CurrentPOI.VirtualCamera.Priority = HighPriority;

            // Invoke the wider OnPOIChanged event, providing both the "old" and "new" POIs
            OnPOIChanged?.Invoke(previousPOI, CurrentPOI);

            if (CurrentPOI.CanMove && !CurrentPOI.MaintainPositionOnTransition)
            {
                //make sure that the position on the path is close to where the current poi is
                CurrentPOI.TrackedDolly.m_PathPosition = GetClosestPathPoint(mainCameraTransform.position, CurrentPOI.Path);

                //make it so that the camera moves instantly on the track
                CurrentPOI.TrackedDolly.m_ZDamping = 0;

                //make sure the camera will be looking in the right direction
                UpdateLookAtAnchorWeights(CurrentPath.EvaluatePositionAtUnit(CurrentPOI.TrackedDolly.m_PathPosition, CinemachinePathBase.PositionUnits.Normalized));
            }

            UpdateMoveAndZoomPanel(CurrentPOI);
            StartBlendingCheck();
            UpdateCurrentViewModeButton();
        }

        /// <summary>
        /// This method will be used to update the view modes button that is highlighted.
        /// The highlighted button represents the POI that the camera is currently at.
        /// </summary>
        public void UpdateCurrentViewModeButton()
        {
            if (ViewButtons == null || !ViewModeButtonPanel.gameObject.activeSelf)
            {
                //the view buttons aren't visible
                return;
            }

            if (CurrentViewButton != null)
            {
                //unhighlight the button
                CurrentViewButton.SetSelected(false);
            }

            //find a button that corresponds to the poi
            CurrentViewButton = ViewButtons.Find(x => x.POI == CurrentPOI);

            if (CurrentViewButton != null)
            {
                //highlight the button
                CurrentViewButton.SetSelected(true);
            }

        }

        #endregion

        #region Coroutines

        /// <summary>
        /// This coroutine is used to smoothly animate the camera back into bounds if 
        /// it has been dragged too far.
        /// </summary>
        /// <returns>Co-Routine</returns>
        private IEnumerator AnimateCameraBackIntoBounds()
        {
            if (CurrentRecomposer == null)
            {
                yield break;
            }

            //get the starting and ending positions
            float startingX = CurrentRecomposer.m_Pan;
            float startingY = CurrentRecomposer.m_Tilt;
            float endingX = Mathf.Clamp(startingX, -maxCameraPan_X, maxCameraPan_X);
            float endingY = Mathf.Clamp(startingY, -maxCameraPan_Y, maxCameraPan_Y);

            //get the time it should take.
            float startingTime = Time.time;
            float endingTime = startingTime + BackIntoBoundsAnimationTime;
            float totalTime = endingTime - startingTime;

            //while the animation isn't over
            while (Time.time <= endingTime)
            {
                float timeRatio = (Time.time - startingTime) / totalTime;

                CurrentRecomposer.m_Pan = Mathf.SmoothStep(startingX, endingX, timeRatio);
                CurrentRecomposer.m_Tilt = Mathf.SmoothStep(startingY, endingY, timeRatio);

                yield return new WaitForEndOfFrame();
            }

            //ensure that it ends exactly at the ending points.
            CurrentRecomposer.m_Pan = endingX;
            CurrentRecomposer.m_Tilt = endingY;

            LastPanningDampening = NoDampening;
        }

        /* Method Header :  StartBlendingCheck
        * This method will start the blending check coroutine.
        * It will first make sure that the coroutine isn't already happening.
        */

        /// <summary>
        /// This method will start the blending check coroutine.
        /// It will first make sure that the coroutine isn't already happening.
        /// </summary>
        private void StartBlendingCheck()
        {
            //only activate if anything has actually attached itself to the events.
            if (OnCameraBlendingEnd != null || OnCameraBlendingStart != null)
            {
                if (BlendingCheckCoroutine != null)
                {
                    StopCoroutine(BlendingCheckCoroutine);
                }

                BlendingCheckCoroutine = StartCoroutine(CameraBlendingCheck());
            }
        }

        /// <summary>
        /// This coroutine will facilitate the OnCameraBlendingStart and
        /// OnCameraBlendingEnd events.These events happen at the start and end 
        /// of a camera transition.
        /// </summary>
        /// <returns>Co-routine</returns>
        private IEnumerator CameraBlendingCheck()
        {
            OnCameraBlendingStart?.Invoke();

            yield return new WaitForSecondsRealtime(0.1f);

            while (Brain.IsBlending)
            {
                yield return new WaitForEndOfFrame();
            }

            OnCameraBlendingEnd?.Invoke();
        }

        /// <summary>
        /// This method provides the functionality of a OnHold
        /// for the lean buttons.
        /// </summary>
        /// <param name="mode">
        /// The mode to move the camera.
        ///  0 = Move camera left,
        ///  1 = Move camera right
        /// </param>
        /// <returns>Co-routine</returns>
        private IEnumerator LeanOnHoldOverrideCoRoutine(int mode)
        {
            UiIsMovingCamera = true;

            while (Input.GetMouseButton(0))
            {
                MoveCamera(mode == 0 ? Directions.Left : Directions.Right);

                yield return new WaitForEndOfFrame();
            }

            UiIsMovingCamera = false;
        }

        #endregion

        #region UI

        /// <summary>
        /// Update the POIs that are visible in the poi button box.
        /// </summary>
        /// <param name="visiblePoints">The POIs that should be shown.</param>
        public void UpdatePoiButtons(List<PointOfInterest> visiblePoints)
        {
            foreach (POIViewModesButton button in ViewButtons)
            {
                button.gameObject.SetActive(visiblePoints.Contains(button.POI));
            }

            ViewModeButtonPanel.Initialize();
        }

        /// <summar>
        /// Show or hide the POI button panel. The alpha on a 
        /// canvas group is changed, so the game object will still be active.
        /// </summary>
        /// <param name="visible">If the panel should be shown</param>
        public void TogglePOIButtonPanelVisability(bool visible)
        {
            ViewModeButtonPanel.CanvasGroup.alpha = visible ? 1 : 0;
            ViewModeButtonPanel.CanvasGroup.blocksRaycasts = visible;
            ViewModeButtonPanel.CanvasGroup.interactable = visible;
        }

        /// <summary>
        /// Toggle what parts of the move and zoom panel are visible.
        /// If both sections of the panel aren't visible, the whole panel
        /// will be hidden.
        /// </summary>
        /// <param name="poi">The POI the camera is going to</param>
        public void UpdateMoveAndZoomPanel(PointOfInterest poi)
        {
            if (poi.CanZoom || poi.CanMove)
            {
                MoveAndZoomPanel.SetActive(true);

                ZoomControlPanel.SetActive(poi.CanZoom);
                MovementControlButtons.SetActive(poi.CanMove);

                //update which buttons are active
                if (poi.CanMove)
                {
                    UpdateMovementLimits();
                }

                if (poi.CanZoom)
                {
                    ZoomSlider.value = Mathf.InverseLerp(MaxCameraFOV, MinCameraFOV, poi.VirtualCamera.m_Lens.FieldOfView);
                }
            }
            else
            {
                MoveAndZoomPanel.SetActive(false);
            }
        }

        /// <summary>
        /// Toggle if any of the Camera Controller UI is visible.
        /// </summary>
        /// <param name="visible">If the UI is visible</param>
        public void ToggleAllUIVisability(bool visible)
        {
            if (visible)
            {
                UpdateMoveAndZoomPanel(CurrentPOI);
                ViewModeButtonPanel.gameObject.SetActive(true);
            }
            else
            {
                MoveAndZoomPanel.SetActive(false);
                ViewModeButtonPanel.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Toggles if a movement button can be clicked.
        /// The SwapLeftRight is used to go between the internal math 
        /// of this class and the orientation of the path.
        /// The class will always treat a value of 1 on the path as fully
        /// left. A value of 0 is fully right.
        /// The orientation of the path is based on the scene and the target group.
        /// </summary>
        /// <param name="side">The internal side to toggle</param>
        /// <param name="on">If the button should be active</param>
        private void ToggleMovementButtons(Directions side, bool on)
        {
            if (CurrentSwapLeftRight)
            {
                side = side == Directions.Left ? Directions.Right : Directions.Left;
            }

            if (side == Directions.Left)
            {
                LeftMovementButton.On = on;
            }
            else
            {
                RightMovementButton.On = on;
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Calculate the distance squared between 2 vectors.
        /// This is useful for comparing distances. It is faster than
        /// calculating the actual distance since we don't need to do a 
        /// square root calculation.
        /// </summary>
        /// <param name="a">The first vector</param>
        /// <param name="b">The second vector</param>
        /// <returns>The distance between the 2 vectors squared</returns>
        private float SquDistance(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x - b.x, 0, a.z - b.z).sqrMagnitude;
        }

        /// <summary>
        /// Calculate how fast a camera should move around a path 
        /// based on its length.
        /// </summary>
        /// <param name="pathLength">The path length</param>
        /// <returns>The speed that should be used for that path length</returns>
        public static float CalculateHorizontalSpeed(float pathLength)
        {
            return PathLengthToSpeed / pathLength;
        }

        /// <summary>
        /// Toggle if another gameobject is using the mouse to drag.
        /// This will lock the panning for the dolly camera.
        /// By locking the dolly's panning ability, you will be
        /// able to drag other objects in the scene while 
        /// the camera is on the dolly.
        /// </summary>
        /// <param name="usingDragging">If an object is dragging the mouse.</param>
        public static void ToggleObjectIsDragging(bool usingDragging)
        {
            if (Instance != null)
            {
                Instance.PanningLocked = usingDragging;
            }
        }

        /// <summary>
        /// Get the closest position on a path. This should be used
        /// when moving to a POI on a path so that the point on the 
        /// path we move to is as close as possible.
        /// </summary>
        /// <param name="current">The point to test the distance from</param>
        /// <param name="path">The path to check</param>
        /// <returns>The location on the path closed to the point passed in</returns>
        public float GetClosestPathPoint(Vector3 current, CinemachinePath path)
        {
            float resolution = 0.1f;
            float closestValue = 0;
            float currentLowestDistance = float.PositiveInfinity;

            for (float i = 0; i <= 1; i += resolution)
            {
                float distance = SquDistance(current, path.EvaluatePositionAtUnit(i, CinemachinePathBase.PositionUnits.Normalized));

                if (distance < currentLowestDistance)
                {
                    currentLowestDistance = distance;
                    closestValue = i;
                }
            }

            return closestValue;
        }

        #endregion

        #region Taskable
        private void CameraTaskCheck()
        {
            cameraChanged = true;
            Taskable.PokeTaskManager();
            cameraChanged = false;
        }

        public object CheckTask(int checkType, object inputData)
        {
            switch (checkType)
            {
                case (int)TaskVertexManager.CheckTypes.Bool:
                    return cameraChanged;
                default:
                    Debug.LogError("Invalid check type passed into CoreCameraController");
                    break;
            }

            return null;
        }
        #endregion
    }
}
