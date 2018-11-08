using System;
using System.Collections.Generic;

public class TetrisBoard
{
	/// <summary>
	/// A mino that can be in a cell of the board
	/// </summary>
	public enum Mino
	{
		Empty  = -1,
		Red    = 0,
		Green  = 1,
		Blue   = 2,
		Orange = 3,
		Yellow = 4,
		Cyan   = 5,
		Purple = 6,
		Gray   = 7,
	};

	public int Height { get; }
	public int Width { get; }
	private Mino[,] Minos;

	public TetrisBoard(int height, int width)
	{
		this.Height = height;
		this.Width = width;
		this.Minos = new Mino[height, width];
		for(int row = 0; row < height; row++)
		{
			for(int col = 0; col < width; col++)
			{
				Minos[row, col] = Mino.Empty;
			}
		}
	}

	public Mino this[int row, int col]
	{
		get
		{
			if(col < 0 || col >= this.Width || row < 0 || row > Height)
			{
				return Mino.Gray;
			}
			return Minos[row, col];
		}
		set { Minos[row, col] = value; }
	}

	public Mino this[Vector2Int pos]
	{
		get { return this[pos.y, pos.x]; }
		set { this[pos.y, pos.x] = value; }
	}

	/// <summary>
	/// Return true if the given row is completely filled.
	/// </summary>
	public bool LineIsFull(int row)
	{
		if(row < this.Height)
		{
			for(int col = 0; col < this.Width; col++)
			{
				if(Minos[row, col] == Mino.Empty)
				{
					return false;
				}
			}
		}
		return true;
	}

	/// <summary>
	/// Move all rows above the given row down by one.
	/// </summary>
	public void ClearRow(int row)
	{
		for(int i = row; i < Height - 1; i++)
		{
			Array.Copy(Minos, (i+1) * Width, Minos, i * Width, Width);
		}
		for(int i = 0; i < Width; i++)
		{
			Minos[Height - 1, i] = Mino.Empty;
		}
	}

	/// <summary>
	/// Returns true if the board is empty.
	/// </summary>
	public bool IsEmpty()
	{
		foreach(Mino mino in Minos)
		{
			if(mino != Mino.Empty)
			{
				return false;
			}
		}
		return true;
	}
}
