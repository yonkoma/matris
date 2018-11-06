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

	private int Height;
	private int Width;
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
}
