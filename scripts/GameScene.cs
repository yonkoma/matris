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
		GetNode("GameOverMenu/ButtonContainer/PlayAgainButton").Connect("pressed", this, nameof(OnPlayAgain));
		GetNode("GameOverMenu/ButtonContainer/MainMenuButton").Connect("pressed", this, nameof(OnMainMenu));
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

	public void OnPlayAgain()
	{
		GetTree().ChangeScene("res://scenes/GameScene.tscn");
	}

	public void OnMainMenu()
	{
		GetTree().ChangeScene("res://scenes/HomeScene.tscn");
	}
}
