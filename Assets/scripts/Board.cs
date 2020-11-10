using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    //Eventually want to consider generating a board randomly
    //FOR NOW:  Board.cs acts as the game manager.
    private Dictionary<string, BoardSquare> boardArray;
    private Camera PlayerCam;
    private GameObject PieceSelectorLight;
    private Light PSLtoggle;
    private MoveCaleulator MoveCalc;
    private List<BoardSquare> moveSet; //when we select a piece this fills out
    private int numCapturedPieces; //Use this and multi to place captured pieces.
    private Vector3 gridPosmulti;
    private PlayerController plCont;

    public GameState gameState;
    public PieceFactory pieceFactory;

    public enum GameState
    {
        GAMEOVER, PLAYER_1, PLAYER_2, PAUSED
    };

    public void Start()
    {
        gameState = GameState.PLAYER_1;
        PlayerCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        PieceSelectorLight = GameObject.Find("PieceSelectorLight");
        PSLtoggle = PieceSelectorLight.GetComponentInChildren<Light>();
        boardArray = new Dictionary<string, BoardSquare>(); //Add all the squares to our board, works with manually placed squares too
        MoveCalc = GameObject.FindObjectOfType<MoveCaleulator>();//Only ever one attaced to Pieces prefab
        pieceFactory = GameObject.FindObjectOfType<PieceFactory>();
        plCont = GameObject.FindObjectOfType<PlayerController>();
        InitBoard();
    }

    public void Update()
    {
        if (this.gameState != GameState.GAMEOVER) //will have another enum val for UI paused
        {
            GetMouseInputs();
        }//else if Gamestate.MENU //GetPausedInput()
    }

    private void InitBoard()
    {
        foreach (BoardSquare sq in FindObjectsOfType<BoardSquare>())
        {
            boardArray.Add(sq.name, sq); //We will want to use boardArray to check moves/squares
        }
        List<Piece> whiteArmy, blackArmy;
        whiteArmy = pieceFactory.getPlayerArmy(true);
        blackArmy = pieceFactory.getPlayerArmy(false);
        assignSquares(whiteArmy, true);
        assignSquares(blackArmy, false);

        MoveCalc.updateLocalBoard(boardArray);
    }

    //WARNING! DO NOT ENTER: insane ternary logic below
    private void assignSquares(List<Piece> army, bool isWhite)
    {
        int x = 0;
        int pawnLine = isWhite ? 1 : 6;
        int backLine = isWhite ? 0 : 7;
        foreach(Piece p in army)
        {
            if(p.GetSquare() == null)
            {
                switch (p.GetPieceType())
                {
                    case "PAWN":
                        {
                            p.SetSquare(boardArray[x + "," + pawnLine]);
                            x++;
                            break;
                        }
                    case "ROOK":
                        {
                            if (boardArray[0+","+ backLine].GetPiece() == null) p.SetSquare(boardArray[0 + "," + backLine]);
                            else p.SetSquare(boardArray[7 + "," + backLine]);
                            break;
                        }
                    case "KNIGHT":
                        {
                            if (boardArray[1 + "," + backLine].GetPiece() == null) p.SetSquare(boardArray[1 + "," + backLine]);
                            else p.SetSquare(boardArray[6 + "," + backLine]);
                            break;
                        }
                    case "BISHOP":
                        {
                            if (boardArray[2 + "," + backLine].GetPiece() == null) p.SetSquare(boardArray[2 + "," + backLine]);
                            else p.SetSquare(boardArray[5 + "," + backLine]);
                            break;
                        }
                    case "KING":
                        {
                            p.SetSquare(boardArray[4 + "," + backLine]);
                            break;
                        }
                    case "QUEEN":
                        {
                            p.SetSquare(boardArray[3 + "," + backLine]);
                            break;
                        }
                }              
            }
            p.MatchPosition();
        }

    }

    //Create a ray and a racast from the current camera to collide with a boardsquare.
    //We are using message sent by the Ray to change board state, we have to keep track of the
    //"selected" tile (SelectedSquare) in order to clear the selected flag.
    private void GetMouseInputs()
    {
        Ray ray;
        
        //May change player cam contingent on gameState
        ray = PlayerCam.ScreenPointToRay(Input.mousePosition);
        plCont.PlayerTurn(ray, boardArray, this.gameState);
        this.gameState = plCont.GetNewPlayerState();
    } 
    
}
