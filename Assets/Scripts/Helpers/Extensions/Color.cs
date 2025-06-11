using UnityEngine;

namespace RemoteEducation.Helpers
{
    ///
    /// <summary>Extension library for Unity Color objects</summary>
    ///
    public static class ColorExtensions
    {
        /// <summary>Converts an unsigned integer value to its corresponding <see cref="Color"/>.</summary>
        /// <returns><see cref="Color"/> where the red channel is taken from the first byte of the unsigned integer, followed by green, blue, and alpha.</returns>
        public static Color ToColor(this uint hex)
        {
            float r, g, b, a;

            r = (hex & 0xFF000000) >> 24;
            g = (hex & 0xFF0000) >> 16;
            b = (hex & 0xFF00) >> 8;
            a = (hex & 0xFF);

            r /= 255.0f;
            g /= 255.0f;
            b /= 255.0f;
            a /= 255.0f;

            return new Color(r, g, b, a);
        }

        /// <summary>Converts a <see cref="Color"/> to its corresponding unsigned integer value.</summary>
        /// <returns>An unsigned integer where the first <see langword="byte"/> represents the red channel, followed by green, blue, and alpha.</returns>
        public static uint ToUInt32(this Color color)
        {
            Color32 c = color;
            return c.ToUInt32();
        }

        /// <summary>Converts a <see cref="Color"/> to its corresponding unsigned integer value.</summary>
        /// <returns>An unsigned integer where the first <see langword="byte"/> represents the red channel, followed by green, blue, and alpha.</returns>
        public static uint ToUInt32(this Color32 color)
        {
            return (uint)((color.r << 24) + (color.g << 16) + (color.b << 8) + color.a);
        }

        /// <summary>
        ///     Determines the hexadecimal string corresponding to
        ///     the Color object. Supports RGB colors.
        /// </summary>
        /// <param name="color"></param>
        /// <returns>Hexadecimal color string</returns>
        public static string ToHexString(this Color color)
        {
            int r = (int)(color.r * 255);
            int g = (int)(color.g * 255);
            int b = (int)(color.b * 255);
            return r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
        }
    }
}