using Godot;
using System;
using static Tetromino;
using static BagGenerator;

public class GameBoard : ColorRect
{
	private TileMap BoardTileMap;
	private Mino[,] TetrisBoard = new Mino[20, 10];
	private Sprite[,] SpriteBoard = new Sprite[20, 10];
	private BagGenerator BagGen = new BagGenerator();
	private Tetromino CurrentTetromino;
	private Tetromino FrozenTetromino;
	private bool GameIsPaused = true;
	private float TimeSinceLastMovement = 0;

	public override void _Ready()
	{
		// Called every time the node is added to the scene.
		// Initialization here
		// Initializes Tetris board and Grid
		Texture tetrominoTexture = (Texture)GD.Load("res://images/tetrominos.png");
		int tetrominoSize = tetrominoTexture.GetHeight();
		for(int row = 0; row < TetrisBoard.GetLength(0); row++)
		{
			for(int col = 0; col < TetrisBoard.GetLength(1); col++)
			{
				TetrisBoard[row, col] = Mino.Empty;
				SpriteBoard[row, col] = new Sprite();
				SpriteBoard[row, col].Texture = tetrominoTexture;
				SpriteBoard[row, col].Centered = false;
				SpriteBoard[row, col].Hframes = 8;
				SpriteBoard[row, col].Position = new Vector2(col * tetrominoSize, (20 - row - 1) * tetrominoSize);
				SpriteBoard[row, col].Visible = false;
				this.AddChild(SpriteBoard[row, col]);
			}
		}
		GetNode("/root/GameRoot").Connect("PlaySignal", this, nameof(OnPlayPause), new object[] {true});
		GetNode("/root/GameRoot").Connect("PauseSignal", this, nameof(OnPlayPause), new object[] {false});
	}

	public void OnPlayPause(bool startedPlaying) {
		GameIsPaused = !startedPlaying;
	}

	public override void _Process(float delta)
	{
		// Called every frame. Delta is time since last frame.
		// Update game logic here.
		if(!GameIsPaused)
		{
			TimeSinceLastMovement += delta;

			if(CurrentTetromino == null)
			{
				CurrentTetromino = BagGen.Dequeue();
			}

			if(TimeSinceLastMovement > 0.1)
			{
				TimeSinceLastMovement = 0;
				CurrentTetromino.Position += Vector2.Down;
			}

			for(int row = 0; row < TetrisBoard.GetLength(0); row++)
			{
				for(int col = 0; col < TetrisBoard.GetLength(1); col++)
				{
					if(TetrisBoard[row, col] == Mino.Empty)
					{
						SpriteBoard[row, col].Visible = false;
					}
					else
					{
						SpriteBoard[row, col].Visible = true;
						SpriteBoard[row, col].Frame = (int)TetrisBoard[row, col];
					}
				}
			}
			foreach(Vector2 relativeMino in CurrentTetromino.MinoTiles)
			{
				Vector2 minoPosition = CurrentTetromino.Position + relativeMino;
				if(minoPosition.x >= 0 && minoPosition.x < 20 && minoPosition.y >= 0 && minoPosition.y < 20)
				{
					SpriteBoard[(int)minoPosition.y, (int)minoPosition.x].Visible = true;
					SpriteBoard[(int)minoPosition.y, (int)minoPosition.x].Frame = (int)CurrentTetromino.Type;
				}
			}
			if(CurrentTetromino.Position.y < -3)
				CurrentTetromino = null;
		}
	}

	// Override draw function to draw the grid
	// Grid is drawn by taking the number of Tetromino slots and drawing lines long enough to match
	public override void _Draw()
	{



	}
}

