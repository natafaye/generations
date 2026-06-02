using System;
using Unity.Properties;
using UnityEngine;

namespace Generations {
    public class PlantData : StructureData
    {
        public GrowAbility GrowAbility;

        // Convenience property for getting the correctly typed Type
        public new PlantType Type { get { return (PlantType)base.Type; } }

        public override Sprite Sprite
        {
            get { return GrowAbility.Harvestable ? Type.harvestableSprite : Type.Sprite; }
        }

        public override JobTypeData[] AvailableJobs
        {
            get
            {
                var cut = JobManager.Instance.GetTypeData(JobType.Cut);
                var harvest = JobManager.Instance.GetTypeData(JobType.Harvest);
                return GrowAbility.Harvestable ? 
                    new JobTypeData[] { cut, harvest } : 
                    new JobTypeData[] { cut };
            }
        }

        // Constructor

        public PlantData(PlantType type, int age = 0) : base(type)
        {
            GrowAbility = new GrowAbility(this, age);
        }
        
        public override void Tick()
        {
            GrowAbility.Grow();
        }

        #region Jobs

        public override int GetJobWorkAmount(JobType type)
        {
            return type switch
            {
                // Destroying takes 1/10th the health in ticks
                JobType.Destroy => (int)Math.Round(Health / (double)10),
                JobType.Cut => Type.timeToCut,
                JobType.Harvest => Type.timeToHarvest,
                JobType.Eat => Type.timeToHarvest,
                _ => 10,
            };
        }

        public override JobResult OnJobFinishedAt(JobWork job)
        {
            if(job.TypeData.Type == JobType.Cut) return DestroySelf();
            else if(job.TypeData.Type == JobType.Harvest) return GrowAbility.Harvest();
            else if(job.TypeData.Type == JobType.Eat) return GrowAbility.Harvest();
            else return base.OnJobFinishedAt(job);
        }

        #endregion
    }
}