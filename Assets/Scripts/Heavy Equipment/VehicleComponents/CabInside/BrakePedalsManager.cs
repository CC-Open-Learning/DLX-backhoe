/*
* SUMMARY: Contains the BrakePedalsManager class, initializes the brakes and attaches the inspectable.
*/

using UnityEngine;
using RemoteEducation.Interactions;
using RemoteEducation.Scenarios.Inspectable;
using RemoteEducation.UI;
using RemoteEducation.Localization;
using System.Collections;

namespace RemoteEducation.Modules.HeavyEquipment
{
    public class BrakePedalsManager : SemiSelectable, IInitializable, IBreakable
    {
        [Tooltip("Left brake pedal.")]
        [SerializeField] private BackhoeBrakePedal LeftPedal;

        [Tooltip("Right brake pedal.")]
        [SerializeField] private BackhoeBrakePedal RightPedal;

        [Tooltip("Connecting rod game object.")]
        [SerializeField] private GameObject ConnectingRod;

        /// <summary>Pedal rotation time.</summary>
        [HideInInspector] public float rotationTime = 0.5f;

        /// <summary>Left pedal animation coroutine.</summary>
        private Coroutine leftPedalAnimation;

        /// <summary>Right pedal animation coroutine.</summary>
        private Coroutine rightPedalAnimation;

        /// <summary>String token for the stuck brake pedal.</summary>
        private const string pedalsStuckToken = "HeavyEquipment.BrakePedalsToastStuck";

        private bool isAnimating = false;

        /// <summary>Brake pedal broken states.</summary>
        public enum PedalsState
        {
            Good,
            NoConnectingRod,
            BrakesStuck
        }

        /// <summary>Brake pedal break mode state.</summary>
        public PedalsState pedalsState;

        /// <summary>Sets up input for the brake pedals</summary>
        public void Initialize(object input = null)
        {
            LeftPedal.SetupToolTips();
            RightPedal.SetupToolTips();
        }

        /// <summary>Attach the inspectable.</summary>
        public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int breakMode)
        {
            SetState(broken, breakMode);

            OnSelect += DepressPedals;

            inspectableElement.OnCurrentlyInspectableStateChanged += (active) =>
            {
                ToggleFlags(!active, Flags.Highlightable | Flags.ExclusiveInteraction);
            };
        }

        /// <summary>Sets the broken state of the brake pedals.</summary>
        /// <param name="broken">true if the pedals are broken, false if good.</param>
        /// <param name="breakMode">break mode value: 0 is missing connecting rod, 1 is pedals stuck.</param>
        private void SetState(bool broken, int breakMode)
        {
            pedalsState = (PedalsState) breakMode;
            if(broken && pedalsState == PedalsState.NoConnectingRod)
            {
                ConnectingRod.SetActive(false);
            }
        }

        public void DepressPedals()
        {
            if (isAnimating) 
                return;

            isAnimating = true;
            // if the rod is intact, let the manager handle the pedals
            if (pedalsState == BrakePedalsManager.PedalsState.Good)
            {
                SetPedals(true);
                StartCoroutine(LeftPedal.AutomatedReverse());
                StartCoroutine(RightPedal.AutomatedReverse());
                StartCoroutine(StopAnimation());
            }
            else if (pedalsState == BrakePedalsManager.PedalsState.NoConnectingRod)
            {
                SetPedalsTesting();
                StartCoroutine(LeftPedal.AutomatedReverse());
                StartCoroutine(RightPedal.AutomatedReverse());
                StartCoroutine(StopAnimation());
            }
            else {
                SetPedals(false);
                isAnimating = false;
            }
        }

        private IEnumerator StopAnimation()
        {
            yield return new WaitForSeconds(2f);
            isAnimating = false;
        }
        
        public void SetPedalsTesting()
        {
            if (leftPedalAnimation != null)
                StopCoroutine(leftPedalAnimation);
            if (rightPedalAnimation != null)
                StopCoroutine(rightPedalAnimation);

            //Animates one pedal and then the other
            leftPedalAnimation = StartCoroutine(LeftPedal.RotatePedals(true, rotationTime / 2, rotationTime / 2));
            rightPedalAnimation = StartCoroutine(RightPedal.RotatePedals(true, rotationTime / 2, 0));
        }

        /// <summary>Sets both of the backhoe brake pedals to depressed, released, or if stuck: a toast message.</summary>
        /// <param name="depress">true if the pedals are being depressed, false if released.</param>
        public void SetPedals(bool depress)
        {
            switch((PedalsState) pedalsState)
            {
                case PedalsState.BrakesStuck:
                    DisplayToast(pedalsStuckToken);
                    break;
                default:
                    if(leftPedalAnimation != null)
                        StopCoroutine(leftPedalAnimation);
                    if(rightPedalAnimation != null)
                        StopCoroutine(rightPedalAnimation);
                    
                    leftPedalAnimation = StartCoroutine(LeftPedal.RotatePedals(depress, rotationTime));
                    rightPedalAnimation = StartCoroutine(RightPedal.RotatePedals(depress, rotationTime));
                    break;
            }
        }

        /// <summary>Displays toast message for the brake pedals.</summary>
        /// <param name="messageToken">String token for the message to be displayed</param>
        private void DisplayToast(string messageToken)
        {
            HEPrompts.CreateToast(Localizer.Localize(messageToken), HEPrompts.ShortToastDuration);
        }
    }
}