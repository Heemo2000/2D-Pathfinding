using System;
using UnityEngine;
public class Grid<T>
{
    
    private T[,] gridArray;
    private Vector2 origin;
    private int width;
    private int height;
    private float cellSize;

    public Vector2 Origin { get => origin; set => origin = value; }
    public int Width { get => width; set => width = value; }
    public int Height { get => height; set => height = value; }
    public float CellSize { get => cellSize; set => cellSize = value; }


    public Grid(Vector2 origin,int width,int height,float cellSize,Func<int,int,T> createTObject)
    {
        this.origin = origin;
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.gridArray = new T[width,height];
        for(int x = 0 ; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                gridArray[x,y] = createTObject(x,y);
            }
        }
    }
    
    public Vector2 GetWorldPosition(Vector2Int posInGrid)
    {
        return GetWorldPosition(posInGrid.x,posInGrid.y);
    }
    public Vector2 GetWorldPosition(int x, int y)
    {
        return origin + new Vector2(x,y) * cellSize;
    }

    public Vector2Int GetXY(Vector2 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition - origin).x/cellSize);
        int y = Mathf.FloorToInt((worldPosition - origin).y/cellSize);
        
        return new Vector2Int(x,y);
    }
    
    public void SetCellValue(int x,int y, T value)
    {
        if((x>=0 && x < this.width) && (y>=0 && y < this.height))
        {
            gridArray[x,y] = value;
        }
    }

    public void SetCellValue(Vector2 worldPosition,T value)
    {
        Vector2Int cellPosInGrid = GetXY(worldPosition);
        SetCellValue(cellPosInGrid.x,cellPosInGrid.y,value);
    }

    public T GetCellValue(int x,int y)
    {
        if((x>=0 && x < this.width) && (y>=0 && y < this.height))
        {
            return gridArray[x,y];
        }
        return default(T);
    }

    public T GetCellValue(Vector2 worldPosition)
    {
        Vector2Int cellPosInGrid = GetXY(worldPosition);
        return GetCellValue(cellPosInGrid.x,cellPosInGrid.y);
    }
    

    
}
