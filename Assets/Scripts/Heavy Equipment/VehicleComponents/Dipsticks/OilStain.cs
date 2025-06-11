/*
 * SUMMARY: Class defines details of what is on the dipstick oilwise
 */
using RemoteEducation.Interactions;
using RemoteEducation.Localization;
using RemoteEducation.UI.Tooltip;
using UnityEngine;

namespace RemoteEducation.Modules.HeavyEquipment
{
    public sealed class OilStain : SemiSelectable, IInitializable
    {
        /// <summary>This variable allows us to check things and do things on the backhoe. Used to check engine state.</summary>
        private BackhoeController backhoeController;

        [Tooltip ("Transform for controlling the oil level.")]
        [SerializeField] private Transform OilLevel;

        [Tooltip("Mesh representing the oil.")]
        [SerializeField] private SkinnedMeshRenderer OilMesh;

        [SerializeField] private bool IsTransmission = false;

        private bool tooltipOn = true;
        private SimpleTooltip tooltip;

        // private float yAdder = 0;

        public void Initialize(object input = null)
        {
            backhoeController = input as BackhoeController;

            OilLevel.transform.localPosition = new Vector3(0, UnityEngine.Random.Range(0.1f, 0.8f), 0); // Initial oil mesh is random and not representative of oil level
            tooltip = gameObject.GetComponent<SimpleTooltip>();
            ToggleTooltip();
        }

        public void ToggleTooltip()
        {
            if (tooltip == null)
                return;

            if(tooltipOn)
            {
                tooltip.ToolTipActive = false;
                tooltipOn = false;
            }
            else
            {
                tooltip.ToolTipActive = true;
                tooltipOn = true;
            }
        }

        /// <summary>Displays a toast of the status of the fluid/oil level.</summary>
        private void OilLevelToast()
        {
            string oilLevel = "";
            DripCheck.FluidStates fluidState = GetComponentInParent<DripCheck>().GetFluidState();
            switch (fluidState)
            {
                case DripCheck.FluidStates.Empty:
                    oilLevel = Localizer.Localize("HeavyEquipment.OilLevelToastLow");
                    break;
                case DripCheck.FluidStates.Overflow:
                    oilLevel = Localizer.Localize("HeavyEquipment.OilLevelToastOverfilled");
                    break;
                case DripCheck.FluidStates.Normal:
                    oilLevel = Localizer.Localize("HeavyEquipment.OilLevelToastNormal");
                    break;
                case DripCheck.FluidStates.Full:
                    oilLevel = Localizer.Localize("HeavyEquipment.OilLevelToastFull");
                    break;
                default:
                    return;
            }

            HEPrompts.CreateToast(oilLevel, HEPrompts.ShortToastDuration);
        }

        /// <summary>Sets oil level based on the given <paramref name="dipState"/>.</summary>
        public void SetDipstickProperties(DripCheck.FluidStates dipState)
        {
            OilMesh.enabled = true;

            float yPos = 0f;

            //Sets the oil size which is dependant if the engine is on or off
            switch (dipState)
            {
                case DripCheck.FluidStates.Empty:
                    //Empty Oil level
                    yPos = !IsTransmission ? 0.1f : 0.1f;
                    break;

                case DripCheck.FluidStates.Normal:
                    //Normal Oil level
                    yPos = !IsTransmission ? 0.28f : 0.45f;
                    break;

                case DripCheck.FluidStates.Full:
                    //Full oil level
                    yPos = !IsTransmission ? 0.35f : 0.65f;
                    break;

                case DripCheck.FluidStates.Overflow:
                    //Overflow oil level
                    yPos = !IsTransmission ? 0.55f : 1.05f;
                    break;
            }

            OilLevel.transform.localPosition = new Vector3(0, yPos, 0);
        }

        /// <summary>Hides the oil mesh when the oil has been wiped clean.</summary>
        internal void Wipe()
        {
            OilMesh.enabled = false;
        }
    }
}