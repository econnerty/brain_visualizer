using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;  // Add this line
using System;

public class BrainConnectivityVisualizer : MonoBehaviour
{
    private string csvFilePath = "coh_eo_mean_df"; // Set this in the Inspector
    private TextAsset csvFile;
// Then parse csvContent as needed

    // Add a field to specify the number of top connections to draw

    private bool isDirected = false; // True for non-symmetrical, false for symmetrical
    public Material lineMaterial;
    public Material nonDirectedMaterial;
    private int numberOfTopConnections = 0;

    float maxValue = 0f;

    private List<GameObject> lineGameObjects = new List<GameObject>();

    private Dictionary<string, GameObject> brainRegions = new Dictionary<string, GameObject>();


    void Start()
    {
        this.csvFile = Resources.Load<TextAsset>(this.csvFilePath);
        ParseAdjacencyMatrix();
        LoadBrainRegions();
        DrawConnections();
    }

    public void UpdateVisualization(string csvName,bool directed)
    {
        // Construct the full path to the CSV file
        this.csvFilePath = csvName;
        this.csvFile = Resources.Load<TextAsset>(this.csvFilePath);
        this.isDirected = directed;
        ClearExistingLines();
        ParseAdjacencyMatrix();
        LoadBrainRegions();
        DrawConnections();

    }
    public void UpdateVisualization(int numberOfConnections)
    {
        // Clear existing lines before drawing new ones
        // Update the number of connections
        this.numberOfTopConnections = numberOfConnections;

        // Code to re-render or update the visualization
        ClearExistingLines();
        ParseAdjacencyMatrix();
        LoadBrainRegions();
        DrawConnections();
    }

    void ClearExistingLines()
    {
        foreach (GameObject lineObj in lineGameObjects)
        {
            if (lineObj != null)
                Destroy(lineObj);
        }
        lineGameObjects.Clear(); // Clear the list after destroying the objects
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
        //string[] lines = File.ReadAllLines(csvFilePath);
        //string[] headers = lines[0].Split(',');
        string[] lines = this.csvFile.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        
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
        //string[] lines = File.ReadAllLines(csvFilePath);
        //string[] headers = lines[0].Split(',');
        string[] lines = csvFile.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
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
            //Debug.Log("Start: " + connection.start.name + ", End: " + connection.end.name + ", Connectivity: " + connection.connectivity);
        }
    }

    void DrawLineBetween(GameObject start, GameObject end, float connectivity)
    {
        LineRenderer lineRenderer = new GameObject("Line").AddComponent<LineRenderer>();
        lineGameObjects.Add(lineRenderer.gameObject);
        lineRenderer.material = isDirected ? new Material(lineMaterial) : new Material(nonDirectedMaterial);

        // Define the minimum and maximum widths
        float minWidth = 0.2f;
        float maxWidth = .8f;
        int maxConnections = 4500; // Maximum expected number of connections

        // Calculate the normalized position between 0 and 1
        float normalized = Mathf.Clamp((numberOfTopConnections - 1) / (float)(maxConnections - 1), 0f, 1f);

        // Interpolate the line width
        float lineWidth = Mathf.Lerp(maxWidth, minWidth, normalized);
        //Debug.Log("Line width: " + lineWidth);

        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start.transform.position);
        lineRenderer.SetPosition(1, end.transform.position);
        lineRenderer.material.mainTextureScale = new Vector2(2, 2); // Adjust as needed


        float power = isDirected ? .2f : .75f; // Square root transformation; adjust as needed
        float normalizedConnectivity = maxValue > 0 ? Mathf.Pow(connectivity / maxValue, power) : 0;

        // Determine color based on normalized connectivity
        Color lineColor = Color.Lerp(Color.blue, Color.red, normalizedConnectivity);
        lineColor.a = .5f;
        lineRenderer.material.SetColor("_Color", lineColor); // Set the main material color
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;

        // Adjust texture mode for scrolling
        //lineRenderer.textureMode = LineTextureMode.Tile;
        //lineRenderer.alignment = LineAlignment.TransformZ;


        //Debug.Log("Connectivity: " + connectivity + ", Color: " + lineColor + ", Normalized: " + normalizedConnectivity);
    }

}
