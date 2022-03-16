using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [SerializeField]private Vector2Int gridSize;
    [SerializeField]private Transform seeker;
    [SerializeField]private Transform target;
    [SerializeField]private LayerMask unwalkableMask;
    
    [SerializeField]private float scanInterval = 1f;
    
    [SerializeField]private float moveSpeed = 1f;

    [SerializeField]private float nextWayPointDistance = 1f;

    [SerializeField]private float cellSize = 0.5f;
    private Pathfinding _pathfinding;

    private List<Node> _path;
    
    private int _currentWayPoint;
    void Awake() {
        _pathfinding = new Pathfinding(transform.position,gridSize.x,gridSize.y,cellSize);
    }

    // Start is called before the first frame update
    void Start()
    {
        _pathfinding.Scan(unwalkableMask);
        StartCoroutine(DoTesting());
        _currentWayPoint = 0;
    }

    private void Update() {
        _pathfinding.DisplayGrid();
        //MoveToDestination();
    }
    private IEnumerator DoTesting()
    {
        while(this.enabled)
        {
            _path = _pathfinding.FindPath(seeker.position,target.position);
            DisplayPath(_path,scanInterval);
            yield return new WaitForSeconds(scanInterval);
        }
    }

    private void DisplayPath(List<Node> pathList,float pathShowingTime)
    {
        if(pathList == null)
        {
            Debug.Log("Path is null.");
            return;
        }
        for(int i = 0; i < pathList.Count - 1; i++)
        {
            Node currentNode = pathList[i];
            Node nextNode = pathList[i+1];

            Vector3 currentNodeWorldPos = _pathfinding.Grid.GetWorldPosition(currentNode.PositionInGrid) + Vector2.one * 0.5f;
            Vector3 nextNodeWorldPos = _pathfinding.Grid.GetWorldPosition(nextNode.PositionInGrid) + Vector2.one * 0.5f;

            currentNodeWorldPos.z = transform.position.z;
            nextNodeWorldPos.z = transform.position.z;
            Debug.DrawLine(currentNodeWorldPos,nextNodeWorldPos,Color.green,pathShowingTime);
        }
    }

    private void MoveToDestination()
    {
        if(_path == null || _currentWayPoint >= _path.Count)
        {
            return;
        }

        Vector2 wayPointWorldPos = _pathfinding.Grid.GetWorldPosition(_path[_currentWayPoint].PositionInGrid);
        float squareDistToWayPt = Vector2.SqrMagnitude(wayPointWorldPos - (Vector2)seeker.position);

        seeker.position = Vector2.Lerp(seeker.position,
                                       wayPointWorldPos,
                                       moveSpeed * Time.deltaTime);

        if(squareDistToWayPt <= nextWayPointDistance * nextWayPointDistance)
        {
            _currentWayPoint++;
        }
    }
}
