using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct IntVector2 : System.IComparable<IntVector2>
{

	public IntVector2(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
	
	public int x;
	public int y;
	
	public static IntVector2 zero
	{
		get
		{
			return new IntVector2(0,0);
		}
	}
	
	public IntVector2 comp
	{
		get
		{
			return new IntVector2(x,-y);
		}
	}
	
	public IntVector2 Reflect
	{
		get
		{
			return new IntVector2(y, x);
		}
	}

	
	public override string ToString()
	{
		return "("+x+","+y+")";
	}

	public int CompareTo(IntVector2 other)
	{
		return x != other.x ? x.CompareTo(other.x) : y.CompareTo(other.y);
	}
	
	public class IntVectorEqualityComparer : IEqualityComparer<IntVector2>
	{
		#region IEqualityComparer[IntVector2] implementation
		public bool Equals (IntVector2 x, IntVector2 y)
		{
			return x.IsEqualTo(y);
		}
		
		public int GetHashCode (IntVector2 obj)
		{
			return (obj.x*obj.y) ^ obj.y;
		}
		#endregion
		
		
	}
	
	public bool IsEqualTo(IntVector2 b)
	{
		return x == b.x && y == b.y;
	}

	public static IntVector2 RoundToInt(Vector2 vec)
	{
		return new IntVector2(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y));
	}

	public static implicit operator Vector2 (IntVector2 v)
	{
		return new Vector2(v.x, v.y);
	}
	
	public static implicit operator Vector3 (IntVector2 v)
	{
		return new Vector3(v.x, v.y, 0);
	}
	
	public static IntVector2 operator+(IntVector2 a, IntVector2 b)
	{
		return new IntVector2(a.x+b.x, a.y+b.y);
	}
	
	public static IntVector2 operator-(IntVector2 a, IntVector2 b)
	{
		return new IntVector2(a.x-b.x, a.y-b.y);
	}

	public static IntVector2 operator-(IntVector2 a)
	{
		return new IntVector2(-a.x, -a.y);
	}
	
	public static IntVector2 operator*(IntVector2 a, int b)
	{
		return new IntVector2(a.x*b, a.y*b);
	}
	
	public static IntVector2 operator*(IntVector2 a, IntVector2 b)
	{
		return new IntVector2(a.x*b.x, a.y*b.y);
	}
	
	public static IntVector2 operator/(IntVector2 a, int b)
	{
		return new IntVector2(a.x/b, a.y/b);
	}
	
	public static IntVector2 operator%(IntVector2 a, int b)
	{
		return new IntVector2(a.x%b, a.y%b);
	}
	
	
}

[System.Serializable]
public struct IntVector3
{
	//	public IntVector2() : this (0,0)
	//	{
	//	}
	public IntVector3(int x, int y, int z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}
	
	public int x;
	public int y;
	public int z;
	
	public static IntVector3 zero
	{
		get
		{
			return new IntVector3(0,0,0);
		}
	}



	public IntVector2 XY
	{
		get
		{
			return new IntVector2(x, y);
		}
	}
	public IntVector2 XZ
	{
		get
		{
			return new IntVector2(x, z);
		}
	}
	public IntVector2 YZ
	{
		get
		{
			return new IntVector2(y, z);
		}
	}
	public IntVector2 YX
	{
		get
		{
			return new IntVector2(y, x);
		}
	}
	public IntVector2 ZX
	{
		get
		{
			return new IntVector2(z, x);
		}
	}
	public IntVector2 ZY
	{
		get
		{
			return new IntVector2(z, y);
		}
	}


	public override string ToString()
	{
		return "("+x+","+y+","+z+")";
	}
	
	
	public class IntVectorEqualityComparer : IEqualityComparer<IntVector3>
	{
		#region IEqualityComparer[IntVector2] implementation
		public bool Equals (IntVector3 x, IntVector3 y)
		{
			return x.IsEqualTo(y);
		}
		
		public int GetHashCode (IntVector3 obj)
		{
			return (((obj.x*obj.y) ^ obj.y)*obj.z) ^ obj.z; // TODO I have no idea if this is any good :/
		}
		#endregion
		
		
	}
	
	public bool IsEqualTo(IntVector3 b)
	{
		return x == b.x && y == b.y;
	}

	public static implicit operator Vector3 (IntVector3 v)
	{
		return new Vector3(v.x, v.y, v.z);
	}
	
	public static IntVector3 operator+(IntVector3 a, IntVector3 b)
	{
		return new IntVector3(a.x+b.x, a.y+b.y, a.z+b.z);
	}
	
	public static IntVector3 operator-(IntVector3 a, IntVector3 b)
	{
		return new IntVector3(a.x-b.x, a.y-b.y, a.z-b.z);
	}

	public static IntVector3 operator-(IntVector3 a)
	{
		return new IntVector3(-a.x, -a.y, -a.z);
	}
	
	public static IntVector3 operator*(IntVector3 a, int b)
	{
		return new IntVector3(a.x*b, a.y*b, a.z*b);
	}
	
	public static IntVector3 operator/(IntVector3 a, int b)
	{
		return new IntVector3(a.x/b, a.y/b, a.z/b);
	}
	
	public static IntVector3 operator%(IntVector3 a, int b)
	{
		return new IntVector3(a.x%b, a.y%b, a.z%b);
	}
	
	
}





















