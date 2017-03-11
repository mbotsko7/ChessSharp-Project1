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

		private List<BoardPosition> moved = new List<BoardPosition>();
		private BoardPosition pawnMovedTwo = new BoardPosition(-1, -1);
		private bool first;
		// TODO:
		// You need a way of keeping track of certain game state flags. For example, a rook cannot perform a castling move
		// if either the rook or its king has moved in the game, so you need a way of determining whether those things have 
		// happened. There are several ways to do it and I leave it up to you.


		/// <summary>
		/// Constructs a new chess board with the default starting arrangement.
		/// </summary>
		public ChessBoard() {
			MoveHistory = new List<IGameMove>();
			first = true;
			Value = 0;
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
				return first == true ? 1 : 2;
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
			ChessMove m = move as ChessMove;
			//record MoveHistory
			//implement move
			if(m.MoveType == ChessMoveType.Normal && PositionIsEnemy(m.EndPosition, CurrentPlayer)){
				m.Captured = GetPieceAtPosition(m.EndPosition);
				if(m.Captured.Player == 1)
					Value -= GetPieceValue(m.Captured.PieceType);
				else
					Value += GetPieceValue(m.Captured.PieceType);
				SetPosition(m.EndPosition, GetPieceAtPosition(m.StartPosition));
				SetPosition(m.StartPosition, new ChessPiecePosition(ChessPieceType.Empty, 0));
			}
			else if(m.MoveType == ChessMoveType.Normal && PositionIsEmpty(m.EndPosition)){
				SetPosition(m.EndPosition, GetPieceAtPosition(m.StartPosition));
				SetPosition(m.StartPosition, new ChessPiecePosition(ChessPieceType.Empty, 0));
			}
			else if(m.MoveType == ChessMoveType.EnPassant){
				m.Captured = GetPieceAtPosition(pawnMovedTwo);
				SetPosition(m.EndPosition, GetPieceAtPosition(m.StartPosition));
				SetPosition(m.StartPosition, new ChessPiecePosition(ChessPieceType.Empty, 0));

				//mBoard[pawnMovedTwo.Row, pawnMovedTwo.Col] = 0;
			}
			else if(m.MoveType == ChessMoveType.CastleKingSide){
				SetPosition(m.EndPosition, m.Piece);
				BoardPosition kRook = m.EndPosition;
				//kRook.Col
				//SetPosition(new )
			}
			first = !first;
			MoveHistory.Add(m);
			
		}

		public IEnumerable<IGameMove> GetPossibleMoves() {
			List<BoardPosition> positions = GetPlayerPieces(CurrentPlayer);
			List<ChessMove> possible = new List<ChessMove>();
			foreach(BoardPosition position in positions){
				ChessPiecePosition cpos = GetPieceAtPosition(position);
				if(cpos.PieceType == ChessPieceType.Pawn){
					List<ChessMove> temp = PawnPossible(position);//convertMoveList(position, PawnPossible(position));
					possible.AddRange(temp);
					
				}
				else if(cpos.PieceType == ChessPieceType.Knight){
					possible.AddRange(KnightPossible(position));//convertMoveList(position, KnightPossible(position)));
				}
				else if(cpos.PieceType == ChessPieceType.Bishop){
					possible.AddRange(BishopPossible(position));//convertMoveList(position, BishopPossible(position)));
				}
				else if(cpos.PieceType == ChessPieceType.RookKing ||
					cpos.PieceType == ChessPieceType.RookPawn ||
					cpos.PieceType == ChessPieceType.RookQueen){
					possible.AddRange(RookPossible(position));//convertMoveList(position, RookPossible(position)));
					}
				else if(cpos.PieceType == ChessPieceType.Queen){
					possible.AddRange(QueenPossible(position));//convertMoveList(position, QueenPossible(position)));
				}
				else
					possible.AddRange(KingPossible(position));//convertMoveList(position, KingPossible(position)));
			}
			//possible = EndangerKing(possible, findKing());
			return possible;
			
		}
		private BoardPosition findKing(){
			int king = CurrentPlayer == 1 ? 7 : -7;
			
			int kRow = -1, kCol = -1;
			for(int r = 0; r < 8; r++){
				for(int c = 0; c < 8; c++){
					if(mBoard[r, c] == king){
						kRow = r;
						kCol = c;
						
					}
				}
			}
			return new BoardPosition(kRow, kCol);
		}

		private List<ChessMove> EndangerKing(List<ChessMove> m, BoardPosition kp){
			int current = CurrentPlayer;
			for(int i = 0; i < m.Count; i++){
				ApplyMove(m[i]);
				var threat = (CurrentPlayer == 1 ? GetThreatenedPositions(1) : GetThreatenedPositions(2)) as List<BoardPosition>;
				foreach(BoardPosition move in threat){

					if(move.Equals(kp)){
						m.RemoveAt(i);
						i--;
						break;
					}
				}
				UndoLastMove();
				
			}
			return m;

		}

		private List<BoardPosition> convertMoveList(List<ChessMove> l){
			List<BoardPosition> ret = new List<BoardPosition>();
			foreach(ChessMove pos in l){
				ret.Add(pos.EndPosition);
			}
			return ret;
		}
		/// <summary>
		/// Gets a sequence of all positions on the board that are threatened by the given player. A king
		/// may not move to a square threatened by the opponent.
		/// </summary>
		public IEnumerable<BoardPosition> GetThreatenedPositions(int byPlayer) {
			// TODO: implement this method. Make sure to account for "special" moves.
			List<BoardPosition> positions = GetPlayerPieces(byPlayer);
			List<ChessMove> threatened = new List<ChessMove>();
			List<BoardPosition> actuallyThreatened = new List<BoardPosition>();
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
					threatened.AddRange(QueenThreat(position));
				}
				else
					threatened.AddRange(KingThreat(position));
			}
			return convertMoveList(threatened);
		}
		private bool check(){
			int king = CurrentPlayer == 1 ? 7 : -7;
			int kRow = -1, kCol = -1;
			for(int r = 0; r < 8; r++){
				for(int c = 0; c < 8; c++){
					if(mBoard[r, c] == king){
						kRow = r;
						kCol = c;
						goto Escape;
					}
				}
			}
			Escape:
			List<BoardPosition> threat = (CurrentPlayer == 1 ? GetThreatenedPositions(2) : GetThreatenedPositions(1)) as List<BoardPosition>;
			UndoLastMove();
			return threat.Contains(new BoardPosition(kRow, kCol));
		}
		public bool IsCheckmate{
			get {return checkmate();}
		}

		public bool IsCheck{
			get{return check();}
		}
		private bool checkmate(){
			return true;
		}

		public bool IsStalemate{
			get{return false;}
		}
		
		private bool PassantPossible(){
			ChessMove cm = MoveHistory[MoveHistory.Count-1] as ChessMove;
			return cm.ForwardTwice;
		}
		private bool hasMoved(BoardPosition bp){
			foreach(ChessMove m  in MoveHistory){
				if(m.EndPosition.Equals(bp) && m.pieceval.Equals(mBoard[bp.Row, bp.Col])){
					return true;
				}
			}
			return false;
		}
		private void Castle(BoardPosition king, ref List<BoardPosition> l){
			BoardPosition queenRook, kingRook;
			if(first){
				queenRook = new BoardPosition(7,0);
				kingRook = new BoardPosition(7,7);
			}
			else{
				queenRook = new BoardPosition(0,0);
				kingRook = new BoardPosition(0, 7);
			}
			int player = first ? 2 : 1;
			List<BoardPosition> threat = GetThreatenedPositions(player) as List<BoardPosition>;
			if(!moved.Contains(queenRook) && !threat.Contains(king) && !moved.Contains(king)){

			}

		}
		private List<ChessMove> KingPossible(BoardPosition pos){
			//List<BoardPosition> possible = new List<BoardPosition>();
			var possible = KingThreat(pos);
			for(int i = 0; i < possible.Count; i++){
				if(!PositionIsEnemy(possible[i].EndPosition, CurrentPlayer) && !PositionIsEmpty(possible[i].EndPosition)){
					possible.RemoveAt(i);
					i--;
				}
			}
			return possible;
		}
		private List<ChessMove> KingThreat(BoardPosition pos){

			//implement king rules on checking and threats
			List<ChessMove> threatened = new List<ChessMove>();
			int player = GetPlayerAtPosition(pos);
			for(int r = pos.Row-1; r < pos.Row+2; r++){
				for(int c = pos.Col-1; c < pos.Col+2;  c++){
					if(r == 0 && c == 0)
						continue;
					else{
						BoardPosition bp = new BoardPosition(r,c);
						if(PositionInBounds(bp)){
							threatened.Add(new ChessMove(pos, bp));
						}

						
					}
				}
			}
			return threatened;
		}

		private List<ChessMove> RookPossible(BoardPosition pos){
			//List<BoardPosition> possible = new List<BoardPosition>();
			var possible = RookThreat(pos);
			for(int i = 0; i < possible.Count; i++){
				if(!PositionIsEnemy(possible[i].EndPosition, CurrentPlayer) && !PositionIsEmpty(possible[i].EndPosition)){
					possible.RemoveAt(i);
					i--;
				}
			}
			
			return possible;

		}
		

		private List<ChessMove> QueenPossible(BoardPosition pos){
			//List<BoardPosition> possible = new List<BoardPosition>();
			var possible = QueenThreat(pos);
			for(int i = 0; i < possible.Count; i++){
				if(!PositionIsEnemy(possible[i].EndPosition, CurrentPlayer) && !PositionIsEmpty(possible[i].EndPosition)){
					possible.RemoveAt(i);
					i--;
				}
			}
			
			return possible;
		}

		private List<ChessMove> QueenThreat(BoardPosition pos){
			List<ChessMove> threatened = new List<ChessMove>();
			threatened.AddRange(RookThreat(pos));
			threatened.AddRange(BishopThreat(pos));
			return threatened;

		}
		private List<ChessMove> RookThreat(BoardPosition pos){
			List<ChessMove> threatened = new List<ChessMove>();
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
						threatened.Add(new ChessMove(pos, bp));
						continue;
					}
					else {
						threatened.Add(new ChessMove(pos, bp));
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
					else if(PositionIsEmpty(bp)){
						threatened.Add(new ChessMove(pos, bp));
						continue;
					}
					else {
						threatened.Add(new ChessMove(pos, bp));
						break;
					}


				}
			}
			return threatened;
		}

		private List<ChessMove> BishopPossible(BoardPosition pos){
			//List<BoardPosition> possible = new List<BoardPosition>();
			var possible = BishopThreat(pos);
			for(int i = 0; i < possible.Count; i++){
				if(!PositionIsEnemy(possible[i].EndPosition, CurrentPlayer) && !PositionIsEmpty(possible[i].EndPosition)){
					possible.RemoveAt(i);
					i--;
				}
			}
			
			return possible;
		}

		private List<ChessMove> BishopThreat(BoardPosition pos){


			List<ChessMove> threatened = new List<ChessMove>();
			int player = GetPlayerAtPosition(pos);
			for(int r = -1; r < 2; r += 2){
				for(int c = -1; c < 2; c += 2){
					BoardPosition bp = pos;
					bool bounds = true;
					while(bounds){
						bp.Row += r;
						bp.Col += c;
						bounds = PositionInBounds(bp);
						if(!bounds) //end if out of bounds
							break;
						else if(PositionIsEmpty(bp)){ //add and continue if empty
							threatened.Add(new ChessMove(pos, bp));
							continue;
						}
						else{
							threatened.Add(new ChessMove(pos, bp)); //add if hit piece and done
							break;
						}

					}
				}
			}
			return threatened;
		}

		private List<ChessMove> KnightPossible(BoardPosition pos){
			//List<BoardPosition> possible = new List<BoardPosition>();
			var possible = KnightThreat(pos);
			for(int i = 0; i < possible.Count; i++){
				if(!PositionIsEnemy(possible[i].EndPosition, CurrentPlayer) && !PositionIsEmpty(possible[i].EndPosition)){

					possible.RemoveAt(i);
					i--;
				}
			}
			
			return possible;
		}
		private List<ChessMove> KnightThreat(BoardPosition pos){
			List<ChessMove> threatened = new List<ChessMove>();
			int player = GetPlayerAtPosition(pos);
			List<ChessMove> position = new List<ChessMove>(){
				new ChessMove(pos, new BoardPosition(pos.Row+2, pos.Col-1)),
				new ChessMove(pos, new BoardPosition(pos.Row+2, pos.Col+1)),

				new ChessMove(pos, new BoardPosition(pos.Row-2, pos.Col+1)),
				new ChessMove(pos, new BoardPosition(pos.Row-2, pos.Col-1)),

				new ChessMove(pos, new BoardPosition(pos.Row-1, pos.Col+2)),
				new ChessMove(pos, new BoardPosition(pos.Row+1, pos.Col+2)),

				new ChessMove(pos, new BoardPosition(pos.Row-1, pos.Col-2)),
				new ChessMove(pos, new BoardPosition(pos.Row+1, pos.Col-2))};
			foreach(ChessMove bp in position){
				if(PositionInBounds(bp.EndPosition))
					threatened.Add(bp);
			}
			return threatened;
		}

		public IEnumerable<BoardPosition> GetPositionsOfPiece(ChessPieceType piece, int player) {
			List<BoardPosition> temp = new List<BoardPosition>();
			for(int r = 0; r < 8; r++){
				for(int c = 0; c < 8; c++){
					if(mBoard[r,c] == GetPieceValue(piece) && GetPlayerAtPosition(new BoardPosition(r,c)) == player){
						temp.Add(new BoardPosition(r,c));
					}
				}
			}
			return temp;
		}
		private List<ChessMove> PawnPossible(BoardPosition position){
			List<ChessMove> possible = PawnThreat(position);
			int player = GetPlayerAtPosition(position), truePlayer = player;
			if(player == 2)
				player = 1;
			else
				player = -1;
			int r = position.Row+player;

			for(int i = 0; i < possible.Count; i++){
				ChessMove m = possible[i];
				if(m.MoveType == ChessMoveType.Normal){
					if(!PositionIsEnemy(m.EndPosition, CurrentPlayer)){
						possible.RemoveAt(i);
						i--;
					}
				}
				else{
					
				}

				
			}
			BoardPosition forward = new BoardPosition(r, position.Col);
			BoardPosition forward2 = new BoardPosition(player+r, position.Col);
			if(PositionInBounds(forward))
				possible.Add(new ChessMove(position, forward, mBoard[position.Row, position.Col]));
			if(PositionInBounds(forward2) && hasMoved(position) == false){
				ChessMove cm = new ChessMove(position, forward2, mBoard[position.Row, position.Col]);
				cm.ForwardTwice = true;
				possible.Add(cm);
			}
			
			return possible;
		}		
		private List<ChessMove> PawnThreat(BoardPosition position){


			List<ChessMove> threatened = new List<ChessMove>();
			int player = GetPlayerAtPosition(position), truePlayer = player;
			if(player == 2)
				player = 1;
			else
				player = -1;
			int r = position.Row+player;
			BoardPosition forwardLeft = new BoardPosition(r, position.Col - 1);
			BoardPosition forwardRight = new BoardPosition(r, position.Col + 1);


			if(PositionInBounds(forwardLeft))
				threatened.Add(new ChessMove(position, forwardLeft, mBoard[position.Row, position.Col]));
			if(PositionInBounds(forwardRight))
				threatened.Add(new ChessMove(position, forwardRight, mBoard[position.Row, position.Col]));
			
			// object passant = isPassant(position);
			// if(passant != null){
			// 	ChessMove en = new ChessMove(position, (BoardPosition) passant);
			// 	en.MoveType = ChessMoveType.EnPassant;
			// }
			return threatened;
		}

		private object isPassant(BoardPosition position){
			BoardPosition left = new BoardPosition(position.Row, position.Col-1);
			BoardPosition right = new BoardPosition(position.Row, position.Col+1);			
			int player = GetPlayerAtPosition(position), truePlayer = player;
			if(player == 2)
				player = 1;
			else
				player = -1;
			int r = position.Row+player;
			if(PositionInBounds(left) && pawnMovedTwo.Equals(left)){
				ChessMove m3 = new ChessMove(position, new BoardPosition(r, position.Col-1));
				m3.MoveType = ChessMoveType.EnPassant;
				//m3.Captured = 
				return left;
			}
			else if(PositionInBounds(right) && pawnMovedTwo.Equals(right))
				return right;
			return null;
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
			if(MoveHistory.Count == 0){
				return;
			}
			ChessMove m = MoveHistory[MoveHistory.Count-1] as ChessMove;
			//change the player
			
			int player = first ? 1:-1;
			if(m.MoveType == ChessMoveType.Normal){
				//first move the position that moved
				SetPosition(m.StartPosition, GetPieceAtPosition(m.EndPosition));
				//then restore a piece if Captured
				if(!m.Captured.Equals(null)){
					SetPosition(m.EndPosition, m.Captured);
					if(m.Captured.Player == 1)
						Value += GetPieceValue(m.Captured.PieceType);
					else
						Value -= GetPieceValue(m.Captured.PieceType);
				}
			}
			else if(m.MoveType == ChessMoveType.EnPassant){
				SetPosition(m.StartPosition, m.Piece);
				BoardPosition bp = m.EndPosition;
				bp.Row += player;
				SetPosition(bp, m.Captured);
			}
			else if(m.MoveType == ChessMoveType.CastleKingSide){
				SetPosition(m.StartPosition, m.Piece);
				int r =  first ? 7 : 0;
				mBoard[r,5] = 0;
				mBoard[r,7] = first ? (sbyte)3 : (sbyte)-3;
				
				
			}
			else if(m.MoveType == ChessMoveType.CastleQueenSide){
				SetPosition(m.StartPosition, m.Piece);
				int r =  first ? 7 : 0;
				mBoard[r,3] = 0;
				mBoard[r,0] = first ? (sbyte)3 : (sbyte)-3;
				
				
			}
			else{
				SetPosition(m.EndPosition, m.Captured);
				// ChessMove m2 = MoveHistory[MoveHistory.Count-2] as ChessMove;
				// SetPosition(m2.StartPosition, m2.Piece);
				// mBoard[m2.EndPosition.Row, m2.EndPosition.Col];
			}
			MoveHistory.RemoveAt(MoveHistory.Count-1);
			first = !first;

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
			if(player != GetPlayerAtPosition(pos) && GetPlayerAtPosition(pos) != 0)
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
			else if(pieceType == ChessPieceType.King)
				return int.MaxValue;
			else
				return 0;
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
