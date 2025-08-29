using System;
using UnityEngine;
using UnityEngine.UIElements;

public class SelectionManager
{
    public Selectable Selected { get; private set; }

    public UIDocument UIDoc;
    private Label _foodLabel;
    private Label _nameLabel;

    public SelectionManager(UIDocument doc)
    {
        UIDoc = doc;
        _foodLabel = UIDoc.rootVisualElement.Q<Label>("FoodLabel");
        _nameLabel = UIDoc.rootVisualElement.Q<Label>("NameLabel");
    }

    public void Register(Selectable selectable)
    {
        selectable.OnClick += OnSelectableClick;
    }

    public void Unregister(Selectable selectable)
    {
        selectable.OnClick -= OnSelectableClick;
    }

    public void OnSelectableClick(Selectable newSelected)
    {
        if (Selected == newSelected) return;
        
        if (Selected)
        {
            Selected.IsSelected = false;
            Selected.OnChange -= UpdateSelectedDisplay;
        }
        newSelected.IsSelected = true;
        newSelected.OnChange += UpdateSelectedDisplay;
        Selected = newSelected;

        UpdateSelectedDisplay(newSelected);
    }

    private void UpdateSelectedDisplay(Selectable selectable)
    {
        if (Selected is Meeple meeple)
        {
            _nameLabel.text = meeple.Name;
            _foodLabel.text = "Food: " + meeple.Food + " Sleep: " + meeple.Sleep;
        }
    }
}