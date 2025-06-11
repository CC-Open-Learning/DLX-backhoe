using UnityEngine;

namespace RemoteEducation.Extensions
{
    public static class FloatExtensions
    {
        /// <summary>Compares two floats and determines if they are close enough within a given <paramref name="tolerance"/> value.</summary>
        /// <returns>True if they close enough within the <paramref name="tolerance"/>.</returns>
        public static bool ApproxEquals(this float a, float b, float tolerance)
        {
            return ((a >= (b - tolerance)) & (a <= (b + tolerance)));
        }

        /// <summary>Skews this float +/- some <paramref name="percent"/>.</summary>
        public static float SkewRandomly(this float value, float percent)
        {
            return value * (1 + UnityEngine.Random.Range(-percent, percent));
        }

        /// <summary> Ensures any rotation is expressed in angles between -180 to 180</summary>
        public static float FixRotation(this float value)
        {
            value %= 360;

            if (Mathf.Abs(value) > 180)
            {
                bool wasNegitive = false;

                if (value < 0)
                {
                    wasNegitive = true;
                    value *= -1;
                }

                if (value > 180)
                {
                    value = -180 + (value - 180);
                }

                if (wasNegitive)
                {
                    value *= -1;
                }
            }

            return value;
        }
    }
}
