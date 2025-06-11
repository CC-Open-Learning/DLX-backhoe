using RemoteEducation.Scenarios;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RemoteEducation.Modules.HeavyEquipment
{
    public class PreTripInspectionData : MonoBehaviour
    {
        public enum CameraLocations
        {
            Engine,
            CoolantCloseup,
            FrontLeftWheel,
            FrontRightWheel,
            BackLeftWheel,
            BackRightWheel,
            UnderFrontAxle,
            Overview,
            InsideCabin,
            LeftLiftInspection,
            RightLiftInspection,
            LiftArmInspection,
            LeftDoor,
            RightDoor,
            Bucket,
            BucketCloseup,
            LoaderBucketBehind,
            LoaderBucketBehind2,
            BackOfBucket,
            InsideCab,
            OutsideDoor,
            RearBoomLeftSide,
            RearBoomFrontSide,
            RearBoomRightSide,
            RearBucketLinkage,
            SwingLinkageCloseUp,
            LightsDollyPOI,
            FuelWaterSeparatorCloseUp,
            UnderRearAxle,
            LeftPadGrouser,
            RightPadGrouser,
            InsideCabRightConsole,
            GrouserDebris,
            RightPadGrouserFront,
            LeftPadGrouserFront,
            EngineLoaderLeftSide,
            EngineLoaderRightSide,
            InteriorSidePanel,
            BucketLinkageDebris,
            WiperPOI,
            WheelsDolly,
            LiftArmDolly,
            GrouserDolly,
            EngineLoaderDolly,
            ChecklistPOI,
            DipstickPOI,
            KeyPOI,
            HydraulicFluidReservoirPOI,
            BoomLatchLeverPOI,
            EngineBeltPOI,
            EngineWiringPOI,
            CabPedalsPOI,
            ParkingBrakePOI,
            GearShifterPOI,
            WiperFluidPOI,
            EngineCoolantPOI,
            TransmissionDipstickPOI,
            RearLinkagePOIRight,
            GrouserZoomLeft,
            GrouserZoomRight,
            BracketLeft,
            EngineFilterPOI,
            InspectionStartPOI,
            RearBoomLockZoomedOutPOI,
            TopLightPOI,
            FrontLeftLightsPOI,
            FrontRightLightsPOI,
            RearLeftLightsPOI
        }

        [Tooltip("The Inspectable group for flat Tires and broken Windows")]
        public InspectableGroup Task1_WheelsAndWindows;

        [Tooltip("Wiper Washer Reservoir")]
        public InspectableGroup WiperFluidReservoir;

        [Tooltip("Front, Top, and Rear Lights of the backhoe")]
        public InspectableGroup BackHoeLights;

        [Tooltip("Front Loader Bucket")]
        public InspectableGroup FrontBucket;

        [Tooltip("Front Right Tire")]
        public InspectableGroup FrontRightTire;

        [Tooltip("Front Left Tire")]
        public InspectableGroup FrontLeftTire;

        [Tooltip("Back Right Tire")]
        public InspectableGroup BackRightTire;

        [Tooltip("Back Left Tire")]
        public InspectableGroup BackLeftTire;

        [Tooltip("Front Axle")]
        public InspectableGroup FrontAxle;

        [Tooltip("Left Piston")]
        public InspectableGroup LeftPiston;

        [Tooltip("Backhoe Bodyframe")]
        public InspectableGroup ROPS;

        [Tooltip("Right Piston")]
        public InspectableGroup RightPiston;

        [Tooltip("Left Hydraulics")]
        public InspectableGroup LeftHydraulics;

        [Tooltip("Right Hydraulics")]
        public InspectableGroup RightHydraulics;

        [Tooltip("Left Door and Windows")]
        public InspectableGroup LeftDoorAndWindows;

        [Tooltip("Right Door and Windows")]
        public InspectableGroup RightDoorAndWindows;

        [Tooltip("Lift Arm")]
        public InspectableGroup LiftArm;

        [Tooltip("Front Loader Hydraulics")]
        public InspectableGroup Stage8Hydraulics;

        [Tooltip("Engine Belt")]
        public InspectableGroup EngineBelt;

        [Tooltip("Engine Wiring")]
        public InspectableGroup EngineWiring;

        [Tooltip("The Inspectable group for Swing Linkage Hydraulics")]
        public InspectableGroup Stage14Hydraulics;

        #region Stage 15 to 19

        [Tooltip("The Inspectable group for Rear Brackets")]
        public InspectableGroup RearBoomBrackets;

        [Tooltip("The Inspectable group for Rear Pistons")]
        public InspectableGroup RearBoomPistons;

        [Tooltip("The Inspectable group for Rear Lines")]
        public InspectableGroup RearBoomLines;


        [Tooltip("The Inspectable group for Rear Boom (right side)")]
        public InspectableGroup RearBoomRightSide;

        [Tooltip("The Inspectable group for Rear Bucket Hydraulic")]
        public InspectableGroup RearBucketHydraulic;

        #endregion

        #region Stage 21

        [Tooltip("Brake Pedals")]
        public InspectableGroup BrakePedals;

        [Tooltip("Parking Brake")]
        public InspectableGroup ParkingBrake;

        [Tooltip("Windshield Wiper")]
        public InspectableGroup Wiper;

        [Tooltip("Reverse Alarm")]
        public InspectableGroup ReverseAlarm;

        [Tooltip("Horn")]
        public InspectableGroup Horn;

        [Tooltip("Gear Shifter")]
        public InspectableGroup GearShifter;

        [Tooltip("Rear Windows")]
        public InspectableGroup RearWindows;

        #endregion

        [Tooltip("The Backhoe controller in the scene")]
        public BackhoeController BackhoeController;

        [SerializeField]
        private List<POIAndEnum> ScenarioCameraLocations;

        /// <summary>
        /// Get a <see cref="PointOfInterest"/> based on the <see cref="CameraLocations"/> passed in.
        /// </summary>
        public PointOfInterest GetPOI(CameraLocations cameraLocation)
        {
            POIAndEnum poi = ScenarioCameraLocations.Find(x => x.CameraLocation == cameraLocation);

            if (poi == null)
            {
                Debug.LogError($"The POI for {cameraLocation} was not found. It may need to be Serialized in the Unity Inspector");
                return null;
            }

            return poi.POI;
        }
    }

    /// <summary>
    /// This class is used so that you can have a list of <see cref="PointOfInterest"/> and with a 
    /// corresponding location tracked with a <see cref="PreTripInspectionData.CameraLocations"/>.
    /// </summary>
    [Serializable]
    public class POIAndEnum
    {
        public PreTripInspectionData.CameraLocations CameraLocation;
        public PointOfInterest POI;
    }
}
