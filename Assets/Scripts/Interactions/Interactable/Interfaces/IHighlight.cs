using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RemoteEducation.Interactions
{
    public interface IHighlight
    {
        object arg { get; set; }

        void Highlight(bool enable);
    }
}
