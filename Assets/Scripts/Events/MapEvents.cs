using System;
using UnityEngine;

namespace Generations
{
    public static class MapEvents
    {
        /// <summary>
        /// Returns the currently hovered over cell, if any<br/>
        /// Handler: MapManager
        /// </summary>
        public static Func<MapCell> GetHoveredCell;

        /// <summary>
        /// Returns a path from the first position to the second position<br/>
        /// Handler: MapManager
        /// </summary>
        public static Func<Vector2, Vector2, Vector2[]> GetPath;

        /// <summary>
        /// When a new cell is hovered over<br/>
        /// Triggers: MapManager<br/>
        /// Listeners: SelectionController, BuildController
        /// </summary>
        public static Action<MapCell> HoveredCellChanged;

        /// <summary>
        /// When an entity is moved<br/>
        /// Triggers: <br/>
        /// Listeners: MapManager
        /// </summary>
        public static Action<EntityData, Vector2Int> EntityMoved;
    }
}