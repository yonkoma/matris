using System;
using System.Collections.Generic;
using static BagGenerator;
using static TetrisBoard;
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

	/// <summary>
	/// Algorithm for rewarding spins.
	/// </summary>
	public enum SpinAlgorithm
	{
		FourPoint, // Currently only works properly for T-Pieces
		Immobile,
	}

	/// <summary>
	/// Spin types
	/// </summary>
	public enum SpinReward
	{
		None,
		Mini,
		Regular,
	}

	public static readonly List<TetrominoType> SpinBonusPieces = new List<TetrominoType> { TetrominoType.T };
	public static readonly SpinAlgorithm SpinAlgo = SpinAlgorithm.FourPoint;

	public TetrominoType Type { get; }
	public Vector2Int[] MinoTiles { get; private set; }
	public Vector2Int Position { get; private set; } = Vector2Int.Zero;
	public TetrisBoard Board { get; private set; }
	public bool Locked { get; set; } = false;
	public SpinReward CurrentSpinReward { get; private set; }
	private Rotation CurrentRotationState;

	/// <summary>
	/// Create a new tetromino
	/// </summary>
	public Tetromino(TetrominoType type)
	{
		this.Type = type;
		this.MinoTiles = (Vector2Int[])TetrominoTiles(type).Clone();
		CurrentRotationState = Rotation.Up;
		CurrentSpinReward = SpinReward.None;
	}

	/// <summary>
	/// Spawn the tetromino at the given position.
	/// Returns false if the tetromino could not be spawned.
	/// </summary>
	public bool Spawn(TetrisBoard board, Vector2Int position)
	{
		this.Position = position;
		this.Board = board;
		return board.IsValidPosition(position, this);
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
				this.CurrentSpinReward = SpinReward.None;
				Locked = false;
				return true;
			}
			Locked = false;
		}
		return false;
	}

	/// <summary>
	/// Moves the tetromino without performing any checks.
	/// </summary>
	public void UnsafeTranslate(Vector2Int vec)
	{
		this.Position += vec;
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
			Vector2Int kickTranslation = Vector2Int.Zero;
			for(int i = 0; i < kickTranslationCount; i++)
			{
				kickTranslation = offsets[CurrentRotationState][i] - offsets[newRotationState][i];
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
				this.CurrentSpinReward = CheckForSpinReward(kickTranslation);
			}
			Locked = false;
		}
		return rotatedSuccessfully;
	}

	private SpinReward CheckForSpinReward(Vector2Int kickTranslation)
	{
		if(SpinBonusPieces.Contains(this.Type))
		{
			switch(SpinAlgo)
			{
				case SpinAlgorithm.FourPoint:
					// Check the four corners of the T
					Vector2Int[] corners = {
						Vector2Int.Up + Vector2Int.Right,
						Vector2Int.Up + Vector2Int.Left,
						Vector2Int.Down + Vector2Int.Right,
						Vector2Int.Down + Vector2Int.Right,
					};
					int filledCorners = 0;
					foreach(Vector2Int corner in corners)
					{
						if(Board[Position + corner] != Mino.Empty)
						{
							filledCorners++;
						}
					}
					if(filledCorners >= 3)
					{
						// Check the two squares adjacent to the T's middle finger
						Vector2Int topLeft = Position + (Vector2Int.Up + Vector2Int.Left).Rotated(this.CurrentRotationState);
						Vector2Int topRight = Position + (Vector2Int.Up + Vector2Int.Right).Rotated(this.CurrentRotationState);
						if(Board[topLeft] != Mino.Empty && Board[topRight] != Mino.Empty)
						{
							return SpinReward.Regular;
						}
						// Check if the T was kicked down to rows
						if(kickTranslation.y == -2)
						{
							return SpinReward.Regular;
						}
						return SpinReward.Mini;
					}
					return SpinReward.None;
				case SpinAlgorithm.Immobile:
					if(!IsValidMovement(Vector2Int.Up, MinoTiles) &&
					   !IsValidMovement(Vector2Int.Left, MinoTiles) &&
					   !IsValidMovement(Vector2Int.Right, MinoTiles) &&
					   !IsValidMovement(Vector2Int.Down, MinoTiles))
					{
						return SpinReward.Regular;
					}
					break;
			}
		}
		return SpinReward.None;
	}

	/// <summary>
	/// Hard drop the tetromino.
	/// Return distance dropped.
	/// </summary>
	public Vector2Int HardDrop()
	{
		if(!Locked)
		{
			Locked = true;
			Vector2Int offset = GetHardDropOffset();
			this.Position += offset;
			Locked = false;
			return offset;
		}
		return Vector2Int.Zero;
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

	public bool IsTouchingBottom()
	{
		return !IsValidMovement(Vector2Int.Down, MinoTiles);
	}

	/// <summary>
	/// Checks if moving the given minos are in all in a valid position after moving them by the given vector.
	/// Does not check intermediate steps along the vector.
	/// </summary>
	private bool IsValidMovement(Vector2Int vec, Vector2Int[] minos)
	{
		return Board.IsValidPosition(this.Position + vec, minos);
	}

	public static Vector2Int[] TetrominoTiles(TetrominoType type)
	{
		switch(type)
		{
			case TetrominoType.Z:
				return Z_TETROMINO;
			case TetrominoType.S:
				return S_TETROMINO;
			case TetrominoType.J:
				return J_TETROMINO;
			case TetrominoType.L:
				return L_TETROMINO;
			case TetrominoType.O:
				return O_TETROMINO;
			case TetrominoType.I:
				return I_TETROMINO;;
			case TetrominoType.T:
				return T_TETROMINO;
			default:
				throw new Exception("Tried to get tetromino of unknown type");
		}
	}

	/// Vector arrays for the seven tetromino
	private static readonly Vector2Int[] Z_TETROMINO = { new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(0, 0), new Vector2Int(1, 0) };
	private static readonly Vector2Int[] S_TETROMINO = { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) };
	private static readonly Vector2Int[] J_TETROMINO = { new Vector2Int(-1, 1), new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0) };
	private static readonly Vector2Int[] L_TETROMINO = { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(1, 1) };
	private static readonly Vector2Int[] O_TETROMINO = { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(1, 0) };
	private static readonly Vector2Int[] I_TETROMINO = { new Vector2Int(-1, 0), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0) };
	private static readonly Vector2Int[] T_TETROMINO = { new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(1, 0) };

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
