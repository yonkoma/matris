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

	public enum RotationDirection
	{
		Left  = -1,
		Right = 1,
	}

	public TetrominoType Type;
	public Vector2[] MinoTiles;
	public Vector2 Position;

	public Tetromino(TetrominoType type, Vector2[] minoTiles)
	{
		this.Type = type;
		this.MinoTiles = (Vector2[])minoTiles.Clone();
		this.Position = new Vector2(4, 23);
	}

	public void Rotate(RotationDirection dir)
	{
		for(int i = 0; i < MinoTiles.Length; i++) {
			MinoTiles[i] = MinoTiles[i].Rotated(Mathf.Pi / 2 * (int)dir);
		}
	}
}