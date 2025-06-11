using System.Diagnostics;

namespace RemoteEducation.Extensions
{
    public static class StopwatchExtensions
    {
        /// <summary>Calculates the duration in milliseconds from a Stopwatch duration.</summary>
        /// <returns>A string of the time taken in milliseconds.</returns>
        public static string GetDurationMS(this Stopwatch sw)
        {
            return ((double)sw.ElapsedTicks / (Stopwatch.Frequency * 1000)) + "ms";
        }
    }
}
