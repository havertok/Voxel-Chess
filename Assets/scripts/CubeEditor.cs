using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[SelectionBase] //the default selected object is the gameobject to which this is attached
[RequireComponent(typeof(BoardSquare))]

public class CubeEditor : MonoBehaviour
{
    BoardSquare BoardSquare;

    //Awake() comes before start, tells the cube editor to find BoardSquare
    private void Awake()
    {
        BoardSquare = GetComponent<BoardSquare>();
    }

    // This editor-level script ensures that the cubes can only be moved on a snapping grid
    // that is the current size of said cube (10 being the scale, in meters.)
    // the Y pos is on a scale of 5, for possible future aesthetics
    //NEW: This will also change the text mesh to display the coordinates
    // NEWv2: Moving entity location data to the BoardSquare script in order to remove
    // dependency on THIS, editor only script (it isn't shipped with production, here
    // only to thelp the dev).
    void Update()
    {
        SnapToGrid();
        UpdateLabel();
    }

    private void SnapToGrid()
    {
        int gridSize = BoardSquare.GetGridSize();

        //Sets the new pos limited by the above lines, label is also divided by grid size
        //so that the coords are on a scale of 1 (e.g. 0,0 0,1 1,1)
        // NEW v2 - grid position is moved to BoardSquare and stored there in a Vector2, hence y
        // is featured twice, getGridPos().y is the vector3 z equivalent.
        transform.position = new Vector3(BoardSquare.GetGridPos().x * gridSize,
            0f, BoardSquare.GetGridPos().y * gridSize);
    }

    private void UpdateLabel()
    {
        int gridSize = BoardSquare.GetGridSize();
        //TextMesh coords = GetComponentInChildren<TextMesh>();
        string labelText = BoardSquare.GetGridPos().x +
            "," + 
            BoardSquare.GetGridPos().y;
        //coords.text = labelText;
        gameObject.name = labelText;
    }
}
