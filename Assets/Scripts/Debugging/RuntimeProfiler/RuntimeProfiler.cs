using UnityEngine;
using System.Text;
using Unity.Profiling;
using TMPro;
using System;

namespace RemoteEducation.Debugging
{
    public class RuntimeProfiler : MonoBehaviour
    {
        private const float FRAMETIME_UPDATE_DELAY = 0.2f;

        private enum Record
        {
            RAM = 0,
            FrameTime = 1,
            TriCount = 2,
            VertCount = 3,
            DrawCalls = 4,
            SetPassCalls = 5
        }

        ProfilerRecorder[] recorders = new ProfilerRecorder[Enum.GetNames(typeof(Record)).Length];

        private StringBuilder sb;

        private TextMeshProUGUI display;

        private string frameTime, framerate;

        private float lastFrameUpdateTime = -1f;

        private void Awake()
        {
            sb = new StringBuilder(500);

            display = GetComponentInChildren<TextMeshProUGUI>();

            if (!PreferenceKeys.DeveloperModeEnabled)
            {
                gameObject.SetActive(false);
            }

            PreferenceKeys.OnPreferenceChanged += PreferenceKeys_OnPreferenceChanged;
        }

        private void PreferenceKeys_OnPreferenceChanged(string key, object value)
        {
            if (key.Equals(PreferenceKeys.DevModePreferenceKey))
            {
                gameObject.SetActive((bool)value);
            }
        }

        static double GetRecorderFrameAverage(ProfilerRecorder recorder)
        {
            var samplesCount = recorder.Capacity;
            if (samplesCount == 0)
                return 0;

            double r = 0;
            // This debugging assembly has been set to allow unsafe code (umanaged memory/pointers/etc) in its assembly definition file.
            // We could replace this with managed memory (a list) but that just creates garbage for the GC to clean up and hurts performance.
            unsafe
            {
                var samples = stackalloc ProfilerRecorderSample[samplesCount];
                recorder.CopyTo(samples, samplesCount);
                for (var i = 0; i < samplesCount; ++i)
                    r += samples[i].Value;
                r /= samplesCount;
            }

            return r;
        }

        void OnEnable()
        {
            recorders[(int)Record.RAM] = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Used Memory");
            recorders[(int)Record.FrameTime] = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 15);
            recorders[(int)Record.TriCount] = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count");
            recorders[(int)Record.VertCount] = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Vertices Count");
            recorders[(int)Record.DrawCalls] = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");
            recorders[(int)Record.SetPassCalls] = ProfilerRecorder.StartNew(ProfilerCategory.Render, "SetPass Calls Count");

        }

        void OnDisable()
        {
            for (int i = 0; i < recorders.Length; i++)
            {
                recorders[i].Dispose();
            }
        }

        void Update()
        {
            var frameTimeAverage = GetRecorderFrameAverage(recorders[(int)Record.FrameTime]) * (1e-6f);

            if(lastFrameUpdateTime < Time.unscaledTime - FRAMETIME_UPDATE_DELAY)
            {
                framerate = $"<size=24>{ 1000 / frameTimeAverage:F0} FPS</size>";
                frameTime = $"{frameTimeAverage:F1} ms";
                lastFrameUpdateTime = Time.unscaledTime;
            } 

            sb.Clear();
            sb.AppendLine(framerate);
            sb.AppendLine(frameTime);
            sb.AppendLine($"{recorders[(int)Record.RAM].LastValue / (1024 * 1024)} MB");
            sb.AppendLine();
            sb.AppendLine($"{recorders[(int)Record.TriCount].LastValue:n0} Tri");
            sb.AppendLine($"{recorders[(int)Record.VertCount].LastValue:n0} Vert");
            sb.AppendLine($"{recorders[(int)Record.DrawCalls].LastValue}/{recorders[(int)Record.SetPassCalls].LastValue} DC/SPC");

            display.text = sb.ToString();
        }
    }
}