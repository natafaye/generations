using UnityEngine;

public enum StructureCategory
{
    Plant,
    Wall,
    Bed
}

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