using Godot;
using System;
using System.Collections.Generic;
using static Tetromino;
using static BagGenerator;
using static TetrisBoard;

public class GameBoard : TextureRect
{
	public const int BOARD_WIDTH = 10;
	public const int BOARD_HEIGHT = 20;
	public const int GHOST_BOARD_HEIGHT = BOARD_HEIGHT*2;
	private const float DROP_RATE = 0.30f;
	private const float SOFT_DROP_RATE = 0.03f;
	private const float LOCK_DELAY = 0.5f;
	private const int MAX_LOCK_DELAY_RESETS = 15;
	private const float AUTO_SHIFT_DELAY = 0.2f;
	private const float SHIFT_SPEED = 0.033f;
	private const int FREEZE_DIST = 3; /// How far a tetromino must have fallen before it can be frozen
	private readonly Vector2Int SPAWN_POS = new Vector2Int(BOARD_WIDTH/2 - 1, BOARD_HEIGHT - 1);
	private readonly Color WHITE = new Color(1, 1, 1);
	private readonly Color DROP_PREVIEW_COLOR = new Color(1, 1, 1, 0.6f);
	private readonly Color FROZEN_COLOR = new Color(0.7f, 0.7f, 1);

	public int Score { get; private set; } = 0;
	public BagGenerator BagGen { get; private set; }
	private TetrisBoard Board = new TetrisBoard(GHOST_BOARD_HEIGHT, BOARD_WIDTH);
	private Sprite[,] SpriteBoard = new Sprite[BOARD_HEIGHT, BOARD_WIDTH];
	private bool BackToBack = false;
	private int Combo = 0;
	private bool GameIsPaused = true;
	private bool GameIsOver = false;
	private float TimeSinceLastMovement = 0;
	private float RemainingLockDelay = LOCK_DELAY;
	private bool TetrominoHasTouchedBottom = false;
	private int RemainingLockResets = MAX_LOCK_DELAY_RESETS;
	private float RemainingShiftDelay = AUTO_SHIFT_DELAY;
	private Vector2Int AutoShiftDirection = Vector2Int.Zero;
	private bool SoftDropping = false;
	private Texture TetrominoTexture;

	[Signal]
	delegate void GameOverSignal();
	[Signal]
	delegate void ScoreUpdateSignal();
	[Signal]
	delegate void BagChangeSignal();

