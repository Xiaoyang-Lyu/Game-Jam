using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    private Grid<int> grid;
    public int width;
    public int height;
    public float cellSize;

    void Start()
    {
        //set an 3x3 grid at selvies position
        grid = new Grid<int>(width, height, cellSize, transform.position);
        grid.DrawGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool GetNearestCell(ref Vector3 worldPositoin)
    {
        return grid.GetNearestCell(ref worldPositoin);
    }
}
