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

    [SerializeField]private float slowDownDistance = 2f;
    private Path _path;
    
    private Coroutine _moveCoroutine;
    
    private Coroutine _updateCoroutine;
    private void Start() {
        PathRequestManager.Instance.PathFinder.Scan(obstacleMask);
    }
    public void MoveToDestination(Vector2 destination)
    {
        /*
        if(_updateCoroutine != null)
        {
            StopCoroutine(_updateCoroutine);
        }
        _updateCoroutine = StartCoroutine(UpdatePath(destination));
        */
        PathRequestManager.Instance.RequestPath(transform.position,destination,OnPathComplete);
    }

    /*
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
    */
    private void OnPathComplete(List<Vector2> wayPoints,bool pathFindSuccess)
    {
        if(pathFindSuccess)
        {
           _path = new Path(wayPoints,transform.position,turnDistance,slowDownDistance);
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
        int finalPathIndex = _path.TurnBoundaries.Count - 1;
        while(i < _path.TurnBoundaries.Count)
        {
            Line turnBoundary = _path.TurnBoundaries[i];
            Vector2 currentWayPoint = _path.WayPoints[i];            

            Vector2 direction = (currentWayPoint - (Vector2)transform.position).normalized;
            float rotateAngle = Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg;
            
            Quaternion targetRotation = Quaternion.AngleAxis(rotateAngle,Vector3.forward);
            transform.rotation = Quaternion.Lerp(transform.rotation,targetRotation,turnSpeed * Time.fixedDeltaTime);
            if(i >= _path.SlowDownIndex)
            {
                float speedPercent = Mathf.Clamp01(_path.TurnBoundaries[finalPathIndex].DistanceFromPoint(transform.position)/slowDownDistance);
                if(speedPercent <= 0.01f)
                {
                    speedPercent = 0f;
                    break;
                }
                Debug.Log("Speed Percent : " + speedPercent);
                transform.Translate(Vector2.right * moveSpeed * speedPercent * Time.fixedDeltaTime,Space.Self);
            }
            else
            {
                transform.Translate(Vector2.right * moveSpeed * Time.fixedDeltaTime,Space.Self);
            }
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
