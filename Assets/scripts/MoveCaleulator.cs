using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCaleulator : MonoBehaviour
{
    private Dictionary<string, BoardSquare> BoardCopy;
    //String is the "0,0" name of boardsquare
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
    0001_0000 0000
     **/

    public void InitStatus(Dictionary<string, BoardSquare> boardArray)
    {
        //First pass sets status to occupied for respective pieces:
        foreach (BoardSquare sq in boardArray.Values)
        {
            
        }
        //Second pass uses utility function to set status for squars (threatened, covered, etc)

    }

    public void updateLocalBoard(Dictionary<string, BoardSquare> board)
    {
        BoardCopy = board;
    }

    public List<BoardSquare> GetValidMoves(Piece piece)
    {
        List<BoardSquare> possibleMoves = new List<BoardSquare>();
        BoardSquare bsq;
        Vector2Int temp = piece.GetSquare().GetGridPos();
        string tempName = "";
        foreach(Vector2Int v in piece.GetMoveOffest())
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
        return possibleMoves;//if piece is king and this is NULL checkmate
    }

    //BEGIN CHAIN HERE
    //Pass possibleMoves down to various checkers for blocking, castling, etc.

}
