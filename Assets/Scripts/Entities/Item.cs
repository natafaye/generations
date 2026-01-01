using UnityEngine;

public enum ItemCategory
{
    Food,
    Clothing,
    Other
}

[RequireComponent(typeof(SpriteRenderer))]
public class Item : MonoBehaviour, IEntity
{
    [field: SerializeField]
    public string Name { get; set; }

    [field: SerializeField]
    public ItemType Type { get; set; }
    // Handle type inheritance of IEntityType to ItemType
    IEntityType IEntity.Type { get => Type; set => Type = (ItemType)value; }

    public bool IsSelected { get; set; }

    public Vector2Int MapPosition { get; set; }
    public Transform Transform { get; set; }
    public SpriteRenderer SpriteRenderer { get; set; }

    public int ItemsInStack { get; set; }

    void Awake()
    {
        Transform = transform;
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public JobType[] GetAvailableJobs()
    {
        return new JobType[] {};
    }

    public int GetJobWorkAmount(JobType jobType)
    {
        return 0;
    }

    public JobResult FinishJob(JobType jobType)
    {
        return new JobResult();
    }
}