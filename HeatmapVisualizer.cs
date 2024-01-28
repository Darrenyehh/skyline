using UnityEngine;
using System.Collections.Generic;

public class HeatmapVisualizer : MonoBehaviour
{
    public Graph graph;
    public Material heatmapMaterial;

    private void Start()
    {
        // Check if the graph is assigned and initialized
        if (graph == null)
        {
            Debug.LogError("Graph is not assigned to HeatmapVisualizer.");
            return;
        }

        if (graph.nodes == null || graph.nodes.Count == 0)
        {
            Debug.LogError("Graph nodes are not initialized.");
            return;
        }

        foreach (var node in graph.nodes)
        {
            CreateHeatmapQuad(node);
        }
    }

    void CreateHeatmapQuad(Node node)
    {
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.transform.position = node.Position + Vector3.down * 0.1f;
        quad.transform.localScale = new Vector3(1, 1, 1);

        Renderer quadRenderer = quad.GetComponent<Renderer>();
        quadRenderer.material = new Material(heatmapMaterial);
        node.QuadRenderer = quadRenderer; // Set the Renderer reference in Node

        UpdateQuadColor(quadRenderer, node);
    }

    void UpdateQuadColor(Renderer renderer, Node node)
    {
        if (graph.trafficDensity != null && graph.trafficDensity.TryGetValue(node, out int density))
        {
            float intensity = Mathf.InverseLerp(0, 100, density);
            renderer.material.color = Color.Lerp(Color.green, Color.red, intensity);
        }
    }

    private void Update()
    {
        if (graph == null || graph.nodes == null || graph.nodes.Count == 0)
        {
            return;
        }

        foreach (var node in graph.nodes)
        {
            if (node.QuadRenderer != null) // Check if the Renderer is set
            {
                UpdateQuadColor(node.QuadRenderer, node);
            }
        }
    }
}