	public override void _Ready()
	{
		// Called every time the node is added to the scene.
		// Initializes Tetris board and Grid
		TetrominoTexture = (Texture)GD.Load("res://images/tetrominos.png");
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
		}
		BagGen = new BagGenerator(this);
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
		if(Board.CurrentTetromino != null && !GameIsPaused && !GameIsOver)
		{
			if(input.IsActionPressed("move_left"))
			{
				RemainingShiftDelay = AUTO_SHIFT_DELAY;
				AutoShiftDirection = Vector2Int.Left;
				if(Board.CurrentTetromino.Translate(Vector2Int.Left))
					ResetLockDelay();
			}
			if(input.IsActionPressed("move_right"))
			{
				RemainingShiftDelay = AUTO_SHIFT_DELAY;
				AutoShiftDirection = Vector2Int.Right;
				if(Board.CurrentTetromino.Translate(Vector2Int.Right))
					ResetLockDelay();
			}
			if(input.IsActionReleased("move_left"))
			{
				RemainingShiftDelay = AUTO_SHIFT_DELAY;
				if(Input.IsActionPressed("move_right"))
					AutoShiftDirection = Vector2Int.Right;
				else
					AutoShiftDirection = Vector2Int.Zero;
			}
			if(input.IsActionReleased("move_right"))
			{
				RemainingShiftDelay = AUTO_SHIFT_DELAY;
				if(Input.IsActionPressed("move_left"))
					AutoShiftDirection = Vector2Int.Left;
				else
					AutoShiftDirection = Vector2Int.Zero;
			}

			if(input.IsActionPressed("rotate_left"))
			{
				if(Board.CurrentTetromino.Rotate(Rotation.Left))
					ResetLockDelay();
			}
			else if(input.IsActionPressed("rotate_right"))
			{
				if(Board.CurrentTetromino.Rotate(Rotation.Right))
					ResetLockDelay();
			}
			if(input.IsActionPressed("hard_drop"))
			{
				Score += Math.Abs(2 * Board.CurrentTetromino.HardDrop().y);
				LockPiece(Board.CurrentTetromino);
				Board.CurrentTetromino = null;
			}
			if(input.IsActionPressed("swap"))
			{
				Board.Swap();
				Tetromino piece = Board.FrozenTetromino;
				if(piece != null)
				{
					HashSet<int> modifiedRows = new HashSet<int>();
					foreach(Vector2Int relativeMinoPos in piece.MinoTiles)
					{
						Vector2Int minoPosition = piece.Position + relativeMinoPos;
						modifiedRows.Add(minoPosition.y);
					}
					CheckLineClears(modifiedRows);
				}
			}
		}
		if(input.IsActionPressed("soft_drop"))
			SoftDropping = true;
		if(input.IsActionReleased("soft_drop"))
			SoftDropping = false;
	}

	public override void _Process(float delta)
	{
		// Called every frame. Delta is time since last frame.
		// Only run loop if game isn't paused
		if(!GameIsPaused && !GameIsOver)
		{
			TimeSinceLastMovement += delta;
			if(AutoShiftDirection != Vector2Int.Zero)
				RemainingShiftDelay -= delta;

			// If we don't have a tetromino anymore, get a new one
			if(Board.CurrentTetromino == null)
			{
				if(Board.FrozenTetromino != null && SPAWN_POS.y - Board.FrozenTetromino.Position.y < FREEZE_DIST)
				{
					Board.CurrentTetromino = Board.FrozenTetromino;
					Board.FrozenTetromino = null;
				}
				else
				{
					Board.CurrentTetromino = BagGen.Dequeue();
					EmitSignal(nameof(BagChangeSignal));
					if(!Board.CurrentTetromino.Spawn(Board, SPAWN_POS))
					{
						GameOver();
					}
				}
			}

			// If enough time has elapsed, move the tetromino down one block
			if(TimeSinceLastMovement > (SoftDropping ? SOFT_DROP_RATE : DROP_RATE))
			{
				TimeSinceLastMovement = 0;
				if(Board.CurrentTetromino.Translate(Vector2Int.Down))
				{
					RemainingLockDelay = LOCK_DELAY;
					RemainingLockResets = MAX_LOCK_DELAY_RESETS;
					if(SoftDropping)
					{
						Score += 1;
						EmitSignal(nameof(ScoreUpdateSignal));
					}
				}
			}

			// Move if auto shifting (DAS) is active and enough time has elapsed
			if(RemainingShiftDelay <= 0)
			{
				if(Board.CurrentTetromino.Translate(AutoShiftDirection))
				{
					ResetLockDelay();
					RemainingShiftDelay = SHIFT_SPEED;
				}
			}

			// Check if the tetromino should be locked in place
			if(TetrominoHasTouchedBottom && Board.CurrentTetromino.IsTouchingBottom())
			{
				RemainingLockDelay -= delta;
				if(RemainingLockDelay <= 0)
				{
					LockPiece(Board.CurrentTetromino);
					Board.CurrentTetromino = null;
				}
			}

			SetTetrisBoardSprites();
			if(Board.CurrentTetromino != null)
			{
				TetrominoHasTouchedBottom = Board.CurrentTetromino.IsTouchingBottom();
				SetDropPreviewSprites();
				SetTetrominoSprites();
			}
			if(Board.FrozenTetromino != null)
			{
				SetFreezeSprites();
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
				if(Board[row, col] == Mino.Empty)
				{
					SpriteBoard[row, col].Visible = false;
				}
				else
				{
					SpriteBoard[row, col].Visible = true;
					SpriteBoard[row, col].Frame = (int)Board[row, col];
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
		Vector2Int hardDropOffset = Board.CurrentTetromino.GetHardDropOffset();
		foreach(Vector2Int relativeMino in Board.CurrentTetromino.MinoTiles)
		{
			Vector2Int minoPosition = Board.CurrentTetromino.Position + hardDropOffset + relativeMino;
			if(minoPosition.x >= 0 && minoPosition.x < BOARD_WIDTH && minoPosition.y >= 0 && minoPosition.y < BOARD_HEIGHT)
			{
				SpriteBoard[minoPosition.y, minoPosition.x].Visible = true;
				SpriteBoard[minoPosition.y, minoPosition.x].Frame = (int)Board.CurrentTetromino.Type;
				SpriteBoard[minoPosition.y, minoPosition.x].Modulate = DROP_PREVIEW_COLOR;
			}
		}
	}

	/// <summary>
	/// Set the sprites for the current tetromino.
	/// </summary>
	private void SetTetrominoSprites()
	{
		foreach(Vector2Int relativeMino in Board.CurrentTetromino.MinoTiles)
		{
			Vector2Int absMino = Board.CurrentTetromino.Position + relativeMino;
			if(absMino.x >= 0 && absMino.x < BOARD_WIDTH && absMino.y >= 0 && absMino.y < BOARD_HEIGHT)
			{
				SpriteBoard[absMino.y, absMino.x].Visible = true;
				SpriteBoard[absMino.y, absMino.x].Frame = (int)Board.CurrentTetromino.Type;
				SpriteBoard[absMino.y, absMino.x].Modulate = WHITE;
			}
		}
	}

	private void SetFreezeSprites()
	{
		foreach(Vector2Int relativeMino in Board.FrozenTetromino.MinoTiles)
		{
			Vector2Int absMino = Board.FrozenTetromino.Position + relativeMino;
			if(absMino.x >= 0 && absMino.x < BOARD_WIDTH && absMino.y >= 0 && absMino.y < BOARD_HEIGHT)
			{
				SpriteBoard[absMino.y, absMino.x].Modulate = FROZEN_COLOR;
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
		HashSet<int> modifiedRows = new HashSet<int>();
		foreach(Vector2Int relativeMinoPos in piece.MinoTiles)
		{
			Vector2Int minoPosition = piece.Position + relativeMinoPos;
			fullyInGhostZone = fullyInGhostZone && minoPosition.y >= BOARD_HEIGHT;
			Board[minoPosition] = (Mino)piece.Type;
			modifiedRows.Add(minoPosition.y);
		}
		int lineClears = CheckLineClears(modifiedRows);
		AwaredScore(lineClears, piece.CurrentSpinReward);
		if(fullyInGhostZone)
		{
			GameOver();
		}
	}

	/// <summary>
	/// Check if there are lines to be cleared on the board.
	/// Return the number of lines cleared.
	/// </summary>
	private int CheckLineClears(IEnumerable<int> rows)
	{
		// Sort the rows so things don't get messed up when we start clearing them.
		SortedSet<int> fullRows = new SortedSet<int>(Comparer<int>.Create(
			(i1, i2) => i2.CompareTo(i1)
		));
		int clearCount = 0;
		foreach(int row in rows)
		{
			if(Board.LineIsFull(row))
			{
				if(Board.FrozenTetromino != null)
				{
					foreach(Vector2Int relativeMino in Board.FrozenTetromino.MinoTiles)
					{
						Vector2Int absMino = Board.FrozenTetromino.Position + relativeMino;
						if(absMino.y == row)
						{
							Tetromino oldFrozen = Board.FrozenTetromino;
							Board.FrozenTetromino = null;
							Board.AddTetromino(oldFrozen);
							break;
						}
					}
				}
				fullRows.Add(row);
			}
		}
		foreach(int row in fullRows)
		{
			clearCount++;
			Board.ClearRow(row);
		}
		return clearCount;
	}

	/// <summary>
	/// Award points for clearing line, spins, combos, etc.
	/// </summary>
	private void AwaredScore(int lineClears, SpinReward spinBonus)
	{
		int score;
		if(spinBonus == SpinReward.Regular)
		{
			score = 400 + lineClears*400;
		}
		else
		{
			switch(lineClears)
			{
				case 1:
					score = 100;
					break;
				case 2:
					score = 300;
					break;
				case 3:
					score = 500;
					break;
				case 4:
					score = 800;
					break;
				default:
					score = 0;
					break;
			}
			if(spinBonus == SpinReward.Mini)
			{
				score += 100;
			}
		}
		// Back to back gives + 50%
		if(spinBonus == SpinReward.Regular || lineClears == 4)
		{
			if(BackToBack)
			{
				score += score/2;
			}
			BackToBack = true;
		}
		else
		{
			BackToBack = false;
		}
		// Combo points
		if(lineClears > 0)
		{
			score += Combo*50;
			Combo++;
		}
		else
		{
			Combo = 0;
		}
		// Check for a perfect clear
		if(Board.IsEmpty())
		{
			switch(lineClears)
			{
				case 1:
					score += 800;
					break;
				case 2:
					score += 1000;
					break;
				case 3:
					score += 1800;
					break;
				case 4:
					score += 2000;
					break;
			}
		}
		this.Score += score;
		EmitSignal(nameof(ScoreUpdateSignal));
	}

	/// <summary>
	/// Enter the game over state.
	/// Emits the game over signal so retry buttons, etc. can be displayed.
	/// </summary>
	private void GameOver()
	{
		GameIsOver = true;
		EmitSignal(nameof(GameOverSignal));
		SetTetrisBoardSprites();
	}

}

