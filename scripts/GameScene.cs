using Godot;
using System;

public class GameScene : Node2D
{
	[Signal]
	delegate void PlaySignal();
	[Signal]
	delegate void PauseSignal();

	private bool GameIsPaused = true;

	public override void _Ready()
	{
		// Called every time the node is added to the scene.
		// Initialization here
	}

	/// <summary>
	/// Captures pressing the play/pause button
	/// </summary>
	public override void _Input(InputEvent x)
	{
		if(x.IsActionPressed("play_pause"))
		{
			EmitSignal(GameIsPaused ? nameof(PlaySignal) : nameof(PauseSignal));
			GameIsPaused = !GameIsPaused;
		}
	}
}
