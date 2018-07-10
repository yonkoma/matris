using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

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

	public enum TetrominoTypes
	{
		Z,
		S,
		J,
		L,
		O,
		I,
		T,
	}

	private static readonly List<TetrominoTypes> STANDARD_BAG = Enum.GetValues(typeof(TetrominoTypes)).Cast<TetrominoTypes>().ToList();

	private List<TetrominoTypes> tetrominoBag = new List<TetrominoTypes>();
	
	public BagGenerator()
	{
		addNewBag();
	}

	private void addNewBag()
	{
		List<TetrominoTypes> newBag = new List<TetrominoTypes>(STANDARD_BAG);
		Random rand = new Random();
		for(int i = newBag.Count; i > 0; i--)
		{
			int chosenTetrominoIndex = rand.Next(i);
			tetrominoBag.Add(newBag.ElementAt(chosenTetrominoIndex));
			newBag.RemoveAt(chosenTetrominoIndex);
		}
	}

}