using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public abstract class BeizerVertex
{
    public void Set(BeizerVertex bp)
    {
        Control = bp.Control;
        Point = bp.Point;
    }

    public abstract Vector3 Point { get; set; }

    public abstract Vector3 Control { get; set; }

}


[System.Serializable]
public class TransformWithDistanceBeizerVertex : BeizerVertex
{
    public Transform pointTrans;
    public float extent;

    public Quaternion Orientation
    {
        get
        {
            if (pointTrans == null) return Quaternion.identity;
            return pointTrans.rotation;
        }
        set
        {
            if (pointTrans == null) return;
            pointTrans.rotation = value;
        }
    }

    public override Vector3 Point
    {
        get
        {
            if (pointTrans == null)
                return Vector3.zero;
            return pointTrans.position;
        }
        set
        {
            if (pointTrans == null)
                return;
            pointTrans.position = value;
        }
    }
    public override Vector3 Control
    {
        get
        {
            if (pointTrans == null)
                return Vector3.zero;
            return Point + pointTrans.forward * extent;
        }
        set
        {
            if (pointTrans == null)
                return;
            pointTrans.LookAt(value);
            extent = (value - Point).magnitude;
        }
    }
}

[System.Serializable]
public class TransformWithTransformBeizerPoint : BeizerVertex
{
    public Transform pointTrans;
    public Transform controlTrans;


    public override Vector3 Point
    {
        get
        {
            if (pointTrans == null) return Vector3.zero;
            return pointTrans.position;
        }
        set
        {
            if (pointTrans == null) return;
            pointTrans.position = value;
        }
    }
    public override Vector3 Control
    {
        get
        {
            if (controlTrans == null) return Vector3.zero;
            return controlTrans.position;
        }
        set
        {
            if (controlTrans == null) return;
            controlTrans.position = value;
        }
    }
}

public static class BeizerCurveTools
{
    public static Vector3 GetCurvePosition(float t, BeizerVertex beizer1, BeizerVertex beizer2, bool reverseSecondControl = false)
    {
        return GetCurvePosition(t, beizer1.Point, beizer1.Control, beizer2.Point, beizer2.Control, reverseSecondControl);
    }
    public static Vector3 GetCurvePosition(float t, Vector3 point1, Vector3 control1, Vector3 point2, Vector3 control2, bool reverseSecondControl = false)
    {
        if (reverseSecondControl)
            control2 = point2 - (control2 - point2);

        float it = 1 - t;
        float it2 = it * it;
        float it3 = it2 * it;

        float t2 = t * t;
        float t3 = t2 * t;

        return
                it3 * point1 +
                3 * it2 * t * control1 +
                3 * it * t2 * control2 +
                t3 * point2;
    }

    public static Vector3 GetCurveTangent(float t, BeizerVertex beizer1, BeizerVertex beizer2, bool reverseSecondControl = false)
    {
        return GetCurveTangent(t, beizer1.Point, beizer1.Control, beizer2.Point, beizer2.Control, reverseSecondControl);
    }
    public static Vector3 GetCurveTangent(float t, Vector3 point1, Vector3 control1, Vector3 point2, Vector3 control2, bool reverseSecondControl = false)
    {
        if (reverseSecondControl)
            control2 = point2 - (control2 - point2);

        float it = 1 - t;
        float it2 = it * it;
        //float it3 = it2 * it;

        float t2 = t * t;
        //float t3 = t2 * t;

        return
            3 * it2 * (control1 - point1) +
                6 * it * t * (control2 - control1) +
                3 * t2 * (point2 - control2);
    }

    public static float CalculateArcLength(BeizerVertex beizer1, BeizerVertex beizer2, bool reverseSecondControl = false, float accuracy = 0.1f, float startF = 0, float endF = 1)
    {
        return CalculateArcLength(beizer1.Point, beizer1.Control, beizer2.Point, beizer2.Control, reverseSecondControl, accuracy, startF, endF);
    }
    public static float CalculateArcLength(Vector3 point1, Vector3 control1, Vector3 point2, Vector3 control2, bool reverseSecondControl = false, float accuracy = 0.1f, float startF = 0, float endF = 1)
    {
        bool started = false;
        Vector3 lastPoint = Vector3.zero;
        float arcLength = 0;

        System.Func<float, Vector3> pathPickFunc = (f) => GetCurvePosition(f, point1, control1, point2, control2, reverseSecondControl);

        foreach (Vector3 point in UniformUnitInterpolation(pathPickFunc, accuracy, startF, endF))
        {
            if (started)
            {
                arcLength += Vector3.Distance(point, lastPoint);
            }
            else
            {
                started = true;
            }
            lastPoint = point;
        }
        return arcLength;
    }

    public static IEnumerable<Vector3> UniformUnitInterpolation(System.Func<float, Vector3> pathFunc, float unitLength, float startF = 0, float endF = 1)
    {
        float f = startF;
        while (f < endF)
        {
            yield return pathFunc(f);
            f += unitLength;
        }
        yield return pathFunc(endF);

    }

}


public static class BeizerPathTools
{

    // Strategies for path position:
    // Interpolate by Vertex (f)
    // Crawl along path with uniform steps
    // Normlize with Arc length


