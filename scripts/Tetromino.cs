using System;
using System.Collections.Generic;
using static BagGenerator;
using static Godot.GD;

public class Tetromino
{
	/// <summary>
	/// A type of tetromino.
	/// Uses the same values as the mino colors, so they can be casted to mino colors later on.
	/// </summary>
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
	private Vector2Int _position;
	public Vector2Int Position {
		get { return _position; }
		private set
		{
			_position = value;
			IsTouchingBottom = !IsValidMovement(Vector2Int.Down, MinoTiles);
		}
	}
	public Mino[,] TetrisBoard { get; }
	public bool Locked { get; set; } = false;
	public bool IsTouchingBottom { get; private set; }
	private Rotation CurrentRotationState;

	/// <summary>
	/// Create a new tetromino
	/// </summary>
	public Tetromino(TetrominoType type, Vector2Int[] minoTiles, Mino[,] tetrisBoard)
	{
		this.Type = type;
		this.MinoTiles = (Vector2Int[])minoTiles.Clone();
		this.TetrisBoard = tetrisBoard;
		CurrentRotationState = Rotation.Up;
	}

	/// <summary>
	/// Spawn the tetromino at the given position.
	/// Returns false if the tetromino could not be spawned.
	/// </summary>
	public bool Spawn(Vector2Int position)
	{
		this.Position = position;
		return IsValidMovement(Vector2Int.Zero, this.MinoTiles);
	}

	/// <summary>
	/// Translate the tetromino by the given vector and return whether the translation was successful.
	/// Fails if tetromino is currently locked.
	/// </summary>
	public bool Translate(Vector2Int vec)
	{
		if(!Locked)
		{
			Locked = true;
			if(IsValidMovement(vec, MinoTiles))
			{
				this.Position += vec;
				Locked = false;
				return true;
			}
			Locked = false;
		}
		return false;
	}

	/// <summary>
	/// Rotate the tetromino in the given direction and return whether the rotation was successful.
	/// Tries to apply SRS kicks. Fails if no kick was possible or the tetromino is locked.
	/// </summary>
	public bool Rotate(Rotation dir)
	{
		bool rotatedSuccessfully = false;
		if(!Locked)
		{
			Locked = true;
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
				if(rotatedSuccessfully = IsValidMovement(kickTranslation, newMinoTiles))
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
			Locked = false;
		}
		return rotatedSuccessfully;
	}

	/// <summary>
	/// Hard drop the tetromino.
	/// </summary>
	public void HardDrop()
	{
		if(!Locked)
		{
			Locked = true;
			this.Position += GetHardDropOffset();
			Locked = false;
		}
	}

	/// <summary>
	/// Find the offset to the current position where the tetromino would end up if hard dropped.
	/// </summary>
	public Vector2Int GetHardDropOffset()
	{
		Vector2Int offset = Vector2Int.Zero;
		while(IsValidMovement(Vector2Int.Down + offset, MinoTiles))
		{
			offset += Vector2Int.Down;
		}
		return offset;
	}

	/// <summary>
	/// Checks if moving the given minos are in all in a valid position after moving them by the given vector.
	/// Does not check intermediate steps along the vector.
	/// </summary>
	private bool IsValidMovement(Vector2Int vec, Vector2Int[] minos)
	{
		for(int i = 0; i < minos.Length; i++)
		{
			Vector2Int newPos = this.Position + vec + minos[i];
			if(newPos.x < 0 || newPos.x >= GameBoard.BOARD_WIDTH || newPos.y < 0 ||
			   (newPos.y < GameBoard.GHOST_BOARD_HEIGHT && TetrisBoard[newPos.y, newPos.x] != Mino.Empty))
			{
				return false;
			}
		}
		return true;
	}

	/// Rotation offsets for pieces other than I and O.
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

	/// Rotation offsets for I pieces.
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

	/// Rotation offsets for O pieces. Only used to adjust for wobble when rotating the O.
	private static readonly Dictionary<Rotation, Vector2Int[]> RotationOffsets_O = new Dictionary<Rotation, Vector2Int[]>
	{
		[Rotation.Up] = new [] { Vector2Int.Zero },
		[Rotation.Right] = new [] { new Vector2Int(0, -1) },
		[Rotation.Down] = new [] { new Vector2Int(-1, -1) },
		[Rotation.Left] = new [] { new Vector2Int(-1, 0) },
	};
}
