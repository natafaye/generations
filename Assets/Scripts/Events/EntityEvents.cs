using System;
using UnityEngine;

namespace Generations
{
    public static class EntityEvents
    {
        /// <summary>
        /// Takes in a position and gives the nearest entity that returns true<br/>
        /// Handler: EntityManager
        /// </summary>
        public static Func<Vector2, Func<EntityData, bool>, EntityData> FindNearestMatchingEntity;

        /// <summary>
        /// When an entity is selected and should show up in the SelectedEntityView<br/>
        /// Triggers: SelectionController<br/>
        /// Listeners: SelectedEntityView
        /// </summary>
        public static Action<EntityData> EntitySelected;

        /// <summary>
        /// When an entity is destroyed<br/>
        /// Triggers: EntityData<br/>
        /// Listeners: EntityManager, MapManager
        /// </summary>
        public static Action<EntityData> EntityDestroyed;

        /// <summary>
        /// Request that an entity be created<br/>
        /// Triggers: GameManager, BuildController, JobManager, MapGenerator<br/>
        /// Listeners: EntityManager
        /// </summary>
        public static Action<EntityData> RequestCreateEntity;

        /// <summary>
        /// When an entity is created<br/>
        /// Triggers: EntityManager<br/>
        /// Listeners: MapManager
        /// </summary>
        public static Action<EntityData> EntityCreated;

        
    }
}