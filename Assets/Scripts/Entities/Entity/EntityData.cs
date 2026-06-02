using System;
using Unity.Properties;
using UnityEngine;

namespace Generations
{
    public class EntityData
    {
        public Action DataChanged;
        public Action RequestDestroyFrame;

        // Unchangeable Properties

        public EntityType Type;
        public string Name;

        #region (MapPosition, Health, IsSelected, QueuedJob, Blueprint)

        public virtual Vector2Int MapPosition { get; set; }

        private int _health;
        [CreateProperty]
        public int Health
        {
            get { return _health; }
            set
            {
                if (value == _health) return;
                _health = value;
                DataChanged?.Invoke();
            }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value == _isSelected) return;
                _isSelected = value;
                DataChanged.Invoke();
            }
        }

        private JobWork _queuedJob;
        public JobWork QueuedJob
        {
            get { return _queuedJob; }
            set
            {
                if (value == _queuedJob) return;
                _queuedJob = value;
                DataChanged?.Invoke();
            }
        }

        private bool _isBlueprint;
        public bool IsBlueprint
        {
            get { return _isBlueprint; }
            set
            {
                if (value == _isBlueprint) return;
                _isBlueprint = value;
                DataChanged?.Invoke();
            }
        }

        #endregion

        // Calculated Properties

        [CreateProperty]
        public virtual Sprite Sprite => Type.Sprite;

        [CreateProperty]
        public virtual string Status => QueuedJob != null ? "Waiting for " + QueuedJob.TypeData.Name : "Chilling";

        public virtual JobTypeData[] AvailableJobs => new JobTypeData[] { };

        // Constructor

        public EntityData(EntityType type)
        {
            Name = type.Name;
            Type = type;
            Health = type.MaxHealth;
        }

        public void Destroy()
        {
            RequestDestroyFrame?.Invoke();
            EntityEvents.EntityDestroyed?.Invoke(this);
        }

        // Virtual Methods

        public virtual void Tick() { }

        public virtual void Update(EntityFrame frame) { }

        public virtual void UpdateSprite(EntityFrame frame) { }

        public virtual int GetJobWorkAmount(JobType jobType) { return 10; }

        public virtual JobResult OnJobFinishedAt(JobWork job) { return new JobResult(); }
    }
}