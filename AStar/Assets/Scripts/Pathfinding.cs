using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class Pathfinding : MonoBehaviour
{
    [SerializeField]
    public GameObject target;

    [SerializeField]
    private Grid grid;

    public Queue<Vector3> wayQueue = new();

    public static bool walkable = true;

    public float moveSpeed;
    public float range;

    public bool isWalk;
    public bool isWalking;

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
        moveSpeed = 20;
        range = 4;
    }

    private void FixedUpdate()
    {
        StartPathFind(transform.position, target.transform.position);
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

        bool isArrive = false;
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
                for(int i = 0; i < openNodes.Count; i++)
                {
                    if(openNodes[i].fCost <= curNode.fCost && openNodes[i].hCost < curNode.hCost)
                    {
                        curNode = openNodes[i];
                    }
                }
                openNodes.Remove(curNode);
                closedNodes.Add(curNode);
                if(curNode == endNode)
                {
                    if(!isArrive)
                    {
                        PushWay(RetracePath(startNode, endNode));
                    }
                    isArrive = true;
                    break;
                }
                foreach(Node neighbourNode in grid.GetNeighbourNodes(curNode))
                {
                    if (!neighbourNode.walkable || closedNodes.Contains(neighbourNode)) continue;
                    int fCost = curNode.gCost + GetDistance(curNode, neighbourNode);
                    if(fCost < neighbourNode.gCost || !openNodes.Contains(neighbourNode))
                    {
                        neighbourNode.gCost = fCost;
                        neighbourNode.hCost = GetDistance(neighbourNode, endNode);
                        neighbourNode.parentNode = curNode;
                        if(!openNodes.Contains(neighbourNode)) openNodes.Add(neighbourNode);
                    }
                }
            }
        }

        yield return null;
        if(isArrive)
        {
            isWalking = true;
            while(wayQueue.Count > 0)
            {
                var dir = wayQueue.First() - transform.position;
                transform.Translate(dir.normalized * moveSpeed * Time.deltaTime);
                if(transform.position == wayQueue.First())
                {
                    wayQueue.Dequeue();
                }
                yield return null;
            }
            isWalking = false;
        }
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
        if(distX > distY) return 14 * distY + 10 * (distX - distY);
        else return 14 * distX + 10 * (distY - distX);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
