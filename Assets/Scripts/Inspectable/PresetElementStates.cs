using System.Collections.Generic;

namespace RemoteEducation.Scenarios.Inspectable
{
    /// <summary>
    /// This class is used to specify the bad elements in to load on an inspectable
    /// object.
    /// The reason this is its own class is so that a list of these can show up in the unity inspector.
    /// </summary>
    [System.Serializable]
    public class PresetElementStates
    {
        public List<InspectableElement> InspectableElements;
    }
}