using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Entity : MonoBehaviour
{
    public string Name;
    public EntityType Type;
    public int Health;
    public Transform Transform;
    public SpriteRenderer SpriteRenderer;
    public GameObject GameObject;
    
    public virtual bool IsSelected { get; set; }
    public virtual Vector2Int MapPosition { get; set; }

    public virtual void Start()
    {
        Health = Type.MaxHealth;
    }
    
    public virtual void Tick() { }

    #region Jobs

    public SpriteRenderer Overlay;

    private JobWork _queuedJob;
    public JobWork QueuedJob
    {
        get
        {
            return _queuedJob;
        }
        set
        {
            _queuedJob = value;
            Overlay.sprite = (_queuedJob != null) ? _queuedJob.sprite : null;
        }
    }

    public virtual JobType[] GetAvailableJobs() { return new JobType[] {}; }

    public virtual int GetJobWorkAmount(JobType jobType) { return 0; }

    public virtual JobResult FinishJob(JobType jobType) {
        QueuedJob = null;
        return new JobResult(); 
    }

    #endregion
}