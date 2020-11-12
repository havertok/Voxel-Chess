using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCaleulator : MonoBehaviour
{
    private Dictionary<string, BoardSquare> BoardCopy;
    //String is the "0,0" name of boardsquare [Zobrist_Hashing]
    private Dictionary<string, int> SquareStatus;
    //We must always know where the kings are
    private BoardSquare WhiteKing; 
    private BoardSquare BlackKing;

    //Denotes an occupied square, used in move calcs for blocking
    private int Occupied = 0b0000_0001;
    //Is the square occupied by white or black(w=0 b=1)
    private int OccWOB = 0b0000_0010;
    //This space is covered by another piece, kings CANNOT move into covered spaces, blocked by all but king
    private int Covered = 0b0000_0100;
    //white or black for covered (w=0 b=1)
    private int CWOB = 0b0000_1000;

    /*
    Threatened Mask (Refers to "Raw" line for sliders, so a king checked by rook can't move back) Kings can't move into threatened spaces
    0001 0000
    WoB
    0010 0000
    isChecker 
    0100 0000
    special pawncheck (for en passant if needed)0000_0000 0000
    This should be set on a square with a peice that just moved, pawnMoveLimiter should check
    square of same name in SquareStatus (To be implemented) 1_0000 0000 means can capture
    0001_0000 0000
     **/

    public void InitStatus(Dictionary<string, BoardSquare> boardArray)
    {
        this.BoardCopy = boardArray;
        List<Piece> allPieces = new List<Piece>();
        foreach (BoardSquare sq in boardArray.Values)
        {
            if (sq.GetPiece() != null) allPieces.Add(sq.GetPiece());
        }
        foreach(Piece p in allPieces)
        {
            GetValidMoves(p); //Part of getting valid moves is setting potential boad sq covered states
        }

    }

    //Called after a move has been made and the OLD moveset is cleared.
    //If peice WAS blocked and now isn't it now covers squares it did not before.
    public void updateLocalBoard()
    {
        List<Piece> allPieces = new List<Piece>();
        foreach (BoardSquare sq in BoardCopy.Values)
        {
            if (sq.GetPiece() != null) allPieces.Add(sq.GetPiece());
        }
        foreach (Piece p in allPieces)
        {
            //Part of getting valid moves is setting potential boad sq covered states
            List<BoardSquare> moves = GetValidMoves(p); 
            if(p.GetPieceType() == "PAWN")
            {
                pawnCoverageStatus(moves, p);
            }
        }
        specialKingCheck(allPieces);
    }

    public List<BoardSquare> GetValidMoves(Piece piece)
    {
        List<BoardSquare> possibleMoves = new List<BoardSquare>();
        BoardSquare bsq;
        Vector2Int temp = piece.GetSquare().GetGridPos();
        string tempName = "";
        foreach (Vector2Int v in piece.GetMoveOffest())
        {
            temp = piece.GetSquare().GetGridPos();
            for (int i = 0; i < piece.GetMoveDistance(); i++)
            {
                temp.x = temp.x + v.x;
                temp.y = temp.y + v.y;
                if (temp.x < 0 || temp.y < 0) break; //Let's not waste time if negative
                tempName = temp.x + "," + temp.y;
                if(BoardCopy.TryGetValue(tempName, out bsq))
                {
                    if(bsq.GetPiece() == null) //Square has no piece we are good
                    {
                        possibleMoves.Add(bsq);
                    }
                    else if(piece.isWhite() != bsq.GetPiece().isWhite()) //Piece is dif color good to add
                    {
                        possibleMoves.Add(bsq);
                        break; //but only that peice can be added (if knight it can jump only once)
                    }
                    else //There's a piece AND the same color, not a valid move
                    {
                        break;
                    }
                }
            }
        }

        //PASS possibleMoves into chain of qualifying functions
        //Objects like lists are passed by reference, not sure I need to set pmoves here
        if (piece.GetPieceType() == "PAWN") possibleMoves = pawnMoveLimiter(possibleMoves, piece);
        if (piece.GetPieceType() == "KING") possibleMoves = kingMoveLimiter(possibleMoves, piece);

        updateSquareStatus(possibleMoves, piece);
        return possibleMoves;
    }

    //Pass possibleMoves down to various checkers for blocking, castling, etc.
    //Somehow add en passant rule here.
    private List<BoardSquare> pawnMoveLimiter(List<BoardSquare> moveSet, Piece piece)
    {
        Vector2Int temp = piece.GetSquare().GetGridPos();
        string tempName = temp.x+","+temp.y;
        List <BoardSquare> removalList = new List<BoardSquare>();
        foreach(BoardSquare bsq in moveSet)
        {
            Vector2Int bsqPos = bsq.GetGridPos();
            //remove 2 len diagonal and 2 forward if pawn has moved
            //if(Mathf.Abs(temp.x - bsqPos.x) > 1) removalList.Add(bsq);
            if(piece.hasMoved && Mathf.Abs(temp.y - bsqPos.y) == 2) removalList.Add(bsq);
            //in the future, we will check board status here isntead of this ugly thing
            if(BoardCopy[bsq.name].GetPiece() != null)
            {
                if (Mathf.Abs(temp.x - bsqPos.x) == 0) removalList.Add(bsq);
            }
            else if(Mathf.Abs(temp.x - bsqPos.x) > 0)
            {
                removalList.Add(bsq);
            }
        }
        //Removes evertyhing in moveset that we need to using super legible lembda
        moveSet.RemoveAll(item => removalList.Contains(item));
        return moveSet;
    }

    private List<BoardSquare> kingMoveLimiter(List<BoardSquare> moveSet, Piece piece)
    {
        List<BoardSquare> removalList = new List<BoardSquare>();
        foreach (BoardSquare bsq in moveSet)
        {
            if (piece.isWhite() && bsq.coveredByBlack())
            {
                removalList.Add(bsq);
            }
            if(!piece.isWhite() && bsq.coveredByWhite())
            {
                removalList.Add(bsq);
            }
        }
        moveSet.RemoveAll(item => removalList.Contains(item));
        return moveSet;
    }

    //If we iterate through all pieces at game start and update boardstatus we should only
    //have to call this after a move
    private void updateSquareStatus(List<BoardSquare> newMoves, Piece thisPiece)
    {
        foreach(BoardSquare sq in newMoves)
        {
            if (thisPiece.isWhite())
            {
                sq.setCoveredByWhite(true);
            }
            else
            {
                sq.setCoveredByBlack(true);
            }
        }
    }

    //TODO special PAWN coverage status updater here, since pawns capture rules are different
    private void pawnCoverageStatus(List<BoardSquare> newMoves, Piece thisPiece)
    {
        
    }

    //Another bug, check was in line 60, but since Lists are orderd randomly, the king would sometimes
    //check for coverage status BEFORE another piece, so a square would be correctly flagged as covered
    //by an opposing piece (and removed by the kingMoveLimiter) but not trigger check message
    private void specialKingCheck(List<Piece> allPieces)
    {
        foreach(Piece p in allPieces)
        {
            if (p.GetPieceType() == "KING") isKingInCheck(GetValidMoves(p), p);
        }
    }

    //ugly
    private void isKingInCheck(List<BoardSquare> myMoves, Piece king)
    {
        PlayerController playerCont = FindObjectOfType<Board>().GetComponent<PlayerController>();
        if (king.isWhite() && king.GetSquare().coveredByBlack())
        {
            playerCont.SendMessage("kingInCheck"); //will send specific to piece color
            if(myMoves == null) //need to check for blocking piece
            {
                print("CheckMate");
            }
        }
        if (!king.isWhite() && king.GetSquare().coveredByWhite())
        {
            playerCont.SendMessage("kingInCheck"); //will send specific to piece color
            if (myMoves == null)
            {
                playerCont.SendMessage("kingInCheck");
            }
        }
    }

}
