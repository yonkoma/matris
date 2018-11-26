using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static Tetromino;
using static TetrisBoard;

public class BagGenerator
{
	/// A bag of all seven tetromino
	private static readonly List<TetrominoType> STANDARD_BAG = Enum.GetValues(typeof(TetrominoType)).Cast<TetrominoType>().ToList();

	private GameBoard GameBoard;
	private Queue<Tetromino> TetrominoBag = new Queue<Tetromino>();

	/// <summary>
	/// Make a new bag generator and add one bag to it.
	/// </summary>
	public BagGenerator(GameBoard board)
	{
		this.GameBoard = board;
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
			TetrominoBag.Enqueue(new Tetromino(addedTetrominoType));
			drawingBag.RemoveAt(chosenTetrominoIndex);
		}
	}
}