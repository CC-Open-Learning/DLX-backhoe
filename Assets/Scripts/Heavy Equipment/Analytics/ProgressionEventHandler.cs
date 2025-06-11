using RemoteEducation.Modules.HeavyEquipment;
using RemoteEducation.Scenarios.Inspectable;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
using VARLab.Analytics;

public class ProgressionEventHandler : MonoBehaviour
{

    //TODO: THIS FILE WILL NEED TO BE UPDATED TO USE THE NEW PACKAGE IN THE NEXT TICKET. 

    [SerializeField]
    private AnalyticsEvent _analyticsChannel;

    private void Start()
    {
        UserTaskVertex.OnInspectionProgress += ProgressionEvent;
    }

    /// <summary>
    /// This method takes in an UserTaskVertex which contains the data it needs to send a progression event.
    /// It checks a few things to make sure it is a valid progression event, then sends the event.
    /// </summary>
    private void ProgressionEvent(UserTaskVertex vertex)
    {
        if (vertex.Title == null) { return; } // If the vertex has no title, it is not a progression event.

        if (vertex.ComponentIdentifier != null) // If it has a component identifier, it usually is a dynamic task like removing debris, so there's no correct/incorrect.
        {
            //SendProgressionEvent(null, vertex.Title, vertex.Id);
        }
        else if (vertex.ComponentType == typeof(InspectableManager))
        {
            if (vertex.ConstData.GetType() == typeof(DynamicInspectableElement))
            {
                InspectableElement element = (InspectableElement)vertex.ConstData;
                //SendProgressionEvent(element.InspectionIsCorrect, element.FullName, vertex.Id);
            }
            else if (vertex.ConstData.GetType() == typeof(List<InspectableElement>))
            {
                List<InspectableElement> elements = (List<InspectableElement>)vertex.ConstData;
                foreach (InspectableElement element in elements)
                {
                    //SendProgressionEvent(element.InspectionIsCorrect, element.FullName, vertex.Id);
                }
            }
        }
    }

    /// <summary>
    /// Helper function to send a progression event.
    /// </summary>
    /// <param name="isCorrect">Is the inspection correct or incorrect? If not applicable, it should be null.</param>
    /// <param name="name">The event name.</param>
    /// <param name="index">The graph vertex index</param>
    //void SendProgressionEvent(bool? isCorrect, string name, int index)
    //{
        //TODO: Update this to use the new package methods. 
        //ProgressContainer data = new ProgressContainer(isCorrect, name, index);
        //_analyticsChannel.Raise(this, data);
    //}
}
