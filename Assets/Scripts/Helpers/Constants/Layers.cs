using UnityEngine;

namespace RemoteEducation
{
    /// <summary>A storage class for the integer values of Unity Layers.</summary>
    public static class Layers
    {
        public static readonly int Default = LayerMask.NameToLayer("Default");
        public static readonly int Highlightable = LayerMask.NameToLayer("Highlightable");
        public static readonly int ArmRotation = LayerMask.NameToLayer("ArmRotation");
        public static readonly int SelfInteracting = LayerMask.NameToLayer("SelfInteracting");
        public static readonly int UI = LayerMask.NameToLayer("UI");
        public static readonly int FloorJackStandLifter = LayerMask.NameToLayer("FloorJackStandLifter");
    }
}
