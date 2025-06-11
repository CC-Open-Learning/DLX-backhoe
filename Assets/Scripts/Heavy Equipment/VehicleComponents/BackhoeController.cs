using RemoteEducation.Interactions;
using RemoteEducation.Scenarios.Inspectable;
using System.Collections.Generic;
using UnityEngine;

namespace RemoteEducation.Modules.HeavyEquipment
{
    public enum Sides
    {
        Right,
        Left,
        Front,
        Back
    }

    public enum Positions
    {
        RightFront,
        RightBack,
        LeftFront,
        LeftBack
    }

    public sealed class BackhoeController : InspectableObject
    {
        /* All of the components that make up the Backhoe should be referenced in this script.
         * (The doors, wipers, etc)
         */

        public Steering SteeringWheel;  

        public FrontHood FrontHood;

        public SidePanel RightSidePanel;

        public SidePanel LeftSidePanel;

        public FrontLoader FrontLoader;

        public LoaderControl LoaderControl;

        public LiftLock LiftLock;

        public List<CabDoor> CabDoors;

        public WindShieldWiperController WindShieldWipers;

        public List<Wheel> Wheels;

        public List<Axle> Axles;

        public List<IndicatorLight> IndicatorLights;

        public List<LightGroup> Lights;

        public Bucket Bucket;

        public BoomBucket BoomBucket;

        public List<CabWindow> Windows;

        public List<PadGrouser> PadGrousers;

        public List<Piston> Pistons;
        public bool IsEngineOn = false;

        public List<HydraulicLine> HydraulicLines;

        public EngineCoolant EngineCoolant;

        public EngineBelt EngineBelt;

        public Horn Horn;

        public ReverseAlarm ReverseAlarm;

        public BoomLockLever BoomLockLever;

        public BoomLockLatch BoomLockLatch;

        public SwingLockoutPin SwingLockoutPin;

        public ROPS ROPS;

        public BrakePedalsManager BackhoeBrakePedals;

        public List<PinGroup> PinsRetainers;

        public HydraulicFluidReservoir HydraulicFluidReservoir;

        public FuelWaterSeparator FuelWaterSeparator;

        public ParkingBrake ParkingBrake;

        public FuelTank FuelTank;

        public PadGrouser LeftLeg;

        public PadGrouser RightLeg;

        [Tooltip("The Engine Drip on the Backhoe")]
        public DripCheck EngineDrip;

        public OilStain EngineOil;

        [Tooltip("The Transmission Drip on Backhoe")]
        public DripCheck TransmissionDrip;

        public OilStain TransmissionOil;

        [Tooltip("The Second Transmission Drip on Backhoe (second part of inspection)")]
        public DripCheck SecondTransmissionDrip;

        public OilStain SecondTransmissionOil;

        public Puddles Puddles;

        public Key Key;

        public Gauge Gauge;

        public GaugePanelController GaugePanel;

        public GaugePanel GaugePanelAnimator;

        public WiperButton WiperBtn;

        public EngineSoundController EngineSound;

        public GearShifter GearShifter;

        public Horn HornBtn;

        public WiperFluidReservoir WiperFluidReservoir;

        public EngineWiring EngineWiring;

        public InteractionManager InteractManager;

        public SteerColumnLock ColumnLock;

        public AirFilterServiceIndicator AirFilter;

        public BackhoeLightController BackhoeLightController;

        public Pin LeftHydraulicPin, RightHydraulicPin;

        public void Initialize()
        {
            foreach (IInitializable initializable in GetComponentsInChildren<IInitializable>())
            {
                initializable.Initialize(this);
            }

            InteractManager.TurnOffAllInteractables(this);
        }
    }
}