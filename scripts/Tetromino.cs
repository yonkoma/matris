using Godot;
using System;
using static BagGenerator;

public class Tetromino
{
	public enum TetrominoType
	{
		Z,
		S,
		J,
		L,
		O,
		I,
		T,
	}

	public enum RotationDirection
	{
		Left  = -1,
		Right = 1,
	}

	private TetrominoType type;
	private Vector2[] minoTiles;

	public Vector2 position;

	public Tetromino(TetrominoType type, Vector2[] minoTiles)
	{
		this.type = type;
		this.minoTiles = (Vector2[])minoTiles.Clone();
	}

	public void Rotate(RotationDirection dir)
	{
		for(int i = 0; i < minoTiles.Length; i++) {
			minoTiles[i] = minoTiles[i].Rotated(Mathf.Pi / 2 * (int)dir);
		}
	}
}