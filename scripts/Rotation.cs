/// <summary>
/// A struct which stores the orientation of a tetromino.
/// Rotation.Up is the neutral state, Right or Left can be added with the + operator to rotate pieces.
/// Rotation automatically wraps.
/// </summary>
public struct Rotation
{
	private int state;

	public static readonly int Up    = 0;
	public static readonly int Right = 1;
	public static readonly int Down  = 2;
	public static readonly int Left  = 3;

	public static implicit operator Rotation(int value)
	{
		return new Rotation {
			state = value
		};
	}

	public static implicit operator int(Rotation rotation)
	{
		return rotation.state;
	}

	public static Rotation operator+(Rotation a, Rotation b)
	{
		return new Rotation {
			state = (a.state + b.state) % 4
		};
	}

	public static Rotation operator-(Rotation a, Rotation b)
	{
		return new Rotation {
			state = (a.state - b.state) % 4
		};
	}
}