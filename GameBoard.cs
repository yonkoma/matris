using Godot;
using System;
using static Tetromino;
using static BagGenerator;

public class GameBoard : ColorRect
{
	private TileMap @TileMap;
	private Mino[,] TetrisBoard = new Mino[20, 10];
	private Tetromino CurrentMino;
	private Tetromino FrozenMino;


	public override void _Ready()
	{
		// Called every time the node is added to the scene.
		// Initialization here
		// Initializes Tetris board and Grid
		@TileMap =  (TileMap)GetNode("TileMap");
		for(int i = 0; i < TetrisBoard.GetLength(0); i++)
		{
			for(int j = 0; j < TetrisBoard.GetLength(1); j++)
			{
				TetrisBoard[i, j] = Mino.Empty;
			}
		}
	}

	//Override draw function to draw the grid
	//Grid is drawn by taking the number of Tetromino slots and drawing lines long enough to match
	public override void _Draw()
	{



	}

/* _Process commented out as it has nothing in it.
	public override void _Process(float delta)
	{
		// Called every frame. Delta is time since last frame.
		// Update game logic here.
	}*/
}

