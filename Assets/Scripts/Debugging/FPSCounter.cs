using TMPro;
using UnityEngine;

namespace RemoteEducation.Debugging
{
    /// <summary>Writes the averaged frames-per-second value to a provided <see cref="TextMeshProUGUI"/> field.</summary>
    public class FPSCounter : MonoBehaviour
    {
        [Tooltip("Text field to display frames-per-second value")]
        public TextMeshProUGUI FramesText;

        [Tooltip("Milliseconds between redraws for frames-per-second indicator (0 updates every frame")]
        [Range(0, 2)]
        public float UpdateSpeed = 0.2f;

        private float lastDisplayTime = -9999f;
        private float[] frametimes = new float[SAMPLES];
        private const int SAMPLES = 120;
        private int i = 0;
        private float total;

        private void OnEnable()
        {
            if (FramesText == null)
            {
                Debug.LogError("No TMProUGUI Assigned to FPSCounter!", gameObject);
                gameObject.SetActive(false);
            }
        }

        public void Update()
        {
            // We are keeping a buffer of all the frametime samples so that we can create an average of all of them to display a smoother and more accurate FPS reading.
            frametimes[i] = Time.unscaledDeltaTime;

            // When i exceeds our number of samples it simply starts over and overwrites the buffer starting at the beginning of the array again.
            i = (i + 1) % SAMPLES;

            if (lastDisplayTime < Time.unscaledTime - UpdateSpeed)
            {
                total = 0;

                for (int i = 0; i < SAMPLES; i++)
                {
                    total += frametimes[i];
                }

                FramesText.text = (SAMPLES / total).ToString("F0");
                lastDisplayTime = Time.unscaledTime;
            }
        }
    }
}
