using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RemoteEducation.Scenarios.Inspectable
{
    public class InspectableObject : MonoBehaviour
    {
        [Header("Interaction Highlight Colours")]
        [HideInInspector]
        public static readonly Color Unchecked_Highlight = new Color(0.8470588f, 0.8313726f, 0.5607843f);
        [HideInInspector]
        public static readonly Color Unchecked_Select = new Color(1f, 0.9803922f, 0f);
        [HideInInspector]
        public static readonly Color Good_Highlight = new Color(0.5803922f, 0.8470588f, 0.5607843f);
        [HideInInspector]
        public static readonly Color Good_Select = new Color(0.1294118f, 1f, 0f);
        [HideInInspector]
        public static readonly Color Bad_Highlight = new Color(0.8470588f, 0.5803922f, 0.5607843f);
        [HideInInspector]
        public static readonly Color Bad_Select = new Color(1f, 0f, 0f);


        /// <summary>Event invoked when an Inspectable Object is selected, in order to play sound effects.</summary>
        /// <remarks>This can be further abstracted through the SelectAction, UnselectAction of <see cref="InspectableElement"/></remarks>
        [Header("Events")]
        public UnityEvent SelectionSoundEvent;
    }
}