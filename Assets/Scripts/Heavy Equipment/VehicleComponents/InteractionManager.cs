using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RemoteEducation.Interactions;

namespace RemoteEducation.Modules.HeavyEquipment
{
    public class InteractionManager : MonoBehaviour
    {
        [Tooltip("Debris Groups")]
        [SerializeField] private List<RemovableDebrisGroup> debrisGroups;

        [Tooltip("Steering Wheel")]
        [SerializeField] private Steering steeringWheel;

        [Tooltip("Lift Arm")]
        [SerializeField] private LiftArm liftArm;

        [Tooltip("Air Filter")]
        [SerializeField] private AirFilterServiceIndicator airFilter;

        [Tooltip("Pins")]
        [SerializeField] private List<Pin> pins;

        [Tooltip("Left Side Panel")]
        [SerializeField] private SidePanel leftPanel;

        [Tooltip("Wheel Nuts")]
        [SerializeField] private List<WheelNut> wheelNuts;

        [Tooltip("Bucket Fasteners")]
        [SerializeField] private List<Bolt> bucketFasteners;

        [Tooltip("Wiper Lever")]
        [SerializeField] private WiperLever lever;

        [Tooltip("Bucket Teeth")]
        [SerializeField] private List<FallingTooth> bucketTeeth;

        [Tooltip("Rear Bucket Bracket")]
        [SerializeField] public List<HydraulicBracket> brackets;

        private BackhoeController backhoeRef;

        public void ToggleAllInteractables(bool turnOn, params Interactable[] interactables)
        {
            foreach (Interactable temp in backhoeRef.Windows)
            {
                temp.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            }

            backhoeRef.FrontHood.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
        }

        public void SetDebris(bool state)
        {
            foreach (RemovableDebrisGroup temp in debrisGroups)
            {
                temp.ToggleGroupInteractability(!state);
            }
        }

        public void SetBucketFasteners(bool state)
        {
            foreach (Bolt temp in bucketFasteners)
            {
                temp.ToggleFlags(state, Interactable.Flags.InteractionsDisabled);
            }
        }

        public void SetPins(bool state)
        {
            foreach (Pin temp in pins)
            {
                temp.ToggleFlags(state, Interactable.Flags.InteractionsDisabled);
            }
        }

        public void SetBoomBucketTeeth(bool state)
        {
            foreach (FallingTooth temp in bucketTeeth)
            {
                temp.ToggleFlags(!state, Interactable.Flags.InteractionsDisabled);
            }
        }

        public void SetWheelNuts(bool state)
        {
            foreach (WheelNut temp in wheelNuts)
            {
                temp.ToggleFlags(state, Interactable.Flags.InteractionsDisabled);
            }
        }

        public void SetSteeringWheel(bool state)
        {
            steeringWheel.ToggleFlags(state, Interactable.Flags.InteractionsDisabled);
        }

        public void SetWheels(bool state)
        {
            foreach (Wheel temp in backhoeRef.Wheels)
            {
                temp.GetComponent<Interactable>().ToggleFlags(state, Interactable.Flags.InteractionsDisabled);
            }
        }

        public void SetAxles(bool state)
        {
            foreach (Axle temp in backhoeRef.Axles)
            {
                temp.GetComponent<Interactable>().ToggleFlags(state, Interactable.Flags.InteractionsDisabled);
            }
        }

        public void SetLiftArm(bool state)
        {
            liftArm.GetComponent<Interactable>().ToggleFlags(state, Interactable.Flags.InteractionsDisabled);
        }

        public void SetGrousers(bool state)
        {
            foreach (PadGrouser temp in backhoeRef.PadGrousers)
            {
                temp.GetComponent<Interactable>().ToggleFlags(state, Interactable.Flags.InteractionsDisabled);
            }
        }

        public void SetLever(bool state)
        {
            lever.ToggleFlags(state, Interactable.Flags.InteractionsDisabled);
        }

