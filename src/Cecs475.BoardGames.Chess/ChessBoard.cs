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
			List<BoardPosition> positions = GetPlayerPieces(CurrentPlayer);
			List<ChessMove> possible = new List<ChessMove>();
			foreach(BoardPosition position in positions){
				ChessPiecePosition cpos = GetPieceAtPosition(position);
				if(cpos.PieceType == ChessPieceType.Pawn)
					possible.AddRange(PawnThreat(position));
				else if(cpos.PieceType == ChessPieceType.Knight){
					possible.AddRange(KnightThreat(position));
				}
				else if(cpos.PieceType == ChessPieceType.Bishop){
					possible.AddRange(BishopThreat(position));
				}
				else if(cpos.PieceType == ChessPieceType.RookKing ||
					cpos.PieceType == ChessPieceType.RookPawn ||
					cpos.PieceType == ChessPieceType.RookQueen){
						possible.AddRange(RookThreat(position));
					}
				else if(cpos.PieceType == ChessPieceType.Queen){
					possible.AddRange(RookThreat(position));
					possible.AddRange(BishopThreat(position));
				}
				else
					possible.AddRange(KingThreat(position));
			}
			
		}

		/// <summary>
		/// Gets a sequence of all positions on the board that are threatened by the given player. A king
		/// may not move to a square threatened by the opponent.
		/// </summary>
		public IEnumerable<BoardPosition> GetThreatenedPositions(int byPlayer) {
			// TODO: implement this method. Make sure to account for "special" moves.
			List<BoardPosition> positions = GetPlayerPieces(byPlayer);
			List<BoardPosition> threatened = new List<BoardPosition>();
			foreach(BoardPosition position in positions){
				ChessPiecePosition cpos = GetPieceAtPosition(position);
				if(cpos.PieceType == ChessPieceType.Pawn)
					threatened.AddRange(PawnThreat(position));
				else if(cpos.PieceType == ChessPieceType.Knight){
					threatened.AddRange(KnightThreat(position));
				}
				else if(cpos.PieceType == ChessPieceType.Bishop){
					threatened.AddRange(BishopThreat(position));
				}
				else if(cpos.PieceType == ChessPieceType.RookKing ||
					cpos.PieceType == ChessPieceType.RookPawn ||
					cpos.PieceType == ChessPieceType.RookQueen){
						threatened.AddRange(RookThreat(position));
					}
				else if(cpos.PieceType == ChessPieceType.Queen){
					threatened.AddRange(RookThreat(position));
					threatened.AddRange(BishopThreat(position));
				}
				else
					threatened.AddRange(KingThreat(position));
			}
			return threatened;
		}

		private List<BoardPosition> KingThreat(BoardPosition pos){
			//implement king rules on checking and threats
			List<BoardPosition> threatened = new List<BoardPosition>();
			int player = GetPlayerAtPosition(pos);
			for(int r = -1; r < 2; r++){
				for(int c = -1; c < 2;  c++){
					if(r == 0 && c == 0)
						continue;
					else{
						BoardPosition bp = new BoardPosition(r,c);
						if(PositionInBounds(bp) && PositionIsEnemy(bp, player)){
							threatened.Add(bp);
						}

						
					}
				}
			}
			return threatened;
		}

		private List<BoardPosition> RookPossible(BoardPosition pos){
			List<BoardPosition> possible = new List<BoardPosition>();
			int player = GetPlayerAtPosition(pos);
			for(int r = -1; r < 2; r+=2){
				BoardPosition bp = pos;
				bool bounds = true;
				while(bounds){
					bp.Row += r;
					bounds = PositionInBounds(bp);
					if(!bounds)
						break;
					else { //friendlies in possible?
						possible.Add(bp);
						break;
					}

				}
			}
			for(int c = -1; c < 2; c+=2){
				BoardPosition bp = pos;
				bool bounds = true;
				while(bounds){
					bp.Col += c;
					bounds = PositionInBounds(bp);
					if(!bounds)
						break;
					else { //friendlies in possible?
						possible.Add(bp);
						break;
					}

				}
			}
			return possible;
		}
		private List<BoardPosition> RookThreat(BoardPosition pos){
			List<BoardPosition> threatened = new List<BoardPosition>();
			int player = GetPlayerAtPosition(pos);
			for(int r = -1; r < 2; r+=2){
				BoardPosition bp = pos;
				bool bounds = true;
				while(bounds){
					bp.Row += r;
					bounds = PositionInBounds(bp);
					if(!bounds)
						break;
					else if(PositionIsEmpty(bp)){
						continue;
					}
					else if(PositionIsEnemy(bp, player)){
						threatened.Add(bp);
						break;
					}
					else
						break;

				}
			}
			for(int c = -1; c < 2; c+=2){
				BoardPosition bp = pos;
				bool bounds = true;
				while(bounds){
					bp.Col += c;
					bounds = PositionInBounds(bp);
					if(!bounds)
						break;
					else if(PositionIsEmpty(bp)){
						continue;
					}
					else if(PositionIsEnemy(bp, player)){
						threatened.Add(bp);
						break;
					}
					else
						break;

				}
			}
			return threatened;
		}

		private List<BoardPosition> BishopPossible(BoardPosition pos){
			List<BoardPosition> possible = new List<BoardPosition>();
			int player = GetPlayerAtPosition(pos);
			for(int r = -1; r < 2; r += 2){
				for(int c = -1; c < 2; c += 2){
					BoardPosition bp = pos;
					bool bounds = true;
					while(bounds){
						bp.Row += r;
						bp.Col += c;
						bounds = PositionInBounds(bp);
						if(!bounds) //not in bounds
							break;
						else { //if enemy, empty, or my friend? 
							possible.Add(bp);
							break;
						}


					}
				}
			}
			return possible;
		}

		private List<BoardPosition> BishopThreat(BoardPosition pos){
			List<BoardPosition> threatened = new List<BoardPosition>();
			int player = GetPlayerAtPosition(pos);
			for(int r = -1; r < 2; r += 2){
				for(int c = -1; c < 2; c += 2){
					BoardPosition bp = pos;
					bool bounds = true;
					while(bounds){
						bp.Row += r;
						bp.Col += c;
						bounds = PositionInBounds(bp);
						if(!bounds)
							break;
						else if(PositionIsEmpty(bp)){
							continue;
						}
						else if(PositionIsEnemy(bp, player)){
							threatened.Add(bp);
							break;
						}
						else
							break;

					}
				}
			}
			return threatened;
		}

		private List<BoardPosition> KnightPossible(BoardPosition pos){
			List<BoardPosition> possible = new List<BoardPosition>();
			int player = GetPlayerAtPosition(pos);
			List<BoardPosition> position = new List<BoardPosition>(){
				new BoardPosition(pos.Row-2, pos.Row-1),
				new BoardPosition(pos.Row-2, pos.Row+1),
				new BoardPosition(pos.Row+2, pos.Row+1),
				new BoardPosition(pos.Row-2, pos.Row-1),
				new BoardPosition(pos.Col-2, pos.Row-1),
				new BoardPosition(pos.Col-2, pos.Row+1),
				new BoardPosition(pos.Col+2, pos.Row+1),
				new BoardPosition(pos.Col-2, pos.Row-1)};
			foreach(BoardPosition bp in position){
				if(PositionInBounds(bp) && PositionIsEnemy(bp, player))
					possible.Add(bp);
			}
			return possible;
		}
		private List<BoardPosition> KnightThreat(BoardPosition pos){
			List<BoardPosition> threatened = new List<BoardPosition>();
			int player = GetPlayerAtPosition(pos);
			List<BoardPosition> position = new List<BoardPosition>(){
				new BoardPosition(pos.Row-2, pos.Row-1),
				new BoardPosition(pos.Row-2, pos.Row+1),
				new BoardPosition(pos.Row+2, pos.Row+1),
				new BoardPosition(pos.Row-2, pos.Row-1),
				new BoardPosition(pos.Col-2, pos.Row-1),
				new BoardPosition(pos.Col-2, pos.Row+1),
				new BoardPosition(pos.Col+2, pos.Row+1),
				new BoardPosition(pos.Col-2, pos.Row-1)};
			foreach(BoardPosition bp in position){
				if(PositionInBounds(bp) && PositionIsEnemy(bp, player))
					threatened.Add(bp);
			}
			return threatened;
		}


		private List<BoardPosition> PawnPossible(BoardPosition position){
			List<BoardPosition> possible = new List<BoardPosition>();
			int player = GetPlayerAtPosition(position), truePlayer = player;
			if(player == 2)
				player = -1;
			else
				player = 1;
			int r = position.Row+player;
			BoardPosition p1 = new BoardPosition(r, position.Col - 1);
			BoardPosition p2 = new BoardPosition(r, position.Col + 1);
			if(PositionInBounds(p1) && PositionIsEnemy(p1, truePlayer))
				possible.Add(p1);
			if(PositionInBounds(p2) && PositionIsEnemy(p2, truePlayer))
				possible.Add(p2);
			//TODO: implement en passant
			return possible;
		}		
		private List<BoardPosition> PawnThreat(BoardPosition position){
			List<BoardPosition> threatened = new List<BoardPosition>();
			int player = GetPlayerAtPosition(position), truePlayer = player;
			if(player == 2)
				player = -1;
			else
				player = 1;
			int r = position.Row+player;
			BoardPosition p1 = new BoardPosition(r, position.Col - 1);
			BoardPosition p2 = new BoardPosition(r, position.Col + 1);
			if(PositionInBounds(p1) && PositionIsEnemy(p1, truePlayer))
				threatened.Add(p1);
			if(PositionInBounds(p2) && PositionIsEnemy(p2, truePlayer))
				threatened.Add(p2);
			//implement en passant
			return threatened;
		}

		private List<BoardPosition> GetPlayerPieces(int player){
			List<BoardPosition> list = new List<BoardPosition>();
			for(int i = 0; i < 8; i++){
				for(int j = 0; j < 8; j++){
					BoardPosition pos = new BoardPosition(i,j);
					if(GetPlayerAtPosition(pos) == player){
						list.Add(pos);
					}
				}
			}
			return list;
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
			if(0 == GetPlayerAtPosition(pos))
				return true;
			return false;
		}

		/// <summary>
		/// Returns true if the given position contains a piece that is the enemy of the given player.
		/// </summary>
		/// <remarks>returns false if the position is not in bounds</remarks>
		public bool PositionIsEnemy(BoardPosition pos, int player) {
			if(player != GetPlayerAtPosition(pos))
				return true;
			return false;
		}

		/// <summary>
		/// Returns true if the given position is in the bounds of the board.
		/// </summary>
		public static bool PositionInBounds(BoardPosition pos) {
			if(pos.Row >= 0 && pos.Row < 8 && pos.Col >= 0 && pos.Col < 8)
				return true;
			return false;
		}

		/// <summary>
		/// Returns which player has a piece at the given board position, or 0 if it is empty.
		/// </summary>
		public int GetPlayerAtPosition(BoardPosition pos) {
			if(mBoard[pos.Row, pos.Col] > 0)
				return 1;
			else if(mBoard[pos.Row, pos.Col] < 0)
				return 2;
			else
				return 0;
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
			if(pieceType == ChessPieceType.Pawn)
				return 1;
			else if(pieceType == ChessPieceType.Knight || pieceType == ChessPieceType.Bishop)
				return 3;
			else if(pieceType == ChessPieceType.RookKing || pieceType == ChessPieceType.RookQueen)
				return 5;
			else if(pieceType == ChessPieceType.Queen)
				return 9;
			else
				return int.MaxValue;
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
