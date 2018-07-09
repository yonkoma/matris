using Godot;
using System;

public class GameBoard : ColorRect
{
	// Member variables here, example:
	// private int a = 2;
	// private string b = "textvar";
	private TileMap tileMap;

	public override void _Ready()
	{
		tileMap = (TileMap)GetNode("TileMap");
		// Called every time the node is added to the scene.
		// Initialization here
		
	}

	public override void _Process(float delta)
	{
		// Called every frame. Delta is time since last frame.
		// Update game logic here.

	}
}
