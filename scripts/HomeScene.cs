using Godot;
using System;

public class HomeScene : Control
{

	public override void _Ready()
	{
		GetNode("/root/HomeRoot/MainMenu/PlayButton").Connect("pressed", this, nameof(OnPlayButtonPressed));
		GetNode("/root/HomeRoot/MainMenu/QuitButton").Connect("pressed", this, nameof(OnQuitButtonPressed));
	}

	public void OnPlayButtonPressed()
	{
		GetTree().ChangeScene("res://scenes/GameScene.tscn");
	}

	public void OnQuitButtonPressed()
	{
		GetTree().Quit();
	}
}
