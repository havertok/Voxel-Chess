using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Helper class to handle player switching
public class PlayerController : MonoBehaviour
{
    private string player_name;//PLAYER_1 PLAYER_2
    private Board.GameState whichPlayer;

    private MoveCaleulator MoveCalc;
    private GameObject PieceSelectorLight;
    private Light PSLtoggle;

    private Piece SelectedPiece;
    private BoardSquare SelectedSquare;
    private List<BoardSquare> moveSet; //when we select a piece this fills out

    private int numCapturedPieces; //Use this and multi to place captured pieces.
    private Vector3 gridPosmulti;

    public void Start()
    {
        MoveCalc = GameObject.FindObjectOfType<MoveCaleulator>();
        PieceSelectorLight = GameObject.Find("PieceSelectorLight");
        PSLtoggle = PieceSelectorLight.GetComponentInChildren<Light>();
        numCapturedPieces = 0;
        gridPosmulti = new Vector3Int(-10, 0, 10);
    }

    public void PlayerTurn(Ray ray, Dictionary<string, BoardSquare> boardArray, Board.GameState gameState)
    {
        RaycastHit hitinfo;
        GameObject obj;
        BoardSquare bsq;
        this.whichPlayer = gameState;

        if (Physics.Raycast(ray, out hitinfo, Mathf.Infinity))
        {
            obj = hitinfo.collider.gameObject;
            if (boardArray.TryGetValue(obj.name, out bsq))
            {
                if (!SelectedSquare) //if there is no selected square
                {
                    SelectedSquare = bsq; //set it
                    bsq.SelectSquare(); //highlight it
                }
                if (SelectedSquare && SelectedSquare != bsq) //if there is a new square
                {
                    bsq.SelectSquare(); //highlight it
                    SelectedSquare.DeselectSquare(); //unhighlight old
                    SelectedSquare = bsq;
                }
            }
            /* else if (SelectedSquare)
             {
                 SelectedSquare.DeselectSquare();
                 SelectedSquare = null;
             }*/

            if (Input.GetMouseButtonDown(0))
            {
                PieceSelector(hitinfo, boardArray);
            }
        }
        else //Raycast returns false if not hitting a collider
        {
            if (SelectedSquare != null)
            {
                SelectedSquare.DeselectSquare();
                SelectedSquare = null;
            }
            if (Input.GetMouseButtonDown(0)) //besure to dehighlight before nulling
            {
                print("Piece deselected");
                DeselectPiece();
                PSLtoggle.enabled = false;
            }
        }
    }

    public Board.GameState GetNewPlayerState()
    {
        return this.whichPlayer;
    }

    private void DeselectPiece()
    {
        SelectedPiece = null;
        DeselectUnHighlight();
        moveSet = null;
    }

    private void HighLightMoves()
    {
        if (moveSet != null)
        {
            foreach (BoardSquare bsq in moveSet)
            {
                bsq.highlight();
            }
        }
    }

    private void DeselectUnHighlight()
    {
        if (moveSet != null)
        {
            foreach (BoardSquare bsq in moveSet)
            {
                bsq.unHighlight();
            }
        }
    }

    //IF MOUSECLICK THEN set SelectePiece as above, check if null (if not null then move to raycasted square)
    private void PieceSelector(RaycastHit hitinfo, Dictionary<string, BoardSquare> boardArray)
    {
        BoardSquare hitSq = hitinfo.collider.GetComponent<BoardSquare>();
        Piece myPiece = hitinfo.collider.GetComponent<Piece>();
        //No piece selected AND we are NOT clicking a square
        if (SelectedPiece == null && hitSq == null)
        {
            if (playerMatch(myPiece))
            {
                SelectedPiece = myPiece;
                PieceSelectorLight.transform.position = SelectedPiece.transform.position;
                PSLtoggle.enabled = true;

                //Now get valid moves and highlight them
                MoveCalc.updateLocalBoard(boardArray);
                moveSet = MoveCalc.GetValidMoves(SelectedPiece);
                HighLightMoves();
            }
        }

        //If we have a piece selected AND we click a square, check if valid, then move if so.
        if (SelectedPiece != null && hitSq != null)
        {
            if (moveSet.Contains(hitSq))
            {
                MovePiece(hitSq);
                changeTurn();
            }
        }

        //Otherwise, if we have a piece selected, we are clicking a piece AND that piece ISN'T
        //the one we just selected, we can capture it (if it's a valid move)
        //The logic for valid moves, e.g. can't capture same color piece, will be in movecalc
        if (SelectedPiece != null && myPiece != null && myPiece != SelectedPiece)
        {
            print(myPiece.name);
            if (moveSet.Contains(myPiece.GetSquare()))
            {
                CapturePiece(myPiece);
                changeTurn();
            }
        }

    }

    private void MovePiece(BoardSquare target)
    {
        SelectedPiece.GetSquare().SetPiece(null);
        SelectedPiece.SetSquare(target);
        SelectedPiece.MatchPosition();
        DeselectPiece();
    }

    private void CapturePiece(Piece target)
    {
        print(SelectedPiece.name + " Capturing " + target.name);
        //Moves attacking piece to square and sets relations
        MovePiece(target.GetSquare());
        target.SetSquare(null);
        //Moves to invisible grid (maybe something else later)
        target.transform.position = Vector3.Scale(gridPosmulti, new Vector3(1, 1, numCapturedPieces));
        numCapturedPieces++;
    }

    private bool playerMatch(Piece p)
    {
        if(whichPlayer == Board.GameState.PLAYER_1 && p.isWhite())
        {
            return true;
        }
        if (whichPlayer == Board.GameState.PLAYER_2 && !p.isWhite())
        {
            return true;
        }
        return false;
    }

    private void changeTurn()
    {
        switch (whichPlayer)
        {
            case Board.GameState.PLAYER_1:
                {
                    this.whichPlayer = Board.GameState.PLAYER_2;
                    break;
                }
            case Board.GameState.PLAYER_2:
                {
                    this.whichPlayer = Board.GameState.PLAYER_1;
                    break;
                }
        }
    }
}
