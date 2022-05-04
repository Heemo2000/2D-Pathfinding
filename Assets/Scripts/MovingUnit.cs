using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingUnit : MonoBehaviour
{
    
    const float MinPathUpdateTime = 0.3f;

    const float MaxTimeSinceLevelLoad = 0.2f;
    [SerializeField]private float moveSpeed = 1f;


    [SerializeField]private float pathUpdateMoveThreshold = 0.6f;
    [SerializeField]private LayerMask obstacleMask;

    [SerializeField]private float turnDistance = 10f;
    [SerializeField]private float turnSpeed = 5f;
    [SerializeField]private bool showPath = true;
    private Path _path;
    
    private Coroutine _moveCoroutine;
    
    private Coroutine _updateCoroutine;
    private void Start() {
        PathRequestManager.Instance.PathFinder.Scan(obstacleMask);
    }
    public void MoveToDestination(Vector2 destination)
    {
        if(_updateCoroutine != null)
        {
            StopCoroutine(_updateCoroutine);
        }
        _updateCoroutine = StartCoroutine(UpdatePath(destination));
    }

    private IEnumerator UpdatePath(Vector2 destination)
    {
        if(Time.timeSinceLevelLoad < MaxTimeSinceLevelLoad)
        {
            yield return new WaitForSeconds(MinPathUpdateTime);
        }
        PathRequestManager.Instance.RequestPath(transform.position,destination,OnPathComplete);

        Vector2 oldPosition = transform.position;
        while(Vector2.SqrMagnitude(destination - (Vector2)transform.position) > 1f)
        {
            float difference = Vector2.SqrMagnitude((Vector2)transform.position - oldPosition);
            if(difference > pathUpdateMoveThreshold * pathUpdateMoveThreshold)
            {
                PathRequestManager.Instance.RequestPath(transform.position,destination,OnPathComplete);
                oldPosition = transform.position;
            }
            yield return new WaitForSeconds(MinPathUpdateTime);
        }
    }
    private void OnPathComplete(List<Vector2> wayPoints,bool pathFindSuccess)
    {
        if(pathFindSuccess)
        {
           _path = new Path(wayPoints,transform.position,turnDistance);
           if(_moveCoroutine != null)
           {
               StopCoroutine(_moveCoroutine);
           }
           _moveCoroutine = StartCoroutine(Move());
           return;
        }
        
    }
    private IEnumerator Move()
    {
        if(_path == null)
        {
            yield break;
        }
        _path.DrawPath(5f);

        int i = 0;
        //Debug.Log("Turn boundaries size : " + _path.TurnBoundaries.Count);
        //Debug.Log("WayPoints size : " + _path.WayPoints.Count);
        while(i < _path.TurnBoundaries.Count)
        {
            Line turnBoundary = _path.TurnBoundaries[i];
            Vector2 currentWayPoint = _path.WayPoints[i];
            //Debug.Log("i = " +i);
            

            Vector2 direction = (currentWayPoint - (Vector2)transform.position).normalized;
            float rotateAngle = Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg;
            
            Quaternion targetRotation = Quaternion.AngleAxis(rotateAngle,Vector3.forward);
            transform.rotation = Quaternion.Lerp(transform.rotation,targetRotation,turnSpeed * Time.fixedDeltaTime);
            transform.Translate(Vector2.right * moveSpeed * Time.fixedDeltaTime,Space.Self);
            if(turnBoundary.HasCrossedLine(transform.position))
            {
                i++;   
            }
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
            
            Vector3 currentNodeWorldPos = pathList[i]; //+ Vector2.one * 0.5f;
            Vector3 nextNodeWorldPos = pathList[i+1]; //+ Vector2.one * 0.5f;

            currentNodeWorldPos.z = transform.position.z;
            nextNodeWorldPos.z = transform.position.z;
            Debug.DrawLine(currentNodeWorldPos,nextNodeWorldPos,Color.green,pathShowingTime);
        }
    }
}
