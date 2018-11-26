using Godot;
using System;
using static Tetromino;

public class PreviewRect : Node2D
{
	private const int MAX_SIZE = 4;

	[Export]
	private TetrominoType _pieceType = TetrominoType.Z;
	public TetrominoType PieceType {
		get { return _pieceType; }
		set
		{
			_pieceType = value;
			UpdatePreview();
		}
	}
	private Vector2Int[] Minos = new Vector2Int[] { Vector2Int.Zero };
	private Sprite[,] SpriteBoard = new Sprite[MAX_SIZE, MAX_SIZE];
	private Texture TetrominoTexture;

	public override void _Ready()
	{
		// Called every time the node is added to the scene.
		TetrominoTexture = (Texture)GD.Load("res://images/tetrominos.png");
		int tetrominoSize = TetrominoTexture.GetHeight();
		for(int col = 0; col < MAX_SIZE; col++)
		{
			for(int row = 0; row < MAX_SIZE; row++)
			{
				SpriteBoard[row, col] = new Sprite();
				SpriteBoard[row, col].Texture = TetrominoTexture;
				SpriteBoard[row, col].Centered = false;
				SpriteBoard[row, col].Hframes = 8;
				SpriteBoard[row, col].Position = new Vector2( (col) * tetrominoSize,
					(MAX_SIZE - row - 2) * tetrominoSize);
				SpriteBoard[row, col].Visible = true;
				SpriteBoard[row, col].Frame = 2;
				this.AddChild(SpriteBoard[row, col]);
			}
		}
		UpdatePreview();
	}

	private void UpdatePreview()
	{
		Minos = Tetromino.TetrominoTiles(_pieceType);
		for(int row = 0; row < MAX_SIZE; row++)
		{
			for(int col = 0; col < MAX_SIZE; col++)
			{
				// SpriteBoard[row, col] = new Sprite();
				SpriteBoard[row, col].Visible = false;
			}
		}
		foreach(Vector2Int mino in Minos)
		{
			int shiftedY = mino.y + 1;
			int shiftedX = mino.x + 1;
			SpriteBoard[shiftedY, shiftedX].Visible = true;
			SpriteBoard[shiftedY, shiftedX].Frame = (int)_pieceType;
		}
	}
}
