using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceFactory : MonoBehaviour
{
    //1 means standard, may support other gamemodes later
    [SerializeField] int gameType;
    [SerializeField] Transform pieceParentTransform;
    [SerializeField] Piece pKing, pPawn, pKnight, pQueen, pRook, pBishop,
                           pKingb, pPawnb, pKnightb, pQueenb, pRookb, pBishopb;

    private GameObject piece;

    public List<Piece> getPlayerArmy(bool isWhite)
    {

        if (gameType == 1) return genRegularArmy(isWhite);

        return null;
    }

    private List<Piece> genRegularArmy(bool isWhite)
    {
        Piece p;
        Quaternion q = Quaternion.identity;
        List<Piece> playerPieceList = new List<Piece>();
        //If the piece is black, rotate it on y 180 so it faces "forward"
        if (!isWhite)
        {
            q = Quaternion.Euler(0, 180, 0);
        }
        //Piece instantiators.
        for(int i = 0; i < 8; i++)
        {
            p = Instantiate(isWhite ? pPawn : pPawnb, new Vector3Int(0, 0, 0), q);
            p.transform.parent = pieceParentTransform;
            playerPieceList.Add(p);
        }
        for (int i = 0; i < 2; i++)
        {
            p = Instantiate(isWhite ? pRook : pRookb, new Vector3Int(0, 0, 0), q);
            p.transform.parent = pieceParentTransform;
            playerPieceList.Add(p);
        }
        for (int i = 0; i < 2; i++)
        {
            p = Instantiate(isWhite ? pKnight : pKnightb, new Vector3Int(0, 0, 0), q);
            p.transform.parent = pieceParentTransform;
            playerPieceList.Add(p);
        }
        for (int i = 0; i < 2; i++)
        {
            p = Instantiate(isWhite ? pBishop : pBishopb, new Vector3Int(0, 0, 0), q);
            p.transform.parent = pieceParentTransform;
            playerPieceList.Add(p);
        }
        p = Instantiate(isWhite ? pQueen : pQueenb, new Vector3Int(0, 0, 0), q);
        p.transform.parent = pieceParentTransform;
        playerPieceList.Add(p);
        p = Instantiate(isWhite ? pKing : pKingb, new Vector3Int(0, 0, 0), q);
        p.transform.parent = pieceParentTransform;
        playerPieceList.Add(p);

        return playerPieceList;
    }

}
