using Godot;
using System;

public class GameScene : Node
{
	[Signal]
	delegate void PlaySignal();
	[Signal]
	delegate void PauseSignal();

	private bool GameIsPaused = true;
	private GameBoard board;

	public override void _Ready()
	{
		board = (GameBoard)GetNode("GameBoard");
		GetNode("GameOverMenu/ButtonContainer/PlayAgainButton").Connect("pressed", this, nameof(OnPlayAgain));
		GetNode("GameOverMenu/ButtonContainer/MainMenuButton").Connect("pressed", this, nameof(OnMainMenu));
		board.Connect("GameOverSignal", this, nameof(OnGameOver));
		board.Connect("ScoreUpdateSignal", this, nameof(OnScoreUpdate));
		board.Connect("BagChangeSignal", this, nameof(OnBagUpdate));
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

	public void OnGameOver()
	{
		CanvasItem blurLayer = (CanvasItem)GetNode("Blur");
		CanvasItem gameOverMenu = (CanvasItem)GetNode("GameOverMenu");
		blurLayer.Visible = true;
		gameOverMenu.Visible = true;
	}

	public void OnScoreUpdate()
	{
		Label scoreLabel = (Label)GetNode("ScoreLabel");
		scoreLabel.Text = board.Score.ToString();
	}

	public void OnBagUpdate()
	{
		Godot.Array previews = GetNode("PiecePreviews").GetChildren();
		for(int i = 0; i < previews.Count; i++)
		{
			PreviewRect preview = (PreviewRect)previews[i];
			preview.PieceType = board.BagGen.ElementAt(i).Type;
		}
	}
}
