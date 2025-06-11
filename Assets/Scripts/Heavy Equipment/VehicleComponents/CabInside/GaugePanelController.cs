using RemoteEducation.Interactions;
using RemoteEducation.Scenarios;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

namespace RemoteEducation.Modules.HeavyEquipment
{
    public class GaugePanelController : MonoBehaviour, IInitializable
    {
        [SerializeField, Tooltip("List of gauge panel lights")]
        private List<GaugePanelLight> listOfGaugeLights;

        [SerializeField, Tooltip("Coolant Gauge Pin")]
        private Transform CoolantGauge;
        [SerializeField, Tooltip("Coolant Gauge Max Position")]
        private Transform CoolantGaugeMax;
        [SerializeField, Tooltip("Coolant Gauge Min Position")]
        private Transform CoolantGaugeMin;

        [SerializeField, Tooltip("Tachometer Pin")]
        private Transform TachometerGauge;
        [SerializeField, Tooltip("Tachometer Max Position")]
        private Transform TachometerMax;
        [SerializeField, Tooltip("Tachometer Min Position")]
        private Transform TachometerMin;

        [SerializeField, Tooltip("Fuel Level Gauge Pin")]
        private Transform FuelLevelGauge;
        [SerializeField, Tooltip("Fuel Level Max Position")]
        private Transform FuelLevelMax;
        [SerializeField, Tooltip("Fuel Level Min Position")]
        private Transform FuelLevelMin;

        [SerializeField, Tooltip("Target Coolant Value from 0 to 1")]
        private float CoolantObj;
        [SerializeField, Tooltip("Target Tachometer Value from 0 to 1")]
        private float TachometerObj;
        [SerializeField, Tooltip("Target Fuel Value from 0 to 1")]
        private float FuelObj;

        [SerializeField, Tooltip("Overall animation size")]
        private float AnimationDuration = 3;

        [SerializeField, Tooltip("The time the lights have to stay on until systems are readable")]
        private float LightsOnDuration = 1;

        [SerializeField, Tooltip("The time it takes to get to max")]
        private float GaugeMaxDuration = 1;

        [SerializeField, Tooltip("The time it takes to get to min")]
        private float GaugeMinDuration = 1;

        public bool IsChanging = false;

        private bool EngineOn = false;

        private bool AlertOn = false;

        private int HoursRunning = 0;

        [Tooltip("Hours of the Panel")]
        public TextMeshProUGUI HoursField;

        public enum DamageScenario
        {
            None,
            Scenario1,
            Scenario2
        }

        public DamageScenario scenario = DamageScenario.None;

        /// <summary>Initialization of Gauge Controller</summary>
        public void Initialize(object input = null)
        {
            HoursRunning = Random.Range(1, 10000);
            HoursField.text = HoursRunning.ToString();
        }

        public void StartEngine()
        {
            if (IsChanging == true)
                return;

            IsChanging = true;

            GaugePanelLight coolantRef = null;
            GaugePanelLight fuelRef = null;

            //Toggle all lights on
            foreach (GaugePanelLight iterLight in listOfGaugeLights)
            {
                iterLight.CheckIfComplete();

                //Load some errors into system based on scenarios
                if (scenario == DamageScenario.Scenario1)
                {
                    if (iterLight.GaugeType == GaugePanelLight.TypeOfGaugeLight.EngineOilPressure ||
                       iterLight.GaugeType == GaugePanelLight.TypeOfGaugeLight.EngineCoolant ||
                       iterLight.GaugeType == GaugePanelLight.TypeOfGaugeLight.HydraulicOilTemp ||
                       iterLight.GaugeType == GaugePanelLight.TypeOfGaugeLight.FuelWaterSeparator)
                    {
                        iterLight.SetIndicatorStatus(true);
                    }
                }
                else if (scenario == DamageScenario.Scenario2)
                {
                    if (iterLight.GaugeType == GaugePanelLight.TypeOfGaugeLight.TransmissionFluidTemp ||
                       iterLight.GaugeType == GaugePanelLight.TypeOfGaugeLight.ChargingSystem ||
                       iterLight.GaugeType == GaugePanelLight.TypeOfGaugeLight.AirFilter ||
                       iterLight.GaugeType == GaugePanelLight.TypeOfGaugeLight.AutomaticEngineSpeedControl)
                    {
                        iterLight.SetIndicatorStatus(true);
                    }
                }

                //Sets the alert on if an error is in the system
                switch (iterLight.GaugeType)
                {
                    case GaugePanelLight.TypeOfGaugeLight.EngineCoolant:
                        {
                            coolantRef = iterLight;
                            if (!AlertOn)
                                if(iterLight.GetIndicatorStatus())
                                    AlertOn = true;
                        }
                        break;
                    case GaugePanelLight.TypeOfGaugeLight.EngineOilPressure:
                    case GaugePanelLight.TypeOfGaugeLight.HydraulicOilTemp:
                    case GaugePanelLight.TypeOfGaugeLight.TransmissionFluidTemp:
                    case GaugePanelLight.TypeOfGaugeLight.HydraulicBypass:
                    case GaugePanelLight.TypeOfGaugeLight.FuelWaterSeparator:
                    case GaugePanelLight.TypeOfGaugeLight.ChargingSystem:
                    case GaugePanelLight.TypeOfGaugeLight.AirFilter:
                        if (!AlertOn)
                            if (iterLight.GetIndicatorStatus())
                                AlertOn = true;
                        break;
                    case GaugePanelLight.TypeOfGaugeLight.LowFuel:
                        fuelRef = iterLight;
                        break;
                }

                //Start Light Animation
                StartCoroutine(StartUpLight(iterLight, false));
            }

            //Because the coolant object only has a min and max value
            CoolantObj = coolantRef.GetIndicatorStatus() ? 1.0f : 0.0f;

            //Because the fuel object only has a min and max value
            FuelObj = fuelRef.GetIndicatorStatus() ? 0.0f : 1.0f;

            //Because the backhoe doesn't move the tachometer doesn't move
            TachometerObj = 0.4f;

            //Gauge Startups
            StartCoroutine(StartUpGauge(CoolantGauge, CoolantGaugeMax, CoolantGaugeMin, CoolantObj));
            StartCoroutine(StartUpGauge(TachometerGauge, TachometerMax, TachometerMin, TachometerObj));
            StartCoroutine(StartUpGauge(FuelLevelGauge, FuelLevelMax, FuelLevelMin, FuelObj));

            StartCoroutine(OverallAnimationTimer(0));
            EngineOn = true;
        }

