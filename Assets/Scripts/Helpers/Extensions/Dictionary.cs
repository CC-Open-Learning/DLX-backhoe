using System.Collections.Generic;

namespace RemoteEducation.Extensions
{
    public static class DictionaryExtensions
    {
        /// <summary>Attempts to get the value for the given <paramref name="key"/>. If it cannot, returns <paramref name="failValue"/>.</summary>
        public static V GetValueSafe<T, V>(this Dictionary<T, V> dict, T key, V failValue = default(V))
        {
            V value;

            if (dict.TryGetValue(key, out value))
                return value;

            return failValue;
        }
    }
}
