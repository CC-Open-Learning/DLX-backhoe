using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RemoteEducation.Helpers.Unity
{
    /// <summary>
    /// This class is simply used for events which pass an int value upon invoke.
    /// </summary>
    [Serializable]
    public class IntEvent : UnityEvent<int>
    {

    }
}