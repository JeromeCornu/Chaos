using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class ListExtensions
    {
        public static List<T> GetRandomDistinctCount<T>(this List<T> entries, int count)
        {
            // Fisher-Yates Shuffle
            for (var i = entries.Count - 1; i > 0; i--)
            {
                var j = Random.Range(0, i + 1);
                (entries[i], entries[j]) = (entries[j], entries[i]);
            }

            // Trim to count
            count = Mathf.Min(count, entries.Count);
            List<T> result = entries.GetRange(0, count);

            return result;
        }
    }
}