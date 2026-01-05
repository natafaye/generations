using System;

public class Item : Entity
{
    // Convenience property for getting the correctly typed Data
    public new ItemData Data { get { return (ItemData)base.Data; } }

    public override int GetJobWorkAmount(JobType jobType)
    {
        if(jobType == JobType.Eat && Data.Type is FoodType type)
            return type.TimeToEat;
        return base.GetJobWorkAmount(jobType);
    }

    public override JobResult OnJobFinishedAt(JobWork job)
    {
        if(job.TypeData.Type == JobType.Eat && Data.Type is FoodType type)
        {
            // If for some reason there's nothing here, nothing is eaten
            if(Data.ItemsInStack == 0) return new JobResult();

            // How much nutritions do we need, and how much is available
            int neededNutrition = 100 - job.Worker.Data.Food;
            int availableNutrition = type.NutritionValue * Data.ItemsInStack;

            // If the eater needs at least as much nutrition as this item has, eat it all
            // If not, eat only as much as won't be wasted (minimum 1, which could waste some)
            int amountToEat = (neededNutrition >= availableNutrition) ? 
                Data.ItemsInStack : 
                Math.Max(neededNutrition / type.NutritionValue, 1);

            // Remove the eaten food from the stack
            Data.ItemsInStack -= amountToEat;

            // If we've hit 0 in the stack, this item should be destroyed
            if(Data.ItemsInStack == 0) GameManager.Instance.DestroyEntity(this);

            // Return what was eaten
            return new JobResult() { type = Data.Type, amount = amountToEat };
        }
        return base.OnJobFinishedAt(job);
    }
}