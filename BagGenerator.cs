using Godot;
using System;
using System.Collections.Generic;

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
		
	}

	private static readonly List<Mino> STANDARD_BAG = new List<Mino>();

}