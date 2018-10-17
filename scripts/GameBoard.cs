using Godot;
using System;
using static Tetromino;
using static BagGenerator;

public class GameBoard : TextureRect
{
	public const int BOARD_WIDTH = 10;
	public const int BOARD_HEIGHT = 20;
	public const int GHOST_BOARD_HEIGHT = BOARD_HEIGHT + 5;
	private const float DROP_RATE = 0.30f;
	private const float SOFT_DROP_RATE = 0.08f;
	private const float LOCK_DELAY = 0.5f;
	private const int MAX_LOCK_DELAY_RESETS = 15;
	private static readonly Color WHITE = new Color(1, 1, 1);
	private static readonly Color DROP_PREVIEW_COLOR = new Color(1, 1, 1, 0.6f);

	private Mino[,] TetrisBoard = new Mino[GHOST_BOARD_HEIGHT, BOARD_WIDTH];
	private Sprite[,] SpriteBoard = new Sprite[BOARD_HEIGHT, BOARD_WIDTH];
	private BagGenerator BagGen;
	private Tetromino CurrentTetromino;
	private Tetromino FrozenTetromino;
	private bool GameIsPaused = true;
	private bool GameIsOver = false;
	private float TimeSinceLastMovement = 0;
	private float RemainingLockDelay = LOCK_DELAY;
	private bool TetrominoHasTouchedBottom = false;
	private int RemainingLockResets = MAX_LOCK_DELAY_RESETS;
	private float DropRate = DROP_RATE;
	private ImageTexture TetrominoTexture;

	public override void _Ready()
	{
		// Called every time the node is added to the scene.
		// Initializes Tetris board and Grid
		TetrominoTexture = new ImageTexture();
		TetrominoTexture.Load("res://images/tetrominos.png");
		int tetrominoSize = TetrominoTexture.GetHeight();
		for(int col = 0; col < BOARD_WIDTH; col++)
		{
			for(int row = 0; row < BOARD_HEIGHT; row++)
			{
				SpriteBoard[row, col] = new Sprite();
				SpriteBoard[row, col].Texture = TetrominoTexture;
				SpriteBoard[row, col].Centered = false;
				SpriteBoard[row, col].Hframes = 8;
				SpriteBoard[row, col].Position = new Vector2(col * tetrominoSize, (BOARD_HEIGHT - row - 1) * tetrominoSize);
				SpriteBoard[row, col].Visible = false;
				this.AddChild(SpriteBoard[row, col]);
			}
			for(int row = 0; row < GHOST_BOARD_HEIGHT; row++)
			{
				TetrisBoard[row, col] = Mino.Empty;
			}
		}
		BagGen = new BagGenerator(this.TetrisBoard);
		GetNode("/root/GameRoot").Connect("PlaySignal", this, nameof(OnPlayPause), new Godot.Array { true });
		GetNode("/root/GameRoot").Connect("PauseSignal", this, nameof(OnPlayPause), new Godot.Array { false });
	}

	public void OnPlayPause(bool startedPlaying)
	{
		GameIsPaused = !startedPlaying;
	}

	/// Handles inputs for moving the tetromino
	public override void _Input(InputEvent input)
	{
		if(CurrentTetromino != null && !GameIsPaused && !GameIsOver)
		{
			if(input.IsActionPressed("move_left"))
			{
				if(CurrentTetromino.Translate(Vector2Int.Left))
					ResetLockDelay();
			}
			else if(input.IsActionPressed("move_right"))
			{
				if(CurrentTetromino.Translate(Vector2Int.Right))
					ResetLockDelay();
			}

			if(input.IsActionPressed("rotate_left"))
			{
				if(CurrentTetromino.Rotate(Rotation.Left))
					ResetLockDelay();
			}
			else if(input.IsActionPressed("rotate_right"))
			{
				if(CurrentTetromino.Rotate(Rotation.Right))
					ResetLockDelay();
			}
			if(input.IsActionPressed("hard_drop"))
			{
				CurrentTetromino.HardDrop();
				LockPiece(CurrentTetromino);
				CurrentTetromino = null;
			}
		}
		if(input.IsActionPressed("soft_drop"))
		{
			DropRate = SOFT_DROP_RATE;
		}
		if(input.IsActionReleased("soft_drop"))
		{
			DropRate = DROP_RATE;
		}
	}

