using UnityEngine;
using System.Collections.Generic;

public class UAMMovement : MonoBehaviour
{
    private bool trailExist = true;
    public UIManager uiMan;
    private float currentSpeed;
    private int currentLayer;
    public LayerInfoManager layInfoMan;


    public Graph graph;
    public List<Node> path;
    private int currentPathIndex;
    public float speed = 150.0f;
    private float targetLayerHeight;
    private bool isAdjustingHeight = false;
    public LayerMask obstacleLayer; // Layer to check for obstacles like buildings
    public LayerMask uamLayer; // Layer to check for other UAMs
    public float checkDistance = 10.0f; // Distance to check for potential collisions
    public float avoidanceHeight = 5.0f; // Height adjustment for collision avoidance
    private TrailRenderer trailRenderer; // Trail renderer component for visualization

    private void Awake()
    {
        // Check if a TrailRenderer already exists on this GameObject.
        trailRenderer = gameObject.GetComponent<TrailRenderer>();
        if (trailRenderer == null)
        {
            trailRenderer = gameObject.AddComponent<TrailRenderer>();
        }

        // Set up the TrailRenderer properties.
        trailRenderer.startWidth = 1.0f; // Increased width
        trailRenderer.endWidth = 0.5f; // Increased width
        trailRenderer.time = Mathf.Infinity; // The trail doesn't fade

        // Ensure the shader "Sprites/Default" exists or use an existing material.
        Material trailMaterial = new Material(Shader.Find("Sprites/Default"));
        if (trailMaterial.shader == null)
        {
            Debug.LogError("Shader not found. Using default material.");
            trailMaterial = new Material(Shader.Find("Standard")); // Fallback to a standard shader
        }
        trailRenderer.material = trailMaterial;

        // Initialize the layerColors list.
        layerColors = new List<Color>
    {
        Color.red, Color.blue, Color.green, Color.yellow,
        Color.magenta, Color.cyan, Color.grey,
        Color.black
    };

        // Initialize the first color based on the initial layer.
        // This is a placeholder. Update it according to your specific requirements.
        if (graph != null && graph.MaxLayerHeight > 0 && path != null && path.Count > 0)
        {
            UpdateTrailColor(path[0].Layer);
        }
    }


    private void Start()
    {
        
        // Initialize the first color based on the initial layer
        if (graph != null && graph.MaxLayerHeight > 0 && path != null && path.Count > 0)
        {
            UpdateTrailColor(path[0].Layer);
        }
        speed = Random.Range(20.0f, 200.0f);
    }

    private List<Color> layerColors = new List<Color>
        {
            Color.red, Color.blue, Color.green, Color.yellow,
            Color.magenta, Color.cyan, Color.grey,
            Color.black
        };

    private void Update()
    {
        trailExist = uiMan.trailExist;
        if(!trailExist){
            trailRenderer.enabled = false;  
        }
        else{
            trailRenderer.enabled = true;
        }
        if (path != null && path.Count > 0)
        {
            if (isAdjustingHeight)
            {
                AdjustToTargetLayerHeight();
            }
            else if (CheckForObstacle())
            {
                AdjustPathForObstacle();
            }
            else if (CheckForUAMCollision())
            {
                AdjustPathForUAMCollision();
            }
            else
            {
                MoveAlongPath();
            }
        }
    }

