using UnityEngine;
using System.Collections.Generic;
using System;

public static class Pathfinder
{
	public static Vector2[] FindPath(MapManager map, Vector2 from, Vector2 to)
	{
		Vector2[] waypoints = new Vector2[0];
		bool pathSuccess = false;

		MapCell startCell = map.GetNearestPassableCell(map.WorldToMap(from));
		MapCell targetCell = map.GetNearestPassableCell(map.WorldToMap(to));

		if (startCell == null || targetCell == null) return waypoints;

		startCell.Parent = startCell;

		Heap<MapCell> openSet = new(map.Width * map.Height);
		HashSet<MapCell> closedSet = new();
		openSet.Add(startCell);

		while (openSet.Count > 0)
		{
			MapCell currentCell = openSet.RemoveFirst();
			closedSet.Add(currentCell);

			if (currentCell == targetCell)
			{
				pathSuccess = true;
				break;
			}

			foreach (MapCell neighbour in map.GetNeighbours(currentCell))
			{
				if (!neighbour.Passable || closedSet.Contains(neighbour))
					continue;

				float newMovementCostToNeighbour = currentCell.GCost + GetDistance(currentCell, neighbour);
				if (newMovementCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour))
				{
					neighbour.GCost = newMovementCostToNeighbour;
					neighbour.HCost = GetDistance(neighbour, targetCell);
					neighbour.Parent = currentCell;

					if (!openSet.Contains(neighbour))
						openSet.Add(neighbour);
					else
						openSet.UpdateItem(neighbour);
				}
			}
		}

		if (pathSuccess)
			waypoints = RetracePath(startCell, targetCell);

		return waypoints;
	}

	static Vector2[] RetracePath(MapCell startCell, MapCell endCell)
	{
		List<MapCell> path = new();
		MapCell currentCell = endCell;

		while (currentCell != startCell)
		{
			path.Add(currentCell);
			currentCell = currentCell.Parent;
		}

		Vector2[] waypoints = SimplifyPath(path);
		Array.Reverse(waypoints);
		return waypoints;
	}

	static Vector2[] SimplifyPath(List<MapCell> path)
	{
		List<Vector2> waypoints = new();
		Vector2 directionOld = Vector2.zero;

		for (int i = 1; i < path.Count; i++)
		{
			Vector2 directionNew = path[i - 1].MapPosition - path[i].MapPosition;
			if (directionNew != directionOld)
			{
				waypoints.Add(path[i].WorldPosition);
			}
			directionOld = directionNew;
		}
		return waypoints.ToArray();
	}

	static float GetDistance(MapCell nodeA, MapCell nodeB)
	{
		float dstX = Mathf.Abs(nodeA.MapPosition.x - nodeB.MapPosition.x);
		float dstY = Mathf.Abs(nodeA.MapPosition.y - nodeB.MapPosition.y);

		if (dstX > dstY)
			return 14 * dstY + 10 * (dstX - dstY);
		return 14 * dstX + 10 * (dstY - dstX);
	}
}