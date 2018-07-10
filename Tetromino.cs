using Godot;
using System;
using static BagGenerator;

public class Tetromino
{
	public enum Rotation
	{
		Left  = -1,
		Right = 1,
	}

	private static readonly Vector2[] Z_TETROMINO = {new Vector2(-1, -1), new Vector2(0, -1), new Vector2(0, 0), new Vector2(1, 0)};
	private static readonly Vector2[] S_TETROMINO = {new Vector2(-1, 0), new Vector2(0, 0), new Vector2(0, -1), new Vector2(1, -1)};
	private static readonly Vector2[] J_TETROMINO = {new Vector2(-1, -1), new Vector2(-1, 0), new Vector2(0, 0), new Vector2(1, 0)};
	private static readonly Vector2[] L_TETROMINO = {new Vector2(-1, 0), new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1)};
	private static readonly Vector2[] O_TETROMINO = {new Vector2(0, 0), new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, 0)};
	private static readonly Vector2[] I_TETROMINO = {new Vector2(-1, 0), new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0)};
	private static readonly Vector2[] T_TETROMINO = {new Vector2(0, 0), new Vector2(-1, 0), new Vector2(0, -1), new Vector2(1, 0)};

	private Vector2[] minoTiles;
	

	public Tetromino(Mino tileType) {
		switch(tileType)
		{
			case Mino.Red:
				minoTiles = (Vector2[])Z_TETROMINO.Clone();
				break;
			case Mino.Green:
				minoTiles = (Vector2[])S_TETROMINO.Clone();
				break;
			case Mino.Blue:
				minoTiles = (Vector2[])J_TETROMINO.Clone();
				break;
			case Mino.Orange:
				minoTiles = (Vector2[])L_TETROMINO.Clone();
				break;
			case Mino.Yellow:
				minoTiles = (Vector2[])O_TETROMINO.Clone();
				break;
			case Mino.Cyan:
				minoTiles = (Vector2[])I_TETROMINO.Clone();
				break;
			case Mino.Purple:
				minoTiles = (Vector2[])T_TETROMINO.Clone();
				break;
			default:
				throw new Exception("Tried to create tetromino of unknown type");
		}
	}
}