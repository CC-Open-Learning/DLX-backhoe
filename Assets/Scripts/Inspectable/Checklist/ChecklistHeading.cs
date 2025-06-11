/*
 *  FILE          :	ChecklistHeading.cs
 *  PROJECT       :	CORE Engine (Inspection Module)
 *  PROGRAMMER    :	David Inglis
 *  FIRST VERSION :	2020-11-23
 *  DESCRIPTION   :
 *      The ChecklistHeading component and associated prefab are used to represent
 *      a group of InspectableElement objects in the InspectionChecklist UI. 
 *      
 *      The ChecklistHeading corresponds directly to the InspectableController component
 *      of the Inspectable hierarchy
 */

using RemoteEducation.Scenarios.Inspectable;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RemoteEducation.Modules.Inspection.Scenarios
{
    public class ChecklistHeading : MonoBehaviour
    {
        // The path to the "ChecklistHeading" prefab resource
        public static readonly string ResourcePath = "UI/Checklist/Checklist Heading";

        [NonSerialized] public InspectableController InspectableController;

        [SerializeField] private TextMeshProUGUI headingField;

        public List<ChecklistItem> Items;
        public List<ReadOnlyChecklistItem> ReadOnlyItems;

        public string Text
        {
            get
            {
                return (headingField != null) ? headingField.text : string.Empty;
            }

            set
            {
                if (headingField != null) { headingField.text = value; }
            }
        }

        protected void Awake()
        {
            if (!headingField)
            {
                // If the 'headingField' has not been specified through serialization,
                // find the first available TextMeshProUGUI attached to the GameObject
                headingField = GetComponentInChildren<TextMeshProUGUI>();
            }

            Items = new List<ChecklistItem>();
            ReadOnlyItems = new List<ReadOnlyChecklistItem>();
        }

        /// <summary>If none of the items for this heading are visible, hide it.</summary>
        public void UpdateVisability()
        {
            bool visible = false;

            foreach(ChecklistItem item in Items)
            {
                if(item.gameObject.activeSelf)
                {
                    visible = true;
                    break;
                }
            }

            gameObject.SetActive(visible);
        }

        public void UpdateReadOnlyVisibility()
        {
            bool visible = false;

            foreach(ReadOnlyChecklistItem item in ReadOnlyItems)
            {
                if (item.gameObject.activeSelf)
                {
                    visible = true;
                    break;
                }
            }
            gameObject.SetActive(visible);
        }
    }
}