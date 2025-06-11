using RemoteEducation.Scenarios;
using RemoteEducation.Scenarios.Inspectable;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RemoteEducation.Modules.Inspection
{
    public class SpawningInspectableElement : InspectableElement
    {
        public enum SpawningModes
        {
            All,
            One,
            Random,
            TwentyFivePercent,
            FiftyPercent,
            SeventyFivePercent
        }

        [SerializeField]
        public GameObject GoodComponent;
        [SerializeField]
        private GameObject BadComponent;

        [SerializeField, Tooltip("How many elements to spawn in badly. If there is only one component, use \"All\"")]
        public SpawningModes BadSpawningMode;

        public override void Initialize(InspectableController parentController, InspectableObject parentObject, string checkListName, string fullName, int stateIdentifier)
        {
            base.Initialize(parentController, parentObject, checkListName, fullName, stateIdentifier);
            SpawnInComponents();
        }

        /* Method Header : SpawnInComponents
         * This method will spawn in a component at each of the element anchors.
         * If bad components are needed, then they will randomly selected. 
         */
        private void SpawnInComponents()
        {
            //get the location for all the anchors 
            List<ElementAnchor> elementAnchors = GetComponentsInChildren<ElementAnchor>().ToList();

            //if this element is good
            if (State == InspectableState.Good)
            {
                //spawn in all good components
                foreach (ElementAnchor elementAnchor in elementAnchors)
                {
                    Instantiate(GoodComponent, elementAnchor.transform);
                }
            }
            else
            {
                //if all components should be bad, do that without randomizing anything 
                if (BadSpawningMode == SpawningModes.All)
                {
                    foreach (ElementAnchor elementAnchor in elementAnchors)
                    {
                        Instantiate(BadComponent, elementAnchor.transform);
                    }
                }
                else
                {
                    //sort the lists by the anchors preferred state while shuffling within those states
                    if (elementAnchors.Find(x => x.PreferredState != ElementAnchor.PreferredStates.Indifferent) != null)
                    {
                        List<ElementAnchor> preferredBad = ShuffleList(elementAnchors.Where(x => x.PreferredState == ElementAnchor.PreferredStates.Bad).ToList());
                        List<ElementAnchor> preferredGood = ShuffleList(elementAnchors.Where(x => x.PreferredState == ElementAnchor.PreferredStates.Good).ToList());
                        List<ElementAnchor> indifferent = ShuffleList(elementAnchors.Where(x => x.PreferredState == ElementAnchor.PreferredStates.Indifferent).ToList());

                        elementAnchors.Clear();
                        elementAnchors.AddRange(preferredBad);
                        elementAnchors.AddRange(indifferent);
                        elementAnchors.AddRange(preferredGood);
                    }

                    //calculate how many components should be bad.
                    int badElementsToSpawn = 1;

                    switch (BadSpawningMode)
                    {
                        case SpawningModes.One:
                            badElementsToSpawn = 1;
                            break;

                        case SpawningModes.TwentyFivePercent:
                            badElementsToSpawn = (int)(elementAnchors.Count() * 0.25f);
                            break;

                        case SpawningModes.FiftyPercent:
                            badElementsToSpawn = (int)(elementAnchors.Count() * 0.50f);
                            break;

                        case SpawningModes.SeventyFivePercent:
                            badElementsToSpawn = (int)(elementAnchors.Count() * 0.75f);
                            break;

                        case SpawningModes.Random:
                            badElementsToSpawn = Random.Range(1, elementAnchors.Count() + 1); //Random.Range(int min, int max) -> max isn't inclusive
                            break;
                    }

                    //ensure that the math above was right
                    badElementsToSpawn = Mathf.Clamp(badElementsToSpawn, 1, elementAnchors.Count());

                    //spawn in a component at each location. 
                    for (int i = 0; i < elementAnchors.Count; i++)
                    {
                        //starting with the bad ones
                        if (i < badElementsToSpawn)
                        {
                            Instantiate(BadComponent, elementAnchors[i].transform);
                        }
                        else
                        {
                            Instantiate(GoodComponent, elementAnchors[i].transform);
                        }
                    }
                }
            }
        }

        /* Method Header : ShuffleList
         * This method will randomize the order of a list.
         */
        private List<ElementAnchor> ShuffleList(List<ElementAnchor> list)
        {
            List<ElementAnchor> outList = new List<ElementAnchor>();

            int length = list.Count();

            for (int i = 0; i < length; i++)
            {
                int index = Random.Range(0, list.Count);

                outList.Add(list[index]);
                list.RemoveAt(index);
            }

            return outList;
        }

        public override InspectableElement AddElementToGameObject(GameObject gameObject)
        {
            SpawningInspectableElement newElement = gameObject.AddComponent<SpawningInspectableElement>();

            newElement.GoodComponent = GoodComponent;
            newElement.BadComponent = BadComponent;
            newElement.BadSpawningMode = BadSpawningMode;
            newElement.StateMessage = StateMessage;
            newElement.State = State;
            newElement.StateIdentifier = StateIdentifier;

            return newElement;
        }
    }
}