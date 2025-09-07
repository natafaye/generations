using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.InputSystem.InputAction;

public class EntityManager : MonoBehaviour
{
    // Data
    public List<Meeple> Meeples;
    public List<Structure> Structures;
    public List<Item> Items;
    private MapManager _map;

    // Selection
    public Selectable Selected { get; private set; }
    public InputManager InputManager;
    public Camera MainCamera;
    public UIDocument UIDoc;
    public VisualTreeAsset SelectedMeepleTemplate;
    private VisualElement _selectedDisplay;

    public void Init()
    {
        _map = GameManager.Instance.MapManager;
        _selectedDisplay = UIDoc.rootVisualElement.Q<VisualElement>("SelectedDisplay");
        InputManager.OnClickInput += OnClick;

        for (int i = 0; i < Meeples.Count; i++)
        {
            Meeples[i].Spawn(_map, _map.GetNearestPassableCell(new Vector2Int(2, 2 + i)));
        }
    }

    private void OnClick(InputValue value)
    {
        // Use ray collision to find what was clicked
        if (value.Get<float>() == 0) return;
        var rayHit = Physics2D.GetRayIntersection(MainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()));
        if (!rayHit.collider) return;

        // Check if a new selectable has been clicked
        GameObject clickedObject = rayHit.collider.gameObject;
        Selectable newSelected = clickedObject.GetComponent<Selectable>();
        if (!newSelected || newSelected == Selected) return;

        // Update the selected
        if (Selected) Selected.IsSelected = false;
        newSelected.IsSelected = true;
        Selected = newSelected;

        // Update the selection display
        _selectedDisplay.Clear();
        if (Selected is Meeple meeple)
        {
            VisualElement newElement = new();
            SelectedMeepleTemplate.CloneTree(newElement);
            newElement.dataSource = meeple;
            _selectedDisplay.Add(newElement);
        }
        else if (Selected is Structure structure)
        {
            Button harvestButton = new();
            harvestButton.text = "Harvest";
            _selectedDisplay.Add(harvestButton);
        }
        else if (Selected is Item item)
        {
            Label label = new();
            label.text = item.Type.ToString();
            _selectedDisplay.Add(label);
        }
    }

    public void Tick()
    {
        foreach (var meeple in Meeples) meeple.Tick();
    }
}