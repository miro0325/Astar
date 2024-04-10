using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

[System.Serializable]
public class Edge : IComparable<Edge>
{
    public float cost;
    public Node node;

    public Edge(Node node = null, float cost = 1f)
    {
        this.node = node;
        this.cost = cost;
    }

    public int CompareTo(Edge other)
    {
        float result = cost - other.cost;
        int a = node.GetHashCode();
        int b = other.node.GetHashCode();
        if(a == b)
            return 0;
        return (int)result;
    }

    public override bool Equals(object obj)
    {
        Edge other = obj as Edge;
        return (other.node.Equals(this.node));
    }

    public override int GetHashCode()
    {
        return this.node.GetHashCode();
    }
}

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
