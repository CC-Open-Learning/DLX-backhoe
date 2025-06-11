using UnityEngine;

namespace RemoteEducation.UI
{
    /// <summary>
    ///     Component which indicates that a resource should be aggregated and managed
    ///     by the CORE Engine <see cref="WindowManager"/>
    /// </summary>
    public class WindowLoader : MonoBehaviour
    {
        [Tooltip("This GameObject should have a LeanWindow component")]
        public GameObject Window;

        [Tooltip("This GameObject should have a LeanButton component")]
        public GameObject Button;
    }
}
