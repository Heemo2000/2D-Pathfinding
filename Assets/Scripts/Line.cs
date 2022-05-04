using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Line
{
    const float verticalLineGradient = 1e5f;
    
    
    private float gradient;
    private float yIntercept;

    private Vector2 pointOnLine1;
    private Vector2 pointOnLine2;
    
    private float perpendicularGradient;

    private bool approachSide;

    public float Gradient { get => gradient;}
    public float YIntercept { get => yIntercept;}
    public Vector2 PointOnLine { get => pointOnLine1;}

    public Line(Vector2 pointOnLine,Vector2 pointPerpendicularToLine)
    {
        float dx = pointOnLine.x - pointPerpendicularToLine.x;
        float dy = pointOnLine.y - pointPerpendicularToLine.y;

        if(dy == 0f)
        {
            perpendicularGradient = verticalLineGradient;
        }
        else
        {
            perpendicularGradient = dx/dy;
        }

        if(perpendicularGradient == 0f)
        {
            gradient = verticalLineGradient;
        }
        else
        {
            gradient = -1f/perpendicularGradient;
        }

        yIntercept = pointOnLine.y - gradient * pointOnLine.x;

        pointOnLine1 = pointOnLine;
        pointOnLine2 = pointOnLine + new Vector2(1,gradient);
        
        approachSide = false;
        approachSide = GetSide(pointPerpendicularToLine);
    }


    bool GetSide(Vector2 point) {
		return (point.x - pointOnLine1.x) * (pointOnLine2.y - pointOnLine1.y) > (point.y - pointOnLine1.y) * (pointOnLine2.x - pointOnLine1.x);
	}

    public bool HasCrossedLine(Vector2 point) {
		return GetSide (point) != approachSide;
	}

    public float DistanceFromPoint(Vector2 point)
    {
        float yPerpendicularIntercept = point.y - perpendicularGradient * point.x;
        float intersectX = (yPerpendicularIntercept - yIntercept)/(gradient - perpendicularGradient);
        float intersectY = gradient * intersectX + yIntercept;

        return Vector2.Distance(point, new Vector2(intersectX,intersectY));
    }
    public void DrawLine(float length,float pathShowingTime)
    {
        Vector2 lineDirection = new Vector2(1,gradient).normalized;
        Vector2 lineCenter = pointOnLine1 + Vector2.up;
        Debug.DrawLine(lineCenter - lineDirection * length/2f,lineCenter + lineDirection * length/2f,Color.white,pathShowingTime);
    }

}
