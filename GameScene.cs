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
    /* _Process commented out because it has nothing in it.
    public override void _Process(float delta)
    {
	// Called every frame. Delta is time since last frame.
	// Update game logic here.
	
	// Send out a signal on play / pause
	
} */

    public override void _Input(InputEvent x)
    {
	if(x.IsActionPressed("play_pause"))
	    EmitSignal(nameof(Play_Pause));
    }
}
