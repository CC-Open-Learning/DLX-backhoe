/*
* SUMMARY: Contains the WheelNut class, used to rotate the wheel nuts of the backhoe in alternating
*          directions indicating they are loose.
*/

using RemoteEducation.Interactions;
using System.Collections;
using UnityEngine;
using RemoteEducation.UI;
using RemoteEducation.Localization;
using RemoteEducation.Audio;

namespace RemoteEducation.Modules.HeavyEquipment
{
    /// <summary>This class allows the wheel nuts of the Heavy Equipment Backhoe to rotate</summary>
    /// <remarks>This rotation animation is used to indicate they are loose.</remarks>
    public sealed class WheelNut : SemiSelectable, IInitializable
    {
        /// <summary>The speed of the overall loosen animation in seconds.</summary>
        private const float LOOSEN_TIME = 1.2f;

        /// <summary>Angle in degrees for wheel nut to rotate in a single direction.</summary>
        private const float LOOSEN_ANGLE = 120f;
        
        /// <summary>Flag to prevent the rotation animation being called while it is already running.</summary>
        private bool rotating = false;

        public bool isInspected = false;

        /// <summary>Flag to determine if the wheel nut is tight or loose. Access needed from WheelNutsclass but not in Unity inspector.</summary>
        [HideInInspector] public bool looseNut;

        [Tooltip("Used to animated the wheel nut rotating away then back.")]
        [SerializeField] private AnimationCurve wheelNutCurve;

        /// <summary>Interactable component of the wheel that uses these nuts.</summary>
        [HideInInspector] public Interactable WheelInteractable;

        /// <summary>String token for the tight wheel nut.</summary>
        private const string tightToastToken = "HeavyEquipment.WheelNutsToastTight";

        /// <summary>String token for the loose wheel nut.</summary>
        private const string looseToastToken = "HeavyEquipment.WheelNutToastLoose";

        [Tooltip("Squeak sound plays when the wheel nut is loose.")]
        [SerializeField] private SoundEffect squeakSoundEffect;

        private WheelNuts groupManager;
        
        /// <summary>Sets up input for the wheel nut</summary>
        /// <remarks>The animation can only be called if the wheel nut has been set to be loose.</remarks>
        public void Initialize(object input = null)
        {
            OnSelect += WheelNutClicked;
        }

        public void SetupGroup(WheelNuts myManager)
        {
            groupManager = myManager;
        }
        
        /// <summary>Handles clicking of the wheel nut.</summary>
        /// <remarks>Displays toast for either state. If loose, wheel nut plays rotation animation and squeak sound effect.</remarks>
        private void WheelNutClicked()
        {
            isInspected = true;

            if (!rotating)
            {
                if(looseNut)
                {
                    DisplayToast(looseToastToken);
                    StartCoroutine(RotateWheelNutCurve(LOOSEN_ANGLE, LOOSEN_TIME));
                    squeakSoundEffect.Play();
                }
                else
                {
                    DisplayToast(tightToastToken);
                }
            }

            groupManager.Poke();
        }

        /// <summary>Displays toast message for the wheel nut.</summary>
        /// <param name="messageToken">String token for the message to be displayed</param>
        private void DisplayToast(string messageToken)
        {
            HEPrompts.CreateToast(Localizer.Localize(messageToken), LOOSEN_TIME);
        }

        /// <summary>Rotates the wheel nut in alternating directions, used to indicate a loose wheel nut.</summary>
        /// <param name="angle">The angle in degrees of a single rotation</param>
        /// <param name="playTime">Total time in seconds for the loosen animation to play</param>
        private IEnumerator RotateWheelNutCurve(float angle, float playTime)
        {
            rotating = true;

            var startRotation = transform.localRotation;
            var endRotation = Quaternion.Euler(transform.localEulerAngles + Vector3.left * angle);

            for(var t = 0f; t < 1; t += Time.deltaTime/playTime)
            {
                transform.localRotation = Quaternion.Lerp(startRotation, endRotation, wheelNutCurve.Evaluate(t));
                yield return null;
            }

            rotating = false;
        }

        /// <summary>Checks if the wheel is selected before it will try to highlight</summary>
        protected override void MouseEnter()
        {
            if(WheelInteractable != null)
            {
                ToggleFlags(!WheelInteractable.IsSelected, Flags.Highlightable);
            }

            base.MouseEnter();
        }
    }
}