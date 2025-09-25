using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class EntityManager : MonoBehaviour
{
    // Data
    public List<Meeple> Meeples;
    public List<Structure> Structures;
    public List<Item> Items;
    private MapManager _map;

    // Creation
    public Transform MeeplesContainer;
    public Transform ItemsContainer;
    public Transform StructuresContainer;
    public Structure StructurePrefab;
    public Item ItemPrefab;
    public Meeple MeeplePrefab;

    // Selection
    public IEntity Selected { get; private set; }
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

    public IEntity Create<T>(T entityType) where T : IEntityType
    {
        IEntity entity;
        if (entityType is StructureType)
        {
            entity = Instantiate(StructurePrefab, StructuresContainer);
            Structures.Add((Structure)entity);
        }
        else if (entityType is ItemType)
        {
            entity = Instantiate(ItemPrefab, ItemsContainer);
            Items.Add((Item)entity);
        }
        else
        {
            entity = Instantiate(MeeplePrefab, MeeplesContainer);
            Meeples.Add((Meeple)entity);
        }
        entity.Type = entityType;
        entity.Name = entityType.Name;
        entity.SpriteRenderer.sprite = entityType.Sprite;
        return entity;
    }

    private void OnClick(InputValue value)
    {
        // Use ray collision to find what was clicked
        if (value.Get<float>() == 0) return;
        var rayHit = Physics2D.GetRayIntersection(MainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()));
        if (!rayHit.collider) return;

        // Check if a different selectable has been clicked
        GameObject clickedObject = rayHit.collider.gameObject;
        var newSelected = clickedObject.GetComponent<IEntity>();
        if (newSelected == null || newSelected == Selected) return;

        // Update the selected
        if (Selected != null) Selected.IsSelected = false;
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
            foreach (JobWorkType job in structure.Type.availableJobs)
            {
                Button jobButton = new();
                jobButton.text = job.Name;
                jobButton.clicked += () => _map.JobManager.AddJob(job, structure);
                _selectedDisplay.Add(jobButton);
            }
        }
        else if (Selected is Item item)
        {
            Label label = new();
            label.text = item.Name;
            _selectedDisplay.Add(label);
        }
    }

    public void Tick()
    {
        foreach (var meeple in Meeples) meeple.Tick();
    }
}