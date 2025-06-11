using UnityEngine;

namespace RemoteEducation.Scenarios
{
    [DisallowMultipleComponent]
    public class PlaybackCanvasDisabler : MonoBehaviour
    {
#if EXCLUDE_PLAYBACK
        private void Awake()
        {
            gameObject.SetActive(false);
        }
#endif
    }
}
