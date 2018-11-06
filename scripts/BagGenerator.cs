using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static Tetromino;
using static TetrisBoard;

public class BagGenerator
{
	/// Vector arrays for the seven tetromino
	private static readonly Vector2Int[] Z_TETROMINO = { new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(0, 0), new Vector2Int(1, 0) };
	private static readonly Vector2Int[] S_TETROMINO = { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) };
	private static readonly Vector2Int[] J_TETROMINO = { new Vector2Int(-1, 1), new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0) };
	private static readonly Vector2Int[] L_TETROMINO = { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, 1) };
	private static readonly Vector2Int[] O_TETROMINO = { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(1, 0) };
	private static readonly Vector2Int[] I_TETROMINO = { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) };
	private static readonly Vector2Int[] T_TETROMINO = { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(1, 0) };

	/// A bag of all seven tetromino
	private static readonly List<TetrominoType> STANDARD_BAG = Enum.GetValues(typeof(TetrominoType)).Cast<TetrominoType>().ToList();

	private TetrisBoard Board;
	private Queue<Tetromino> TetrominoBag = new Queue<Tetromino>();

	/// <summary>
	/// Make a new bag generator and add one bag to it.
	/// </summary>
	public BagGenerator(TetrisBoard board)
	{
		this.Board = board;
		AddNewBag();
	}

	/// <summary>
	/// Dequeue a tetromino from the generator's queue.
	/// Add another bag to the queue if there are less than 7 left.
	/// </summary>
	public Tetromino Dequeue()
	{
		if(TetrominoBag.Count < 7)
			AddNewBag();
		return TetrominoBag.Dequeue();
	}

	/// <summary>
	/// Find the tetromino at the given index in the generator's queue.
	/// </summary>
	public Tetromino ElementAt(int index)
	{
		return TetrominoBag.ElementAt(index);
	}

	/// <summary>
	/// Add a new bag of 7 tetrominos to the bag generator queue.
	/// </summary>
	private void AddNewBag()
	{
		// Copy a new bag of all 7 pieces.
		List<TetrominoType> drawingBag = new List<TetrominoType>(STANDARD_BAG);
		Random rand = new Random();
		// Choose random tetrominos from the bag and add them to the generator queue.
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
			TetrominoBag.Enqueue(new Tetromino(addedTetrominoType, addedTetrominoMinos, this.Board));
			drawingBag.RemoveAt(chosenTetrominoIndex);
		}
	}
}