    // This should be part of an abstract BeizerPath class
    public static void NonUniformUnitToPathTransform(float f, int beizerVertexCount, out int beizerIndex, out float position)
    {
        int i = -1;
        if (f == 1)
        {
            i = beizerVertexCount - 2;
        }
        else
        {
            f *= (beizerVertexCount - 1);
            i = Mathf.FloorToInt(f);
            f %= 1;
            //			Debug.Log (i+":"+f);
        }
        beizerIndex = i;
        position = f;

    }
    public static Vector3 GetPathPosition(float f, BeizerVertex[] beizerVerticies)
    {
        int i;
        NonUniformUnitToPathTransform(f, beizerVerticies.Length, out i, out f);

        return BeizerCurveTools.GetCurvePosition(f, beizerVerticies[i], beizerVerticies[i + 1], true);
    }

    public static Vector3 GetPathTangent(float f, BeizerVertex[] beizerVerticies)
    {
        int i;

        NonUniformUnitToPathTransform(f, beizerVerticies.Length, out i, out f);

        return BeizerCurveTools.GetCurveTangent(f, beizerVerticies[i], beizerVerticies[i + 1], true);
    }


    public static IEnumerable<float> GetPathPositionsNormalised(BeizerVertex[] beizerVerticies, int numberOfPoints, int divisionsPerVertex)
    {
        if (beizerVerticies.Length < 2)
            yield break;
        int segments = beizerVerticies.Length - 1;
        int sectionsPerSegment = divisionsPerVertex;
        int sections = sectionsPerSegment * segments;
        //		Debug.Log ("Segments: "+segments);


        float[] cumulativeArcLengths = new float[sections + 1];
        float totalLength = 0f;

        cumulativeArcLengths[0] = 0;
        for (int i = 0; i < segments; i++)
        {
            for (int s = 0; s < sectionsPerSegment; s++)
            {
                int index = i * sectionsPerSegment + s;
                float sectionFStart = (float)(s + 0) / sectionsPerSegment;
                float sectionFEnd = (float)(s + 1) / sectionsPerSegment;

                float arcLength = BeizerCurveTools.CalculateArcLength(beizerVerticies[i], beizerVerticies[i + 1], true, 1f, sectionFStart, sectionFEnd);
                totalLength += arcLength;
                cumulativeArcLengths[index + 1] = totalLength;
                //				Debug.Log((index+1)+":"+cumulativeArcLengths[index+1]);
            }
        }

        int currentArc_p1 = 0;
        for (int p = 0; p < numberOfPoints; p++)
        {
            float w = (float)p / (numberOfPoints - 1); // u == unwieghted \in [0,1]

            float w_real = w * totalLength;
            if (w == 1)
            {
                yield return 1;
                continue;
            }
            else if (w == 0)
            {
                currentArc_p1 = 1;
            }
            else
            {
                float compare = cumulativeArcLengths[currentArc_p1];
                while (w_real >= compare)
                {
                    currentArc_p1 += 1;
                    compare = cumulativeArcLengths[currentArc_p1];
                }

            }

            // find i  s.t. weighted[i] < w < weighted[i+1]
            // is is currentArc

            int i = currentArc_p1 - 1;

            float w_i = cumulativeArcLengths[i];
            float w_ip1 = cumulativeArcLengths[i + 1];

            float u_i = (float)i / sections;
            float u_ip1 = (float)(i + 1) / sections;

            float relativeW = (w_real - w_i) / (w_ip1 - w_i);

            float u = u_i + relativeW * (u_ip1 - u_i);

            yield return u;

        }

        yield break;
    }

    public static IEnumerable<float> GetPathPositionsNormalisedByVertex(int numberOfPoints, BeizerVertex[] beizerVerticies)
    {
        float[] arcLengths = new float[beizerVerticies.Length - 1];
        float[] cumulaitiveArcLengths = new float[beizerVerticies.Length];
        float totalLength = 0f;

        cumulaitiveArcLengths[0] = 0;
        for (int i = 0; i < arcLengths.Length; i++)
        {
            arcLengths[i] = BeizerCurveTools.CalculateArcLength(beizerVerticies[i], beizerVerticies[i + 1], true, 0.05f);
            totalLength += arcLengths[i];
            cumulaitiveArcLengths[i + 1] = totalLength;
        }

        int currentArc = 0;
        for (int i = 0; i < numberOfPoints; i++)
        {
            float uniformI = (float)i / (numberOfPoints - 1) * totalLength;

            float f = 100000;

            while (f > 1)
            {
                f = (uniformI - cumulaitiveArcLengths[currentArc]) / arcLengths[currentArc];

                if (f > 1)
                    currentArc += 1;

                if (currentArc >= beizerVerticies.Length - 1)
                {
                    f = 1;
                    currentArc = beizerVerticies.Length - 2;
                    break;
                }
            }


            yield return (f + currentArc) / (beizerVerticies.Length - 1);
            //yield return BeizerCurve.GetCurvePosition(f, beizerVerticies[currentArc], beizerVerticies[currentArc+1]);
        }

        yield break;
    }

}










