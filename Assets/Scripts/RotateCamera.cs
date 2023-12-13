using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // For UI elements
public class CircularCameraRotation : MonoBehaviour
{
    public float rotationSpeed = 20.0f;
    public float zoomSpeed = 1.0f;
    public float manualRotationSpeed = 1.0f;
    public Transform brainCluster;

    private bool isRotating = true;
    private Vector3 previousPosition;
    private Quaternion previousRotation;
    private Vector3 lastMousePosition;

 

    void Update()
    {
        if (isRotating)
        {
            RotateAroundBrainCluster();
        }
        else
        {
            ManualControl();
        }
    }

    private void RotateAroundBrainCluster()
    {
        transform.RotateAround(brainCluster.position, Vector3.up, rotationSpeed * Time.deltaTime);
        transform.LookAt(brainCluster);
    }
private void ManualControl()
{
    // Check if the pointer is over a UI element
    if (EventSystem.current.IsPointerOverGameObject())
    {
        return; // Do nothing if the pointer is over a UI element
    }

    if (Input.GetMouseButtonDown(0))
    {
        lastMousePosition = Input.mousePosition;
    }

    if (Input.GetMouseButton(0))
    {
        Vector3 delta = Input.mousePosition - lastMousePosition;
        lastMousePosition = Input.mousePosition;

        // Adjust rotation based on camera's view direction
        Vector3 cameraRight = transform.right;
        Vector3 cameraUp = transform.up;

        transform.RotateAround(brainCluster.position, cameraUp, delta.x * manualRotationSpeed * Time.deltaTime);
        transform.RotateAround(brainCluster.position, cameraRight, -delta.y * manualRotationSpeed * Time.deltaTime);
    }

    float scroll = Input.GetAxis("Mouse ScrollWheel");
    transform.Translate(0, 0, scroll * zoomSpeed, Space.Self);
}

    public void ToggleRotation()
    {
        isRotating = !isRotating;

        if (isRotating)
        {
            // Reset to the original position and rotation when unpaused
            transform.position = previousPosition;
            transform.rotation = previousRotation;
        }
        else{
            // Save the original position and rotation when paused
            this.previousPosition = transform.position;
            this.previousRotation = transform.rotation;

        }
    }
}
