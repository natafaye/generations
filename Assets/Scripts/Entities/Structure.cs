using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Structure : MonoBehaviour, IEntity
{
    [field: SerializeField]
    public string Name { get; set; }

    [field: SerializeField]
    public StructureType Type { get; set; }
    // Handle type inheritance of IEntityType to StructureType
    IEntityType IEntity.Type { get => Type; set => Type = (StructureType)value; }

    private int _health;
    public int Health
    {
        get { return _health; }
        set { _health = value; }
    }
    
    public bool IsSelected { get; set; }

    public Vector2Int MapPosition { get; set; }
    public Transform Transform { get; set; }
    public SpriteRenderer SpriteRenderer { get; set; }

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
            //_job.OnFinish += OnJobFinish;
        }
    }

    public virtual JobType[] GetAvailableJobs() { return Array.Empty<JobType>(); }

    public virtual int GetJobWorkAmount(JobType jobType) { return 0; }

    public virtual JobResult FinishJob(JobType jobType) {
        QueuedJob = null;
        return new JobResult(); 
    }

    #endregion

    void Awake()
    {
        Transform = transform;
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        Health = Type.maxHealth;
    }

    public virtual void Tick() { }
}