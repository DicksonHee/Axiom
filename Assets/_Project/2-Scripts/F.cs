using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class F
{
	public static float FastDistance (Vector3 start, Vector3 end) => (end - start).sqrMagnitude;
    public static bool Between (this float value, float min, float max, bool exclusive = false) => exclusive ? value > min && value < max : value >= min && value <= max;
    public static bool Between (this int value, int min, int max, bool exclusive = false) => exclusive ? value > min && value < max : value >= min && value <= max;
    public static bool Between (this Vector2 value, Vector2 min, Vector2 max) => value.x >= min.x && value.x <= max.x && value.y >= min.y && value.y <= max.y;
    public static bool Between (this Vector2Int value, Vector2Int min, Vector2Int max) => value.x >= min.x && value.x <= max.x && value.y >= min.y && value.y <= max.y;
    public static bool Within (this float value, float target, float threshold) => value >= target - threshold / 2f && value <= target + threshold / 2f;
    public static float Wrap (this float value, float min = 0, float max = 1) => value - (max - min) * Mathf.Floor (value / (max - min));
    public static int Wrap (this int value, int min = 0, int max = 1) => value - (max - min) * Mathf.FloorToInt((float)value / (max - min));
    public static float Heading (this Vector2 vector) => Mathf.Atan2 (vector.x, vector.y) * Mathf.Rad2Deg;
    //public static float NormalizeAngle (this float a) => a - 180f * Mathf.Floor ((a + 180f) / 180f);
    public static float NormalizeAngle(this float a) => a > 180 ? a - 360 : a;
    public static T Last<T> (this List<T> list) => list.Count > 0 ? list[list.Count - 1] : default(T);
    public static T Last<T> (this T[] array) => array.Length > 0 ? array[array.Length - 1] : default(T);
    public static T Random<T> (this List<T> list) => list.Count > 0 ? list[UnityEngine.Random.Range(0, list.Count)] : default(T);
    public static T Random<T> (this T[] array) => array.Length > 0 ? array[UnityEngine.Random.Range(0, array.Length)] : default(T);
    public static void Order66 (this Transform parent) { for (int i = 0; i < parent.childCount; i++) Object.Destroy (parent.GetChild (i).gameObject); }
    public static Vector2 ClampPoint(this Vector2 point, Rect bounds) => new Vector2(Mathf.Clamp(point.x, bounds.xMin, bounds.xMax), Mathf.Clamp(point.y, bounds.yMin, bounds.yMax));
    public static Vector3 Ground(this Vector3 vector) => new Vector3(vector.x, 0, vector.z);
    public static bool WithinBounds (Vector2 point, Rect bounds) => point.x >= bounds.xMin && point.x <= bounds.xMax && point.y >= bounds.yMin && point.y <= bounds.yMax;
    public static float Map(this float v, float fromMin, float fromMax, float toMin = 0f, float toMax = 1f) => toMin + (v - fromMin) * (toMax - toMin) / (fromMax - fromMin);

    public static Rect GetViewportRect(this Camera cam)
    {
        Rect viewRect = new Rect(cam.transform.position, Vector2.one);
        viewRect.min = cam.ViewportToWorldPoint(new Vector3(0, 0, 0));
        viewRect.max = cam.ViewportToWorldPoint(new Vector3(1, 1, 0));
        return viewRect;
    }

    public static Rect ClampWithinBounds (ref this Rect rect, Rect bounds)
    {
        bounds.min += rect.size / 2;
        bounds.max -= rect.size / 2;
        rect.position = rect.center.ClampPoint(bounds);
        return rect;
    }

    public static void DrawRect(Rect rect, Color color, float duration = 0)
    {
        Debug.DrawLine(new Vector2(rect.xMin, rect.yMin), new Vector2(rect.xMin, rect.yMax), color, duration);
        Debug.DrawLine(new Vector2(rect.xMin, rect.yMax), new Vector2(rect.xMax, rect.yMax), color, duration);
        Debug.DrawLine(new Vector2(rect.xMax, rect.yMax), new Vector2(rect.xMax, rect.yMin), color, duration);
        Debug.DrawLine(new Vector2(rect.xMax, rect.yMin), new Vector2(rect.xMin, rect.yMin), color, duration);
    }
}