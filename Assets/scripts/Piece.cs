using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Moveset
{
    PAWN, ROOK, KNIGHT, BISHOP, QUEEN, KING
};

public class Piece : MonoBehaviour
{
    [SerializeField] Moveset myMoveset;
    [SerializeField] bool isWhite;
    [SerializeField] BoardSquare mySquare;
    private Vector2Int position;
    private List<Vector2Int> moveDirections;
    private int moveDistance;

    public void Start()
    {
        moveDirections = new List<Vector2Int>();
        GenRawMoves();
    }

    public BoardSquare GetSquare()
    {
        return this.mySquare;
    }

    public void SetSquare(BoardSquare b)
    {
        this.mySquare = b;
    }

    //matchPosition is called by Board to setup the board
    //instantly resets it's position to match it's assigned BoardSquare
    public void MatchPosition()
    {
        this.transform.position = this.mySquare.transform.position;
        this.mySquare.SetPiece(this);
    }

    public bool validateMove(BoardSquare toSq)
    {
        if (toSq.IsOccupied())
        {
            return false;//need to check occping piece
        }
        return true;
    }

    //Now generates and stores a list of direction offsets
    //Always start at N or NW and moves clockwise
    public void GenRawMoves()
    {
        switch (myMoveset)
        {
            case Moveset.PAWN:
                {
                    moveDirections.Add(new Vector2Int(1,0));//Board will handle first turn exceptions
                    moveDistance = 1;
                    break;
                }
            case Moveset.BISHOP:
                {
                    moveDirections.Add(new Vector2Int(1, 1));
                    moveDirections.Add(new Vector2Int(-1, 1));
                    moveDirections.Add(new Vector2Int(-1, -1));
                    moveDirections.Add(new Vector2Int(-1, 1));
                    moveDistance = 8;//Maybe change this?
                    break;
                }
            case Moveset.KING:
                {
                    moveDirections.Add(new Vector2Int(1, 0));
                    moveDirections.Add(new Vector2Int(1, 1));
                    moveDirections.Add(new Vector2Int(0, 1));
                    moveDirections.Add(new Vector2Int(-1, 1));
                    moveDirections.Add(new Vector2Int(-1, 0));
                    moveDirections.Add(new Vector2Int(-1, -1));
                    moveDirections.Add(new Vector2Int(0, -1));
                    moveDirections.Add(new Vector2Int(1, -1));
                    moveDistance = 1;
                    break;
                }
            default: break;
        }


    }

    public List<Vector2Int> GetMoveDirection()
    {
        return this.moveDirections;
    }

    public int GetMoveDistance()
    {
        return this.moveDistance;
    }

}
