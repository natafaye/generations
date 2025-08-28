using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MeepleManager: MonoBehaviour
{
    public List<Meeple> Meeples;
    private Meeple m_SelectedMeeple;

    public UIDocument UIDoc;
    private Label m_FoodLabel;
    private Label m_NameLabel;
    private Button m_FeedButton;

    public void Init()
    {
        m_SelectedMeeple = Meeples[0];
        m_SelectedMeeple.Selected = true;
        m_FoodLabel = UIDoc.rootVisualElement.Q<Label>("FoodLabel");
        m_NameLabel = UIDoc.rootVisualElement.Q<Label>("NameLabel");
        m_FeedButton = UIDoc.rootVisualElement.Q<Button>("FeedButton");
        m_FeedButton.clicked += OnFeedClick;

        var map = GameManager.Instance.MapManager;
        for (int i = 0; i < Meeples.Count; i++)
        {
            Meeples[i].Spawn(map, map.GetNearestPassableCell(new Vector2Int(2, 2 + i)));
            Meeples[i].OnClick += OnMeepleClick;
        }

        UpdateSelectedMeepleDisplay();
    }

    public void OnMeepleClick(Meeple sender)
    {
        if (m_SelectedMeeple == sender) return;

        m_SelectedMeeple.Selected = false;
        sender.Selected = true;
        m_SelectedMeeple = sender;

        UpdateSelectedMeepleDisplay();
    }

    public void OnFeedClick()
    {
        m_SelectedMeeple.Food++;

        UpdateSelectedMeepleDisplay();
    }

    public void Tick()
    {
        foreach (var meeple in Meeples)
        {
            meeple.Tick();
        }

        UpdateSelectedMeepleDisplay();
    }

    private void UpdateSelectedMeepleDisplay()
    {
        m_NameLabel.text = m_SelectedMeeple.Name;
        m_FoodLabel.text = "Food: " + m_SelectedMeeple.Food + " Sleep: " + m_SelectedMeeple.Sleep;
    }
}