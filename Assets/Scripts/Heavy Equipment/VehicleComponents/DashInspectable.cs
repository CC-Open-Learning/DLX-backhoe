/*
*  FILE          :	DashInspectable.cs
*  PROGRAMMER    :	Leon Vong
*  FIRST VERSION :	2021-19-08
*  DESCRIPTION   :  This file is used to inspect for number of hours running on machine.
*/
using RemoteEducation.Interactions;
using RemoteEducation.Scenarios.Inspectable;
using System.Collections;
using UnityEngine;

public class DashInspectable : MonoBehaviour, IInitializable, IInspectable<int>
{
    /// <summary> Variable for hours running </summary>
    [SerializeField]
    private int hoursRunning = 0;

    /// <summary> Initializing of Object </summary>
    public void Initialize(object input = null)
    {
        hoursRunning = Random.Range(1, 72);
    }

    /// <summary> Utilizing Generic Inspectable, function that is fired off by UserInputPanel</summary>
    public void CheckIfComplete(int userValueNumber)
    {
        if(userValueNumber == hoursRunning)
        {
            //Set completion status to element.
            gameObject.GetComponent<TextBasedElement>().CompletionStatus = true;
        }  
    }
}