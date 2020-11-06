using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSquare : MonoBehaviour
{
    [SerializeField] bool isWhite;
    [SerializeField] private Piece piece; //want to see it in editor for later
    const int squareSize = 10;

    private bool highlighted = false;
    private bool occupied = false;
    private Renderer highlightRend;
    private Renderer validMoveRend;
    private Vector2Int gridPos;
    

    public void Start()
    {
        //We need to get the child mesh and NOT the default mesh (also a child of prefab)
        highlightRend = this.transform.Find("highlightPlane").GetComponent<Renderer>();
        validMoveRend = this.transform.Find("validMoveHighlight").GetComponent<Renderer>();
    }

    public void Update()
    {
        this.highlightRend.enabled = this.highlighted;
    }

    public int GetGridSize()
    {
        return squareSize;
    }

    public Vector2Int GetGridPos()
    {
        return new Vector2Int(
            Mathf.RoundToInt(transform.position.x / squareSize),
            Mathf.RoundToInt(transform.position.z / squareSize)
        );
    }

    public Piece GetPiece()
    {
        return this.piece;
    }

    public void SetPiece(Piece p)
    {
        this.piece = p;
    }

    public bool IsOccupied()
    {
        return this.occupied;
    }

    public void ToggleOccupied()
    {
        this.occupied = !occupied;
    }

    public void SelectSquare()
    {
        highlighted = true;
    }

    public void DeselectSquare()
    {
        highlighted = false;
    }

    public void highlight()
    {
        validMoveRend.enabled = true;
    }

    public void unHighlight()
    {
        validMoveRend.enabled = false;
    }

}
