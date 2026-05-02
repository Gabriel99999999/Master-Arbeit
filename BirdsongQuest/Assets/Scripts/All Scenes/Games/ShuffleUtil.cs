using System;
using System.Collections.Generic;
using UnityEngine; // für UnityEngine.Random

namespace Utils
{
    public static class ShuffleUtil
    {
        /// <summary>
        /// In-place Fisher–Yates Shuffle.
        /// Wenn 'seed' gesetzt ist, wird System.Random genutzt (reproduzierbar).
        /// Sonst nutzt es UnityEngine.Random (für Gameplay-Zufall).
        /// </summary>
        public static void ShuffleInPlace<T>(this IList<T> list, int? seed = null)
        {
            if (list == null || list.Count <= 1)
                return;

            if (seed.HasValue)
            {
                // deterministische Variante (für Tests)
                var randomNumberGenerator = new System.Random(seed.Value);
                for (int i = list.Count - 1; i > 0; i--)
                {
                    // j ∈ [0, i]
                    int j = randomNumberGenerator.Next(i + 1);
                    (list[i], list[j]) = (list[j], list[i]);
                }
            }
            else
            {
                // Gameplay-Variante (Unity RNG, respektiert z.B. Random.InitState)
                for (int i = list.Count - 1; i > 0; i--)
                {
                    // j ∈ [0, i]
                    int j = UnityEngine.Random.Range(0, i + 1);
                    (list[i], list[j]) = (list[j], list[i]);
                }
            }
        }

        /// <summary>
        /// Gibt eine NEUE Liste zurück, die gemischt ist (Original bleibt unverändert).
        /// </summary>
        public static List<T> ShuffledCopy<T>(this IEnumerable<T> source, int? seed = null)
        {
            var copy = new List<T>(source);
            copy.ShuffleInPlace(seed);
            return copy;
        }
    }
}
