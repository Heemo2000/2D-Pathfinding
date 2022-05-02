using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathRequestManager))]
public class Pathfinding : MonoBehaviour
{
    private const int DiagonalCost = 14;
    private const int StraightCost = 10;
    
    [SerializeField]private PathfindingData pathfindingData;
    private LayerMask _walkable;

    private Dictionary<int,int> _walkableDictionary;
    private Grid<Node> _grid;

    public Grid<Node> Grid { get => _grid; }

    private void Awake() 
    {
        _grid = new Grid<Node>(transform.position,pathfindingData.width,pathfindingData.height,pathfindingData.cellSize,(x,y) => new Node(new Vector2Int(x,y),true,0));
        
        if(pathfindingData.shouldUsePenaltyCost)
        {
            _walkableDictionary = new Dictionary<int, int>();
        
            foreach(TerrainType terrainType in pathfindingData.terrainTypes)
            {
                _walkable.value |= terrainType.terrainMask.value;
                _walkableDictionary.Add((int)Mathf.Log(terrainType.terrainMask.value,2), terrainType.terrainPenalty);
            }
            Debug.Log(System.Convert.ToString(_walkable,2));
        }
        
    }
    
    public void Scan(LayerMask unwalkableMask)
    {
        for(int x = 0; x < pathfindingData.width; x++)
        {
            for(int y = 0; y < pathfindingData.height; y++)
            {
                Node currentNode = _grid.GetCellValue(x,y);
                Vector2 worldPosition = _grid.GetWorldPosition(x,y);

                bool walkable = Physics2D.OverlapCircle(worldPosition,pathfindingData.cellSize/2f,unwalkableMask) == null;
                currentNode.Walkable = walkable;
                
                int movementPenalty = 0;
                if(currentNode.Walkable && pathfindingData.shouldUsePenaltyCost)
                {
                    RaycastHit2D hit = Physics2D.CircleCast(
                                                            _grid.GetWorldPosition(currentNode.PositionInGrid),
                                                            pathfindingData.cellSize,Vector2.zero,1f,_walkable.value
                      
                                                            );
                    if(hit.collider != null)
                    {   
                        Debug.Log("GameObject Name : " + hit.transform.gameObject + ", Layer : " + hit.transform.gameObject.layer);
                        _walkableDictionary.TryGetValue(hit.transform.gameObject.layer,out movementPenalty);
                    }
                    //Debug.Log("Movement Penalty : " + movementPenalty);
                }
                
                currentNode.MovementPenalty = movementPenalty;
                
            }
        }
    }

    public void StartFindingPath(Vector2 startPosition,Vector2 endPosition)
    {
        StartCoroutine(FindPath(startPosition,endPosition));
    }

    
    private IEnumerator FindPath(Vector2 startPosition,Vector2 endPosition)
    {
        
        Node startNode = this._grid.GetCellValue(startPosition);
        Node endNode = this._grid.GetCellValue(endPosition);
        bool pathFindSuccess = false;

        if(startNode.Walkable == true && endNode.Walkable == true)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            List<Node> openList = new List<Node>();
            List<Node> closeList = new List<Node>();

            openList.Add(startNode);
            while(openList.Count > 0)
            {
                Node currentNode = GetLeastNode(openList);

                if(currentNode == endNode)
                {
                    stopwatch.Stop();
                    Debug.Log("Found Path in " + stopwatch.ElapsedMilliseconds + " ms.");
                    pathFindSuccess = true;
                    break;
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
                    
                    int newMoveCostToNeighbour = currentNode.GCost + GetDistance(currentNode,neighbour) + neighbour.MovementPenalty;
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
        }
        yield return null;

        List<Vector2> requiredPath = null;
        if(pathFindSuccess)
        {
            requiredPath = SimplifyPath(RetracePath(startNode,endNode));
        }
        
        PathRequestManager.Instance.OnPathFindFinish(requiredPath,pathFindSuccess);
    }

    public void DisplayGrid()
    {
        for(int x = 0; x < pathfindingData.width; x++)
        {
            for(int y = 0; y < pathfindingData.height; y++)
            {
                Vector2 cellWorldPosition = _grid.GetWorldPosition(x,y) + Vector2.one * pathfindingData.cellSize/2f;
                if(!_grid.GetCellValue(cellWorldPosition).Walkable)
                {
                    DrawSquare(cellWorldPosition,pathfindingData.cellSize - 0.1f,Color.red);
                }
                else
                {
                    DrawSquare(cellWorldPosition,pathfindingData.cellSize - 0.1f,Color.white);
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

    private List<Vector2> SimplifyPath(List<Node> path)
    {
        List<Vector2> result = new List<Vector2>();

        Vector2Int previousDirection = Vector2Int.zero;
        if(path.Count > 0)
        {
            result.Add(_grid.GetWorldPosition(path[0].PositionInGrid));
        }
        for(int i = 1; i < path.Count; i++)
        {
            Vector2Int currentDirection = path[i-1].PositionInGrid - path[i].PositionInGrid;
            if(previousDirection != currentDirection)
            {
                result.Add(_grid.GetWorldPosition(path[i].PositionInGrid));
            }
            previousDirection = currentDirection;
        }

        if(path.Count > 0)
        {
            result.Add(_grid.GetWorldPosition(path[path.Count - 1].PositionInGrid));
        }
        return result;
    }
    private List<Node> GetNeighbours(Node node)
    {
        Vector2Int posInGrid = node.PositionInGrid;

        List<Node> neighbourList = new List<Node>();

        //Left
        if(posInGrid.x - 1 >= 0)
        {
            neighbourList.Add(this._grid.GetCellValue(posInGrid.x - 1, posInGrid.y));
            //Down left
            if(posInGrid.y - 1 >= 0)
            {
                neighbourList.Add(this._grid.GetCellValue(posInGrid.x - 1,posInGrid.y - 1));
            }
            //Up Left.
            if(posInGrid.y + 1 < pathfindingData.height)
            {
                neighbourList.Add(this._grid.GetCellValue(posInGrid.x - 1,posInGrid.y + 1));
            }
        }
        //Right
        if(posInGrid.x + 1 < pathfindingData.width)
        {
            neighbourList.Add(this._grid.GetCellValue(posInGrid.x + 1, posInGrid.y));
            //Down Right
            if(posInGrid.y - 1 >= 0)
            {
                neighbourList.Add(this._grid.GetCellValue(posInGrid.x + 1,posInGrid.y - 1));
            }
            //Up Right.
            if(posInGrid.y + 1 < pathfindingData.height)
            {
                neighbourList.Add(this._grid.GetCellValue(posInGrid.x + 1,posInGrid.y + 1));
            }
        }
        //Up
        if(posInGrid.y - 1 >= 0)
        {
            neighbourList.Add(this._grid.GetCellValue(posInGrid.x,posInGrid.y - 1));
        }
        //Down
        if(posInGrid.y + 1 < pathfindingData.height)
        {
            neighbourList.Add(this._grid.GetCellValue(posInGrid.x,posInGrid.y + 1));
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
