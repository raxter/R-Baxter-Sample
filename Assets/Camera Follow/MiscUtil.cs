using UnityEngine;
using System.Collections;

public static class MiscUtil
{
    public static Rect GetWorldRect(this RectTransform rect)
    {
        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);
        return new Rect(corners[0], corners[2] - corners[0]);
    }

    public static Bounds ForceToAspect(this Bounds b, float aspect, bool inner)
    {
        float frustumHeight;
        if (inner)
            frustumHeight = Mathf.Min((b.size.x) / aspect, b.size.y);
        else
            frustumHeight = Mathf.Max((b.size.x) / aspect, b.size.y);

        float frustumWidth = frustumHeight * aspect;

        Vector3 newSize = b.size;
        newSize.y = frustumHeight;
        newSize.x = frustumWidth;
        b.size = newSize;

        return b;
    }

    #region Debug drawing

    public static void DrawBounds(Bounds b)
    {
        DrawBounds(b, Color.white);
    }

    public static void DrawBounds(Bounds b, Color c)
    {
        Vector3 min = b.min;
        Vector3 max = b.max;

        Debug.DrawLine(new Vector3(max.x, min.y, min.z), new Vector3(max.x, max.y, min.z), c);
        Debug.DrawLine(new Vector3(max.x, max.y, min.z), new Vector3(min.x, max.y, min.z), c);
        Debug.DrawLine(new Vector3(min.x, max.y, min.z), new Vector3(min.x, min.y, min.z), c);
        Debug.DrawLine(new Vector3(min.x, min.y, min.z), new Vector3(max.x, min.y, min.z), c);

        Debug.DrawLine(new Vector3(max.x, min.y, max.z), new Vector3(max.x, max.y, max.z), c);
        Debug.DrawLine(new Vector3(max.x, max.y, max.z), new Vector3(min.x, max.y, max.z), c);
        Debug.DrawLine(new Vector3(min.x, max.y, max.z), new Vector3(min.x, min.y, max.z), c);
        Debug.DrawLine(new Vector3(min.x, min.y, max.z), new Vector3(max.x, min.y, max.z), c);

        Debug.DrawLine(new Vector3(max.x, min.y, min.z), new Vector3(max.x, min.y, max.z), c);
        Debug.DrawLine(new Vector3(max.x, max.y, min.z), new Vector3(max.x, max.y, max.z), c);
        Debug.DrawLine(new Vector3(min.x, max.y, min.z), new Vector3(min.x, max.y, max.z), c);
        Debug.DrawLine(new Vector3(min.x, min.y, min.z), new Vector3(min.x, min.y, max.z), c);
    }
    public static void DrawRect(Bounds b, Color c)
    {
        DrawRect(b.min, b.max, c);
    }
    public static void DrawRect(Rect r, Color c)
    {
        DrawRect(r.min, r.max, c);
    }
    public static void DrawRect(Vector3 min, Vector3 max, Color c)
    {
        float averageZ = (min.z + max.z) / 2;
        Debug.DrawLine(new Vector3(min.x, min.y, averageZ), new Vector3(max.x, max.y, averageZ), c);
        Debug.DrawLine(new Vector3(max.x, min.y, averageZ), new Vector3(min.x, max.y, averageZ), c);

        Debug.DrawLine(new Vector3(max.x, min.y, averageZ), new Vector3(max.x, max.y, averageZ), c);
        Debug.DrawLine(new Vector3(max.x, max.y, averageZ), new Vector3(min.x, max.y, averageZ), c);
        Debug.DrawLine(new Vector3(min.x, max.y, averageZ), new Vector3(min.x, min.y, averageZ), c);
        Debug.DrawLine(new Vector3(min.x, min.y, averageZ), new Vector3(max.x, min.y, averageZ), c);
    }

    #endregion
}
