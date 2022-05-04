using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Pathfinding Data", fileName = "Pathfinding Data")]
public class PathfindingData : ScriptableObject
{
    
    public int width = 1;
    public int height = 1;
    
    public float cellSize = 0.5f;

    public bool shouldUsePenaltyCost;
    public TerrainType[] terrainTypes;

    public int obstaclePenaltyCost = 10;
    
}
