/*
 * SUMMARY: The purpose of this class is to model the behaviour and properties of groups of pins.
 *          This includes inspecting pins as groups in the checklist, and randomizing which pins 
 *          are bad if the group is in the bad state.
 */



using RemoteEducation.Scenarios.Inspectable;
using System.Collections.Generic;
using UnityEngine;



namespace RemoteEducation.Modules.HeavyEquipment
{
    public class PinGroup : MonoBehaviour, IBreakable
    {
        [SerializeField, Tooltip("List of individual pins in this group.")]
        private List<Pin> pinList;

        /// <summary>Minimum number of pins that will be bad in a group if the group is in the bad state</summary>
        private const int MINIMUM_BAD_COUNT = 1;

        /// <summary>Broken states for pins</summary>
        private enum BreakMode
        {
            NotAcceptingGrease = 0,
            NotSecure = 1
        }

        public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int breakMode)
        {
            RandomizePins(false, breakMode);
            pinList.ForEach(x => x.Setup());
        }

        /// <summary>This method randomizes which pins are still good in the group so it is different in every play through</summary>
        /// <param name="breakMode">The bad state it is currently in, 0 for not accepting grease, and 1 for not secure</param>
        /// /// <param name="broken">Whether the pin group is in a bad state or not</param>
        private void RandomizePins(bool broken, int breakMode)
        {
            //Set all pins good by default
            pinList.ForEach(x => x.SetPinOkay());

            if (broken)
            {
                /// Get a bad value between <paramref name="MINIMUM_BAD_COUNT"> and 1 less than the number of list elements, so not all pins are bad
                int badCount = Random.Range(MINIMUM_BAD_COUNT, pinList.Count);

                List<int> badIndexes = new List<int>();

                //Fill a list of ints with the indexes that will be bad
                while (badIndexes.Count < badCount)
                {
                    int randomIndex = Random.Range(0, pinList.Count);

                    //If the random index isn't in there yet, then add it. Otherwise random again
                    if (badIndexes.Contains(randomIndex) == false)
                    {
                        badIndexes.Add(randomIndex);
                    }
                }

                //Set the random pins bad
                foreach (int index in badIndexes)
                {
                    switch ((BreakMode)breakMode)
                    {
                        //Not Accepting Grease
                        case BreakMode.NotAcceptingGrease:
                            pinList[index].SetPinBlocked();
                            break;
                        //Not Secure
                        case BreakMode.NotSecure:
                            pinList[index].SetPinNotSecure();
                            break;
                    }
                }
            }
        }
    }
}