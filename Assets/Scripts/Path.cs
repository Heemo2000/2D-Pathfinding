using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    private List<Vector2> wayPoints;
    private List<Line> turnBoundaries;
    public List<Line> TurnBoundaries { get => turnBoundaries;}
    public List<Vector2> WayPoints { get => wayPoints;}

    public Path(List<Vector2> wayPoints,Vector2 startPosition,float turnDistance)
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
