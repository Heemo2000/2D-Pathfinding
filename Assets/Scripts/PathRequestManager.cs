using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pathfinding))]
public class PathRequestManager : GenericSingleton<PathRequestManager>
{
    [SerializeField]private bool showGrid = true;
    public Action<List<Vector2>,bool> OnPathFindFinish;
    private Queue<PathRequest> _pathRequests;
    
    private PathRequest _currentRequest;
    private bool _isProcessingPath;

    private Pathfinding _pathFinding;

    public Pathfinding PathFinder { get => _pathFinding;}

    protected override void Awake()
    {
        base.Awake();
        _pathRequests = new Queue<PathRequest>();
        _pathFinding = GetComponent<Pathfinding>();
    }
    private void Start() {
        OnPathFindFinish += FinishedPathFinding;
    }
    
    private void Update() {
        if(showGrid)
        {
            _pathFinding.DisplayGrid();
        }
    }
    public void RequestPath(Vector2 startPosition,Vector2 endPosition,System.Action<List<Vector2>,bool> OnPathComplete)
    {
        PathRequest newRequest = new PathRequest(startPosition,endPosition,OnPathComplete);
        Instance._pathRequests.Enqueue(newRequest);
        Instance.TryProcessRequest();
    }

    private void TryProcessRequest()
    {
        if(_isProcessingPath == false && _pathRequests.Count > 0)
        {
            _currentRequest = _pathRequests.Dequeue();
            _pathFinding.StartFindingPath(_currentRequest.startPosition,_currentRequest.endPosition);
            _isProcessingPath = true;
        }
    }

    private void FinishedPathFinding(List<Vector2> pathList,bool success)
    {
        if(_isProcessingPath == true)
        {
            _currentRequest.OnPathComplete?.Invoke(pathList,success);
            _isProcessingPath = false;
        }
    }

    

    struct PathRequest
    {
        public Vector2 startPosition;
        public Vector2 endPosition;
        public System.Action<List<Vector2>,bool> OnPathComplete;

        public PathRequest(Vector2 startPosition,Vector2 endPosition,System.Action<List<Vector2>,bool> OnPathComplete)
        {
            this.startPosition = startPosition;
            this.endPosition = endPosition;
            this.OnPathComplete = OnPathComplete;
        }
    }
}
