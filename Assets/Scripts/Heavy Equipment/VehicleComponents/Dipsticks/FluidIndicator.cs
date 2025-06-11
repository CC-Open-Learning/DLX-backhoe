/*
 * SUMMARY: Defines the fluid indicator.
 */
using RemoteEducation.Interactions;
using UnityEngine;

namespace RemoteEducation.Modules.HeavyEquipment
{
    public sealed class FluidIndicator : SemiSelectable, IInitializable
    {
        [Tooltip ("DripCheck instance to use for the current state of the fluid.")]
        [SerializeField] private DripCheck FluidContainer;

        /// <summary>This variable allows us to check things and do things on the backhoe. Used to check engine state.</summary>
        private BackhoeController backhoeController;

        public void Initialize(object input = null)
        {
            backhoeController = input as BackhoeController;

            UpdateIndicator();
        }

        /// <summary>Sets the fluid indicator needle's rotation based on the current state of the fluid.</summary>
        public void UpdateIndicator()
        {
            float yRot = 0f;

            switch (FluidContainer.GetFluidState())
            {
                case DripCheck.FluidStates.Empty:
                    yRot = -95f;
                    break;

                case DripCheck.FluidStates.Normal:
                    yRot = backhoeController.IsEngineOn ? -15f : -75f;
                    break;

                case DripCheck.FluidStates.Full:
                    yRot = backhoeController.IsEngineOn ? 45f : -45f;
                    break;

                case DripCheck.FluidStates.Overflow:
                    yRot = backhoeController.IsEngineOn ? 95f : -15f;
                    break;
            }

            transform.localRotation = Quaternion.Euler(0, yRot, 0);
        }
    }
}