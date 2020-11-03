using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSelector : MonoBehaviour
{
    public GameObject tileHighlightPrefab;
    private GameObject tileHighlight;
    private BoardSquare boardSquare;

    private void Start()
    {
        
    }

    public void EnterState()
    {
        enabled = true;
    }
}
