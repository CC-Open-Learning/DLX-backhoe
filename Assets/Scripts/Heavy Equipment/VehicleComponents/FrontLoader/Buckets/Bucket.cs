/*
 * SUMMARY: The purpose of this file is to model the behaviour and properties of the front loader bucket on the backhoe.
 *          The bucket has 3 states: Good (All is okay - uses good mesh), bad (damaged bucket - uses bad mesh), and loose bolts
 *          (loose bolts on the bucket - uses good mesh). Additionally, this class tracks all bolts that are attached to the 
 *          bucket and randomizes which ones are loose/tight if the bucket is in the "Loose Bolts" breakMode.
 */



using UnityEngine;
using RemoteEducation.Scenarios.Inspectable;
using RemoteEducation.Interactions;
using System.Linq;
using System.Collections.Generic;
using RemoteEducation.Scenarios;



namespace RemoteEducation.Modules.HeavyEquipment
{
    /// <summary>Bucket class allows for determining if the bucket shown will be broken or not.</summary>
    public class Bucket : SemiSelectable, IBreakable, ITaskable
    {
        [SerializeField, Tooltip("The bad bucket decal object.")]
        private GameObject BadDecals;

        /// <summary>All bolts on the bucket</summary>
        [SerializeField, Tooltip("The nuts on the bucket.")]
        private List<Bolt> nutList;

        public bool Cleared = false;

        public TaskableObject Taskable { get; private set; }

        public DynamicInspectableElement InspectableElement { get; private set; }

        ///<summary>Minimum number of bolts that will be loose if the bucket is in the loose bad state</summary>
        private const int MINIMUM_LOOSE_BOLT_COUNT = 1;

        public enum BreakState
        {
            DamagedBucket = 0,
            LooseNuts = 1
        }

        ///<summary>Current number of loose bolts if in bad state. Always 1 - 6</summary>
        private int looseBoltCount;


        public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int _breakMode)
        {
            SetTaskable();
            nutList.ForEach(x => x.Setup(this));
            
            SetBucketState(broken, _breakMode);
            InspectableElement = inspectableElement;
        }

        public void SetTaskable()
        {
            Taskable = new TaskableObject(this);
        }


        /// <summary>Setter for the buckets state</summary>
        /// <remarks>If broken is passed in we show the broken bucket, else we show the ok bucket. If breakMode == 1 then the good 
        /// mesh will be used and the bolts ill be loose.</remarks>
        /// <param name="broken">Used to tell if the broken bucket mesh needs to be active or the ok bucket mesh.</param>
        /// <param name="breakMode">If 0 then bucket is broken. If 1 then bolts are loose</param>
        public void SetBucketState(bool broken, int breakMode)
        {
            if (!broken)
            {
                return;
            }

            switch ((BreakState)breakMode)
            {
                case BreakState.DamagedBucket:
                    BadDecals.SetActive(true);
                    break;

                case BreakState.LooseNuts:
                    RandomizeLooseBolts();
                    break;
            }
        }

        /// <summary>Randomizes which bolts on the bucket are loose. Always have at least 1 loose bolt on the bucket</summary>
        private void RandomizeLooseBolts()
        {
            //Randomize how many bolts will be loose
            looseBoltCount = Random.Range(MINIMUM_LOOSE_BOLT_COUNT, nutList.Count+1);
            
            List<int> looseIndexes = new List<int>();

            //Fill a list of ints with the indexes that will be loose
            while (looseIndexes.Count < looseBoltCount)
            {
                int randomIndex = Random.Range(0, nutList.Count);
                
                //If the random index isn't in there yet, then add it. Otherwise random again
                if (looseIndexes.Contains(randomIndex) == false)
                {
                    looseIndexes.Add(randomIndex);
                }
            }

            //Set the random bolts loose
            foreach (int index in looseIndexes)
            {
                nutList[index].SetBoltLoose();
            }
        }

        public void PokeBolts()
        {
            foreach (Bolt bolt in nutList)
            {
                if (!bolt.CheckTask())
                {
                    return;
                }
            }

            Cleared = true;
            Taskable.PokeTaskManager();
        }

        public object CheckTask(int checkType, object inputData)
        {
            switch (checkType)
            {
                case (int)TaskVertexManager.CheckTypes.Bool:
                    return Cleared;
                default:
                    Debug.LogError($"Invalid check type passed into {GetType().Name}");
                    break;
            }

            return null;
        }
    }
}