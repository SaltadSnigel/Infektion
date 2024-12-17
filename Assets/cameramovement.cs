using UnityEngine;

public class CameraGrabControl : MonoBehaviour
{
    public float zoomSpeed = 10f;   // Speed of zooming
    public float minZoom = 5f;      // Minimum zoom level
    public float maxZoom = 20f;     // Maximum zoom level
    public float dragSpeed = 1f;    // Speed of dragging the scene

    private Vector3 dragOrigin;     // Mouse position where dragging starts
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        HandleZoom();
        HandleDrag();
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0.0f)
        {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - scroll * zoomSpeed, minZoom, maxZoom);
        }
    }

    void HandleDrag()
    {
        if (Input.GetMouseButtonDown(1)) // Right mouse button pressed
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(1)) // Right mouse button held
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
            transform.position += difference; // Move the camera
        }
    }
}
