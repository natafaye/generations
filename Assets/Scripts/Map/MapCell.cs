using System.Linq;
using UnityEngine;

namespace Generations {
	public class MapCell : IHeapItem<MapCell> 
	{

		// Data about that cell's tile (passable, walk cost, etc)
		public bool Passable => CellType.Passable;
		// TODO: Add && Contents?.type != WallType

		public bool Empty => Contents == null;

		// Position in the map by index
		public Vector2Int MapPosition;
		// Unity world position of the cell corner
		public Vector3 WorldPosition => new(MapPosition.x, MapPosition.y, 0);

		public float height;
		public float firmness;


		// Layers of the Cell
		public bool HasRoof;
		public EntityData Contents;
		// TODO: Floor
		public CellType CellType;

		// The Map this cell is a part of
		public MapManager Map;

		public MapCell(CellType cellType, Vector2Int mapPosition, MapManager map)
		{
			CellType = cellType;
			MapPosition = mapPosition;
			Map = map;
		}

		/// <summary>
		/// Checks if a particular structure fits
		/// </summary>
		/// <param name="type">The type of structure</param>
		/// <param name="rotation">What rotation it's at</param>
		/// <returns>true if it fits, false if it doesn't</returns>
		public bool DoesBlueprintFit(StructureType type, int rotation)
		{
			if (type == null)
				return false;
			// Get the indexes of all the cells overlapped by the structure
			var indexes = MapUtilities.CellIndexesInArea(type.CellSize, MapPosition, rotation);
			// Check that they're all empty
			return indexes.All(index => Map.GetMapCell(index)?.Empty == true);
		}


		#region Pathfinding

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

		#endregion
	}
}