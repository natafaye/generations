using UnityEngine;

// For meeples, items, and structures
public interface IEntity
{
    public bool IsSelected { get; set; }
    public string Name { get; set; }
    public IEntityType Type { get; set; }
    public Vector2Int MapPosition { get; set; }
    public Transform Transform { get; set; }
    public SpriteRenderer SpriteRenderer { get; set; }

    public JobType[] GetAvailableJobs();
    public int GetJobWorkAmount(JobType jobType);
    public JobResult FinishJob(JobType jobType);
}