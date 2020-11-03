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
    public GameState gameState;

    public enum GameState
    {
        GAMEOVER, PLAYER_1, PLAYER_2
    };

    public void Start()
    {
        gameState = GameState.PLAYER_1;
        PlayerCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        PieceSelectorLight = GameObject.Find("PieceSelectorLight");
        PSLtoggle = PieceSelectorLight.GetComponentInChildren<Light>();
        boardArray = new Dictionary<string, BoardSquare>(); //Add all the squares to our board, works with manually placed squares too
        InitBoard();
    }

    public void Update()
    {
        GetMouseInputs();
    }

    private void InitBoard()
    {
        foreach (BoardSquare sq in FindObjectsOfType<BoardSquare>())
        {
            boardArray.Add(sq.name, sq); //We will want to use boardArray to check moves/squares
            //print(sq.name);
        }
        //Used to programattically place pieces after other functionality is done
        foreach (Piece p in FindObjectsOfType<Piece>())
        {
            if(p.GetSquare() != null)
            {
                p.MatchPosition();
            }
        }
    }

    //TODO maybe move this to tile selector once I get it working
    //Create a ray and a racast from the current camera to collide with a boardsquare.
    //We are using message sent by the Ray to change board state, we have to keep track of the
    //"selected" tile (SelectedSquare) in order to clear the selected flag.
    private void GetMouseInputs()
    {
        Ray ray;
        RaycastHit hitinfo;
        GameObject obj;
        BoardSquare bsq;

        if (this.gameState != GameState.GAMEOVER)
        {
            ray = PlayerCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hitinfo, Mathf.Infinity))
            {
                obj = hitinfo.collider.gameObject;
                if(boardArray.TryGetValue(obj.name, out bsq))
                {
                    if (!SelectedSquare)
                    {
                        SelectedSquare = bsq;
                        bsq.SelectSquare();
                    }
                    if (SelectedSquare && SelectedSquare != bsq)
                    {
                        bsq.SelectSquare();
                        SelectedSquare.DeselectSquare();
                        SelectedSquare = bsq;
                    }
                }
                else if(SelectedSquare)
                {
                    SelectedSquare.DeselectSquare();
                    SelectedSquare = null;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    PieceSelector(hitinfo);
                }
            }
        }
    }

    //IF MOUSECLICK THEN set SelectePiece as above, check if null (if not null then move to raycasted square)
    private void PieceSelector(RaycastHit hitinfo)
    {
        if(hitinfo.collider == null) //Allows deslselection by clicking empty space
        {
            SelectedPiece = null;
            PSLtoggle.enabled = false;
        }
        else if(SelectedPiece == null)
        {
            PieceSelectorLight.transform.position = hitinfo.transform.position;
            PSLtoggle.enabled = true;
            SelectedPiece = hitinfo.collider.GetComponent<Piece>();
        }
        else
        {
            //check moves here
            SelectedPiece.GetSquare().SetPiece(null);
            SelectedPiece.SetSquare(SelectedSquare);
            SelectedPiece.MatchPosition();
            SelectedPiece = null;
        }

    }

    private void movePiece(Piece p)
    {

    }

    //Find and activate spotlight from PieceSelectorLight



}
