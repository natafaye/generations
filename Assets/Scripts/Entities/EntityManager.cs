using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Manages a list of the entities and the entity game objects
public class EntityManager : MonoBehaviour
{
    // Main list
    public List<Entity> Entities;

    // Sub lists
    public List<Meeple> Meeples { 
        get { return Entities.Where(e => e.Type is MeepleType).Cast<Meeple>().ToList(); }
    }
    public List<Structure> Structures { 
        get { return Entities.Where(e => e.Type is StructureType).Cast<Structure>().ToList(); }
    }
    public List<Item> Items { 
        get { return Entities.Where(e => e.Type is ItemType).Cast<Item>().ToList(); }
    }

    // Game Object Creation
    public Transform MeeplesContainer;
    public Transform ItemsContainer;
    public Transform StructuresContainer;
    public Structure StructurePrefab;
    public Plant PlantPrefab;
    public Item ItemPrefab;
    public Meeple MeeplePrefab;

    public void Tick()
    {
        // Make a copy of the entitites list in case entities are added or removed during ticks
        var entities = Entities.ToArray();
        // Loop over all the entities and make them each tick
        foreach (Entity entity in entities) entity.Tick();
    }

    public Entity CreateEntity(EntityType entityType)
    {
        Entity entity;
        if (entityType is StructureType)
        {
            if (entityType is PlantType) 
                entity = Instantiate(PlantPrefab, StructuresContainer);
            else 
                entity = Instantiate(StructurePrefab, StructuresContainer);
        }
        else if (entityType is ItemType)
        {
            entity = Instantiate(ItemPrefab, ItemsContainer);
        }
        else
        {
            entity = Instantiate(MeeplePrefab, MeeplesContainer);
        }
        entity.Name = entityType.Name;
        entity.Type = entityType;
        entity.SpriteRenderer.sprite = entityType.Sprite;
        Entities.Add(entity);
        return entity;
    }

    public void DestroyEntity(Entity entity)
    {
        Entities.Remove(entity);
        Destroy(entity.GameObject);
    }
}