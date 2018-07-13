using Godot;
using System;

public class PlayPrompt : Label
{
	// Member variables here

	public override void _Ready()
	{
		// Called every time the node is added to the scene.
		// Initialization here
		GetNode("/root/GameRoot").Connect("PlaySignal", this, nameof(OnPlay));
		GetNode("/root/GameRoot").Connect("PauseSignal", this, nameof(OnPause));
	}

	public void OnPlay()
	{
		this.Text = "";
	}

	public void OnPause()
	{
		this.Text = "Paused! Press F to resume.";
	}
}
