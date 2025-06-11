using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RemoteEducation.Interactions
{
    public class ScaleUpHighlight : IHighlight
    {
        public object arg { get; set; }

        private readonly Transform transform;
        private readonly Material material;

        private readonly Vector3 startScale;
        private readonly Vector3 highlightScale;

        private readonly Color startColor;
        private readonly Color highlightColor;

        public ScaleUpHighlight(GameObject objectToHighlight, float scaleFactor, float brightenFactor)
        {
            transform = objectToHighlight.transform;
            startScale = transform.localScale;
            highlightScale = startScale * scaleFactor;

            material = objectToHighlight.GetComponentInChildren<Renderer>().material;
            startColor = material.color;
            highlightColor = material.color + new Color(brightenFactor, brightenFactor, brightenFactor);
        }

        public ScaleUpHighlight(GameObject objectToHighlight, float scaleFactor, float brightenFactor, Color highlightColour) : this(objectToHighlight, scaleFactor, brightenFactor)
        {
            highlightColor = highlightColour + new Color(brightenFactor, brightenFactor, brightenFactor);
        }

        public void Highlight(bool enable)
        {
            if (enable)
            {
                transform.localScale = highlightScale;
                material.color = highlightColor;
            }
            else
            {
                transform.localScale = startScale;
                material.color = startColor;
            }
        }
    }
}
