using Godot;
using System;

public class PlayPrompt : Label
{
    // Member variables here
    bool paused = true;
    public override void _Ready()
    {
	GetNode("/root/RootNode").Connect("Play_Pause", this, nameof(On_Pause));
        // Called every time the node is added to the scene.
        // Initialization here
        
    }
    
    public void On_Pause() 
    {
	if(paused)
	    this.Text = "";
	else
	    this.Text = "Paused! Press F to resume.";
	paused = !paused;
    }
    
    //    public override void _Process(float delta)
    //    {
    //        // Called every frame. Delta is time since last frame.
    //        // Update game logic here.
    //        
    //    }
}
