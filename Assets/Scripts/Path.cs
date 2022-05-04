using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    private List<Vector2> wayPoints;
    private List<Line> turnBoundaries;
    private int slowDownIndex;
    
    public List<Line> TurnBoundaries { get => turnBoundaries;}
    public List<Vector2> WayPoints { get => wayPoints;}
    public int SlowDownIndex { get => slowDownIndex; }

    public Path(List<Vector2> wayPoints,Vector2 startPosition,float turnDistance,float slowDownDistance)
    {
        this.wayPoints = wayPoints;
        this.turnBoundaries = new List<Line>(wayPoints.Count);

        Vector2 previousPoint = startPosition;
        for(int i = 0; i < wayPoints.Count; i++)
        {
            Vector2 currentPoint = wayPoints[i];
            Vector2 currentPointDirection = (currentPoint - previousPoint).normalized;
            Vector2 turnBoundaryPoint = (i == wayPoints.Count - 1) ? currentPoint : currentPoint - currentPointDirection * turnDistance;
            Line newLine = new Line(currentPoint,previousPoint- currentPointDirection * turnDistance);
            this.turnBoundaries.Add(newLine);
            previousPoint = turnBoundaryPoint;
        }

        float currentDistance = 0f;
        for(int i = wayPoints.Count - 1; i > 0 ; i--)
        {
            currentDistance += Vector2.Distance(wayPoints[i],wayPoints[i-1]);
            if(currentDistance > slowDownDistance)
            {
                slowDownIndex = i;
                break;
            }
        }
    }

    public void DrawPath(float pathShowingTime)
    {
        Gizmos.color = Color.white;
        
        foreach(Line line in turnBoundaries)
        {
            line.DrawLine(1f,pathShowingTime);
        }
    }
}
