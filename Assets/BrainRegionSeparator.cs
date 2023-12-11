using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;  // Add this line

public class BrainConnectivityVisualizer : MonoBehaviour
{
    public string csvFilePath; // Set this in the Inspector
    // Add a field to specify the number of top connections to draw

    public bool isDirected = true; // True for non-symmetrical, false for symmetrical
    public Material lineMaterial;
    public int numberOfTopConnections = 10;

    float maxValue = 0f;

    private Dictionary<string, GameObject> brainRegions = new Dictionary<string, GameObject>();

    void Start()
    {
        ParseAdjacencyMatrix();
        LoadBrainRegions();
        DrawConnections();
    }

    public struct Connection
    {
        public GameObject start;
        public GameObject end;
        public float connectivity;

        public Connection(GameObject start, GameObject end, float connectivity)
        {
            this.start = start;
            this.end = end;
            this.connectivity = connectivity;
        }
    }


    void LoadBrainRegions()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (var obj in allObjects)
        {
            // Assuming all your brain region GameObjects are tagged "BrainRegion"
            if (obj.tag == "BrainRegion")
            {
                brainRegions[obj.name] = obj;
            }
        }
    }



    void ParseAdjacencyMatrix()
    {
        string[] lines = File.ReadAllLines(csvFilePath);
        string[] headers = lines[0].Split(',');

        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');
            for (int j = 1; j < headers.Length; j++)
            {
                float connectivity = float.Parse(values[j]);
                if (connectivity > maxValue)
                {
                    maxValue = connectivity;
                }
            }
        }
    }

    void DrawConnections()
    {
        string[] lines = File.ReadAllLines(csvFilePath);
        string[] headers = lines[0].Split(',');

        List<Connection> allConnections = new List<Connection>();

        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');
            int startIndex = isDirected ? 1 : i; // Adjust start index based on symmetry

            for (int j = startIndex; j < headers.Length; j++)
            {
                float connectivity = float.Parse(values[j]);
                if (connectivity > 0)
                {
                    allConnections.Add(new Connection(brainRegions[headers[i - 1]], brainRegions[headers[j]], connectivity));
                }
            }
        }

        // Sort all connections by connectivity in descending order and take the top 'n'
        var topConnections = allConnections.OrderByDescending(c => c.connectivity).Take(numberOfTopConnections);

        foreach (var connection in topConnections)
        {
            DrawLineBetween(connection.start, connection.end, connection.connectivity);
        }
    }

    void DrawLineBetween(GameObject start, GameObject end, float connectivity)
    {
        LineRenderer lineRenderer = new GameObject("Line").AddComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;//new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        lineRenderer.startWidth = 0.15f;
        lineRenderer.endWidth = 0.15f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start.transform.position);
        lineRenderer.SetPosition(1, end.transform.position);

        // Normalize the connectivity value
        float normalizedConnectivity = maxValue > 0 ? connectivity / maxValue : 0;

        // Adjust color based on connectivity
        Color lineColor = Color.Lerp(Color.blue, Color.red, normalizedConnectivity);
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;

        Debug.Log("Connectivity: " + connectivity + ", Color: " + lineColor + ", Normalized: " + normalizedConnectivity);
    }

}
