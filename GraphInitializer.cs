using UnityEngine;

public class GraphInitializer : MonoBehaviour
{
    public Graph graph;
    public GameObject UAMVehicle;

    void Start()
    {
        BuildingTopNodeCreator nodeCreator = GetComponent<BuildingTopNodeCreator>();
        if (nodeCreator != null)
        {
            graph = nodeCreator.graph;
        }
        else
        {
            Debug.LogError("NodeCreator not found.");
        }
    }
}