        public void StopEngine()
        {
            if (IsChanging == true)
                return;

            IsChanging = true;

            foreach (GaugePanelLight iterLight in listOfGaugeLights)
            {
                iterLight.SetLightStatus(false);
            }

            Transform tempCoolantPos = CoolantGauge;
            Transform tempFuelPos = FuelLevelGauge;
            Transform tempTachoPos = TachometerGauge;

            StartCoroutine(ShutDownGauge(CoolantGauge, CoolantGaugeMin, tempCoolantPos, 0.5f));
            StartCoroutine(ShutDownGauge(TachometerGauge, TachometerMin, tempFuelPos, 0.5f));
            StartCoroutine(ShutDownGauge(FuelLevelGauge, FuelLevelMin, tempTachoPos, 0.5f));
            StartCoroutine(OverallAnimationTimer(1));
            EngineOn = false;
        }

        private IEnumerator StartUpLight(GaugePanelLight light, bool AlwaysOn)
        {
            float startTimer = Time.time;
            float endTimer = startTimer + AnimationDuration;
            float firstAnimEndTime = startTimer + LightsOnDuration;

            while(Time.time <= endTimer)
            {
                if (Time.time <= firstAnimEndTime)
                {
                    //Turn on all lights
                    light.SetLightStatus(true);
                }
                else
                {
                    if (!AlwaysOn)
                    {
                        bool status = light.GetIndicatorStatus();

                        //Check for activity and set state respectively
                        light.SetLightStatus(status);

                        if(AlertOn && light.GaugeType == GaugePanelLight.TypeOfGaugeLight.ActionLight)
                        {
                            StartCoroutine(FlashActionLight(light, status));
                        }

                    }
                }
                yield return null;
            }
        }

        private IEnumerator FlashActionLight(GaugePanelLight light, bool status)
        {
            float oneSecond = Time.time + 1;
            bool lightStatus = status;
            while(EngineOn)
            {
                if (Time.time >= oneSecond)
                {
                    light.SetLightStatus(lightStatus = lightStatus ? false: true); 
                    oneSecond = Time.time + 1;
                }
                yield return null;
            }

            AlertOn = false;
        }    

        private IEnumerator StartUpGauge(Transform Gauge, Transform ToLocation, Transform FromLocation, float TargetValue)
        {
            float startTimer = Time.time;
            float endTimer = startTimer + AnimationDuration;
            float firstAnimEndTime = startTimer + GaugeMaxDuration;
            float secondAnimEndTime = firstAnimEndTime + GaugeMinDuration;
            float TimeRatio = 0;

            while (Time.time <= endTimer)
            {
                if(Time.time <= firstAnimEndTime)
                {
                    //Going to maximum
                    TimeRatio = (Time.time - startTimer) / GaugeMaxDuration;
                    Gauge.localRotation = Quaternion.Lerp(FromLocation.localRotation, ToLocation.localRotation, TimeRatio);
                }
                else if(Time.time <= secondAnimEndTime)
                {
                    //Going to minimum
                    TimeRatio = (Time.time - firstAnimEndTime) / GaugeMinDuration;
                    Gauge.localRotation = Quaternion.Lerp(ToLocation.localRotation, FromLocation.localRotation, TimeRatio);
                }
                else
                {
                    //Going to value
                    TimeRatio = (Time.time - secondAnimEndTime) / (AnimationDuration - GaugeMaxDuration - GaugeMinDuration);
                    if(TimeRatio <= TargetValue)
                        Gauge.localRotation = Quaternion.Lerp(FromLocation.localRotation, ToLocation.localRotation, TimeRatio);
                }

                yield return null;
            }
        }

        private IEnumerator ShutDownGauge(Transform Gauge, Transform ToLocation, Transform FromLocation, float Duration)
        {
            float startTimer = Time.time;
            float endTimer = startTimer + Duration;

            while (Time.time <= endTimer)
            {
                float TimeRatio = (Time.time - startTimer) / Duration;
                Gauge.localRotation = Quaternion.Lerp(FromLocation.localRotation, ToLocation.localRotation, TimeRatio);
                yield return null;
            }
        }

        private IEnumerator OverallAnimationTimer(float specialCase)
        {
            float startTimer = Time.time;
            float endTimer = startTimer + AnimationDuration;

            if (specialCase != 0)
            {
                endTimer = startTimer + specialCase;
            }

            while (Time.time <= endTimer)
            {
                yield return null;
            }

            IsChanging = false;
        }

        public GaugePanelLight GetPanelLight(GaugePanelLight.TypeOfGaugeLight TargetPanelLight)
        {
            foreach (GaugePanelLight iterLight in listOfGaugeLights)
            {
                if(iterLight.GaugeType == TargetPanelLight)
                {
                    return iterLight;
                }
            }
            return null;
        }

        public int GetHoursRunning()
        {
            return HoursRunning;
        }
    }
}
