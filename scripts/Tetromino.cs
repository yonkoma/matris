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

	public TetrominoType Type;
	public Vector2Int[] MinoTiles;
	public Vector2Int Position;

	public Tetromino(TetrominoType type, Vector2Int[] minoTiles)
	{
		this.Type = type;
		this.MinoTiles = (Vector2Int[])minoTiles.Clone();
		this.Position = new Vector2Int(4, 23);
	}

	public void Rotate(Vector2Int.RotationDirection dir)
	{
		for(int i = 0; i < MinoTiles.Length; i++) {
			MinoTiles[i] = MinoTiles[i].Rotated(dir);
		}
	}
}