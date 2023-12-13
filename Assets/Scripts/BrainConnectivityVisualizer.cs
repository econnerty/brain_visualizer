using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;  // Add this line
using System;

public class BrainConnectivityVisualizer : MonoBehaviour
{
    private string csvFilePath = "ddtf_eo_mean_df"; // Set this in the Inspector
    private TextAsset csvFile;
// Then parse csvContent as needed

    // Add a field to specify the number of top connections to draw

    private bool isDirected = true; // True for non-symmetrical, false for symmetrical
    public Material lineMaterial;
    public Material nonDirectedMaterial;
    private int numberOfTopConnections = 0;

    float maxValue = 0f;

    private float mean = 0f;
    private float sumOfSquaredDifferences = 0f;

    private float std = 0f;
    private int count = 0;

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
        this.mean = 0f;
        this.std = 0f;
        this.sumOfSquaredDifferences = 0f;
        this.count = 0;
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
        this.mean = 0f;
        this.std = 0f;
        this.sumOfSquaredDifferences = 0f;
        this.count = 0;
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
            for (int j = 0; j < headers.Length; j++)
            {
                if ((i-1) == j)
                {
                    continue;
                }
                float connectivity = float.Parse(values[j]);
                this.count++;

                this.mean += connectivity;
                if (connectivity > maxValue)
                {
                    maxValue = connectivity;
                }
            }
        }
        this.mean /= this.count;

        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');
            for (int j = 0; j < headers.Length; j++)
            {
                if ((i-1) == j)
                {
                    continue;
                }
                float connectivity = float.Parse(values[j]);
                this.sumOfSquaredDifferences += Mathf.Pow(connectivity - this.mean, 2);
            }
        }
        this.std = Mathf.Sqrt(sumOfSquaredDifferences / count);
    }

    float NormalizeValue(float value)
    {
        // Z-score normalization
        return (value - mean) / this.std;
    }


    void DrawConnections()
    {
        //string[] lines = File.ReadAllLines(csvFilePath);
        //string[] headers = lines[0].Split(',');
        string[] lines = csvFile.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        string[] headers = lines[0].Split(',');

        List<Connection> allConnections = new List<Connection>();

        for (int i = 1; i < lines.Length; i++) // Start from 1 to skip the header
        {
            string[] values = lines[i].Split(',');

            for (int j = 0; j < headers.Length; j++) // Iterate over all headers/columns
            {
                if (i - 1 != j) // Always exclude the diagonal (self-connectivity)
                {
                    if (isDirected || i > j) // For undirected, only take lower triangular matrix
                    {
                        float connectivity;
                        if (float.TryParse(values[j], out connectivity) && connectivity > 0)
                        {
                            allConnections.Add(new Connection(brainRegions[headers[i - 1]], brainRegions[headers[j]], connectivity));
                        }
                    }
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
        float maxWidth = .6f;
        int maxConnections = 4700; // Maximum expected number of connections

        // Calculate the normalized position between 0 and 1
        float normalized = Mathf.Clamp((numberOfTopConnections - 1) / (float)(maxConnections - 1), 0f, 1f);

        // Interpolate the line width
        float lineWidth = Mathf.Lerp(maxWidth, minWidth, normalized);
        //Debug.Log("Line width: " + lineWidth);

        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        //lineRenderer.positionCount = 2;
        //lineRenderer.SetPosition(0, start.transform.position);
        //lineRenderer.SetPosition(1, end.transform.position);
        lineRenderer.material.mainTextureScale = new Vector2(2, 2); // Adjust as needed

        // Set up Bézier curve points
        Vector3 startPoint = start.transform.position;
        Vector3 endPoint = end.transform.position;
        Vector3 controlPoint1 = CalculateControlPoint(startPoint, endPoint, true);
        //Vector3 controlPoint2 = CalculateControlPoint(startPoint, endPoint, false);



        //float power = isDirected ? .1f : .5f; // Square root transformation; adjust as needed
        float normalizedConnectivity = NormalizeValue(connectivity);
        normalizedConnectivity = (normalizedConnectivity + 1) / 2; // Adjusting from [-1, 1] to [0, 1]
        normalizedConnectivity = Mathf.Clamp(normalizedConnectivity, 0f, 1f); // Ensuring it stays within [0, 1]

        // Determine color based on normalized connectivity
        Color lineColor = Color.Lerp(Color.blue, Color.red, normalizedConnectivity);
        lineColor = lineColor * 0.8f; // Increase the intensity of the color
        lineColor.a = .7f;
        lineRenderer.material.SetColor("_Color", lineColor); // Set the main material color
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;

         // Set the number of points on the line
        int pointsCount = 20; // More points for a smoother curve
        Vector3[] curvePoints = new Vector3[pointsCount];

        for (int i = 0; i < pointsCount; i++)
        {
            float t = i / (float)(pointsCount - 1);
            curvePoints[i] = CalculateBezierPoint(t, startPoint, controlPoint1, endPoint);
        }

        lineRenderer.positionCount = pointsCount;
        lineRenderer.SetPositions(curvePoints);

        // Adjust texture mode for scrolling
        //lineRenderer.textureMode = LineTextureMode.Tile;
        //lineRenderer.alignment = LineAlignment.TransformZ;


        //Debug.Log("Connectivity: " + connectivity + ", Color: " + lineColor + ", Normalized: " + normalizedConnectivity);
    }

    Vector3 CalculateControlPoint(Vector3 startPoint, Vector3 endPoint, bool isStartControlPoint)
    {
        Vector3 midPoint = (startPoint + endPoint) / 2;
        Vector3 upDirection = Vector3.up; // This can be adjusted based on your specific needs

        // Adjust the multiplier to change the curve's height
        float heightMultiplier = .5f; // Decrease this value to reduce the curve's height

        if (isStartControlPoint)
        {
            return midPoint + upDirection * (Vector3.Distance(startPoint, endPoint) / 2 * heightMultiplier);
        }
        else
        {
            return midPoint - upDirection * (Vector3.Distance(startPoint, endPoint) / 2 * heightMultiplier);
        }
    }

    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        // Quadratic Bézier curve formula
        return (1 - t) * (1 - t) * p0 + 2 * (1 - t) * t * p1 + t * t * p2;
    }

}