        public void TurnOffAllInteractables(object backhoe = null)
        {
            if (backhoe != null)
            {
                backhoeRef = backhoe as BackhoeController;
            }

            if (backhoeRef == null)
            {
                Debug.Log("Unable to get reference to backhoe for Interaction Manager");
                return;
            }

            foreach (Interactable temp in backhoeRef.Windows)
            {
                temp.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            }

            foreach (Wheel temp in backhoeRef.Wheels)
            {
                temp.GetComponent<Interactable>().ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            }

            foreach (RemovableDebrisGroup temp in debrisGroups)
            {
                temp.ToggleGroupInteractability(true);
            }

            backhoeRef.FrontHood.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            backhoeRef.EngineCoolant.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            backhoeRef.EngineCoolant.Cap.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            backhoeRef.EngineCoolant.Fluid.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            backhoeRef.WiperFluidReservoir.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            backhoeRef.EngineDrip.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            backhoeRef.EngineOil.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            backhoeRef.TransmissionDrip.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            backhoeRef.SecondTransmissionDrip.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            backhoeRef.TransmissionOil.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            backhoeRef.FuelWaterSeparator.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            backhoeRef.Gauge.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            backhoeRef.Puddles.GetComponent<Interactable>().ToggleFlags(true, Interactable.Flags.InteractionsDisabled);

            foreach(CabDoor temp in backhoeRef.CabDoors)
            {
                temp.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            }

            backhoeRef.Key.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            backhoeRef.FuelTank.GetFuelGauge().ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            backhoeRef.FuelTank.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            backhoeRef.Bucket.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            backhoeRef.LiftLock.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            backhoeRef.LoaderControl.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            backhoeRef.RightSidePanel.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            backhoeRef.EngineBelt.GetComponent<Interactable>().ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            backhoeRef.EngineWiring.GetComponent<Interactable>().ToggleFlags(true, Interactable.Flags.InteractionsDisabled);

            
            foreach(Axle temp in backhoeRef.Axles)
            {
                temp.GetComponent<Interactable>().ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            }

            steeringWheel.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            backhoeRef.ColumnLock.steerLock.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            backhoeRef.ROPS.GetComponent<Interactable>().ToggleFlags(true, Interactable.Flags.InteractionsDisabled);

            foreach(PadGrouser temp in backhoeRef.PadGrousers)
            {
                temp.GetComponent<Interactable>().ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            }

            backhoeRef.ReverseAlarm.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            backhoeRef.BackhoeBrakePedals.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            backhoeRef.ParkingBrake.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);

            foreach(HydraulicLine temp in backhoeRef.HydraulicLines)
            {
                temp.GetComponent<Interactable>().ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            }

            foreach(Piston temp in backhoeRef.Pistons)
            {
                temp.GetComponent<Interactable>().ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            }

            liftArm.GetComponent<Interactable>().ToggleFlags(true, Interactable.Flags.InteractionsDisabled);

            airFilter.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);

            backhoeRef.GaugePanelAnimator.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);

            foreach (Pin temp in pins)
            {
                temp.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            }

            backhoeRef.GearShifter.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);

            backhoeRef.RightSidePanel.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            leftPanel.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);

            foreach (Bolt temp in bucketFasteners)
            {
                temp.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            }

            foreach (WheelNut temp in wheelNuts)
            {
                temp.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            }

            backhoeRef.WiperBtn.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            backhoeRef.WindShieldWipers.GetComponent<Interactable>().ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            lever.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);

            backhoeRef.HornBtn.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);

            foreach (FallingTooth temp in bucketTeeth)
            {
                temp.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            }
            backhoeRef.BoomBucket.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);

            backhoeRef.ColumnLock.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);

            backhoeRef.AirFilter.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);

            backhoeRef.HydraulicFluidReservoir.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            backhoeRef.SwingLockoutPin.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);

            foreach (IndicatorLight iL in backhoeRef.IndicatorLights) iL.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);

            foreach (LightGroup lG in backhoeRef.Lights) lG.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);

            foreach (Interactable temp in brackets)
            {
                temp.ToggleFlags(true, Interactable.Flags.InteractionsDisabled);
            }
        }
    }
}