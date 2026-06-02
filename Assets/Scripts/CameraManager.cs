using Generations;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Generations
{
    public class CameraManager : MonoBehaviour
    {
        // Pan
        public float PanSpeed = 3f;
        public float PanDampening = 15f;
        public float Margin = 1;
        private int _mapWidth;
        private int _mapHeight;
        private Vector3 _panDirection;
        private Vector3 _targetPosition;

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
            _mapWidth = mapWidth;
            _mapHeight = mapHeight;
            Vector3 startPos = new Vector3(_mapWidth / 2f, _mapHeight / 2f, -10);
            transform.position = startPos;
            _targetPosition = startPos;
            _camera = GetComponentInChildren<Camera>();

            InputEvents.ScrollWheel += OnScrollWheel;
        }

        void Update()
        {
            // Prevent repeating errors when other code errors
            if (!_camera) return;

            // Poll keyboard input directly to avoid Navigate release callback issues
            Vector3 inputDirection = Vector3.zero;
            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) inputDirection.y += 1f;
                if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) inputDirection.y -= 1f;
                if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) inputDirection.x -= 1f;
                if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) inputDirection.x += 1f;
            }
            _panDirection = inputDirection.normalized;

            // ---- PAN ----
            // Scale the target position linearly
            _targetPosition += _zoomSize * PanSpeed * Time.deltaTime * _panDirection;
            
            // Clamp target position to the edges of the map
            _targetPosition = new Vector3(
                Mathf.Clamp(_targetPosition.x, 0 - Margin, _mapWidth + Margin),
                Mathf.Clamp(_targetPosition.y, 0 - Margin, _mapHeight + Margin),
                _targetPosition.z
            );
            
            // Smoothly move transform towards target position
            transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * PanDampening);

            // ---- ZOOM ----
            _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _zoomSize, Time.deltaTime * ZoomDampening);
        }

        private void OnScrollWheel(InputValue value)
        {
            float scrollAmount = value.Get<Vector2>().normalized.y;
            _zoomSize = Mathf.Clamp(_zoomSize + scrollAmount * StepSize, MinZoom, MaxZoom);
        }
    }
}
