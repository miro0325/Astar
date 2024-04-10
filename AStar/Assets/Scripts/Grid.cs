using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public bool displayGridGizmos;

    public LayerMask obstacle;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    private Node[,] grid;

    public List<Node> nodes = new();

    private float nodeDiameter;

    private int gridSizeX, gridSizeY;

    [SerializeField]
    private Pathfinding player;

    public List<Node> path;

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        for(int x = 0; x < gridSizeX; x++)
        {
            for(int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + ((Vector3.right  * (x * nodeDiameter + nodeRadius)) + (Vector3.forward * (y * nodeDiameter + nodeRadius)));
                bool walkable = !(Physics.CheckBox(worldPoint,Vector3.one * nodeRadius,Quaternion.identity,obstacle));
                grid[x,y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public List<Node> GetNeighbourNodes(Node node)
    {
        List<Node> neighbours = new List<Node>();   
        
        for(int x = -1; x <= 1; x++)
        {
            for(int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;
                int pointX = node.gridX + x;
                int pointY = node.gridY + y;
                if(IsInGrid(pointX,pointY))
                {
                    //Debug.Log(x + " " + y);
                    if (!grid[node.gridX, pointY].walkable && !grid[pointX, node.gridY].walkable) continue;
                    if (!grid[node.gridX, pointY].walkable || !grid[pointX, node.gridY].walkable) continue;

                    neighbours.Add(grid[pointX,pointY]);
                }
            }
        }
        return neighbours;
    }

    public Node GetWorldPointToNode(Vector3 worldPos)
    {
        float percentX = (worldPos.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPos.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        //int x, y;
        //x = Mathf.RoundToInt((worldPos.x * gridWorldSize.x) / nodeRadius);
        //y = Mathf.RoundToInt((worldPos.z * gridWorldSize.y) / nodeRadius);
        return grid[x, y];
    }


    public bool IsInGrid(int x, int y)
    {
        if(x >= 0 && x < gridSizeX && y >= 0 && y < gridSizeY)
        {
            return true;
        }
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector2Int GetSize()
    {
        return new Vector2Int(gridSizeX, gridSizeY);
    }

    private void OnDrawGizmos()
    {
        if (!displayGridGizmos) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if(grid != null)
        {
            
            Node playerNode = GetWorldPointToNode(player.transform.position);
            Node endNode = GetWorldPointToNode(player.target.transform.position);
            foreach(Node node in grid)
            {
                
                //Debug.Log("test");
                Gizmos.color = (node.walkable) ? new Color(1,1,1,0.3f) : new Color(1,0,0,0.3f);
                //if(!node.walkable)
                if (nodes.Contains(node))
                {
                    Gizmos.color = new Color(0.8f, 0f, 0.8f, 0.3f);
                }
                if (path != null)
                {
                    if(path.Contains(node))
                    {
                        Gizmos.color = new Color(1,1,0,0.3f);
                    }

                }

                if(node == playerNode || node == endNode)
                {
                    Gizmos.color = new Color(0, 0, 1, 0.3f);
                }
                //Debug.Log(node.worldPos);
               
                Gizmos.DrawCube(node.worldPos, new Vector3(1, 1, 1) * (nodeDiameter - 0.1f));
                
            }
           

        }
    }
}
