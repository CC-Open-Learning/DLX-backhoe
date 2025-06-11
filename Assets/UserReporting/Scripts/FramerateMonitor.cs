using UnityEngine;

namespace RemoteEducation.UserReporting
{
    /// <summary>Represents a behavior that monitors the application for framerate issues and automatically submits a user report.</summary>
    public class FramerateMonitor : UserReportingMonitor
    {
        public FramerateMonitor()
        {
            this.MaximumDurationInSeconds = 10;
            this.MinimumFramerate = 15;
        }

        private float duration;
        public float MaximumDurationInSeconds;
        public float MinimumFramerate;

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            float framerate = 1.0f / deltaTime;
            if (framerate < this.MinimumFramerate)
            {
                this.duration += deltaTime;
            }
            else
            {
                this.duration = 0;
            }

            if (this.duration > this.MaximumDurationInSeconds)
            {
                this.duration = 0;
                this.Trigger();
            }
        }
    }
}