    private bool CheckForObstacle()
    {
        // Collision check with obstacles like buildings
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, checkDistance, obstacleLayer))
        {
            Debug.Log(gameObject.name + " detected an obstacle: " + hit.transform.name);
            return true;
        }
        return false;
    }

    private void AdjustPathForObstacle()
    {
        // Adjust path when an obstacle is detected
        Debug.Log(gameObject.name + " adjusting path for obstacle");
        ChooseNewDestination();
    }

    private bool CheckForUAMCollision()
    {
        // Collision check with other UAMs
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, checkDistance, uamLayer))
        {
            Debug.Log(gameObject.name + " detected another UAM: " + hit.transform.name);
            return true;
        }
        return false;
    }

    private void AdjustPathForUAMCollision()
    {
        // Adjust path when a potential collision with another UAM is detected
        Debug.Log(gameObject.name + " adjusting path for UAM collision");
        targetLayerHeight += avoidanceHeight;
        targetLayerHeight = Mathf.Clamp(targetLayerHeight, 0, graph.MaxLayerHeight);
        isAdjustingHeight = true;
    }

    private void MoveAlongPath()
    {
        if (currentPathIndex < path.Count)
        {
            Vector3 targetPosition = path[currentPathIndex].Position; // Use Position

            // Move towards the target position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // Calculate the rotation towards the target position
            if (targetPosition != transform.position) // Ensure that targetPosition and current position are not the same
            {
                Vector3 direction = (targetPosition - transform.position).normalized;
                direction = new Vector3(direction.x, 0, direction.z); // Keep the y-axis rotation unchanged
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                // Adjust for the initial rotation offset of the prefab
                // Rotating -90 degrees around the y-axis (opposite direction)
                Quaternion initialRotationOffset = Quaternion.Euler(-90, -90, 0);
                targetRotation *= initialRotationOffset;

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * speed);
            }

            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                // Clear the trail when reaching the node
                trailRenderer.Clear();

                currentPathIndex++;
                if (currentPathIndex < path.Count)
                {
                    SetTargetLayerHeight(path[currentPathIndex]);
                }
                else if (currentPathIndex >= path.Count)
                {
                    ChooseNewDestination();
                }
            }
            currentSpeed = speed * Time.deltaTime;
            currentLayer = path[currentPathIndex].Layer;
        }
    }

    public void SetPath(List<Node> newPath, Graph newGraph)
    {
        path = newPath;
        graph = newGraph;
        currentPathIndex = 0;
        if (path.Count > 0)
        {
            SetTargetLayerHeight(path[0]);
        }
    }

    private void SetTargetLayerHeight(Node node)
    {
        targetLayerHeight = node.Position.y; // Use the Position property of Node
        isAdjustingHeight = true;

        // Update trail color based on new layer
        UpdateTrailColor(node.Layer);
    }

    private void AdjustToTargetLayerHeight()
    {
        if (!Mathf.Approximately(transform.position.y, targetLayerHeight))
        {
            Vector3 verticalTargetPosition = new Vector3(transform.position.x, targetLayerHeight, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, verticalTargetPosition, speed * Time.deltaTime);
        }
        else
        {
            isAdjustingHeight = false;
        }
    }

    private void ChooseNewDestination()
    {
        layInfoMan.totNodesHit += 1;
        if (graph == null || graph.nodes == null || graph.nodes.Count <= 1)
        {
            Debug.LogError("Graph is not initialized or does not have enough nodes.");
            return;
        }

        Node currentDestination;
        if (path != null && path.Count > 0 && currentPathIndex > 0)
        {
            currentDestination = path[currentPathIndex - 1];
        }
        else
        {
            currentDestination = graph.nodes[Random.Range(0, graph.nodes.Count)];
        }

        int randomEndIndex = Random.Range(0, graph.nodes.Count);
        while (randomEndIndex == graph.nodes.IndexOf(currentDestination) && graph.nodes.Count > 1)
        {
            randomEndIndex = Random.Range(0, graph.nodes.Count);
        }

        List<Node> newPath = graph.FindPathAStar(currentDestination, graph.nodes[randomEndIndex]);
        if (newPath != null && newPath.Count > 0)
        {
            SetPath(newPath, graph);
        }
        else
        {
            Debug.Log("No valid new path found. Retrying...");
        }
    }

    private void UpdateTrailColor(int layer)
    {

        //new
        
        //
        //Debug.Log(trailExist + "");
        if(trailExist){
        if (trailRenderer == null)
        {
            
            Debug.LogError("TrailRenderer is not initialized.");
            return;
        }
        if (layerColors == null || layer < 0 || layer >= layerColors.Count)
        {
            Debug.LogError("Invalid layer index or layerColors not initialized.");
            return;
        }

        Debug.Log("Start : " + trailRenderer.startColor);
        Debug.Log("End : " + trailRenderer.endColor);

        trailRenderer.startColor = layerColors[layer];
        trailRenderer.endColor = new Color(layerColors[layer].r, layerColors[layer].g, layerColors[layer].b, 0.5f); // Slightly transparent
        }
    }
    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    public int GetCurrentLayer()
    {
        return currentLayer;
    }
    public void setSpeedMuilti(float speed){
        this.speed = speed;
    }

}