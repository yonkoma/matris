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

	private Queue<Tetromino> TetrominoBag = new Queue<Tetromino>();

	public BagGenerator()
	{
		AddNewBag();
	}

	public Tetromino Dequeue()
	{
		return TetrominoBag.Dequeue();
	}

	public Tetromino ElementAt(int index)
	{
		return TetrominoBag.ElementAt(index);
	}

	private void AddNewBag()
	{
		List<TetrominoType> drawingBag = new List<TetrominoType>(STANDARD_BAG);
		Random rand = new Random();
		for(int i = drawingBag.Count; i > 0; i--)
		{
			int chosenTetrominoIndex = rand.Next(i);
			TetrominoType addedTetrominoType = drawingBag[chosenTetrominoIndex];
			Vector2[] addedTetrominoMinos;
			switch(addedTetrominoType)
			{
				case TetrominoType.Z:
					addedTetrominoMinos = Z_TETROMINO;
					break;
				case TetrominoType.S:
					addedTetrominoMinos = S_TETROMINO;
					break;
				case TetrominoType.J:
					addedTetrominoMinos = J_TETROMINO;
					break;
				case TetrominoType.L:
					addedTetrominoMinos = L_TETROMINO;
					break;
				case TetrominoType.O:
					addedTetrominoMinos = O_TETROMINO;
					break;
				case TetrominoType.I:
					addedTetrominoMinos = I_TETROMINO;
					break;
				case TetrominoType.T:
					addedTetrominoMinos = T_TETROMINO;
					break;
				default:
					throw new Exception("Tried to create tetromino of unknown type");
			}
			TetrominoBag.Enqueue(new Tetromino(addedTetrominoType, addedTetrominoMinos));
			drawingBag.RemoveAt(chosenTetrominoIndex);
		}
	}
}