	public override void _Process(float delta)
	{
		// Called every frame. Delta is time since last frame.
		// Only run loop if game isn't paused
		if(!GameIsPaused && !GameIsOver)
		{
			TimeSinceLastMovement += delta;

			// If we don't have a tetromino anymore, get a new one
			if(CurrentTetromino == null)
			{
				CurrentTetromino = BagGen.Dequeue();
				if(!CurrentTetromino.Spawn(new Vector2Int(BOARD_WIDTH/2 - 1, BOARD_HEIGHT + 1)))
				{
					OnGameOver();
				}
			}

			// If enough time has elapsed, move the tetromino down one block
			if(TimeSinceLastMovement > DropRate)
			{
				TimeSinceLastMovement = 0;
				if(CurrentTetromino.Translate(Vector2Int.Down))
				{
					RemainingLockDelay = LOCK_DELAY;
					RemainingLockResets = MAX_LOCK_DELAY_RESETS;
				}
			}

			// Check if the tetromino should be locked in place
			if(TetrominoHasTouchedBottom && CurrentTetromino.IsTouchingBottom)
			{
				RemainingLockDelay -= delta;
				if(RemainingLockDelay <= 0)
				{
					LockPiece(CurrentTetromino);
					CurrentTetromino = null;
				}
			}

			SetTetrisBoardSprites();
			if(CurrentTetromino != null)
			{
				TetrominoHasTouchedBottom = CurrentTetromino.IsTouchingBottom;
				SetDropPreviewSprites();
				SetTetrominoSprites();
			}
		}
	}

	/// <summary>
	/// Resets the lock delay if there are resets remaining.
	/// </summary>
	private void ResetLockDelay()
	{
		if(RemainingLockResets > 0)
		{
			RemainingLockResets--;
			RemainingLockDelay = LOCK_DELAY;
		}
	}

	/// <summary>
	/// Set all the sprites according to the tetris board data.
	/// </summary>
	private void SetTetrisBoardSprites()
	{
		for(int row = 0; row < BOARD_HEIGHT; row++)
		{
			for(int col = 0; col < BOARD_WIDTH; col++)
			{
				if(TetrisBoard[row, col] == Mino.Empty)
				{
					SpriteBoard[row, col].Visible = false;
				}
				else
				{
					SpriteBoard[row, col].Visible = true;
					SpriteBoard[row, col].Frame = (int)TetrisBoard[row, col];
					SpriteBoard[row, col].Modulate = WHITE;
				}
			}
		}
	}

	/// <summary>
	/// Set the sprites for the drop preview.
	/// </summary>
	private void SetDropPreviewSprites()
	{
		Vector2Int hardDropOffset = CurrentTetromino.GetHardDropOffset();
		foreach(Vector2Int relativeMino in CurrentTetromino.MinoTiles)
		{
			Vector2Int minoPosition = CurrentTetromino.Position + hardDropOffset + relativeMino;
			if(minoPosition.x >= 0 && minoPosition.x < BOARD_WIDTH && minoPosition.y >= 0 && minoPosition.y < BOARD_HEIGHT)
			{
				SpriteBoard[minoPosition.y, minoPosition.x].Visible = true;
				SpriteBoard[minoPosition.y, minoPosition.x].Frame = (int)CurrentTetromino.Type;
				SpriteBoard[minoPosition.y, minoPosition.x].Modulate = DROP_PREVIEW_COLOR;
			}
		}
	}

	/// <summary>
	/// Set the sprites for the current tetromino.
	/// </summary>
	private void SetTetrominoSprites()
	{
		foreach(Vector2Int relativeMino in CurrentTetromino.MinoTiles)
		{
			Vector2Int minoPosition = CurrentTetromino.Position + relativeMino;
			if(minoPosition.x >= 0 && minoPosition.x < BOARD_WIDTH && minoPosition.y >= 0 && minoPosition.y < BOARD_HEIGHT)
			{
				SpriteBoard[minoPosition.y, minoPosition.x].Visible = true;
				SpriteBoard[minoPosition.y, minoPosition.x].Frame = (int)CurrentTetromino.Type;
				SpriteBoard[minoPosition.y, minoPosition.x].Modulate = WHITE;
			}
		}
	}

	/// <summary>
	/// Lock the given tetromino in place.
	/// Adds all it's minos to the tetris board.
	/// </summary>
	private void LockPiece(Tetromino piece)
	{
		piece.Locked = true;
		bool fullyInGhostZone = true;
		foreach(Vector2Int relativeMinoPos in piece.MinoTiles)
		{
			Vector2Int minoPosition = piece.Position + relativeMinoPos;
			fullyInGhostZone = fullyInGhostZone && minoPosition.y >= BOARD_HEIGHT;
			TetrisBoard[minoPosition.y, minoPosition.x] = (Mino)piece.Type;
		}
		if(fullyInGhostZone)
		{
			OnGameOver();
		}
	}

	private void OnGameOver()
	{
		GameIsOver = true;
		SetTetrisBoardSprites();
		CanvasItem blurLayer = (CanvasItem)GetNode("/root/GameRoot/Blur");
		CanvasItem gameOverMenu = (CanvasItem)GetNode("/root/GameRoot/GameOverMenu");
		blurLayer.Visible = true;
		gameOverMenu.Visible = true;
	}

}

