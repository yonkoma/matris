using Godot;
using System;
using static Tetromino;
using static BagGenerator;

public class GameBoard : TextureRect
{
	public const int BOARD_WIDTH = 10;
	public const int BOARD_HEIGHT = 20;
	private const float DROP_RATE = 0.30f;
	private const float SOFT_DROP_RATE = 0.08f;
	private const float LOCK_DELAY = 0.5f;
	private const int MAX_LOCK_DELAY_RESETS = 15;
	private static readonly Color WHITE = new Color(1, 1, 1);
	private static readonly Color DROP_PREVIEW_COLOR = new Color(1, 1, 1, 0.6f);

	private TileMap BoardTileMap;
	private Mino[,] TetrisBoard = new Mino[BOARD_HEIGHT + 5, BOARD_WIDTH];
	private Sprite[,] SpriteBoard = new Sprite[BOARD_HEIGHT, BOARD_WIDTH];
	private BagGenerator BagGen;
	private Tetromino CurrentTetromino;
	private Tetromino FrozenTetromino;
	private bool GameIsPaused = true;
	private float TimeSinceLastMovement = 0;
	private float RemainingLockDelay = LOCK_DELAY;
	private bool TetrominoHasTouchedBottom = false;
	private int RemainingLockResets = MAX_LOCK_DELAY_RESETS;
	private float DropRate = DROP_RATE;
	private ImageTexture TetrominoTexture;

	public override void _Ready()
	{
		// Called every time the node is added to the scene.
		// Initialization here
		// Initializes Tetris board and Grid
		TetrominoTexture = new ImageTexture();
		TetrominoTexture.Load("res://images/tetrominos.png");
		int tetrominoSize = TetrominoTexture.GetHeight();
		for(int row = 0; row < BOARD_HEIGHT; row++)
		{
			for(int col = 0; col < BOARD_WIDTH; col++)
			{
				TetrisBoard[row, col] = Mino.Empty;
				SpriteBoard[row, col] = new Sprite();
				SpriteBoard[row, col].Texture = TetrominoTexture;
				SpriteBoard[row, col].Centered = false;
				SpriteBoard[row, col].Hframes = 8;
				SpriteBoard[row, col].Position = new Vector2(col * tetrominoSize, (BOARD_HEIGHT - row - 1) * tetrominoSize);
				SpriteBoard[row, col].Visible = false;
				this.AddChild(SpriteBoard[row, col]);
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

	public override void _Input(InputEvent input)
	{
		if(CurrentTetromino != null && !GameIsPaused)
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
		}
		if(input.IsActionPressed("soft_drop"))
		{
			DropRate = SOFT_DROP_RATE;
		}
		if(input.IsActionReleased("soft_drop"))
		{
			DropRate = DROP_RATE;
		}
		if(input.IsActionPressed("hard_drop"))
		{
			CurrentTetromino.HardDrop();
			LockPiece(CurrentTetromino);
			CurrentTetromino = null;
		}
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
				CurrentTetromino.Spawn(new Vector2Int(4, 21));
			}

			if(TimeSinceLastMovement > DropRate)
			{
				TimeSinceLastMovement = 0;
				if(CurrentTetromino.Translate(Vector2Int.Down))
				{
					RemainingLockDelay = LOCK_DELAY;
					RemainingLockResets = MAX_LOCK_DELAY_RESETS;
				}
			}

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

	private void ResetLockDelay()
	{
		if(RemainingLockResets > 0)
		{
			RemainingLockResets--;
			RemainingLockDelay = LOCK_DELAY;
		}
	}

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

	public void LockPiece(Tetromino piece)
	{
		piece.Locked = true;
		foreach(Vector2Int relativeMinoPos in piece.MinoTiles)
		{
			Vector2Int minoPosition = piece.Position + relativeMinoPos;
			TetrisBoard[minoPosition.y, minoPosition.x] = (Mino)piece.Type;
		}
	}

	// Override draw function to draw the grid
	// Grid is drawn by taking the number of Tetromino slots and drawing lines long enough to match
	public override void _Draw()
	{



	}
}

