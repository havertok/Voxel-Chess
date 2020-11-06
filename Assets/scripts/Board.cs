using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    //Eventually want to consider generating a board randomly
    //FOR NOW:  Board.cs acts as the game manager.
    private Dictionary<string, BoardSquare> boardArray;

    private Piece SelectedPiece;
    private BoardSquare SelectedSquare;
    private Camera PlayerCam;


    private GameObject PieceSelectorLight;
    private Light PSLtoggle;
    private MoveCaleulator MoveCalc;
    private List<BoardSquare> moveSet; //when we select a piece this fills out
    private int numCapturedPieces; //Use this and multi to place captured pieces.
    private Vector3 gridPosmulti;


    public GameState gameState;
    private PlayerController plCont;

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
        //Used to programattically place pieces after other functionality is done
        //TODO Generate/instance pieces in Pieces<PieceFactory>
        foreach (Piece p in FindObjectsOfType<Piece>())
        {
            if(p.GetSquare() != null)
            {
                p.MatchPosition();
            }
        }
        MoveCalc.updateLocalBoard(boardArray);
    }

    //TODO maybe move this to tile selector once I get it working
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
