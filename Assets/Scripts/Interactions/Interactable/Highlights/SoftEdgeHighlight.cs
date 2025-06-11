using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RemoteEducation.Interactions
{
    public class SoftEdgeHighlight : IHighlight
    {
        protected HighlightObject highlight;
        public object arg { get; set; }

        public SoftEdgeHighlight(GameObject objectToHighlight, float outlineWidth, HighlightObject.Mode highlightMode = HighlightObject.Mode.OutlineVisible)
        {
            highlight = objectToHighlight.GetComponent<HighlightObject>();

            if (highlight == null)
            {
                highlight = objectToHighlight.AddComponent<HighlightObject>();
            }

            highlight.OutlineMode = highlightMode;
            highlight.OutlineWidth = outlineWidth;
        }

        public virtual void Highlight(bool enable)
        {
            if (arg == null)
                arg = Color.black;

            highlight.Glow(enable, (Color)arg );
        }
    }
}
