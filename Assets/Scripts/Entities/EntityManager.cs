using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EntityManager: MonoBehaviour
{
    public List<Meeple> Meeples;
    public List<Structure> Structures;
    public List<Item> Items;
    public UIDocument UIDoc;

    private MapManager _map;
    private SelectionManager _selectionManager;

    public void Init()
    {
        _map = GameManager.Instance.MapManager;
        _selectionManager = new(UIDoc);

        for (int i = 0; i < Meeples.Count; i++)
        {
            Meeples[i].Spawn(_map, _map.GetNearestPassableCell(new Vector2Int(2, 2 + i)));
            _selectionManager.Register(Meeples[i]);
        }
        foreach (var structure in Structures) _selectionManager.Register(structure);
        foreach (var item in Items) _selectionManager.Register(item);
    }

    public void Tick()
    {
        foreach (var meeple in Meeples) meeple.Tick();
    }
}