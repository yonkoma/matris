using Godot;
using System;

public class Tetromino
{
	public enum Tile
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

	private static readonly Vector2[] Z_MINO = {new Vector2(-1, -1), new Vector2(0, -1), new Vector2(0, 0), new Vector2(1, 0)};
	private static readonly Vector2[] S_MINO = {new Vector2(-1, 0), new Vector2(0, 0), new Vector2(0, -1), new Vector2(1, -1)};
	private static readonly Vector2[] J_MINO = {new Vector2(-1, -1), new Vector2(-1, 0), new Vector2(0, 0), new Vector2(1, 0)};
	private static readonly Vector2[] L_MINO = {new Vector2(-1, 0), new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1)};
	private static readonly Vector2[] O_MINO = {new Vector2(0, 0), new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, 0)};
	private static readonly Vector2[] I_MINO = {new Vector2(-1, 0), new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0)};
	private static readonly Vector2[] T_MINO = {new Vector2(0, 0), new Vector2(-1, 0), new Vector2(0, -1), new Vector2(1, 0)};

	private Vector2[] minoTiles;

	public Tetromino(Tile tileType) {
		switch(tileType)
		{
			case Tile.Red:
				minoTiles = (Vector2[])Z_MINO.Clone();
				break;
			case Tile.Green:
				minoTiles = (Vector2[])S_MINO.Clone();
				break;
			case Tile.Blue:
				minoTiles = (Vector2[])J_MINO.Clone();
				break;
			case Tile.Orange:
				minoTiles = (Vector2[])L_MINO.Clone();
				break;
			case Tile.Yellow:
				minoTiles = (Vector2[])O_MINO.Clone();
				break;
			case Tile.Cyan:
				minoTiles = (Vector2[])I_MINO.Clone();
				break;
			case Tile.Purple:
				minoTiles = (Vector2[])T_MINO.Clone();
				break;
			default:
				throw new Exception("Tried to create tetromino of unknown type");
		}
	}
}