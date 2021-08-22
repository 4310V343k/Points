namespace Points.Tools
{
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public static class Tools
    {
        public static Vector3 RoundVector3(this Vector3 vector)
        {
            return new Vector3(vector.x.RoundVector(),
                vector.y.RoundVector(), vector.z.RoundVector());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float RoundVector(this float f)
        {
            const int roundPower = 10000;
            return Mathf.Round(f * roundPower) / roundPower;
        }

        /// <summary>
        ///     Shuffle a list using Unity.
        /// </summary>
        public static void UnityShuffle<T>(this IList<T> list)
        {
            for (var i = list.Count - 1; i > 1; i--)
            {
                var rnd = Random.Range(0, i + 1);

                var value = list[rnd];
                list[rnd] = list[i];
                list[i] = value;
            }
        }
    }
}