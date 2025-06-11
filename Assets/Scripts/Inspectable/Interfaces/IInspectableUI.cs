using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RemoteEducation.Scenarios.Inspectable
{
    /// <summary>
    /// Any class that is a UI for the Inspectable system must implement this interface.
    /// This interface allows for the <see cref="InspectableManager"/> to not have anything 
    /// to do with the UI, other than the initialization. 
    /// </summary>
    public interface IInspectableUI
    {
        /// <summary>
        /// This method will be called by the <see cref="InspectableManager"/> in the scene.
        /// When this method is called, all the <see cref="InspectableController"/>s and <see cref="InspectableElement"/>s 
        /// will be set up. Therefore all the initial UI setup can be done in this method if the UI needs it. 
        /// All of the events on the <see cref="InspectableManager"/> should be subscribed to in this method.
        /// </summary>
        /// <param name="inspectableManager">The inspectable manager this UI is being attached to.</param>
        void InitializeUI(InspectableManager inspectableManager);
    }
}