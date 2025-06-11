/*
 * SUMMARY: This file contains the Coolant Fluid class which only checks
 *          when the fluid is selected
 */

using RemoteEducation.Interactions;
using RemoteEducation.Scenarios;
using UnityEngine;

namespace RemoteEducation.Modules.HeavyEquipment
{
    public sealed class CoolantFluid : Selectable, IInitializable, ITaskable
    {
        public enum FluidStates
        {
            FullClean = 0,
            FullDirty = 1,
            LowClean = 2,
            LowDirty = 3
        }

        [SerializeField, Tooltip("The GameObject for a full level of clean fluid")]
        private GameObject FullClean;
        [SerializeField, Tooltip("The GameObject for a full level of dirty fluid")]
        private GameObject FullDirty;
        [SerializeField, Tooltip("The GameObject for a low level of clean fluid")]
        private GameObject LowClean;
        [SerializeField, Tooltip("The GameObject for a low level of dirty fluid")]
        private GameObject LowDirty;

        private FluidStates currentFluidState;
        private bool FluidIsFull => currentFluidState == FluidStates.FullClean || currentFluidState == FluidStates.FullDirty;
        // Created for UT purpose
        public FluidStates CurrentFluidState => currentFluidState;
        public bool IsFluidFull => FluidIsFull;

        public TaskableObject Taskable { get; private set; }

        public void Initialize(object input = null)
        {
            OnSelect += FillFluid;

            Taskable = new TaskableObject(this);
        }

        /// <summary>
        /// Sets the fluid state based on if the fluid is clean and if it is full.
        /// </summary>
        /// <remarks>
        /// See the order of the <see cref="FluidStates"/> enum to understand the logic
        /// this method.
        /// </remarks>
        public void SetFluidState(bool isFull, bool isClean)
        {
            int index = 0;

            if (!isFull)
            {
                index += 2;
            }

            if (!isClean)
            {
                index += 1;
            }

            SetFluidState((FluidStates)index);
        }

        /// <summary>
        /// Sets the state of the fluid in the tank.
        /// This also updates that gameobject that is visible.
        /// </summary>
        public void SetFluidState(FluidStates state)
        {
            currentFluidState = state;

            GameObject[] fluidObjects = new GameObject[] { FullClean, FullDirty, LowClean, LowDirty };

            for (int i = 0; i < fluidObjects.Length; i++)
            {
                fluidObjects[i].SetActive((int)state == i);
            }
        }

        /// <summary>
        /// Fills the fluid in the tank while preserving if it is clean or dirty.
        /// </summary>
        public void FillFluid()
        {
            if (currentFluidState == FluidStates.LowDirty)
            {
                SetFluidState(FluidStates.FullDirty);
            }
            else if (currentFluidState == FluidStates.LowClean)
            {
                SetFluidState(FluidStates.FullClean);
            }

            Taskable.PokeTaskManager();
        }

        public object CheckTask(int checkType, object inputData)
        {
            switch (checkType)
            {
                case (int)TaskVertexManager.CheckTypes.Bool:
                    return FluidIsFull;

                default:
                    Debug.LogError("CoolantFluid was passed a check type it could not handle");
                    return null;
            }
        }
    }
}
