using UnityEngine;

public class MapCell : IHeapItem<MapCell> 
{
	// Data about that cell's tile (passable, walk cost, etc)
	public bool Passable
	{
		get { return CellType.Passable; }
	}

	// Position in the map by index
	public Vector2Int MapPosition;
	// Unity world position of the cell corner
	public Vector3 WorldPosition
	{
		get { return new(MapPosition.x, MapPosition.y, 0); }
	}

	public float height;
	public float firmness;


	// Layers of the Cell
	public bool HasRoof;
	public IEntity Contents;
	// TODO: Floor
	public CellType CellType;


	public MapCell(CellType cellType, Vector2Int mapPosition)
	{
		CellType = cellType;
		MapPosition = mapPosition;
	}

	public void MoveToCell(IEntity entity)
	{
		entity.MapPosition = MapPosition;
		entity.Transform.position = WorldPosition;
		Contents = entity;
	}


	/** PATHFINDING **/

	public MapCell Parent;

	// Actual distance from start to this node
	public float GCost;
	// Estimated distance (optimistically) from this node to end node
	public float HCost;
	// Total estimated distance
	public float FCost {
		get {
			return GCost + HCost;
		}
	}

	public int HeapIndex { get; set; }

	public int CompareTo(MapCell nodeToCompare)
	{
		int compare = FCost.CompareTo(nodeToCompare.FCost);
		if (compare == 0)
		{
			compare = HCost.CompareTo(nodeToCompare.HCost);
		}
		return -compare; // TODO: Why negative?
	}
}