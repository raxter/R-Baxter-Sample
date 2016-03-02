using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
		return list.Reduce(conversionFunction, 0, (x, y) => x + y)/list.Count;
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

	public static Dictionary<LookupT, T> CreateLookup <T, LookupT>(this IEnumerable<T> items, System.Func<T, LookupT> getLoopup)
	{
		Dictionary<LookupT, T> lookup = new Dictionary<LookupT, T>();
		foreach(T t in items)
		{
			lookup[getLoopup(t)] = t;
		}
		return lookup;
	}
	
	public static Dictionary<T, LookupT> CreateReverseLookup <T, LookupT>(this IEnumerable<T> items, System.Func<T, LookupT> getLoopup)
	{
		Dictionary<T, LookupT> lookup = new Dictionary<T, LookupT>();
		foreach(T t in items)
		{
			lookup[t] = getLoopup(t);
		}
		return lookup;
	}
	
	public static Dictionary<T, int> CreateIndexLookup <T>(this IEnumerable<T> items)
	{
		Dictionary<T, int> lookup = new Dictionary<T, int>();
		int i = 0;
		foreach(T t in items)
		{
			lookup[t] = i;
			i++;
		}
		return lookup;
	}

	public static Dictionary<V, K> CreateReverseLookup <K, V>(this Dictionary<K, V> lookup)
	{
		Dictionary<V, K> reverseLookup = new Dictionary<V, K>();
		foreach(KeyValuePair<K, V> pair in lookup)
		{
			reverseLookup[pair.Value] = pair.Key;
		}
		return reverseLookup;
	}
	
	public static Dictionary<V, SubK> CreateReverseLookup <K, V, SubK>(this Dictionary<K, V> lookup, System.Func<K, SubK> transformKey)
	{
		Dictionary<V, SubK> reverseLookup = new Dictionary<V, SubK>();
		foreach(KeyValuePair<K, V> pair in lookup)
		{
			reverseLookup[pair.Value] = transformKey(pair.Key);
		}
		return reverseLookup;
	}



	public static T First <T>(this HashSet<T> hashSet) where T : class
	{
		foreach (T t in hashSet)
		{
			return t;
		}
		return null;
	}
	
