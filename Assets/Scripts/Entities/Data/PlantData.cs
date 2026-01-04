using System;
using Unity.Properties;
using UnityEngine;

public class PlantData : StructureData
{
    // Convenience property for getting the correctly typed Type
    public new PlantType Type { get { return (PlantType)base.Type; } }

    #region Changeable Properties (Age, NextMinHarvestAge, NextFullHarvestAge)

    // How many ticks has this plant grown
    private int _age;
    public int Age
    {
        get { return _age; }
        set
        {
            if(value == _age) return;
            _age = value;
            OnChange?.Invoke();
        }
    }

    // At what age will this plant next be minimum harvestable (low chance of any products)
    private int _nextMinHarvestAge;
    public int NextMinHarvestAge
    {
        get { return _nextMinHarvestAge; }
        set
        {
            if(value == _nextMinHarvestAge) return;
            _nextMinHarvestAge = value;
            OnChange?.Invoke();
        }
    }
    // At what age will this plant next be fully harvestable (high chance of max products)
    public int NextFullHarvestAge;

    #endregion

    #region Calculated Properties (Harvestable, HarvestablePercentage, CurrentMaturity, MaxMaturity, Sprite, AvailableJobs)

    // Is this plant harvestable
    public bool Harvestable { get { return Age > NextMinHarvestAge; } }

    // How harvestable is this plant
    public float HarvestablePercentage { 
        get { return (Age - NextMinHarvestAge) / (NextFullHarvestAge - NextMinHarvestAge); }
    }

    // Pretty version of harvestability/age for display
    [CreateProperty]
    public float CurrentMaturity
    {
        get { return 100 - Math.Max(0, NextFullHarvestAge - Age); }
    }
    [CreateProperty]
    public float MaxMaturity
    {
        get { return Type.ageToStartHarvestCycle + Type.timeToFullHarvest; }
    }

    public override Sprite Sprite
    {
        get { return Harvestable ? Type.harvestableSprite : Type.Sprite; }
    }

    public override JobTypeData[] AvailableJobs
    {
        get
        {
            var cut = JobManager.Instance.GetTypeData(JobType.Cut);
            var harvest = JobManager.Instance.GetTypeData(JobType.Harvest);
            return Harvestable ? 
                new JobTypeData[] { cut, harvest } : 
                new JobTypeData[] { cut };
        }
    }

    #endregion

    // Constructor

    public PlantData(EntityType type, int age = 0) : base(type)
    {
        Age = age;
        NextMinHarvestAge = Type.ageToStartHarvestCycle + Type.timeToMinHarvest;
        NextFullHarvestAge = Type.ageToStartHarvestCycle + Type.timeToFullHarvest;
    }
}