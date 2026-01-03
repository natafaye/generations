using System;
using UnityEngine;

public class Plant : Structure
{
    // Convenience property for getting Data as PlantData
    public new PlantData Data {  get { return (PlantData)base.Data; } }

    protected override void OnDataChange()
    {
        base.OnDataChange();
        SpriteRenderer.sprite = Data.Sprite;
    }
    
    public override void Tick()
    {
        base.Tick();
        Grow();
    }

    #region Grow & Harvest

    void Grow()
    {
        // TODO: check if in good condition to grow
        Data.Age++;
    }

    JobResult Harvest()
    {
        // Figure out how many products
        float harvestPercentage = Data.HarvestablePercentage;
        if(harvestPercentage > 1) harvestPercentage = 1;
        float unroundedAmount = harvestPercentage * Data.Type.maxProductAmount;
        int amount = (int)Math.Round(unroundedAmount, MidpointRounding.AwayFromZero);
        // Use the leftover as a percent chance of getting one more
        float randomNumber = new System.Random().Next();
        if(randomNumber >= unroundedAmount % 1) amount++;
            
        // Reset harvest cycle
        Data.NextMinHarvestAge = Data.Age + Data.Type.timeToMinHarvest;
        Data.NextFullHarvestAge = Data.Age + Data.Type.timeToFullHarvest;
        SpriteRenderer.sprite = Data.Type.Sprite;
        if(Data.Type.destroyedByHarvest) DestroyEntity();

        return new JobResult() { type = Data.Type.productType, amount = amount };
    }

    #endregion

    #region Jobs

    public override int GetJobWorkAmount(JobType type)
    {
        return type switch
        {
            // Destroying takes 1/10th the health
            JobType.Destroy => (int)Math.Round(Data.Health / (double)10),
            JobType.Cut => Data.Type.timeToCut,
            JobType.Harvest => Data.Type.timeToHarvest,
            _ => 10,
        };
    }

    public override JobResult FinishJob(JobType type)
    {
        base.FinishJob(type);
        if(type == JobType.Cut) return DestroyEntity();
        else if(type == JobType.Harvest) return Harvest();
        else return new JobResult();
    }

    #endregion
}