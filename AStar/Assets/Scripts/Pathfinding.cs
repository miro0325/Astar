using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using TMPro;

public class Pathfinding : MonoBehaviour
{
    [SerializeField]
    public GameObject target;
    public TextMeshPro textMeshPro;

    [SerializeField]
    private Grid grid;

    public Queue<Vector3> wayQueue = new();

    public static bool walkable = true;

    public float moveSpeed;
    public float delay;

    public bool isWalk;
    public bool isWalking;

    public int weight;

    private bool isSearch = false;

    private void Awake()
    {
        if(grid == null)
            grid = GameObject.FindObjectOfType<Grid>();
        walkable = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        isWalking = false;

        StartPathFind(transform.position, target.transform.position);
    }

    private void FixedUpdate()
    {
        Move();
    }
        

    public void StartPathFind(Vector3 startPos, Vector3 targetPos)
    {
        StopAllCoroutines();
        StartCoroutine(FindPath(startPos,targetPos));
    }

    private IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.GetWorldPointToNode(startPos);
        Node endNode = grid.GetWorldPointToNode(targetPos);
        isSearch = false;
        if(!startNode.walkable)
        {
            Debug.LogError("StartNode is not walkable");
            yield break;
        }
        if(endNode.walkable)
        {
            List<Node> openNodes = new();
            List<Node> closedNodes = new();

            openNodes.Add(startNode);

            while(openNodes.Count > 0)
            {
                Node curNode = openNodes[0];
                if (closedNodes.Contains(curNode)) continue;
                for (int i = 0; i < openNodes.Count; i++)
                {
                    if (openNodes[i].fCost <= curNode.fCost && openNodes[i].hCost < curNode.hCost)
                    {
                        curNode = openNodes[i];
                    }
                }
                openNodes.Remove(curNode);
                closedNodes.Add(curNode);
                grid.nodes.Add(curNode);
                if(curNode == endNode)
                {
                    if(!isSearch)
                    {
                        PushWay(RetracePath(startNode, endNode));
                    }
                    isSearch = true;
                    yield break;
                }

                foreach(Node neighbourNode in grid.GetNeighbourNodes(curNode))
                {
                    if (!neighbourNode.walkable || closedNodes.Contains(neighbourNode)) continue;
                    int fCost = curNode.gCost + GetDistance(curNode, neighbourNode);
                    if(fCost < neighbourNode.gCost || !openNodes.Contains(neighbourNode))
                    {
                        //neighbourNode.gCost = GetDistance(neighbourNode,startNode);
                        neighbourNode.gCost = fCost;
                        neighbourNode.hCost = GetDistance(neighbourNode, endNode);
                        neighbourNode.parentNode = curNode;
                        grid.nodes.Add(neighbourNode);
                        if(!openNodes.Contains(neighbourNode)) openNodes.Add(neighbourNode);
                    }
                }
                var text = Instantiate(textMeshPro, curNode.worldPos, Quaternion.Euler(90, 90, -180));
                text.text = $"{curNode.gCost}|{GetDistance(curNode, endNode)}";
                //text.text = $"{curNode.gCost + GetDistance(curNode, endNode)}";
                yield return new WaitForSeconds(delay);
            }
        }
        Debug.Log("End Finding");
    }

    private void PushWay(Vector3[] array)
    {
        wayQueue.Clear();
        foreach(Vector3 pos in array)
        {
            wayQueue.Enqueue(pos);
        }
    }

    private Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new();
        Node curNode = endNode;
        while(curNode != startNode)
        {
            path.Add(curNode);
            curNode = curNode.parentNode;
        }
        path.Reverse();
        grid.path = path;
        Vector3[] wayPoints = SimplifyPath(path);
        return wayPoints;
    }

    private Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> wayPoints = new();
        for(int i = 0; i < path.Count; i++)
        {
            wayPoints.Add(path[i].worldPos);
        }
        return wayPoints.ToArray();
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        int remaining = Mathf.Abs(distX - distY);
        int dist;
        //if(distX > distY) dist = 14 * distY + 10 * (distX - distY);
        //else dist = 14 * distX + 10 * (distY - distX);
        //return dist;
        //dist = (int)(weight * (distX + distY));
        dist = 14 * Mathf.Min(distX, distY) + 10 * remaining;
        return dist;
    }

    private void Move()
    {
        if (isSearch)
        {
            isWalking = true;
            if (wayQueue.Count > 0)
            {
                //var dir = wayQueue.First() - transform.position;
                //StartPathFind(transform.position, target.transform.position);
                //transform.Translate(dir.normalized * moveSpeed * Time.deltaTime);
                var pos = wayQueue.First();
                pos.y = transform.position.y;
                transform.position = Vector3.MoveTowards(transform.position, pos, moveSpeed * Time.deltaTime);
                if (transform.position == pos)
                {
                    wayQueue.Dequeue();
                }
            }
            isWalking = false;
        }
    }

   
}
