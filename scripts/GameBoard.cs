using Godot;
using System;
using static Tetromino;
using static BagGenerator;

public class GameBoard : ColorRect
{
	private TileMap BoardTileMap;
	private Mino[,] TetrisBoard = new Mino[20, 10];
	private BagGenerator BagGen = new BagGenerator();
	private Tetromino CurrentTetromino;
	private Tetromino FrozenTetromino;
	private bool GameIsPaused;


	public override void _Ready()
	{
		// Called every time the node is added to the scene.
		// Initialization here
		// Initializes Tetris board and Grid
		BoardTileMap =  (TileMap)GetNode("TileMap");
		for(int i = 0; i < TetrisBoard.GetLength(0); i++)
		{
			for(int j = 0; j < TetrisBoard.GetLength(1); j++)
			{
				TetrisBoard[i, j] = Mino.Empty;
			}
		}
	}


	public override void _Process(float delta)
	{
		// Called every frame. Delta is time since last frame.
		// Update game logic here.
		if(!GameIsPaused)
		{
			if(CurrentTetromino == null)
			{
				CurrentTetromino = BagGen.Dequeue();
			}
		}
	}

	// Override draw function to draw the grid
	// Grid is drawn by taking the number of Tetromino slots and drawing lines long enough to match
	public override void _Draw()
	{



	}
}