//	public static bool TrueForOneShortCircuited<T>(this List<T> list, System.Predicate<T> predicate)
//	{
//		foreach (T t in list)
//		{
//			if (predicate(t))
//				return true;
//		}
//		return false;
//	}

	public static int MaxIndex<T, F>(this List<T> list, System.Func<T, F> scoreFunc) where F : System.IComparable
	{
		F max = default(F);
		bool first = true;
		int index = -1;
//		foreach (T t in list)
		for (int i = 0 ; i < list.Count ; i++)
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
	
	public static T Max<T, F>(this List<T> list, System.Func<T, F> scoreFunc) where F: System.IComparable
	{
		int index = list.MaxIndex(scoreFunc);
		return list[index];
	}
//	public static T Max<T>(this List<T> list, System.Func<T, float> scoreFunc) where T: struct
//	{
//		return list[list.MaxIndex(scoreFunc)];
//	}
	
	public static int MinIndex<T, F>(this List<T> list, System.Func<T, F> scoreFunc) where F: System.IComparable
	{
		F min = default(F);
		bool first = true;
		int index = -1;
//		foreach (T t in list)
		for (int i = 0 ; i < list.Count ; i++)
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
	public static T Min<T, F>(this List<T> list, System.Func<T, F> scoreFunc) where F: System.IComparable
	{
		int index = list.MinIndex(scoreFunc);
		return list[index];
	}
//	public static T Min<T>(this List<T> list, System.Func<T, float> scoreFunc) where T: struct
//	{
////		return null;
//		return list[list.MinIndex(scoreFunc)];
//	}


}


public static class TransformExtensions
{
	
	public static RectTransform rectTransform(this Component c)
	{
		return (RectTransform)(c.transform);
	}
	public static RectTransform rectTransform(this GameObject c)
	{
		return (RectTransform)(c.transform);
	}

	public static Rect WorldRect(this RectTransform t)
	{
		Vector3 [] corners = new Vector3[4];
		t.GetWorldCorners(corners); 
		Bounds b = new Bounds(corners[0], Vector3.zero);
		for (int i = 1 ; i < 4 ; i++)
			b.Encapsulate(corners[i]);

		return Rect.MinMaxRect(b.min.x, b.min.y, b.max.x, b.max.y);
	}


	public static T GetComponentOfParent<T>(this GameObject go, int maxSearchDepth = -1) where T: MonoBehaviour
	{
		return go.transform.GetComponentOfParent<T>(maxSearchDepth);
	}
	public static T GetComponentOfParent<T>(this Transform trans, int maxSearchDepth = -1) where T: MonoBehaviour
	{
		Transform current = trans;
		while (current != null)
		{
			T t = current.GetComponent<T>();
			if (t != null)
				return t;
			current = current.parent;
		}
		return null;
	}

	public static Vector2 MultiplyElementwise(this Vector2 a, Vector2 b)
	{
		return new Vector2(a.x*b.x, a.y*b.y);
	}
	
	public static Vector2 DivideElementwise(this Vector2 a, Vector2 b)
	{
		return new Vector2(a.x/b.x, a.y/b.y);
	}

	public static Vector3 MultiplyElementwise(this Vector3 a, Vector3 b)
	{
		return new Vector3(a.x*b.x, a.y*b.y, a.z*b.z);
	}

	public static Vector3 DivideElementwise(this Vector3 a, Vector3 b)
	{
		return new Vector3(a.x/b.x, a.y/b.y, a.z/b.z);
	}
	
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
		return Mathf.Round (value/radix)*radix;
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

public static class UGUIExtensions
{
	[System.Obsolete("Please use the new Unity 4.6 rectTransform field")]
	public static RectTransform RectTrans(this Component c)
	{
		return c.GetComponent<RectTransform>();
	}

	
	public static Vector2 [] GetWorldCorners(this BoxCollider2D boxC)
	{
		Vector2 [] corners = new Vector2[4];
		
		corners[0].x = boxC.bounds.min.x; corners[0].y = boxC.bounds.min.y;
		corners[1].x = boxC.bounds.max.x; corners[1].y = boxC.bounds.min.y;
		corners[2].x = boxC.bounds.max.x; corners[2].y = boxC.bounds.max.y;
		corners[3].x = boxC.bounds.min.x; corners[3].y = boxC.bounds.max.y;
		
		return corners;
	}


	public static Vector3 CalculateWorldPosition(this PointerEventData eventData, bool setZtoZero)
	{
		Vector3 worldPos = eventData.pressEventCamera.ScreenToWorldPoint(eventData.position);
		if (setZtoZero)
			worldPos.z = 0;

		return worldPos;
	}



}

public static class GameObjectExtensions
{

	public static T InstantiateSafe<S,T>(S parent, T prefab) where S: Component where T: Component
	{
		GameObject ret = InstantiateSafe(parent.gameObject, prefab.gameObject);
		return ret == null ? null : ret.GetComponent<T>();
	}
	public static T InstantiateSafe<T>(GameObject parent, T prefab) where T: Component
	{
		GameObject ret = InstantiateSafe(parent, prefab.gameObject);
		return ret == null ? null : ret.GetComponent<T>();
	}
	public static GameObject InstantiateSafe<T>(T parent, GameObject prefab) where T: Component
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
			
//			Transform t = instance.transform;
//			t.parent = parent.transform;
//			t.localPosition = Vector3.zero;
//			t.localRotation = Quaternion.identity;
//			t.localScale = Vector3.one;
//			instance.layer = parent.layer;
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
	
	public static void DestroyGameObjectSafe (Component c)
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
	public static void DestroyGameObjectSafe (GameObject go)
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