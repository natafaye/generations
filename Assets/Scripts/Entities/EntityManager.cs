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
        get { return Entities.Where(e => e.Data.Type is MeepleType).Cast<Meeple>().ToList(); }
    }
    public List<Structure> Structures { 
        get { return Entities.Where(e => e.Data.Type is StructureType).Cast<Structure>().ToList(); }
    }
    public List<Item> Items { 
        get { return Entities.Where(e => e.Data.Type is ItemType).Cast<Item>().ToList(); }
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

    public Entity CreateEntity(EntityData data)
    {
        Entity entity;
        if (data.Type is StructureType)
        {
            if (data.Type is PlantType) 
                entity = Instantiate(PlantPrefab, StructuresContainer);
            else 
                entity = Instantiate(StructurePrefab, StructuresContainer);
        }
        else if (data.Type is ItemType)
        {
            entity = Instantiate(ItemPrefab, ItemsContainer);
        }
        else
        {
            entity = Instantiate(MeeplePrefab, MeeplesContainer);
        }
        entity.Data = data;
        entity.SpriteRenderer.sprite = data.Type.Sprite;
        Entities.Add(entity);
        return entity;
    }

    public void DestroyEntity(Entity entity)
    {
        Entities.Remove(entity);
        Destroy(entity.GameObject);
    }
}