using System.Linq;
using Generations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UIElements;

namespace Generations
{
    /// <summary>
    /// Translates InputSystem events into InputEvents 
    /// for the CameraManager and SelectionController to listen to
    /// </summary>
    public class InputController : MonoBehaviour
    {
        public Camera MainCamera;
        public UIDocument UIDocument;

        private void OnNavigate(InputValue value)
        {
            InputEvents.Navigate?.Invoke(value);
            // If we've navigated the camera, the world mouse position will have changed
            OnPoint();
        }

        private void OnScrollWheel(InputValue value)
        {
            InputEvents.ScrollWheel?.Invoke(value);
            // If we've navigated the camera, the world mouse position will have changed
            OnPoint();
        }

        private void OnPoint()
        {
            // Convert from screen position to 2D world position and notify
            Vector3 screenPosition = Mouse.current.position.ReadValue();
            Vector3 worldPosition = MainCamera.ScreenToWorldPoint(new Vector3(
                screenPosition.x, screenPosition.y, -MainCamera.transform.position.z
            ));
            Vector2 worldPosition2D = new(worldPosition.x, worldPosition.y);
            InputEvents.PointerMove?.Invoke(worldPosition2D);
        }

        private void OnClick(InputValue value)
        {
            // Only handle the down click, not the release
            if (value.Get<float>() == 0) return;

            // Get all the objects this click overlapped
            var screenLocation = Mouse.current.position.ReadValue();
            Ray ray = MainCamera.ScreenPointToRay(screenLocation);
            RaycastHit2D[] rayHits = Physics2D.GetRayIntersectionAll(ray);

            // If the click overlapped with a UI element, then let the UI element handle it
            IPanel panel = UIDocument.rootVisualElement.panel;
            Vector2 correctedScreenPosition = new(screenLocation.x, Screen.height - screenLocation.y);
            VisualElement picked = panel.Pick(RuntimePanelUtils.ScreenToPanel(panel, correctedScreenPosition));
            if (picked != null) return;

            // Notify if this was a left click
            if (value.Get<float>() == 1) InputEvents.LeftClick?.Invoke(rayHits);
        }

        private void OnAnyKey(InputValue value)
        {
            // Ignore the letting up of a key
            if (value.Get<float>() == 0 || !Keyboard.current.anyKey.wasPressedThisFrame) return;
            // Get the pressed key (there's only ever one at a time)
            KeyControl key = Keyboard.current.allKeys.First(key => key.wasPressedThisFrame);
            // Notify anyone listening
            InputEvents.KeyDown?.Invoke(key.keyCode);
        }
    }
}