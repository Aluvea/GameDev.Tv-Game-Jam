using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MathfExtensions
{
    public static class MathfExt
    {
        private static float fractionValue;
        private static float range;
        /// <summary>
        /// Oscillates a number between two values
        /// </summary>
        /// <param name="from">The number to oscillate from</param>
        /// <param name="to">The number to oscillate to</param>
        /// <param name="value">The value of the oscillation value (Note: A full oscillation ranges from 0 to 1; the oscillation value loops when a value larger than 1 is entered)</param>
        /// <returns></returns>
        public static float Oscillate(float from, float to, float value)
        {
            range = to - from;

            fractionValue = value * Mathf.PI;

            fractionValue = Mathf.Abs(Mathf.Sin(fractionValue));

            return from + (fractionValue * range);
        }
    }
}

