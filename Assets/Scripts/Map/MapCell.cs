using UnityEngine;

public class MapCell : IHeapItem<MapCell> 
{
	// Data about that cell's tile (passable, walk cost, etc)
	public CellData CellData;
	public bool Passable
	{
		get { return CellData.Passable; }
	}

	// Position in the map by index
	public Vector2Int MapPosition;
	// Unity world position of the cell center
	public Vector3 WorldPosition;

	public MapCell(CellData cellData, Vector2Int mapPosition, Vector3 worldPosition)
	{
		CellData = cellData;
		MapPosition = mapPosition;
		WorldPosition = worldPosition;
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