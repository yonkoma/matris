using Godot;
using System;
using static Tetromino;
using static BagGenerator;

public class GameBoard : ColorRect
{
	private TileMap _tileMap;
	private Mino[,] _tetrisBoard = new Mino[20, 10];
	private Tetromino currentMino;
	private Tetromino frozenMino;

	public override void _Ready()
	{
		// Called every time the node is added to the scene.
		// Initialization here
		_tileMap = (TileMap)GetNode("TileMap");
		for(int i = 0; i < _tetrisBoard.GetLength(0); i++)
		{
			for(int j = 0; j < _tetrisBoard.GetLength(1); j++)
			{
				_tetrisBoard[i, j] = Mino.Empty;
			}
		}
	}

/* _Process commented out as it has nothing in it.
	public override void _Process(float delta)
	{
		// Called every frame. Delta is time since last frame.
		// Update game logic here.
	}*/
}

