using System;
using Godot;

public struct Vector2Int : IEquatable<Vector2Int>
{
	public int x { get; set; }
	public int y { get; set; }

	public Vector2Int(int xy)
	{
		this.x = xy;
		this.y = xy;
	}

	public Vector2Int(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public int this[int index]
	{
		get
		{
			switch(index)
			{
				case 0:
					return x;
				case 1:
					return y;
				default:
					throw new IndexOutOfRangeException();
			}
		}

		set
		{
			switch(index)
			{
				case 0:
					x = value;
					break;
				case 1:
					y = value;
					break;
				default:
					throw new IndexOutOfRangeException();
			}
		}
	}

	public Vector2Int Rotated(Rotation dir)
	{
		return new Vector2Int(y*dir, -x*dir);
	}

	public static implicit operator Vector2(Vector2Int v)
	{
		return new Vector2(v.x, v.y);
	}

	public static Vector2Int operator+(Vector2Int a, Vector2Int b)
	{
		return new Vector2Int(a.x + b.x, a.y + b.y);
	}

	public static Vector2Int operator-(Vector2Int a, Vector2Int b)
	{
		return new Vector2Int(a.x - b.x, a.y - b.y);
	}

	public static Vector2Int operator*(Vector2Int a, Vector2Int b)
	{
		return new Vector2Int(a.x * b.x, a.y * b.y);
	}

	public static Vector2Int operator*(Vector2Int a, int b)
	{
		return new Vector2Int(a.x * b, a.y * b);
	}

	public static bool operator==(Vector2Int lhs, Vector2Int rhs)
	{
		return lhs.x == rhs.x && lhs.y == rhs.y;
	}

	public static bool operator!=(Vector2Int lhs, Vector2Int rhs)
	{
		return !(lhs == rhs);
	}

	public override bool Equals(object other)
	{
		if(!(other is Vector2Int)) return false;

		return Equals((Vector2Int)other);
	}

	public bool Equals(Vector2Int other)
	{
		return x.Equals(other.x) && y.Equals(other.y);
	}

	public override int GetHashCode()
	{
		return x.GetHashCode() ^ (y.GetHashCode() << 2);
	}

	public override string ToString()
	{
		return String.Format("({0}, {1})", x, y);
	}

	private static readonly Vector2Int _zero   = new Vector2Int(0, 0);
	private static readonly Vector2Int _one    = new Vector2Int(1, 1);
	private static readonly Vector2Int _negOne = new Vector2Int(-1, -1);

	private static readonly Vector2Int _up     = new Vector2Int(0, 1);
	private static readonly Vector2Int _down   = new Vector2Int(0, -1);
	private static readonly Vector2Int _right  = new Vector2Int(1, 0);
	private static readonly Vector2Int _left   = new Vector2Int(-1, 0);

	public static Vector2Int Zero   { get { return _zero; } }
	public static Vector2Int One    { get { return _one; } }
	public static Vector2Int NegOne { get { return _negOne; } }

	public static Vector2Int Up     { get { return _up; } }
	public static Vector2Int Down   { get { return _down; } }
	public static Vector2Int Right  { get { return _right; } }
	public static Vector2Int Left   { get { return _left; } }

	public static Vector2Int UnitX  { get { return _right; } }
	public static Vector2Int UnitY  { get { return _up; } }
}