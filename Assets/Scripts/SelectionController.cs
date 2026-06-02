using System;
using System.Linq;
using Generations;
using UnityEngine;

namespace Generations
{
    /// <summary>
    /// Manages the Selection Indicator
    /// Listens for InputEvents and notifies of EntityEvents
    /// (like HoveredCellChanged and EntitySelected)
    /// </summary>
    public class SelectionController : MonoBehaviour
    {
        EntityData _lastSelectedEntity;

        public GameObject SelectionIndicator;

        void Start()
        {
            InputEvents.LeftClick += UpdateSelectedEntity;
            MapEvents.HoveredCellChanged += UpdateSelectionIndicator;
        }

        private void UpdateSelectionIndicator(MapCell cell)
        {
            if (cell != null)
            {
                SelectionIndicator.SetActive(true);
                SelectionIndicator.transform.position = cell.WorldPosition;
            }
            else
            {
                SelectionIndicator.SetActive(false);
            }
        }

        private void UpdateSelectedEntity(RaycastHit2D[] rayHits)
        {
            // Get all the overlapping entities
            EntityData[] clickedEntities = rayHits
                .Select(hit => hit.collider.gameObject.GetComponent<EntityFrame>()?.Data)
                .Where(entity => entity != null)
                .ToArray();

            // Is the currently selected entity in the array, and at what index?
            int alreadySelectedIndex = Array.IndexOf(clickedEntities, _lastSelectedEntity);
            EntityData newSelected;

            // If it's the same entity as before (including null), ignore this click
            if (alreadySelectedIndex != -1 && clickedEntities.Length == 1 || _lastSelectedEntity == null && clickedEntities.Length == 0)
            {
                return;
            }
            // If one of the entities is the same as before, rotate to the next one
            else if (alreadySelectedIndex != -1)
            {
                int newIndex = (alreadySelectedIndex + 1) % clickedEntities.Length;
                newSelected = clickedEntities[newIndex];
            }
            // If no entitites were clicked, then clear the selection
            else if (clickedEntities.Length == 0)
            {
                newSelected = null;
            }
            // If a brand new list of entities was clicked, then pick the first one
            else
            {
                newSelected = clickedEntities[0];
            }

            // Update what's selected
            if (_lastSelectedEntity != null) _lastSelectedEntity.IsSelected = false;
            if (newSelected != null) newSelected.IsSelected = true;
            _lastSelectedEntity = newSelected;

            // Notify of entity selection
            EntityEvents.EntitySelected?.Invoke(_lastSelectedEntity);
        }
    }
}