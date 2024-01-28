using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BuildingTopNodeCreator : MonoBehaviour
{
    public Graph graph;
    public string buildingsParentName = "NYBlock01/Buildings";
    public GameObject UAMVehiclePrefab;
    public int numberOfVehicles = 10;
    public int numberOfTallestBuildings = 5;
    public int numberOfLayers = 3;
    public float layerHeight = 20.0f;
    public float maxHeight;

    void Start()
    {
        graph = GetComponent<Graph>();
        if (graph == null)
        {
            Debug.LogError("Graph component not found on the GameObject.");
            return;
        }

        maxHeight = CalculateMaxHeight();
        CreateNodesAtTallestBuildings(maxHeight);
        InitializeUAMVehicles();
    }

    private float CalculateMaxHeight()
    {
        Transform buildingsParent = GameObject.Find(buildingsParentName).transform;
        float maxHeight = 0f;
        foreach (Transform building in buildingsParent)
        {
            float buildingHeight = GetHighestPoint(building).y;
            if (buildingHeight > maxHeight)
                maxHeight = buildingHeight;
        }
        return maxHeight;
    }

    public void CreateNodesAtTallestBuildings(float maxHeight)
    {
        Transform buildingsParent = GameObject.Find(buildingsParentName).transform;
        List<Transform> buildings = new List<Transform>();
        foreach (Transform building in buildingsParent)
        {
            buildings.Add(building);
        }

        var sortedBuildings = buildings.OrderByDescending(b => CalculateBuildingHeight(b)).Take(numberOfTallestBuildings);

        for (int layer = 0; layer < numberOfLayers; layer++)
        {
            //Debug.Log(layer);
            foreach (var building in sortedBuildings)
            {
                Vector3 nodePosition = GetHighestPoint(building) + Vector3.up * (layer * layerHeight);
                GameObject nodeObj = new GameObject($"Node_{building.name}_Layer{layer}");
                nodeObj.transform.position = nodePosition;

                Node newNode = graph.AddNode(nodePosition, maxHeight, numberOfLayers);

                foreach (var otherNode in graph.nodes)
                {
                    if (otherNode != newNode)
                    {
                        graph.AddEdge(newNode, otherNode);
                        graph.AddEdge(otherNode, newNode);
                    }
                }
            }
        }
    }

    public void InitializeUAMVehicles(int startIndex = 0)
    {
        //Debug.Log("Initializing UAM Vehicles from index: " + startIndex);
        if (graph.nodes.Count > 0){
        for (int i = startIndex; i < numberOfVehicles; i++)
        {
            //Debug.Log("Graph Node Count: " + graph.nodes.Count);

            GameObject newVehicle = Instantiate(UAMVehiclePrefab);
            newVehicle.name = $"UAMVehicle_{i}";
            SetVehicleInitialPath(newVehicle);
        }
        }
    }


    void SetVehicleInitialPath(GameObject vehicle)
    {
        if (graph.nodes.Count > 0)
        {
            int randomStartIndex = Random.Range(0, graph.nodes.Count);
            vehicle.transform.position = graph.nodes[randomStartIndex].Position;

            int randomEndIndex = Random.Range(0, graph.nodes.Count);
            while (randomEndIndex == randomStartIndex)
            {
                randomEndIndex = Random.Range(0, graph.nodes.Count);
            }

            List<Node> path = graph.FindPathAStar(graph.nodes[randomStartIndex], graph.nodes[randomEndIndex]);
            vehicle.GetComponent<UAMMovement>().SetPath(path, graph);
        }
    }

    Vector3 GetHighestPoint(Transform building)
    {
        var renderers = building.GetComponentsInChildren<Renderer>();
        Bounds bounds = new Bounds(building.position, Vector3.zero);

        foreach (var renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }

        return new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
    }

    float CalculateBuildingHeight(Transform building)
    {
        var renderers = building.GetComponentsInChildren<Renderer>();
        Bounds bounds = new Bounds(building.position, Vector3.zero);

        foreach (var renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }

        return bounds.size.y;
    }
}
