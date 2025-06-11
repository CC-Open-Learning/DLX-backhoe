/*
* SUMMARY: File contains all logic related to the Boom Bucket component.
* Contains methods for toggling the broken/ok state.
*/
using UnityEngine;
using System.Collections;
using RemoteEducation.Scenarios.Inspectable;
using RemoteEducation.Interactions;
using System.Linq;
using System.Collections.Generic;
using RemoteEducation.Scenarios;

namespace RemoteEducation.Modules.HeavyEquipment
{

    /// <summary>BloomBucket class allows for determining if the bucket shown will have worn teeth or not.</summary>
    public class BoomBucket : SemiSelectable, IBreakable, ITaskable
    {
        [SerializeField, Tooltip("The damaged decals for the main bucket part.")]
        private GameObject DamagedDecals;

        [SerializeField, Tooltip("The teeth on the bucket.")]
        private GameObject[] teeth;

        [SerializeField, Tooltip("The worn teeth on the bucket.")]
        private GameObject[] wornTeeth;

        private const float PROBABILITY_OF_MISSING_TOOTH = 0.3f;

        private const int MIN_LOOSE_TEETH = 1;

        private List<FallingTooth> toothList;

        private bool TeethInspected = false;

        public DynamicInspectableElement InspectableElement { get; private set; }
        public TaskableObject Taskable { get; private set; }

        public enum BreakState
        {
            WornTeeth = 0,
            LooseTeeth = 1,
            DamagedBucket = 2
        }

        [SerializeField, Tooltip("The worn teeth object.")]
        private GameObject WornTeeth;

        public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int _breakMode)
        {
            Taskable = new TaskableObject(this);
            InspectableElement = inspectableElement;

            GetTeeth();

            toothList.ForEach(x => x.Setup(this));
            SetBucketState(broken, _breakMode);
        }

        /// <summary>function to get a list of the teeth with the FallingTooth script</summary>
        private void GetTeeth()
        {
            toothList = GetComponentsInChildren<FallingTooth>(true).ToList();
        }

        /// <summary>Setter for the boom buckets teeth</summary>
        /// <remarks>If broken is passed in we randomize how many teeth are worn</remarks>
        /// <param name="broken">Used to tell if it's broken.</param>
        /// <param name="_breakMode">Used to tell what broken state it'll be in.</param>
        private void SetBucketState(bool broken, int _breakMode)
        {
            if (!broken)
            {
                return;
            }

            switch ((BreakState)_breakMode)
            {
                case BreakState.LooseTeeth:
                    RandomizeLooseTeeth();
                    break;

                case BreakState.DamagedBucket:
                    DamagedDecals.SetActive(true);
                    break;

                case BreakState.WornTeeth:
                    WornTeeth.SetActive(true);

                    List<WornTooth> wornList;
                    wornList = WornTeeth.GetComponentsInChildren<WornTooth>(true).ToList();
                    wornList.ForEach(x => x.Setup(this));

                    ToggleWornTeethInteraction(false);

                    int activeTeeth = 0; //counter to see how many teeth are disabled

                    for (int i = 0; i < teeth.Length; i++)
                    {
                        if (Random.value < PROBABILITY_OF_MISSING_TOOTH)
                        {
                            teeth[i].SetActive(false);
                            wornTeeth[i].SetActive(true);
                            activeTeeth++;
                        }
                        else
                        {
                            teeth[i].SetActive(true);
                            wornTeeth[i].SetActive(false);
                        }
                    }

                    if (activeTeeth == 0)
                    {
                        SetRandomToothActive(true);
                    }
                    else if (activeTeeth == teeth.Length)
                    {
                        SetRandomToothActive(false);
                    }

                    break;
            }
        }

        public void ToggleWornTeethInteraction(bool state)
        {
            List<WornTooth> wornList;
            wornList = WornTeeth.GetComponentsInChildren<WornTooth>(true).ToList();
            wornList.ForEach(x => x.ToggleFlags(!state, Flags.InteractionsDisabled));
        }

        /// <summary>function to randomize which teeth will be loose for that bad state</summary>
        private void RandomizeLooseTeeth()
        {
            int numberOfLooseTeeth = Random.Range(MIN_LOOSE_TEETH, teeth.Length + 1);
            List<bool> isLoose = new List<bool>();
            for (int i = 0; i < 5; i++)
            {
                isLoose.Add(false);
            }

            while (numberOfLooseTeeth != 0)
            {
                int looseToothIndex = Random.Range(0, teeth.Length);
                if (!isLoose[looseToothIndex])
                {
                    toothList[looseToothIndex].ToggleLoose();
                    isLoose[looseToothIndex] = true;
                    numberOfLooseTeeth--;
                }
            }
        }

        /// <summary>function to enable/disable teeth randomly if they either all enabled or disabled during a broken state</summary>
        /// <remarks>If active is passed as true, enable teeth, if false, disable them</remarks>
        /// <param name="active">Used to tell need to be activated or deactivated.</param>
        private void SetRandomToothActive(bool active)
        {
            int temp = Random.Range(0, teeth.Length);
            teeth[temp].SetActive(active);
            wornTeeth[temp].SetActive(!active);
        }

        public void PokeTeeth()
        {
            int numberOfTeethInspected = 0;
            foreach (GameObject tempTeeth in teeth)
            {
                if (tempTeeth.GetComponent<FallingTooth>().isInspected)
                    numberOfTeethInspected++;
            }

            foreach (GameObject tempTeeth in wornTeeth)
            {
                if (tempTeeth.GetComponent<WornTooth>().isInspected)
                    numberOfTeethInspected++;
            }

            if(numberOfTeethInspected == 5)
            {
                TeethInspected = true;
                Taskable.PokeTaskManager();
            }
        }

        public object CheckTask(int checkType, object inputData)
        {
            switch (checkType)
            {
                case (int)TaskVertexManager.CheckTypes.Bool:
                    return TeethInspected;
                default:
                    Debug.LogError("A checktype that is not defined was passed to the " + GetType().ToString());
                    break;
            }

            return null;
        }
    }
}