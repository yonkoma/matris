using Godot;
using System;

public class GameScene : Node2D
{
	[Signal]
	delegate void Play_Pause();

	public override void _Ready()
	{
		// Called every time the node is added to the scene.
		// Initialization here
	}

	public override void _Input(InputEvent x)
	{
		if(x.IsActionPressed("play_pause"))
			EmitSignal(nameof(Play_Pause));
	}
}
