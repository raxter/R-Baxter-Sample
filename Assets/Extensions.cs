using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

/*
A collection of extension functions that I've found useful. Includes general, easy to use code for summing values of a lists, finding angles, object instantiation and destruction and so on
*/

public static class DataStructureExtensions
{
    public static int SumToInt<T>(this List<T> list, System.Func<T, int> conversionFunction)
    {
        return list.Reduce(conversionFunction, 0, (x, y) => x + y);
    }
    public static float Sum<T>(this List<T> list, System.Func<T, float> conversionFunction)
    {
        return list.Reduce(conversionFunction, 0, (x, y) => x + y);
    }
    public static float Average<T>(this List<T> list, System.Func<T, float> conversionFunction)
    {
        return list.Reduce(conversionFunction, 0, (x, y) => x + y) / list.Count;
    }

    public static Ret Reduce<Ret, T>(this List<T> list, System.Func<T, Ret> conversionFunction, Ret initial, System.Func<Ret, Ret, Ret> operation)
    {
        Ret f = initial;
        foreach (T t in list)
        {
            f = operation(f, conversionFunction(t));
        }
        return f;
    }

    public static Dictionary<LookupT, T> CreateLookup<T, LookupT>(this IEnumerable<T> items, System.Func<T, LookupT> getLoopup)
    {
        Dictionary<LookupT, T> lookup = new Dictionary<LookupT, T>();
        foreach (T t in items)
        {
            lookup[getLoopup(t)] = t;
        }
        return lookup;
    }

    public static Dictionary<T, LookupT> CreateReverseLookup<T, LookupT>(this IEnumerable<T> items, System.Func<T, LookupT> getLoopup)
    {
        Dictionary<T, LookupT> lookup = new Dictionary<T, LookupT>();
        foreach (T t in items)
        {
            lookup[t] = getLoopup(t);
        }
        return lookup;
    }

    public static Dictionary<T, int> CreateIndexLookup<T>(this IEnumerable<T> items)
    {
        Dictionary<T, int> lookup = new Dictionary<T, int>();
        int i = 0;
        foreach (T t in items)
        {
            lookup[t] = i;
            i++;
        }
        return lookup;
    }

    public static Dictionary<V, K> CreateReverseLookup<K, V>(this Dictionary<K, V> lookup)
    {
        Dictionary<V, K> reverseLookup = new Dictionary<V, K>();
        foreach (KeyValuePair<K, V> pair in lookup)
        {
            reverseLookup[pair.Value] = pair.Key;
        }
        return reverseLookup;
    }

    public static Dictionary<V, SubK> CreateReverseLookup<K, V, SubK>(this Dictionary<K, V> lookup, System.Func<K, SubK> transformKey)
    {
        Dictionary<V, SubK> reverseLookup = new Dictionary<V, SubK>();
        foreach (KeyValuePair<K, V> pair in lookup)
        {
            reverseLookup[pair.Value] = transformKey(pair.Key);
        }
        return reverseLookup;
    }



    public static T First<T>(this HashSet<T> hashSet) where T : class
    {
        foreach (T t in hashSet)
        {
            return t;
        }
        return null;
    }


    public static int MaxIndex<T, F>(this List<T> list, System.Func<T, F> scoreFunc) where F : System.IComparable
    {
        F max = default(F);
        bool first = true;
        int index = -1;
        for (int i = 0; i < list.Count; i++)
        {
            F score = scoreFunc(list[i]);
            if (first || score.CompareTo(max) > 0)
            {
                first = false;
                max = score;
                index = i;
            }
        }
        return index;
    }

    public static T Max<T, F>(this List<T> list, System.Func<T, F> scoreFunc) where F : System.IComparable
    {
        int index = list.MaxIndex(scoreFunc);
        return list[index];
    }

    public static int MinIndex<T, F>(this List<T> list, System.Func<T, F> scoreFunc) where F : System.IComparable
    {
        F min = default(F);
        bool first = true;
        int index = -1;
        for (int i = 0; i < list.Count; i++)
        {
            F score = scoreFunc(list[i]);
            if (first || score.CompareTo(min) < 0)
            {
                first = false;
                min = score;
                index = i;
            }
        }
        return index;
    }

    public static T Min<T, F>(this List<T> list, System.Func<T, F> scoreFunc) where F : System.IComparable
    {
        int index = list.MinIndex(scoreFunc);
        return list[index];
    }


}


public static class VectorExtensions
{
    public static Vector2 RotateAround(this Vector2 location, Vector2 pivot, float rotation) // degrees
    {
        float s = Mathf.Sin(rotation);
        float c = Mathf.Cos(rotation);

        // translate point back to origin:
        location.x -= pivot.x;
        location.y -= pivot.y;

        // rotate point
        float xnew = location.x * c - location.y * s;
        float ynew = location.x * s + location.y * c;

        // translate point back:
        location.x = xnew + pivot.x;
        location.y = ynew + pivot.y;
        return location;

    }

    public static float RoundToNearest(this float value, float radix)
    {
        return Mathf.Round(value / radix) * radix;
    }

    public static float AngleClockwise(this Vector2 fromVector2, Vector2 toVector2)
    {
        float ang = Vector2.Angle(fromVector2, toVector2);
        Vector3 cross = Vector3.Cross(fromVector2, toVector2);

        if (cross.z > 0)
            ang = 360 - ang;

        return ang;
    }
}


public static class GameObjectExtensions
{

    public static T InstantiateSafe<S, T>(S parent, T prefab) where S : Component where T : Component
    {
        GameObject ret = InstantiateSafe(parent.gameObject, prefab.gameObject);
        return ret == null ? null : ret.GetComponent<T>();
    }
    public static T InstantiateSafe<T>(GameObject parent, T prefab) where T : Component
    {
        GameObject ret = InstantiateSafe(parent, prefab.gameObject);
        return ret == null ? null : ret.GetComponent<T>();
    }
    public static GameObject InstantiateSafe<T>(T parent, GameObject prefab) where T : Component
    {
        GameObject ret = InstantiateSafe(parent.gameObject, prefab);
        return ret;
    }

    public static GameObject InstantiateSafe(GameObject parent, GameObject prefab)
    {
        GameObject instance = null;
        if (Application.isPlaying)
        {
            instance = GameObject.Instantiate(prefab) as GameObject;
        }
        else
        {
#if UNITY_EDITOR
            instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
#endif
        }
        if (instance != null)
        {
            if (parent != null)
            {
                instance.transform.SetParent(parent.transform);
                instance.layer = parent.layer;
            }
        }
        return instance;
    }

    public static void DestroyGameObjectSafe(Component c)
    {
        if (Application.isPlaying)
        {
            GameObject.Destroy(c.gameObject);
        }
        else
        {
#if UNITY_EDITOR
            GameObject.DestroyImmediate(c.gameObject);
#endif
        }
    }
    public static void DestroyGameObjectSafe(GameObject go)
    {
        if (Application.isPlaying)
        {
            GameObject.Destroy(go);
        }
        else
        {
#if UNITY_EDITOR
            GameObject.DestroyImmediate(go);
#endif
        }
    }

}