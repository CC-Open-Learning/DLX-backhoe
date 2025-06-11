using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RemoteEducation.Scenarios.Inspectable;
using RemoteEducation.Interactions;
using RemoteEducation.UI.Tooltip;
using UnityEngine.UI;
using RemoteEducation.Localization;

namespace RemoteEducation.Modules.HeavyEquipment
{
    [RequireComponent(typeof(SimpleTooltip))]
    [RequireComponent(typeof(Image))]
    public class GaugePanelLight : MonoBehaviour
    {
        public enum TypeOfGaugeLight
        {
            None,
            EngineCoolant,
            HydraulicOilTemp,
            TransmissionFluidTemp,
            FourWheel,
            AutomaticEngineSpeedControl,
            EngineOilPressure,
            ChargingSystem,
            ParkBrake,
            LowFuel,
            HydraulicBypass,
            ActionLight,
            FuelWaterSeparator,
            AirFilter,
            WaitToStartIndicator,
            EngineCoolant2
        }

        public TypeOfGaugeLight GaugeType;

        public GameObject ObjectReference;

        private bool IndicatorStatus;

        private SimpleTooltip simpleTooltip;

        private Image image;

        /// <summary> Utilizing Generic Inspectable, function that is fired off by UserInputPanel</summary>
        void Start()
        {
            image = GetComponent<Image>();

            simpleTooltip = GetComponent<SimpleTooltip>();

            switch (GaugeType)
            {
                case TypeOfGaugeLight.EngineCoolant:
                    simpleTooltip.tooltipText = Localizer.Localize("HeavyEquipment.GaugePanelLightEngineCoolant");
                    break;
                case TypeOfGaugeLight.HydraulicOilTemp:
                    simpleTooltip.tooltipText = Localizer.Localize("HeavyEquipment.GaugePanelLightHydraulicOilTemp");
                    break;
                case TypeOfGaugeLight.TransmissionFluidTemp:
                    simpleTooltip.tooltipText = Localizer.Localize("HeavyEquipment.GaugePanelLightTransmissionFluidTemp");
                    break;
                case TypeOfGaugeLight.FourWheel:
                    simpleTooltip.tooltipText = Localizer.Localize("HeavyEquipment.GaugePanelLightFourWheel");
                    break;
                case TypeOfGaugeLight.AutomaticEngineSpeedControl:
                    simpleTooltip.tooltipText = Localizer.Localize("HeavyEquipment.GaugePanelLightAutomaticEngineSpeedControl");
                    break;
                case TypeOfGaugeLight.EngineOilPressure:
                    simpleTooltip.tooltipText = Localizer.Localize("HeavyEquipment.GaugePanelLightEngineOilPressure");
                    break;
                case TypeOfGaugeLight.ChargingSystem:
                    simpleTooltip.tooltipText = Localizer.Localize("HeavyEquipment.GaugePanelLightChargingSystem");
                    break;
                case TypeOfGaugeLight.ParkBrake:
                    simpleTooltip.tooltipText = Localizer.Localize("HeavyEquipment.GaugePanelLightParkBrake");
                    break;
                case TypeOfGaugeLight.HydraulicBypass:
                    simpleTooltip.tooltipText = Localizer.Localize("HeavyEquipment.GaugePanelLightHydraulicBypass");
                    break;
                case TypeOfGaugeLight.ActionLight:
                    simpleTooltip.tooltipText = Localizer.Localize("HeavyEquipment.GaugePanelLightActionLight");
                    break;
                case TypeOfGaugeLight.FuelWaterSeparator:
                    simpleTooltip.tooltipText = Localizer.Localize("HeavyEquipment.GaugePanelLightFuelWaterSeparator");
                    break;
                case TypeOfGaugeLight.AirFilter:
                    simpleTooltip.tooltipText = Localizer.Localize("HeavyEquipment.GaugePanelLightAirFilter");
                    break;
                case TypeOfGaugeLight.WaitToStartIndicator:
                    simpleTooltip.tooltipText = Localizer.Localize("HeavyEquipment.GaugePanelLightWaitToStartIndicator");
                    break;
                default:
                    simpleTooltip.tooltipText = "";
                    simpleTooltip.enabled = false;
                    break;
            }
        }

        public void CheckIfComplete()
        {
            switch (GaugeType)
            {
                case TypeOfGaugeLight.EngineCoolant:
                    IndicatorStatus = CheckEngineCoolant();
                    break;
                case TypeOfGaugeLight.HydraulicOilTemp:
                    IndicatorStatus = CheckHydraulicOilTemp();
                    break;
                case TypeOfGaugeLight.TransmissionFluidTemp:
                    IndicatorStatus = CheckTransmissionFluidTemp();
                    break;
                case TypeOfGaugeLight.FourWheel:
                    IndicatorStatus = CheckFourWheel();
                    break;
                case TypeOfGaugeLight.AutomaticEngineSpeedControl:
                    IndicatorStatus = CheckAutomaticEngineSpeedControl();
                    break;
                case TypeOfGaugeLight.EngineOilPressure:
                    IndicatorStatus = CheckEngineOilPressure();
                    break;
                case TypeOfGaugeLight.ChargingSystem:
                    IndicatorStatus = CheckChargingSystem();
                    break;
                case TypeOfGaugeLight.ParkBrake:
                    IndicatorStatus = CheckParkBrake();
                    break;
                case TypeOfGaugeLight.LowFuel:
                    IndicatorStatus = CheckLowFuel();
                    break;
                case TypeOfGaugeLight.HydraulicBypass:
                    IndicatorStatus = CheckHydraulicBypass();
                    break;
                case TypeOfGaugeLight.ActionLight:
                    IndicatorStatus = CheckActionLight();
                    break;
                case TypeOfGaugeLight.FuelWaterSeparator:
                    IndicatorStatus = CheckFuelWaterSeparator();
                    break;
                case TypeOfGaugeLight.AirFilter:
                    IndicatorStatus = CheckAirFilter();
                    break;
                case TypeOfGaugeLight.WaitToStartIndicator:
                    IndicatorStatus = CheckWaitToStartIndicator();
                    break;
            }
        }

