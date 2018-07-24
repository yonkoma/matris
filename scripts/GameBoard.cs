using Godot;
using System;
using static Tetromino;
using static BagGenerator;

public class GameBoard : ColorRect
{
	public const int BOARD_WIDTH = 10;
	public const int BOARD_HEIGHT = 20;
	private const float DROP_RATE = 0.15f;
	private const float SOFT_DROP_RATE = DROP_RATE/2;

	private TileMap BoardTileMap;
	private Mino[,] TetrisBoard = new Mino[BOARD_HEIGHT, BOARD_WIDTH];
	private Sprite[,] SpriteBoard = new Sprite[BOARD_HEIGHT, BOARD_WIDTH];
	private BagGenerator BagGen = new BagGenerator();
	private Tetromino CurrentTetromino;
	private Tetromino FrozenTetromino;
	private bool GameIsPaused = true;
	private float TimeSinceLastMovement = 0;
	private float DropRate = DROP_RATE;

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
				SpriteBoard[row, col].Position = new Vector2(col * tetrominoSize, (BOARD_HEIGHT - row - 1) * tetrominoSize);
				SpriteBoard[row, col].Visible = false;
				this.AddChild(SpriteBoard[row, col]);
			}
		}
		GetNode("/root/GameRoot").Connect("PlaySignal", this, nameof(OnPlayPause), new object[] { true });
		GetNode("/root/GameRoot").Connect("PauseSignal", this, nameof(OnPlayPause), new object[] { false });
	}

	public void OnPlayPause(bool startedPlaying)
	{
		GameIsPaused = !startedPlaying;
	}

	public override void _Input(InputEvent input)
	{
		if(CurrentTetromino != null && !GameIsPaused)
		{
			if(input.IsAction("move_left"))
			{
				CurrentTetromino.Translate(Vector2Int.Left);
			}
			else if(input.IsAction("move_right"))
			{
				CurrentTetromino.Translate(Vector2Int.Right);
			}

			if(input.IsActionPressed("rotate_left"))
			{
				CurrentTetromino.Rotate(Vector2Int.RotationDirection.Left);
			}
			else if(input.IsActionPressed("rotate_right"))
			{
				CurrentTetromino.Rotate(Vector2Int.RotationDirection.Right);
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
		// Update game logic here.
		if(!GameIsPaused)
		{
			TimeSinceLastMovement += delta;

			if(CurrentTetromino == null)
			{
				CurrentTetromino = BagGen.Dequeue();
				CurrentTetromino.TetrisBoard = TetrisBoard;
			}

			if(TimeSinceLastMovement > DropRate)
			{
				TimeSinceLastMovement = 0;
				if(!CurrentTetromino.Translate(Vector2Int.Down))
				{
					foreach(Vector2Int relativeMinoPos in CurrentTetromino.MinoTiles)
					{
						Vector2Int minoPosition = CurrentTetromino.Position + relativeMinoPos;
						TetrisBoard[minoPosition.y, minoPosition.x] = (Mino)CurrentTetromino.Type;
					}
					CurrentTetromino = null;
				}
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
			foreach(Vector2Int relativeMino in CurrentTetromino.MinoTiles)
			{
				Vector2Int minoPosition = CurrentTetromino.Position + relativeMino;
				if(minoPosition.x >= 0 && minoPosition.x < BOARD_WIDTH && minoPosition.y >= 0 && minoPosition.y < BOARD_HEIGHT)
				{
					SpriteBoard[minoPosition.y, minoPosition.x].Visible = true;
					SpriteBoard[minoPosition.y, minoPosition.x].Frame = (int)CurrentTetromino.Type;
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

