/*
* SUMMARY: File contains all logic related to the fluid puddles that could appear under/around the vehicle
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RemoteEducation.Scenarios.Inspectable;
using System.Linq;
using RemoteEducation.Scenarios;

public class Puddles : MonoBehaviour, IBreakable, ITaskable
{
    [SerializeField, Tooltip("The puddles that can appear.")]
    private GameObject[] Puddle;

    /// <summary>Probability of a puddle appearing.</summary>
    private const float PROBABILITY_OF_PUDDLE_APPEARING = 0.3f;
    public DynamicInspectableElement InspectableElement { get; private set; }
    public TaskableObject Taskable { get; private set; }

    private bool isBroken = false;

    public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int _breakMode)
    {
        Taskable = new TaskableObject(this);
        SetPuddleState(broken);
        InspectableElement = inspectableElement;
        inspectableElement.OnInspect += () => { Taskable.PokeTaskManager(); };
    }

    public bool CheckIfBroken()
    {
        return isBroken;
    }

    /// <summary>Setter for the puddles</summary>
    /// <remarks>If broken is passed in we randomize how many puddles spawn</remarks>
    /// <param name="broken">Used to tell if puddles spawn or not.</param>
    private void SetPuddleState(bool broken)
    {
        isBroken = broken;

        foreach (GameObject p in Puddle)
        {
            p.SetActive(false);
        }

        if(!broken)
        {
            return;
        }

        int activePuddles = 0;

        for (int i = 0; i < Puddle.Length; i++)
        {
            if (Random.value < PROBABILITY_OF_PUDDLE_APPEARING)
            {
                Puddle[i].SetActive(true);
                activePuddles++;
            }
        }

        //randomize one puddle to turn on if none are turned on
        if (activePuddles == 0)
        {
            Puddle[Random.Range(0, Puddle.Length - 1)].SetActive(true);
        }
    }

    public object CheckTask(int checkType, object inputData)
    {
        switch (checkType)
        {
            case (int)TaskVertexManager.CheckTypes.ElementsInspected:
                if(InspectableElement.HasBeenInspected)
                {
                    return true;
                }
                else
                {
                    return !isBroken;
                }
            default:
                Debug.LogError("A checktype that is not defined was passed to the " + GetType().ToString());
                break;
        }

        return null;
    }
}
