using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{

    public bool walkable;
    public Vector3 worldPos;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public int fCost
    {
        get { return gCost + hCost; }
    }

    public Node parentNode;
    public Node prevNode;

    public Node (bool walkable,Vector3 worldPos,int gridX, int gridY)
    {
        this.walkable = walkable;
        this.worldPos = worldPos;
        this.gridX = gridX;
        this.gridY = gridY;
    }
   
}
