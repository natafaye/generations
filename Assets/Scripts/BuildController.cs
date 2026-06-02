using Generations;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manages the build preview and placing blueprints
/// Interfaces through events with the BuildMenu
/// </summary>
public class BuildController : MonoBehaviour
{
    // Settings
    public Color blueprintColor;
    public Color badBlueprintColor;

    // GameObjects
    public SpriteRenderer BuildPreview;

    // Data
    private StructureType _selectedType;
    private int _rotation = 0;

    void Start()
    {
        BuildEvents.SelectedTypeChanged += OnTypeChanged;
    }

    private void OnHoveredCellChanged(MapCell cell)
    {
        UpdateBuildPreview();
    }

    private void OnTypeChanged(StructureType type)
    {
        if (type != null && _selectedType == null)
        {
            // If we're switching into build mode
            MapEvents.HoveredCellChanged += OnHoveredCellChanged;
            InputEvents.KeyDown += OnKeyDown;
            InputEvents.LeftClick += AttemptPlaceBlueprint;
        }
        else if(type == null && _selectedType != null)
        {
            // If we're switching out of build mode
            MapEvents.HoveredCellChanged -= OnHoveredCellChanged;
            InputEvents.KeyDown -= OnKeyDown;
            InputEvents.LeftClick -= AttemptPlaceBlueprint;
        }
        _selectedType = type;
        UpdateBuildPreview();
    }

    private void OnKeyDown(Key key)
    {
        if (key == Key.R)
        {
            _rotation = (_rotation + 1) % 4;
            UpdateBuildPreview();
        }
    }

    private void UpdateBuildPreview()
    {
        MapCell hoveredCell = MapEvents.GetHoveredCell();
        if (hoveredCell == null || _selectedType == null)
        {
            BuildPreview.gameObject.SetActive(false);
            return;
        }

        BuildPreview.gameObject.SetActive(true);
        BuildPreview.color = hoveredCell.DoesBlueprintFit(_selectedType, _rotation) ? blueprintColor : badBlueprintColor;

        BuildPreview.sprite = _selectedType.GetRotatedSprite(_rotation);
        BuildPreview.flipX = _rotation == 2;
        BuildPreview.flipY = _rotation == 3;
        BuildPreview.transform.position = hoveredCell.WorldPosition;
    }

    private void AttemptPlaceBlueprint(RaycastHit2D[] _)
    {
        MapCell hoveredCell = MapEvents.GetHoveredCell();
        if (!hoveredCell.DoesBlueprintFit(_selectedType, _rotation)) return;

        StructureData data = new(_selectedType, _rotation)
        {
            IsBlueprint = true,
            MapPosition = hoveredCell.MapPosition
        };

        EntityEvents.RequestCreateEntity?.Invoke(data);
    }
}