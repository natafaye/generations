using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Generations
{
    /// <summary>
    /// Manages a list of the entities and the entity game objects
    /// </summary>
    public class EntityManager : MonoBehaviour
    {
        // Main list
        public List<EntityData> Entities = new();

        // Sub lists
        public List<MeepleData> Meeples
        {
            get { return Entities.Where(e => e.Type is MeepleType).Cast<MeepleData>().ToList(); }
        }
        public List<StructureData> Structures
        {
            get { return Entities.Where(e => e.Type is StructureType).Cast<StructureData>().ToList(); }
        }
        public List<ItemData> Foods
        {
            get { return Entities.Where(e => e.Type is ItemType).Cast<ItemData>().ToList(); }
        }

        // Game Object Creation
        public EntityFrame EntityPrefab;

        void Awake()
        {
            EntityEvents.RequestCreateEntity += CreateEntity;
            EntityEvents.EntityDestroyed += RemoveEntity;
            EntityEvents.FindNearestMatchingEntity = FindNearestMatchingEntity;
        }

        public void Tick()
        {
            // Make a copy of the entitites list in case entities are added or removed during ticks
            var entities = Entities.ToArray();
            // Loop over all the entities and make each tick
            foreach (EntityData entity in entities) entity.Tick();
        }

        public void CreateEntity(EntityData entityData)
        {
            EntityFrame frame = Instantiate(EntityPrefab, transform);
            frame.Data = entityData;
            frame.transform.position = (Vector2)entityData.MapPosition;
            Entities.Add(entityData);
            EntityEvents.EntityCreated?.Invoke(entityData);
        }

        public void RemoveEntity(EntityData entity)
        {
            Entities.Remove(entity);
        }

        public EntityData FindNearestMatchingEntity(Vector2 location, Func<EntityData, bool> isMatch)
        {
            return Entities
                .Where(isMatch)
                .OrderBy((a) => MapUtilities.DistanceBetween(a.MapPosition, location))
                .FirstOrDefault();
        }
    }
}