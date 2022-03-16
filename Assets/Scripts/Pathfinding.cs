using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Pathfinding
{
    private const int DiagonalCost = 14;
    private const int StraightCost = 10;
    
    private Vector2 origin;
    private int width;
    private int height;
    private float cellSize;

    private Grid<Node> grid;

    public Grid<Node> Grid { get => grid; }

    public Pathfinding(Vector2 origin,int width,int height,float cellSize)
    {
        this.origin = origin;
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.grid = new Grid<Node>(origin,width,height,cellSize,(x,y) => new Node(new Vector2Int(x,y),true));
    }


    public void Scan(LayerMask unwalkableMask)
    {
        for(int x = 0; x < this.width; x++)
        {
            for(int y = 0; y < this.height; y++)
            {
                Node currentNode = this.grid.GetCellValue(x,y);
                Vector2 worldPosition = this.grid.GetWorldPosition(x,y);

                bool walkable = Physics2D.OverlapCircle(worldPosition,this.cellSize/2f,unwalkableMask) == null;
                currentNode.Walkable = walkable;
                /*
                if(!currentNode.Walkable)
                {
                    Debug.Log("Not walkable at " + this.grid.GetWorldPosition(x,y));
                }
                */
            }
        }
    }
    public List<Node> FindPath(Vector2 startPosition,Vector2 endPosition)
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();
        List<Node> openList = new List<Node>();
        List<Node> closeList = new List<Node>();

        Node startNode = this.grid.GetCellValue(startPosition);
        Node endNode = this.grid.GetCellValue(endPosition);

        openList.Add(startNode);
        while(openList.Count > 0)
        {
            Node currentNode = GetLeastNode(openList);

            if(currentNode == endNode)
            {
                stopwatch.Stop();
                Debug.Log("Found Path in " + stopwatch.ElapsedMilliseconds + " ms.");
                return RetracePath(startNode,endNode);
            }

            openList.Remove(currentNode);
            closeList.Add(currentNode);

            List<Node> neighbours = GetNeighbours(currentNode);

            foreach(Node neighbour in neighbours)
            {
                if(closeList.Contains(neighbour) || !neighbour.Walkable)
                {
                    continue;
                }
                
                int newMoveCostToNeighbour = currentNode.GCost + GetDistance(currentNode,neighbour);
                if(newMoveCostToNeighbour < neighbour.GCost || !openList.Contains(neighbour))
                {
                    neighbour.GCost = newMoveCostToNeighbour;
                    neighbour.HCost = GetDistance(neighbour,endNode);
                    neighbour.ParentNode = currentNode;
                    if(!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }
            }
        }

        stopwatch.Stop();
        return null;
    }

    public void DisplayGrid()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Vector2 cellWorldPosition = grid.GetWorldPosition(x,y) + Vector2.one * cellSize/2f;
                if(!grid.GetCellValue(cellWorldPosition).Walkable)
                {
                    DrawSquare(cellWorldPosition,cellSize,Color.red);
                }
                else
                {
                    DrawSquare(cellWorldPosition,cellSize - 0.1f,Color.white);
                }
                
            }
        }
    }

    private void DrawSquare(Vector2 worldPosition,float squareSize,Color squareColor)
    {
        //Taking points clockwise.
        Vector2 topRightPosition = new Vector2(worldPosition.x + squareSize/2f,worldPosition.y + squareSize/2f);
        
        Vector2 bottomRightPosition = new Vector2(worldPosition.x + squareSize/2f,worldPosition.y - squareSize/2f);

        Vector2 bottomLeftPosition = new Vector2(worldPosition.x - squareSize/2f,worldPosition.y - squareSize/2f);

        Vector2 topLeftPosition = new Vector2(worldPosition.x - squareSize/2f,worldPosition.y + squareSize/2f);
        
        Debug.DrawLine(topRightPosition,bottomRightPosition,squareColor);
        Debug.DrawLine(bottomRightPosition,bottomLeftPosition,squareColor);
        Debug.DrawLine(bottomLeftPosition,topLeftPosition,squareColor);
        Debug.DrawLine(topLeftPosition,topRightPosition,squareColor);
        
    }
    private int GetDistance(Node nodeA,Node nodeB)
    {
        int xDistance = Mathf.Abs(nodeA.PositionInGrid.x - nodeB.PositionInGrid.x);
        int yDistance = Mathf.Abs(nodeA.PositionInGrid.y - nodeB.PositionInGrid.y);
        
        int remainingDistance = Mathf.Abs(xDistance - yDistance);
        return Mathf.Min(xDistance,yDistance) * DiagonalCost +  remainingDistance * StraightCost;
    }
    private List<Node> RetracePath(Node startNode,Node endNode)
    {
        List<Node> result = new List<Node>();

        Node currentNode = endNode;
        
        while(currentNode != startNode)
        {
            result.Add(currentNode);
            currentNode = currentNode.ParentNode;
        }
        result.Add(startNode);
        result.Reverse();
        return result;
    }

    private List<Node> GetNeighbours(Node node)
    {
        Vector2Int posInGrid = node.PositionInGrid;

        List<Node> neighbourList = new List<Node>();

        //Left
        if(posInGrid.x - 1 >= 0)
        {
            neighbourList.Add(this.grid.GetCellValue(posInGrid.x - 1, posInGrid.y));
            //Down left
            if(posInGrid.y - 1 >= 0)
            {
                neighbourList.Add(this.grid.GetCellValue(posInGrid.x - 1,posInGrid.y - 1));
            }
            //Up Left.
            if(posInGrid.y + 1 < this.height)
            {
                neighbourList.Add(this.grid.GetCellValue(posInGrid.x - 1,posInGrid.y + 1));
            }
        }
        //Right
        if(posInGrid.x + 1 < this.width)
        {
            neighbourList.Add(this.grid.GetCellValue(posInGrid.x + 1, posInGrid.y));
            //Down Right
            if(posInGrid.y - 1 >= 0)
            {
                neighbourList.Add(this.grid.GetCellValue(posInGrid.x + 1,posInGrid.y - 1));
            }
            //Up Right.
            if(posInGrid.y + 1 < this.height)
            {
                neighbourList.Add(this.grid.GetCellValue(posInGrid.x + 1,posInGrid.y + 1));
            }
        }
        //Up
        if(posInGrid.y - 1 >= 0)
        {
            neighbourList.Add(this.grid.GetCellValue(posInGrid.x,posInGrid.y - 1));
        }
        //Down
        if(posInGrid.y + 1 < this.height)
        {
            neighbourList.Add(this.grid.GetCellValue(posInGrid.x,posInGrid.y + 1));
        }

        return neighbourList;
    }
    private Node GetLeastNode(List<Node> nodeList)
    {
        Node minNode = nodeList[0];

        for(int i = 1; i < nodeList.Count; i++)
        {
            if(minNode.FCost > nodeList[i].FCost || (minNode.FCost == nodeList[i].FCost && minNode.HCost > nodeList[i].HCost))
            {
                minNode = nodeList[i];
            }
        }

        return minNode;
    }
}
