using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static Tetromino;

public class BagGenerator
{
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

	private static readonly Vector2[] Z_TETROMINO = {new Vector2(-1, -1), new Vector2(0, -1), new Vector2(0, 0), new Vector2(1, 0)};
	private static readonly Vector2[] S_TETROMINO = {new Vector2(-1, 0), new Vector2(0, 0), new Vector2(0, -1), new Vector2(1, -1)};
	private static readonly Vector2[] J_TETROMINO = {new Vector2(-1, -1), new Vector2(-1, 0), new Vector2(0, 0), new Vector2(1, 0)};
	private static readonly Vector2[] L_TETROMINO = {new Vector2(-1, 0), new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, -1)};
	private static readonly Vector2[] O_TETROMINO = {new Vector2(0, 0), new Vector2(0, -1), new Vector2(1, -1), new Vector2(1, 0)};
	private static readonly Vector2[] I_TETROMINO = {new Vector2(-1, 0), new Vector2(0, 0), new Vector2(1, 0), new Vector2(2, 0)};
	private static readonly Vector2[] T_TETROMINO = {new Vector2(0, 0), new Vector2(-1, 0), new Vector2(0, -1), new Vector2(1, 0)};

	private static readonly List<TetrominoType> STANDARD_BAG = Enum.GetValues(typeof(TetrominoType)).Cast<TetrominoType>().ToList();

	private List<Vector2[]> TetrominoBag = new List<Vector2[]>();

	public BagGenerator()
	{
		addNewBag();
	}

	private void addNewBag()
	{
		List<TetrominoType> drawingBag = new List<TetrominoType>(STANDARD_BAG);
		Random rand = new Random();
		for(int i = drawingBag.Count; i > 0; i--)
		{
			int chosenTetrominoIndex = rand.Next(i);
			TetrominoType addedTetrominoType = drawingBag.ElementAt(chosenTetrominoIndex);
			switch(addedTetrominoType)
			{
				case TetrominoType.Z:
					TetrominoBag.Add((Vector2[])Z_TETROMINO.Clone());
					break;
				case TetrominoType.S:
					TetrominoBag.Add((Vector2[])S_TETROMINO.Clone());
					break;
				case TetrominoType.J:
					TetrominoBag.Add((Vector2[])J_TETROMINO.Clone());
					break;
				case TetrominoType.L:
					TetrominoBag.Add((Vector2[])L_TETROMINO.Clone());
					break;
				case TetrominoType.O:
					TetrominoBag.Add((Vector2[])O_TETROMINO.Clone());
					break;
				case TetrominoType.I:
					TetrominoBag.Add((Vector2[])I_TETROMINO.Clone());
					break;
				case TetrominoType.T:
					TetrominoBag.Add((Vector2[])T_TETROMINO.Clone());
					break;
				default:
					throw new Exception("Tried to create tetromino of unknown type");
			}
			drawingBag.RemoveAt(chosenTetrominoIndex);
		}
	}

}