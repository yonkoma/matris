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

	public override void _Process(float delta)
	{
		// Called every frame. Delta is time since last frame.
		// Update game logic here.

	    // Send out a signal on play / pause
	    if (Input.IsActionPressed("play_pause"))
	    {
			EmitSignal(nameof(Play_Pause));
	    }
	}
}
