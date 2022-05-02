using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingUnit : MonoBehaviour
{
    
    [SerializeField]private float moveSpeed = 1f;

    [SerializeField]private float nextWayPointDistance = 1f;
    [SerializeField]private LayerMask obstacleMask;
    private List<Vector2> _path;
    
    private int _currentWayPoint;

    private Coroutine _moveCoroutine;
    private void Start() {
        PathRequestManager.Instance.PathFinder.Scan(obstacleMask);
    }
    public void MoveToDestination(Vector2 destination)
    {
        PathRequestManager.Instance.RequestPath(transform.position,destination,OnPathComplete);
    }


    private void OnPathComplete(List<Vector2> path,bool pathFindSuccess)
    {
        if(pathFindSuccess)
        {
            _path = path;
            _currentWayPoint = 0;
            if(_moveCoroutine != null)
            {
                StopCoroutine(_moveCoroutine);
            }
            _moveCoroutine = StartCoroutine(Move());
        }
    }
    private IEnumerator Move()
    {
        if(_path == null)
        {
            yield break;
        }
        

        while(_currentWayPoint <= _path.Count - 1)
        {
            Vector2 currentPosition = _path[_currentWayPoint];
            Vector2 difference = currentPosition - (Vector2)transform.position;
            Vector2 direction = difference.normalized;

            float squareDistToWayPt = Vector2.SqrMagnitude(difference);

            if(squareDistToWayPt <= nextWayPointDistance * nextWayPointDistance)
            {
                _currentWayPoint++;
            }

            transform.position = Vector2.MoveTowards(transform.position,currentPosition,moveSpeed * Time.fixedDeltaTime);
            //transform.Translate(direction * moveSpeed * Time.fixedDeltaTime); 

            yield return null;
        }
    }

    private void DisplayPath(List<Vector2> pathList,float pathShowingTime)
    {
        if(pathList == null)
        {
            Debug.Log("Path is null.");
            return;
        }
        for(int i = 0; i < pathList.Count - 1; i++)
        {
            
            Vector3 currentNodeWorldPos = pathList[i] + Vector2.one * 0.5f;
            Vector3 nextNodeWorldPos = pathList[i+1] + Vector2.one * 0.5f;

            currentNodeWorldPos.z = transform.position.z;
            nextNodeWorldPos.z = transform.position.z;
            Debug.DrawLine(currentNodeWorldPos,nextNodeWorldPos,Color.green,pathShowingTime);
        }
    }
}
