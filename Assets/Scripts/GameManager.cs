using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set;  }

    public MapManager MapManager;
    public EntityManager EntityManager;
    public CameraManager CameraManager;

    public MeepleType Human;

    public UIDocument UIDoc;

    public int MapWidth = 8;
    public int MapHeight = 8;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        MapManager.Init(MapWidth, MapHeight);
        CameraManager.Init(MapWidth, MapHeight);

        CreateEntity(new MeepleData(Human), new Vector2Int(2, 2));
        
        InvokeRepeating(nameof(Tick), 1f, 1f);
    }

    void Tick()
    {
        EntityManager.Tick();
    }

    public Entity CreateEntity(EntityData data, Vector2Int location)
    {
        Entity entity = EntityManager.CreateEntity(data);
        MapManager.MoveEntity(entity, location);
        return entity;
    }

    public void DestroyEntity(Entity entity)
    {
        EntityManager.DestroyEntity(entity);
        MapManager.RemoveEntity(entity);
    }

    public Entity FindNearestUnreservedEntityOfType(EntityType entityType, Vector2 location)
    {
        return EntityManager.Entities
            .Where(e => e.Data.Type == entityType && e.Data.QueuedJob == null)
            .OrderBy((a) => Distance.Between(a.MapPosition, location))
            .FirstOrDefault();
    }
}
