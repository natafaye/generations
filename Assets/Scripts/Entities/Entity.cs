using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Entity : MonoBehaviour
{
    protected EntityData _data;
    public EntityData Data
    {
        get { return _data; }
        set
        {
            _data = value;
            _data.OnChange += OnDataChange;
        }
    }

    public Transform Transform;
    public SpriteRenderer SpriteRenderer;
    public GameObject GameObject;
    
    public virtual Vector2Int MapPosition { get; set; }
    
    public virtual void Tick() { }

    protected virtual void OnDataChange() { }

    #region Jobs

    public SpriteRenderer Overlay;

    public virtual int GetJobWorkAmount(JobType jobType) { return 0; }

    public virtual JobResult FinishJob(JobType jobType) {
        Data.QueuedJob = null;
        return new JobResult(); 
    }

    #endregion
}