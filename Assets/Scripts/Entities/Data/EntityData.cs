using System;
using Unity.Properties;
using UnityEngine;

public class EntityData
{
    public Action OnChange;

    // Unchangeable Properties

    public EntityType Type;
    public string Name;

    // Changeable Properties

    private int _health;
    public int Health { 
        get { return _health; }
        set
        {
            if(value == _health) return;
            _health = value;
            OnChange?.Invoke();
        }
    }

    private JobWork _queuedJob;
    public JobWork QueuedJob
    {
        get { return _queuedJob; }
        set
        {
            if(value == _queuedJob) return;
            _queuedJob = value;
            OnChange?.Invoke();
        }
    }

    private bool _isSelected;
    public bool IsSelected { 
        get { return _isSelected; } 
        set
        {
            if(value == _isSelected) return;
            _isSelected = value;
            OnChange.Invoke();
        }
    }

    // Calculated Properties

    [CreateProperty]
    public virtual Sprite Sprite
    {
        get { return Type.Sprite; }
    }

    [CreateProperty]
    public virtual string Status
    {
        get { return QueuedJob != null ? "Waiting for " + QueuedJob.TypeData.Name : "Chilling"; }
    }

    public virtual JobTypeData[] AvailableJobs
    {
        get { return new JobTypeData[] {}; }
    }

    // Constructor

    public EntityData(EntityType type)
    {
        Name = type.Name;
        Type = type;
        _health = type.MaxHealth;
    }
}