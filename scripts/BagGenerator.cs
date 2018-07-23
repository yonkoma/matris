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

	private static readonly Vector2Int[] Z_TETROMINO = { new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(0, 0), new Vector2Int(1, 0) };
	private static readonly Vector2Int[] S_TETROMINO = { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) };
	private static readonly Vector2Int[] J_TETROMINO = { new Vector2Int(-1, 1), new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0) };
	private static readonly Vector2Int[] L_TETROMINO = { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, 1) };
	private static readonly Vector2Int[] O_TETROMINO = { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(1, 0) };
	private static readonly Vector2Int[] I_TETROMINO = { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) };
	private static readonly Vector2Int[] T_TETROMINO = { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(1, 0) };

	private static readonly List<TetrominoType> STANDARD_BAG = Enum.GetValues(typeof(TetrominoType)).Cast<TetrominoType>().ToList();

	private Queue<Tetromino> TetrominoBag = new Queue<Tetromino>();

	public BagGenerator()
	{
		AddNewBag();
	}

	public Tetromino Dequeue()
	{
		if(TetrominoBag.Count < 7)
			AddNewBag();
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
			Vector2Int[] addedTetrominoMinos;
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