        public void SetLightStatus(bool status)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, status ? 1 : 0.1f);
        }

        public void SetIndicatorStatus(bool newStatus)
        {
            IndicatorStatus = newStatus;
        }

        public bool GetIndicatorStatus()
        {
            return IndicatorStatus;
        }

        private bool CheckEngineCoolant()
        {
            EngineCoolant tempCoolant = ObjectReference.GetComponent<EngineCoolant>();

            if (tempCoolant == null)
            {
                Debug.Log("Error: Coolant Object Reference Not Found. Value may be null or be non-existent on object. No changes to value.");
                return IndicatorStatus;
            }

            return tempCoolant.GetTemperature() == EngineCoolant.Temperature.Hot ? true : false;
        }

        private bool CheckHydraulicOilTemp()
        {
            Gauge tempGauge = ObjectReference.GetComponent<Gauge>();

            if (tempGauge == null)
            {
                Debug.Log("Error: Gauge Object Reference Not Found. Value may be null or be non-existent on object. No changes to value.");
                return IndicatorStatus;
            }

            return tempGauge.GetTemperature() == Gauge.Temperature.Hot ? true : false;
        }

        private bool CheckTransmissionFluidTemp()
        {
            //There is no inspection for torque converter therefore we simply assume that it's good unless dictated otherwise
            return false;
        }

        private bool CheckFourWheel()
        {
            //There is no inspection/mechanics for four wheel drive so we assume that it's all good unless dictated otherwise
            return false;
        }

        private bool CheckAutomaticEngineSpeedControl()
        {
            //There is no inspection/mechanics for automatic speed control so we assume that it's all good unless dictated otherwise
            return false;
        }

        private bool CheckEngineOilPressure()
        {
            //There is no inspection/mechanics for pressure, only temp so we assume that it's all good unless dictated otherwise
            return false;
        }

        private bool CheckChargingSystem()
        {
            //There is no inspection/mechanics for the battery so we assume that it's all good unless dictated otherwise
            return false;
        }

        private bool CheckParkBrake()
        {
            ParkingBrake tempCheck = ObjectReference.GetComponent<ParkingBrake>();

            if (tempCheck == null)
            {
                Debug.Log("Error: Parking Brake Object Reference Not Found. Value may be null or be non-existent on object. No changes to value.");
                return IndicatorStatus;
            }

            return tempCheck.CheckIfEngaged() ? true : false;
        }

        private bool CheckLowFuel()
        {
            Gauge tempGauge = ObjectReference.GetComponent<Gauge>();

            if (tempGauge == null)
            {
                Debug.Log("Error: Gauge Object Reference Not Found. Value may be null or be non-existent on object. No changes to value.");
                return IndicatorStatus;
            }

            return tempGauge.Full ? false : true;
        }

        private bool CheckHydraulicBypass()
        {
            //Assume it to be similar to hydraulic oil temp because filter does not have functionality
            Gauge tempGauge = ObjectReference.GetComponent<Gauge>();

            if (tempGauge == null)
            {
                Debug.Log("Error: Gauge Object Reference Not Found. Value may be null or be non-existent on object. No changes to value.");
                return IndicatorStatus;
            }

            return tempGauge.GetTemperature() == Gauge.Temperature.Hot ? true : false;
        }

        private bool CheckActionLight()
        {
            //There is no inspection/mechanics for determing a malfunction in the machine system so we assume that it's all good unless dictated otherwise
            return false;
        }

        private bool CheckFuelWaterSeparator()
        {
            FuelWaterSeparator tempCheck = ObjectReference.GetComponent<FuelWaterSeparator>();

            if (tempCheck == null)
            {
                Debug.Log("Error: Fuel Water Separator Object Reference Not Found. Value may be null or be non-existent on object. No changes to value.");
                return IndicatorStatus;
            }

            return tempCheck.CheckDrained() ? false : true;
        }

        private bool CheckAirFilter()
        {
            //There is no inspection/mechanics for the air filter so we assume that it's all good unless dictated otherwise
            return false;
        }

        private bool CheckWaitToStartIndicator()
        {
            //The wait to start mechanic simply asks for the user not to turn on the engine while it is on
            //it defaults off afterwards therefore needs no checks but the function was made for consistency
            return false;
        }
    }
}
