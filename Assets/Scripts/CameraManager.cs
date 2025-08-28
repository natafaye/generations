using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    // Pan
    public float PanSpeed = 150;
    public float PanDampening = 15f;
    public float Margin = 1;
    public int MapWidth;
    public int MapHeight;
    private Vector3 _panDirection;
    
    // Pan with mouse near screen edge
    public float EdgeTolerance = 0.05f;
    private Vector3 _mousePanDirection;
    public bool UseScreenEdge = true;

    // Zoom
    public float StepSize = -1;
    public float ZoomSpeed = 2;
    public float ZoomDampening = 7.5f;
    public float MinZoom = 3;
    public float MaxZoom = 10;
    private float _zoomSize = 5;
    private Camera _camera;

    public void Init(int mapWidth, int mapHeight)
    {
        MapWidth = mapWidth;
        MapHeight = mapHeight;
        transform.position = new Vector3(MapWidth / 2, MapHeight / 2, -10);
        _camera = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        // ---- PAN ----
        // Only use screen edge panning if there is no keyboard panning currently happening
        var moveDirection = _panDirection;
        if (_panDirection.sqrMagnitude < 0.1f && UseScreenEdge) moveDirection = _mousePanDirection;
        // Scale the pan speed by the zoom size, so you pan faster when more zoomed out
        Vector3 targetPosition = transform.position + (_zoomSize * PanSpeed * Time.deltaTime * moveDirection);
        // Don't allow panning past the edges of the map
        targetPosition = new Vector3(
            Mathf.Clamp(targetPosition.x, 0 - Margin, MapWidth + Margin),
            Mathf.Clamp(targetPosition.y, 0 - Margin, MapHeight + Margin),
            transform.position.z
        );
        // Use lerp to smoothly move towards the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * PanDampening);

        // ---- ZOOM ----
        _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _zoomSize, Time.deltaTime * ZoomDampening);
    }

    private void OnNavigate(InputValue value)
    {
        _panDirection = value.Get<Vector2>().normalized;
    }

    private void OnPoint()
    {
        // If the mouse has moved, check if it's near an edge for screen edge panning
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 moveDirection = Vector3.zero;

        if (mousePosition.x < EdgeTolerance * Screen.width)
            moveDirection += Vector3.left;
        else if (mousePosition.x > (1f - EdgeTolerance) * Screen.width)
            moveDirection += Vector3.right;

        if (mousePosition.y < EdgeTolerance * Screen.height)
            moveDirection += Vector3.down;
        else if (mousePosition.y > (1f - EdgeTolerance) * Screen.height)
            moveDirection += Vector3.up;

        _mousePanDirection = moveDirection * 0.25f;
    }

    private void OnScrollWheel(InputValue value)
    {
        float scrollAmount = value.Get<Vector2>().normalized.y;
        _zoomSize = Mathf.Clamp(_zoomSize + scrollAmount * StepSize, MinZoom, MaxZoom);
    }
}
