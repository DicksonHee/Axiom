using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bezier
{
    public static class F
    {
        public static float Remap(this float value, float fromMin, float fromMax, float toMin, float toMax) => toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
        public static bool Between (this float value, float min, float max, bool exclusive = false) => exclusive ? value > min && value < max : value >= min && value <= max;
        public static float Wrap (this float value, float min = 0, float max = 1) => value - (max - min) * Mathf.Floor (value / (max - min));
    }
}
