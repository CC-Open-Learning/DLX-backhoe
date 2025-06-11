/*
 *  FILE          :	ElementSpawningData.cs
 *  PROJECT       :	Inspection Module
 *  PROGRAMMER    :	Duane Cressman
 *  FIRST VERSION :	2020-12-01
 *  RELATED FILES : InspectableController.cs, InspectableElement.cs, ElementAnchor.cs,
 *  DESCRIPTION   : This file contains the ElementSpawningData class. 
 *                  This class will be used to by the InspectableController to know where to add the
 *                  InspectableElement class. The InspectableElement script will be added to the GameObject
 *                  that this script is on.
 *                  The information entered into this class from the Unity Inspector will be used to name the 
 *                  InspectableElement.               
 *                  
 *                  The Gameobject structure should go:
 *                  
 *                  >InspectableController
 *                      >ElementSpawnningData (the InspectableElement will be added on this gameobject by the InspectableController
 *                          >ElementAnchor
 *                              >(the component will be added here)
 */

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RemoteEducation.Scenarios.Inspectable
{
    public class ElementSpawningData : MonoBehaviour
    {
        [Tooltip("Shall be set in the Unity Inspector. This name will be shown on the UI")]
        public string ElementName;

        [Tooltip("Indicate whether a tooltip should be added for the element.")]
        public bool AddTooltip = true;

        [Tooltip("Shall be set in the Unity Inspector. This name will displayed in the on-hover tooltip. If left blank the 'Element Name' will be used")]
        public string ElementTooltip;

        [Tooltip("(Optional) Information for any sub Gameobjects that get loaded below this object")]
        public string SubGameobjectContext;

        [Tooltip("(Optional) Indicates what position/step this inspectable is in for the ResultsPanel/ProgressIndicator for Guided Mode if the inspection must go in a certain order")]
        public int position = -1;

        [Tooltip("(Optional) Indicates what position/step this inspectable is in for the ResultsPanel/Progress Indicator if the inspection must go in a certain order")]
        public int AdvancedModePosition = -1;
    }
}