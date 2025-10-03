using UnityEngine;

public enum StructureCategory
{
    Plant,
    Wall,
    Bed
}

[RequireComponent(typeof(SpriteRenderer))]
public class Structure : MonoBehaviour, IEntity
{
    [field: SerializeField]
    public string Name { get; set; }

    [field: SerializeField]
    public StructureType Type { get; set; }
    // Handle type inheritance of IEntityType to StructureType
    IEntityType IEntity.Type { get => Type; set => Type = (StructureType)value; }
    
    public bool IsSelected { get; set; }

    public Vector2Int MapPosition { get; set; }
    public Transform Transform { get; set; }
    public SpriteRenderer SpriteRenderer { get; set; }

    public SpriteRenderer Overlay;
    private JobWork _job;
    public JobWork QueuedJob
    {
        get
        {
            return _job;
        }
        set
        {
            _job = value;
            Overlay.sprite = _job.type.sprite;
            _job.OnFinish += OnJobFinish;
        }
    }
    public void OnJobFinish()
    {
        Overlay.sprite = null;
        _job = null;
    }

    private int _maxHealth = 10;
    private int _health;
    public int Health
    {
        get { return _health; }
        set
        {
            _health = value;
        }
    }

    void Awake()
    {
        Health = _maxHealth;
        Transform = transform;
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }
}