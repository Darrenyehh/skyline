using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 50f;
    public float turnSpeed = 4f;
    public float zoomSpeed = 15f;
    private Vector3 startPosition;
    private Quaternion startRotation;

    void Start()
    {
        // Set the start position and rotation to the camera's current values
        startPosition = transform.position;
        startRotation = transform.rotation;

        // Optionally, you can reset the camera to these values each time the game starts
        transform.position = startPosition;
        transform.rotation = startRotation;
    }

    void Update()
    {
        // Movement
        float h = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float v = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        transform.Translate(h, 0, v, Space.Self);

        // Mouse Look
        if (Input.GetMouseButton(1)) // Right-click drag to look around
        {
            float yaw = turnSpeed * Input.GetAxis("Mouse X");
            float pitch = turnSpeed * Input.GetAxis("Mouse Y");
            transform.eulerAngles += new Vector3(-pitch, yaw, 0);
        }

        // Zoom with Scroll Wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        transform.position += transform.forward * scroll * zoomSpeed;

        // Reset position with f
        if (Input.GetKeyDown(KeyCode.F))
        {
            transform.position = startPosition;
            transform.rotation = startRotation;
        }

        // Move down with 'Shift' key
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            float descent = -moveSpeed * Time.deltaTime;
            transform.Translate(0, descent, 0, Space.World);
        }

        // Move up with 'Space' key
        if (Input.GetKey(KeyCode.Space))
        {
            float ascent = moveSpeed * Time.deltaTime;
            transform.Translate(0, ascent, 0, Space.World);
        }
    }   
}