using Godot;
using System;
using static BagGenerator;

public class Tetromino
{
	public enum TetrominoType
	{
		Z = Mino.Red,
		S = Mino.Green,
		J = Mino.Blue,
		L = Mino.Orange,
		O = Mino.Yellow,
		I = Mino.Cyan,
		T = Mino.Purple,
	}

	public TetrominoType Type { get; }
	public Vector2Int[] MinoTiles { get; }
	public Vector2Int Position { get; private set; }
	public Mino[,] TetrisBoard { get; set; }

	public Tetromino(TetrominoType type, Vector2Int[] minoTiles)
	{
		this.Type = type;
		this.MinoTiles = (Vector2Int[])minoTiles.Clone();
		this.Position = new Vector2Int(4, 23);
	}

	public bool Translate(Vector2Int vec)
	{
		if(TestTranslation(vec))
		{
			this.Position += vec;
			return true;
		}
		return false;
	}

	public void Rotate(Vector2Int.RotationDirection dir)
	{
		for(int i = 0; i < MinoTiles.Length; i++)
		{
			MinoTiles[i] = MinoTiles[i].Rotated(dir);
		}
	}

	private bool TestTranslation(Vector2Int vec)
	{
		for(int i = 0; i < MinoTiles.Length; i++)
		{
			Vector2Int newPos = Position + MinoTiles[i] + vec;
			if(newPos.x < 0 || newPos.x >= GameBoard.BOARD_WIDTH || newPos.y < 0 ||
			   (newPos.y < GameBoard.BOARD_HEIGHT && TetrisBoard[newPos.y, newPos.x] != Mino.Empty))
			{
				return false;
			}
		}
		return true;
	}
}