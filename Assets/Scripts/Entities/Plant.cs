using System;
using System.Linq;
using UnityEngine;

public class Plant : Structure
{
    // Convenience property for getting Type as PlantType
    public PlantType PlantType { get { return (PlantType)Type; } }
    // How many ticks has this plant grown
    public int Age = 0;
    // At what age will this plant next be minimum harvestable (low chance of any products)
    public int NextMinHarvestAge;
    // At what age will this plant next be fully harvestable (high chance of max products)
    public int NextFullHarvestAge;
    // Is this plant harvestable
    public bool Harvestable { get { return Age > NextMinHarvestAge; } }

    #region Plant-Specific Methods

    void Grow()
    {
        Age++;
        // Set sprite based on if it's harvestable or not
        SpriteRenderer.sprite = Harvestable ? PlantType.harvestableSprite : PlantType.Sprite;
    }

    JobResult Harvest()
    {
        // Figure out how many products
        float harvestPercentage = (Age - NextMinHarvestAge) / (NextFullHarvestAge - NextMinHarvestAge);
        if(harvestPercentage > 1) harvestPercentage = 1;
        float unroundedAmount = harvestPercentage * PlantType.maxProductAmount;
        int amount = (int)Math.Round(unroundedAmount, MidpointRounding.AwayFromZero);
        // Use the leftover as a percent chance of getting one more
        float randomNumber = new System.Random().Next();
        if(randomNumber >= unroundedAmount % 1) amount++;
            
        // Reset harvest cycle
        NextMinHarvestAge = Age + PlantType.timeToMinHarvest;
        NextFullHarvestAge = Age + PlantType.timeToFullHarvest;
        SpriteRenderer.sprite = PlantType.Sprite;
        if(PlantType.destroyedByHarvest) Destroy();

        return new JobResult() { type = PlantType.productType, amount = amount };
    }

    JobResult Destroy()
    {
        return new JobResult() { type = PlantType.destroyProductType, amount = PlantType.destroyProductAmount };
    }

    #endregion

    #region Structure Methods

    void Start()
    {
        NextMinHarvestAge = PlantType.ageToStartHarvestCycle + PlantType.timeToMinHarvest;
        NextFullHarvestAge = PlantType.ageToStartHarvestCycle + PlantType.timeToFullHarvest;
    }
    
    public override void Tick()
    {
        base.Tick();
        // TODO: check if in good condition to grow
        Grow();
    }

    public override JobType[] GetAvailableJobs()
    {
        Debug.Log("Getting jobs!");
        // Get inherited jobs
        var jobTypes = base.GetAvailableJobs();
        // Add constant jobs
        jobTypes = jobTypes.Concat(new JobType[] {
            JobType.Cut
        }).ToArray();
        // Only add harvest job if harvestable
        if(Harvestable) jobTypes = jobTypes.Concat(new JobType[] {
            JobType.Harvest
        }).ToArray();
        // Return all jobs
        return jobTypes;
    }

    public override int GetJobWorkAmount(JobType type)
    {
        return type switch
        {
            // Destroying takes 1/10th the health
            JobType.Destroy => (int)Math.Round(Health / (double)10),
            JobType.Cut => PlantType.timeToCut,
            JobType.Harvest => PlantType.timeToHarvest,
            _ => 10,
        };
    }

    public override JobResult FinishJob(JobType type)
    {
        base.FinishJob(type);
        if(type == JobType.Destroy) return Destroy();
        else if(type == JobType.Harvest) return Harvest();
        else return new JobResult();
    }

    #endregion
}