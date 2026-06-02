using System;

namespace Generations
{
    public class ItemData : EntityData
    {
        // Convenience property for getting the correctly typed Type
        public new ItemType Type { get { return (ItemType)base.Type; } }

        private int _itemsInStack;
        public int ItemsInStack
        {
            get { return _itemsInStack; }
            set
            {
                if (value == _itemsInStack) return;
                _itemsInStack = value;
                DataChanged?.Invoke();
            }
        }

        public ItemData(ItemType type, int itemsInStack) : base(type)
        {
            ItemsInStack = itemsInStack;
        }

        public override int GetJobWorkAmount(JobType jobType)
        {
            if (jobType == JobType.Eat && Type is FoodType type)
                return type.TimeToEat;
            return base.GetJobWorkAmount(jobType);
        }

        public override JobResult OnJobFinishedAt(JobWork job)
        {
            if (job.TypeData.Type == JobType.Eat && Type is FoodType type)
            {
                // If for some reason there's nothing here, nothing is eaten
                if (ItemsInStack == 0) return new JobResult();

                // How much nutritions do we need, and how much is available
                int neededNutrition = job.Worker.NutritionNeeded;
                int availableNutrition = type.NutritionValue * ItemsInStack;

                // If the eater needs at least as much nutrition as this item has, eat it all
                // If not, eat only as much as won't be wasted (minimum 1, which could waste some)
                int amountToEat = (neededNutrition >= availableNutrition) ?
                    ItemsInStack :
                    Math.Max(neededNutrition / type.NutritionValue, 1);

                // Remove the eaten food from the stack
                ItemsInStack -= amountToEat;

                // If we've hit 0 in the stack, this item should be destroyed
                if (ItemsInStack == 0) Destroy();

                // Return what was eaten
                return new JobResult() { type = Type, amount = amountToEat };
            }
            return base.OnJobFinishedAt(job);
        }
    }
}