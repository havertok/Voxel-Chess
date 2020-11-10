using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] string Type;
    [SerializeField] public bool IsWhite; //Use this to calc forward x=1 for white x=-1 for black
    [SerializeField] BoardSquare mySquare;
    private Vector2Int position;
    private List<Vector2Int> moveDirections;
    private int moveDistance;
    public bool hasMoved; //really only used for rooks and kings for castling (and pawns)


    public void Start()
    {
        moveDirections = new List<Vector2Int>();
        GenMoveOffset();
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
    //This NOT only moves the peice but also matches the assigned square.
    //Set square for piece then matchPos() to keep things organized
    public void MatchPosition()
    {
        this.transform.position = this.mySquare.transform.position;
        this.mySquare.SetPiece(this);
    }

    public bool isWhite()
    {
        return this.IsWhite;
    }

    public string GetPieceType()
    {
        return this.Type;
    }

    //Now generates and stores a list of direction offsets
    //Always start at N or NW and moves clockwise
    //Could be moved to movecalc helper class
    public void GenMoveOffset()
    {
        switch (Type)
        {
            case "PAWN":
                {
                    //Forward for white and black are opposites
                    moveDirections.Add(new Vector2Int(0, this.IsWhite ? 1 : -1));
                    moveDirections.Add(new Vector2Int(1, this.IsWhite ? 1 : -1));
                    moveDirections.Add(new Vector2Int(-1, this.IsWhite ? 1 : -1));
                    moveDistance = 2; //we will limit this in movecalc
                    break;
                }
            case "BISHOP":
                {
                    moveDirections.Add(new Vector2Int(1, 1));
                    moveDirections.Add(new Vector2Int(-1, 1));
                    moveDirections.Add(new Vector2Int(-1, -1));
                    moveDirections.Add(new Vector2Int(1, -1));
                    moveDistance = 8;//Maybe change this?
                    break;
                }
            case "KING":
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
            case "KNIGHT":
                {
                    moveDirections.Add(new Vector2Int(1, 2));
                    moveDirections.Add(new Vector2Int(2, 1));
                    moveDirections.Add(new Vector2Int(2, -1));
                    moveDirections.Add(new Vector2Int(1, -2));
                    moveDirections.Add(new Vector2Int(-1, -2));
                    moveDirections.Add(new Vector2Int(-2, -1));
                    moveDirections.Add(new Vector2Int(-2, 1));
                    moveDirections.Add(new Vector2Int(-1, 2));
                    moveDistance = 1;
                    break;
                }
            case "QUEEN":
                {
                    moveDirections.Add(new Vector2Int(1, 1));
                    moveDirections.Add(new Vector2Int(-1, 1));
                    moveDirections.Add(new Vector2Int(-1, -1));
                    moveDirections.Add(new Vector2Int(1, -1));
                    moveDirections.Add(new Vector2Int(0, 1));
                    moveDirections.Add(new Vector2Int(1, 0));
                    moveDirections.Add(new Vector2Int(0, -1));
                    moveDirections.Add(new Vector2Int(-1, 0));
                    moveDistance = 8;
                    break;
                }
            case "ROOK":
                {
                    moveDirections.Add(new Vector2Int(0, 1));
                    moveDirections.Add(new Vector2Int(1, 0));
                    moveDirections.Add(new Vector2Int(0, -1));
                    moveDirections.Add(new Vector2Int(-1, 0));
                    moveDistance = 8;
                    break;
                }
            case "CUSTOM":
                moveDirections.Add(new Vector2Int(0, this.IsWhite ? 1 : -1));
                moveDirections.Add(new Vector2Int(1, this.IsWhite ? 1 : -1));
                moveDirections.Add(new Vector2Int(-1, this.IsWhite ? 1 : -1));
                moveDistance = 8;
                break;
            default: break;
        }


    }

    public List<Vector2Int> GetMoveOffest()
    {
        return this.moveDirections;
    }

    public int GetMoveDistance()
    {
        return this.moveDistance;
    }

}
