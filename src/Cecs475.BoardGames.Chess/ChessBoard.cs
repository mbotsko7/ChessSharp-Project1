using System;
using System.Collections.Generic;
using System.Linq;

namespace Cecs475.BoardGames.Chess {
	
	public class ChessBoard : IGameBoard {
		/// <summary>
		/// The number of rows and columns on the chess board.
		/// </summary>
		public const int BOARD_SIZE = 8;

		// Reminder: there are 3 different types of rooks
		private sbyte[,] mBoard = new sbyte[8, 8] {
			{-2, -4, -5, -6, -7, -5, -4, -3 },
			{-1, -1, -1, -1, -1, -1, -1, -1 },
			{0, 0, 0, 0, 0, 0, 0, 0 },
			{0, 0, 0, 0, 0, 0, 0, 0 },
			{0, 0, 0, 0, 0, 0, 0, 0 },
			{0, 0, 0, 0, 0, 0, 0, 0 },
			{1, 1, 1, 1, 1, 1, 1, 1 },
			{2, 4, 5, 6, 7, 5, 4, 3 }
		};
		
		// TODO:
		// You need a way of keeping track of certain game state flags. For example, a rook cannot perform a castling move
		// if either the rook or its king has moved in the game, so you need a way of determining whether those things have 
		// happened. There are several ways to do it and I leave it up to you.


		/// <summary>
		/// Constructs a new chess board with the default starting arrangement.
		/// </summary>
		public ChessBoard() {
			MoveHistory = new List<IGameMove>();

			// TODO:
			// Finish any other one-time setup.
		}

		/// <summary>
		/// Constructs a new chess board by only placing pieces as specified.
		/// </summary>
		/// <param name="startingPositions">a sequence of tuple pairs, where each pair specifies the starting
		/// position of a particular piece to place on the board</param>
		public ChessBoard(IEnumerable<Tuple<BoardPosition, ChessPiecePosition>> startingPositions)
			
			: this() { // NOTE THAT THIS CONSTRUCTOR CALLS YOUR DEFAULT CONSTRUCTOR FIRST


			foreach (int i in Enumerable.Range(0, 8)) { // another way of doing for i = 0 to < 8
				foreach (int j in Enumerable.Range(0, 8)) {
					mBoard[i, j] = 0;
				}
			}
			foreach (var pos in startingPositions) {
				SetPosition(pos.Item1, pos.Item2);
			}
		}

		/// <summary>
		/// A difference in piece values for the pieces still controlled by white vs. black, where
		/// a pawn is value 1, a knight and bishop are value 3, a rook is value 5, and a queen is value 9.
		/// </summary>
		public int Value { get; private set; }


		
		public int CurrentPlayer {
			get {
				// TODO: implement the CurrentPlayer property.
				throw new NotImplementedException();
			}
		}

		// An auto-property suffices here.
		public IList<IGameMove> MoveHistory {
			get; private set;
		}

		/// <summary>
		/// Returns the piece and player at the given position on the board.
		/// </summary>
		public ChessPiecePosition GetPieceAtPosition(BoardPosition position) {
			var boardVal = mBoard[position.Row, position.Col];
			return new ChessPiecePosition((ChessPieceType)Math.Abs(mBoard[position.Row, position.Col]),
				boardVal > 0 ? 1 : boardVal < 0 ? 2 : 0);
		}


		public void ApplyMove(IGameMove move) {
			// TODO: implement this method. 
			throw new NotImplementedException();
		}

		public IEnumerable<IGameMove> GetPossibleMoves() {
			// TODO: implement this method by returning a list of all possible moves.
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets a sequence of all positions on the board that are threatened by the given player. A king
		/// may not move to a square threatened by the opponent.
		/// </summary>
		public IEnumerable<BoardPosition> GetThreatenedPositions(int byPlayer) {
			// TODO: implement this method. Make sure to account for "special" moves.
			throw new NotImplementedException();
		}

		public void UndoLastMove() {
			// TODO: implement this method. Make sure to account for "special" moves.
			throw new NotImplementedException();
		}

		
		/// <summary>
		/// Returns true if the given position on the board is empty.
		/// </summary>
		/// <remarks>returns false if the position is not in bounds</remarks>
		public bool PositionIsEmpty(BoardPosition pos) {
			// TODO: implement this method, using GetGetPieceAtPosition for convenience.
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns true if the given position contains a piece that is the enemy of the given player.
		/// </summary>
		/// <remarks>returns false if the position is not in bounds</remarks>
		public bool PositionIsEnemy(BoardPosition pos, int player) {
			// TODO: implement this method.
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns true if the given position is in the bounds of the board.
		/// </summary>
		public static bool PositionInBounds(BoardPosition pos) {
			// TODO: implement this method. 
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns which player has a piece at the given board position, or 0 if it is empty.
		/// </summary>
		public int GetPlayerAtPosition(BoardPosition pos) {
			// TODO: implement this method, returning 1, 2, or 0.
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the value weight for a piece of the given type.
		/// </summary>
		/*
		 * VALUES:
		 * Pawn: 1
		 * Knight: 3
		 * Bishop: 3
		 * Rook: 5
		 * Queen: 9
		 * King: infinity (maximum integer value)
		 */
		public int GetPieceValue(ChessPieceType pieceType) {
			// TODO: implement this method.
			throw new NotImplementedException();
		}


		/// <summary>
		/// Manually places the given piece at the given position.
		/// </summary>
		// This is used in the constructor
		private void SetPosition(BoardPosition position, ChessPiecePosition piece) {
			mBoard[position.Row, position.Col] = (sbyte)((int)piece.PieceType * (piece.Player == 2 ? -1 :
				piece.Player));
		}
	}
}
