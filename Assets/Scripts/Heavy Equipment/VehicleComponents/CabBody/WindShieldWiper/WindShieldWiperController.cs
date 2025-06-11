/*
* SUMMARY: File contains all logic related to the WindShieldWiper class.
* Contains methods for toggling the on/off state, animation for the blades, etc.
*/
using RemoteEducation.Scenarios.Inspectable;
using RemoteEducation.Interactions;
using System.Collections;
using UnityEngine;
using RemoteEducation.Scenarios;
using CheckTypes = TaskVertexManager.CheckTypes;

namespace RemoteEducation.Modules.HeavyEquipment
{
    /// <summary>WindShieldWiper class allows for wiping motion of wipers.</summary>
    public class WindShieldWiperController : SemiSelectable, IBreakable, IInitializable, ITaskable
    {
        [Tooltip("Windshield wipers for this vehicle.")]
        [SerializeField] private WiperRotations wiper;

        [SerializeField, Tooltip("Duration of the wiper animation")]
        private float wipeDuration = 2f;

        [SerializeField, Tooltip("Curve representing a properly functioning windshield wiper.")]
        private AnimationCurve GoodAnimationCurve;

        [SerializeField, Tooltip("Curve representing a broken windshield wiper.")]
        private AnimationCurve BadAnimationCurve;

        [SerializeField, Tooltip("Whether the wiper is on or off.")] 
        private bool isOn = false;

        private Coroutine activeAnimation;
        private bool isBroken = false;

        public TaskableObject Taskable { get; private set; }

        public DynamicInspectableElement InspectableElement { get; private set; }

        public void Initialize(object input = null)
        {
            wiper.Init();

            //make it so the object highlights even when covered by other renderers
            GetComponent<HighlightObject>().OutlineMode = HighlightObject.Mode.OutlineAll;

            Taskable = new TaskableObject(this);
        }

        /// <summary>Toggles the wiper blades on and off.</summary>
        /// <remarks>Will either extend blades or retract based on the current state.</remarks>
        public void ToggleWiper()
        {
            isOn = !isOn;
            if (isOn && activeAnimation == null)
            {
                activeAnimation = StartCoroutine(AnimateWipers());
            }

            Taskable.PokeTaskManager();
        }

        public bool GetState()
        {
            return isOn;
        }

        public void AttachInspectable(DynamicInspectableElement _inspectableElement, bool broken, int _breakMode)
        {
            InspectableElement = _inspectableElement;
            isBroken = broken;
        }

        /// <summary>This method handles the animation of the wiper blades</summary>
        /// <remarks>Uses the speed via <see cref="wipeDuration"/> and calculates the remaining difference between
        /// the wiper's current pos. and the end. It will also show 2 different animations depending on if <see cref="isBroken"/> is set.</remarks>
        /// <param name="extend">Used to tell if the blade is extending out or not.</param>
        private IEnumerator AnimateWipers()
        {
            while (isOn)
            {
                float initTime = Time.time;
                float finishTime = initTime + wipeDuration;
                float diff = finishTime - initTime;

                while (Time.time <= finishTime)
                {
                    if (diff == 0)
                    {
                        Debug.LogError("Wiper duration can not be 0");
                        yield break;
                    }
                    float ratio = (Time.time - initTime) / diff;

                    if (isBroken)
                    {
                        wiper.transform.localRotation = Quaternion.Lerp(wiper.RetractedRotation, wiper.ExtendedRotation, BadAnimationCurve.Evaluate(ratio));
                    }
                    else
                    {
                        wiper.transform.localRotation = Quaternion.Lerp(wiper.RetractedRotation, wiper.ExtendedRotation, GoodAnimationCurve.Evaluate(ratio));
                    }

                    yield return null;
                }
            }

            activeAnimation = null;
        }

        public object CheckTask(int checkType, object inputData)
        {
            switch (checkType)
            {
                case (int)CheckTypes.Bool: //if the windshield wiper is on.

                    return isOn;

                default:

                    Debug.LogError("A checktype that is not defined was passed to the " + GetType().ToString());
                    break;
            }

            return null;
        }
    }
}