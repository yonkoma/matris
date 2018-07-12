using Godot;
using System;

public class PlayPrompt : Label
{
	// Member variables here
	bool Paused = true;

	public override void _Ready()
	{
		// Called every time the node is added to the scene.
		// Initialization here
		GetNode("/root/RootNode").Connect("Play_Pause", this, nameof(On_Pause));
	}

	public void On_Pause()
	{
		if(Paused)
			this.Text = "";
		else
			this.Text = "Paused! Press F to resume.";
		Paused = !Paused;
	}
}
