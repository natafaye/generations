using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
	// Game data
	public int Width;
    public int Height;
	private MapCell[,] _cells;
    private Vector2Int[] _movementsByDistance;

	// Job manager
	public JobManager JobManager;

	// Unity objects
	public TileBase GrassTile;
    public TileBase WaterTile;
    public TileBase EdgeTile;
    private Grid _grid;
    private Tilemap _tilemap;
	
	// Debugging
	public bool DisplayGridGizmos;
	
	// Settings
	private readonly float _cellSize = 1;
	private readonly Vector2 _worldBottomLeft = Vector2.zero;

    public void Init(int width, int height)
    {
        Width = width;
        Height = height;

        _tilemap = GetComponentInChildren<Tilemap>();
        _grid = GetComponentInChildren<Grid>();
        _cells = new MapCell[Width + 2, Height + 2];
        _movementsByDistance = MovementsByDistanceGenerator.Generate(Width, Height);

		JobManager = new();

		GenerateMap();
    }

	bool InBounds(int x, int y)
	{
		return x >= 0 && x < Width + 2 && y >= 0 && y < Height + 2;
	}

    public Vector3 MapToWorld(Vector2Int mapPosition)
	{
		return _grid.GetCellCenterWorld((Vector3Int)mapPosition);
	}

	public Vector2Int WorldToMap(Vector3 worldPosition)
	{
		// Find the percent location
		float percentX = (worldPosition.x + _worldBottomLeft.x) / Width;
		float percentY = (worldPosition.y + _worldBottomLeft.y) / Height;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		// Get the x and y at that percent
		int x = Mathf.RoundToInt(Width * percentX);
		int y = Mathf.RoundToInt(Height * percentY);

		return new Vector2Int(x, y);
	}

    public MapCell GetMapCell(Vector2Int cellIndex)
	{
		if (!InBounds(cellIndex.x, cellIndex.y)) return null;
		return _cells[cellIndex.x, cellIndex.y];
	}

	public Vector2[] FindPath(Vector2 from, Vector2 to)
	{
		return Pathfinder.FindPath(this, from, to);
	}

    public MapCell GetNearestPassableCell(Vector2Int start)
	{
		var startCell = GetMapCell(start);

		// If this cell is passable, that's the nearest one
		if (startCell.Passable) return startCell;

		// Loop over possible movements sorted by distance 
		// and find the first passable one (and thus the nearest)
		foreach (Vector2Int coordinate in _movementsByDistance)
		{
			var cell = GetMapCell(start + coordinate);
			if (cell != null && cell.Passable)
				return cell;
		}

		// If there are no passable cells on the map return null
		return null;
	}

	private void GenerateMap()
	{
		// Add 2 to the width and height for the transparent edge cells
		for (int y = 0; y < Height + 2; ++y)
		{
			for (int x = 0; x < Width + 2; ++x)
			{
				TileBase tile;
				CellData data = new();

				if (x == 0 || y == 0 || x == Width + 1 || y == Width + 1)
				{
					// Put edge tiles around the outside
					tile = EdgeTile;
					data.Passable = false;
				}
				else
				{
					// 20% Chance of Water
					// 80% Chance of Grass
					if (Random.Range(1, 5) == 1)
					{
						tile = WaterTile;
						data.Passable = false;
					}
					else
					{
						tile = GrassTile;
						data.Passable = true;
					}
				}

				var mapPosition = new Vector2Int(x, y);
				var worldPosition = MapToWorld(mapPosition);
				_cells[x, y] = new MapCell(data, mapPosition, worldPosition);

				_tilemap.SetTile(new Vector3Int(x, y, 0), tile);
			}
		}
	}

	public List<MapCell> GetNeighbours(MapCell cell)
	{
		List<MapCell> neighbours = new();

		// Get all the nodes in a 3 by 3 square centered on the original node
		for (int x = -1; x <= 1; x++)
		{
			for (int y = -1; y <= 1; y++)
			{
				// The node isn't it's own neighbor
				if (x == 0 && y == 0) continue;

				int checkX = cell.MapPosition.x + x;
				int checkY = cell.MapPosition.y + y;

				// If we're not off the edge of the grid, add it to the neighbors
				if (checkX >= 0 && checkX < Width && checkY >= 0 && checkY < Height)
				{
					neighbours.Add(_cells[checkX, checkY]);
				}
			}
		}

		return neighbours;
	}

	// public MapNode ClosestPassableNode(MapNode node)
	// {
	// 	int maxRadius = Mathf.Max(gridSizeX, gridSizeY) / 2;
	// 	for (int i = 1; i < maxRadius; i++)
	// 	{
	// 		MapNode n = FindPassableInRadius(node.GridPosition.x, node.GridPosition.y, i);
	// 		if (n != null) return n;
	// 	}
	// 	return null;
	// }

	// MapNode FindPassableInRadius(int centerX, int centerY, int radius)
	// {
	// 	for (int i = -radius; i <= radius; i++)
	// 	{
	// 		int verticalSearchX = i + centerX;
	// 		int horizontalSearchY = i + centerY;

	// 		// top
	// 		if (InBounds(verticalSearchX, centerY + radius))
	// 		{
	// 			if (grid[verticalSearchX, centerY + radius].Passable)
	// 			{
	// 				return grid[verticalSearchX, centerY + radius];
	// 			}
	// 		}

	// 		// bottom
	// 		if (InBounds(verticalSearchX, centerY - radius))
	// 		{
	// 			if (grid[verticalSearchX, centerY - radius].Passable)
	// 			{
	// 				return grid[verticalSearchX, centerY - radius];
	// 			}
	// 		}
	// 		// right
	// 		if (InBounds(centerY + radius, horizontalSearchY))
	// 		{
	// 			if (grid[centerX + radius, horizontalSearchY].Passable)
	// 			{
	// 				return grid[centerX + radius, horizontalSearchY];
	// 			}
	// 		}

	// 		// left
	// 		if (InBounds(centerY - radius, horizontalSearchY))
	// 		{
	// 			if (grid[centerX - radius, horizontalSearchY].Passable)
	// 			{
	// 				return grid[centerX - radius, horizontalSearchY];
	// 			}
	// 		}
	// 	}

	// 	return null;
	// }

	void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(
			transform.position + new Vector3(Width / 2, Height / 2),
			new Vector2(Width, Height)
		);
		if (_cells != null && DisplayGridGizmos)
		{
			foreach (MapCell cell in _cells)
			{
				Gizmos.color = Color.red.WithAlpha(0.25f);
				if (cell.Passable)
					Gizmos.color = Color.white.WithAlpha(0.5f);

				Gizmos.DrawCube(MapToWorld(cell.MapPosition), Vector3.one * (_cellSize - .1f));
			}
		}
	}
}
