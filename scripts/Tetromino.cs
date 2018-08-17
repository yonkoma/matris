using Godot;
using System;
using System.Collections.Generic;
using static BagGenerator;

public class Tetromino
{
	public enum TetrominoType
	{
		Z = Mino.Red,
		S = Mino.Green,
		J = Mino.Blue,
		L = Mino.Orange,
		O = Mino.Yellow,
		I = Mino.Cyan,
		T = Mino.Purple,
	}

	public TetrominoType Type { get; }
	public Vector2Int[] MinoTiles { get; private set; }
	public Vector2Int Position { get; private set; }
	public Mino[,] TetrisBoard { get; set; }
	public bool Locked { get; set; } = false;
	private Rotation CurrentRotationState = Rotation.Up;

	public Tetromino(TetrominoType type, Vector2Int[] minoTiles)
	{
		this.Type = type;
		this.MinoTiles = (Vector2Int[])minoTiles.Clone();
		this.Position = new Vector2Int(4, 23);
	}

	public bool Translate(Vector2Int vec)
	{
		if(!Locked && TestMovement(vec, MinoTiles))
		{
			this.Position += vec;
			return true;
		}
		return false;
	}

	public void Rotate(Rotation dir)
	{
		if(!Locked)
		{
			bool rotatedSuccessfully = true;
			Vector2Int[] newMinoTiles = new Vector2Int[MinoTiles.Length];
			Rotation newRotationState = CurrentRotationState + dir;
			for(int i = 0; i < MinoTiles.Length; i++)
			{
				newMinoTiles[i] = MinoTiles[i].Rotated(dir);
			}
			Dictionary<Rotation, Vector2Int[]> offsets;
			switch(Type)
			{
				case TetrominoType.I:
					offsets = RotationOffsets_I;
					break;
				case TetrominoType.O:
					offsets = RotationOffsets_O;
					break;
				default:
					offsets = RotationOffsets;
					break;
			}

			int kickTranslationCount = offsets[CurrentRotationState].Length;
			for(int i = 0; i < kickTranslationCount; i++)
			{
				Vector2Int kickTranslation = offsets[CurrentRotationState][i] - offsets[newRotationState][i];
				if(rotatedSuccessfully = TestMovement(kickTranslation, newMinoTiles))
				{
					this.Position += kickTranslation;
					break;
				}
			}

			if(rotatedSuccessfully)
			{
				CurrentRotationState = newRotationState;
				for(int i = 0; i < MinoTiles.Length; i++)
				{
					MinoTiles[i] = newMinoTiles[i];
				}
			}
		}
	}

	private bool TestMovement(Vector2Int vec, Vector2Int[] minos)
	{
		for(int i = 0; i < minos.Length; i++)
		{
			Vector2Int newPos = Position + minos[i] + vec;
			if(newPos.x < 0 || newPos.x >= GameBoard.BOARD_WIDTH || newPos.y < 0 ||
			   (newPos.y < GameBoard.BOARD_HEIGHT && TetrisBoard[newPos.y, newPos.x] != Mino.Empty))
			{
				return false;
			}
		}
		return true;
	}

	private static readonly Dictionary<Rotation, Vector2Int[]> RotationOffsets = new Dictionary<Rotation, Vector2Int[]>
	{
		[Rotation.Up] = new [] { Vector2Int.Zero, Vector2Int.Zero, Vector2Int.Zero, Vector2Int.Zero, Vector2Int.Zero },
		[Rotation.Right] = new [] { Vector2Int.Zero,
			                        new Vector2Int(1, 0),
			                        new Vector2Int(1, -1),
			                        new Vector2Int(0, 2),
			                        new Vector2Int(1, 2) },
		[Rotation.Down] = new [] { Vector2Int.Zero, Vector2Int.Zero, Vector2Int.Zero, Vector2Int.Zero, Vector2Int.Zero },
		[Rotation.Left] = new [] { Vector2Int.Zero,
			                       new Vector2Int(-1, 0),
			                       new Vector2Int(-1, -1),
			                       new Vector2Int(0, 2),
			                       new Vector2Int(-1, 2) },
	};

	private static readonly Dictionary<Rotation, Vector2Int[]> RotationOffsets_I = new Dictionary<Rotation, Vector2Int[]>
	{
		[Rotation.Up] = new [] { Vector2Int.Zero,
			                     new Vector2Int(-1, 0),
			                     new Vector2Int(+2, 0),
			                     new Vector2Int(-1, 0),
			                     new Vector2Int(+2, 0) },
		[Rotation.Right] = new [] { new Vector2Int(-1, 0),
			                        new Vector2Int(0, 0),
			                        new Vector2Int(0, 0),
			                        new Vector2Int(0, 1),
			                        new Vector2Int(0, -2) },
		[Rotation.Down] = new [] { new Vector2Int(-1, 1),
			                       new Vector2Int(1, 1),
			                       new Vector2Int(-2, 1),
			                       new Vector2Int(1, 0),
			                       new Vector2Int(-2, 0) },
		[Rotation.Left] = new [] { new Vector2Int(0, 1),
			                       new Vector2Int(0, 1),
			                       new Vector2Int(0, 1),
			                       new Vector2Int(0, -1),
			                       new Vector2Int(0, 2) },
	};

	private static readonly Dictionary<Rotation, Vector2Int[]> RotationOffsets_O = new Dictionary<Rotation, Vector2Int[]>
	{
		[Rotation.Up] = new [] { Vector2Int.Zero },
		[Rotation.Right] = new [] { new Vector2Int(0, -1) },
		[Rotation.Down] = new [] { new Vector2Int(-1, -1) },
		[Rotation.Left] = new [] { new Vector2Int(-1, 0) },
	};
}