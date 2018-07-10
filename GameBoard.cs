using Godot;
using System;
using static Tetromino;

public class GameBoard : ColorRect
{
	private TileMap _tileMap;
	private Tile[,] _tetrisBoard = new Tile[20, 10];
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
				_tetrisBoard[i, j] = Tile.Empty;
			}
		}
	}

	public override void _Process(float delta)
	{
		// Called every frame. Delta is time since last frame.
		// Update game logic here.
	}
}
