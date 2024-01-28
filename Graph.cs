using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    public List<Node> nodes = new List<Node>();
    public float MaxLayerHeight { get; private set; }
    public Dictionary<Node, int> trafficDensity = new Dictionary<Node, int>();

    public void SetMaxLayerHeight(float maxHeight)
    {
        MaxLayerHeight = maxHeight;
    }

    public void UpdateTrafficDensity(Node node)
    {
        if (!trafficDensity.ContainsKey(node))
        {
            trafficDensity[node] = 0;
        }
        trafficDensity[node]++;
    }

    public Node AddNode(Vector3 position, float maxHeight, int numberOfLayers)
    {
        Node newNode = new Node();
        newNode.Initialize(position, maxHeight, numberOfLayers);
        nodes.Add(newNode);
        return newNode;
    }

    public void AddEdge(Node from, Node to)
    {
        if (!from.neighbors.Contains(to))
        {
            from.neighbors.Add(to);
        }
    }

    public List<Node> FindPathAStar(Node start, Node end)
    {
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(start);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == end)
            {
                return RetracePath(start, end);
            }

            foreach (Node neighbor in currentNode.neighbors)
            {
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                float newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, end);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return new List<Node>(); // Return empty path if no path is found
    }

    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }

    private float GetDistance(Node nodeA, Node nodeB)
    {
        return Vector3.Distance(nodeA.Position, nodeB.Position);
    }
}

public class Node
{
    public Vector3 Position { get; private set; }
    public List<Node> neighbors;
    public int Layer { get; private set; }
    public float gCost, hCost;
    public Node parent;
    public Renderer QuadRenderer; // For HeatmapVisualizer

    public float fCost { get { return gCost + hCost; } }

    public Node()
    {
        neighbors = new List<Node>();
    }

    public void Initialize(Vector3 position, float maxHeight, int numberOfLayers)
    {
        Position = position;
        Layer = DetermineLayer(position.y, maxHeight, numberOfLayers);
    }

    private int DetermineLayer(float height, float maxHeight, int numberOfLayers)
    {
        int layer = (int)Mathf.Floor((height / maxHeight) * numberOfLayers);
        return Mathf.Clamp(layer, 0, numberOfLayers - 1);
    }
}
