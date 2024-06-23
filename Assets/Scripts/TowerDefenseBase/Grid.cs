using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UIElements;


public class Grid<T>
{
    private int width;
    private int height;
    private float cellSize;
    private T[,] gridArray;
    private Vector3 position;

    public Grid(int width, int height,float cellSize, Vector3 position)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.position = position;
        gridArray = new T[width, height];
    }
 
    public void DrawGrid()
    {
        for (int i = 0; i <= height; i++)
        {
            Debug.DrawLine(new Vector3(position.x, position.y - i * cellSize, 0), new Vector3(position.x + width * cellSize, position.y - i * cellSize, 0), Color.white, 100f);
        }
        for (int i = 0; i <= width; i++)
        {
            Debug.DrawLine(new Vector3(position.x + i * cellSize, position.y, 0), new Vector3(position.x + i * cellSize, position.y - height * cellSize, 0), Color.white, 100f);
        }
    }

    public bool GetNearestCell(ref Vector3 worldPositoin)
    {
        
        if (worldPositoin.x < position.x - cellSize || worldPositoin.x > position.x + (width+1) * cellSize || worldPositoin.y > position.y + cellSize || worldPositoin.y < position.y - (height+1) * cellSize)
        {
            return false;
        }
        int x = Mathf.FloorToInt((worldPositoin.x - position.x) / cellSize);
        int y = Mathf.FloorToInt((position.y - worldPositoin.y) / cellSize);
        x = Mathf.Clamp(x, 0, width - 1);
        y = Mathf.Clamp(y, 0, height - 1);
        worldPositoin =  new Vector3(position.x + x * cellSize + cellSize / 2, position.y - y * cellSize - cellSize / 2, 0);
        return true;
    }
}
