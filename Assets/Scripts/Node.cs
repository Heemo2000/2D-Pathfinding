using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    private Vector2Int positionInGrid;
    private int gCost;
    private int hCost;    
    private bool walkable;
    private int movementPenalty;
    private Node parentNode;


    public Node(Vector2Int positionInGrid,bool walkable,int movementPenalty)
    {
        this.positionInGrid = positionInGrid;
        this.walkable = walkable;
        this.movementPenalty = movementPenalty;
        this.parentNode = null;
    }

    public Vector2Int PositionInGrid { get => positionInGrid; set => positionInGrid = value; }
    public int GCost { get => gCost; set => gCost = value; }
    public int HCost { get => hCost; set => hCost = value; }

    public int FCost { get =>gCost + hCost;}
    public Node ParentNode { get => parentNode; set => parentNode = value; }
    public bool Walkable { get => walkable; set => this.walkable = value;}
    public int MovementPenalty { get => movementPenalty; set => movementPenalty = value; }
}
