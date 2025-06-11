using System.Collections.Generic;

namespace RemoteEducation.Extensions
{
    public static class ListExtensions
    {
        private static System.Random rng;

        /// <summary>Shuffles all elements in a List.</summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            if (rng == null)
                 rng = new System.Random();

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
