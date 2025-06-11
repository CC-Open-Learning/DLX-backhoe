/*
 *  FILE          :	ElementAnchor.cs
 *  PROJECT       :	Inspection Module
 *  PROGRAMMER    :	Duane Cressman
 *  FIRST VERSION :	2020-11-24
 *  RELATED FILES : InspectableController.cs, InspectableElement.cs, ElementSpawnningData.cs,
 *  DESCRIPTION   : This file contains the ElementAnchor class. 
 *                  The class is used to show InspectableElement objects where to spawn their
 *                  components. The component GameObject will be a child of this GameObject.
 *                  The PreferredState enum is used to tell the Inspectable component which objects should
 *                  be good or bad. This is useful for if an InspectableElement needs to spawn in multiple 
 *                  component. If some are less visible than others, the more visible ones should be bad.
 *                  
 *                  The Gameobject structure should go:
 *                  
 *                  >InspectableController
 *                      >ElementSpawnningData (the InspectableElement will be added on this gameobject by the InspectableController
 *                          >ElementAnchor
 *                              >(the component will be added here)
 */

using UnityEngine;

namespace RemoteEducation.Scenarios.Inspectable
{
    public class ElementAnchor : MonoBehaviour
    {
        public enum PreferredStates
        {
            Indifferent,
            Good,
            Bad
        }

        [SerializeField, Tooltip("If possible, what state would this anchor like to be in.")]
        public PreferredStates PreferredState;
    }
}