using UnityEngine;

public class CircularCameraRotation : MonoBehaviour
{
    public float rotationSpeed = 20.0f; // Rotation speed in degrees per second
    public float radius = 5.0f; // Distance from the center (origin)

    private float angle; // Current angle of rotation

    void Update()
    {
        // Update the angle based on the rotation speed and time
        angle += rotationSpeed * Time.deltaTime;
        angle %= 360; // Keep the angle within 0-360 degrees

        // Calculate the new position
        float x = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
        float z = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
        transform.position = new Vector3(x, transform.position.y, z);

        // Look at the origin
        transform.LookAt(Vector3.zero);
    }
}
