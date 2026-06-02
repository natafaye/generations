using System;
using Unity.Properties;

namespace Generations {
    public class GrowAbility
    {
        readonly PlantData _plant;

        // How many ticks has this plant grown
        private int _age;
        [CreateProperty]
        public int Age
        {
            get { return _age; }
            set
            {
                if(value == _age) return;
                _age = value;
                _plant.DataChanged?.Invoke();
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
                _plant.DataChanged?.Invoke();
            }
        }
        // At what age will this plant next be fully harvestable (high chance of max products)
        public int NextFullHarvestAge;

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
            get { return MaxMaturity - Math.Max(NextFullHarvestAge - Age, 0); }
        }
        [CreateProperty]
        public float MaxMaturity
        {
            get { return _plant.Type.ageToStartHarvestCycle + _plant.Type.timeToFullHarvest; }
        }

        // Constructor

        public GrowAbility(PlantData plant, int age)
        {
            _plant = plant;
            Age = age;
            NextMinHarvestAge = plant.Type.ageToStartHarvestCycle + plant.Type.timeToMinHarvest;
            NextFullHarvestAge = plant.Type.ageToStartHarvestCycle + plant.Type.timeToFullHarvest;
        }

        /// <summary>
        /// Grow this plant by one tick
        /// </summary>
        public void Grow()
        {
            // TODO: check if in good condition to grow
            Age++;
        }

        /// <summary>
        /// Harvest this plant
        /// </summary>
        /// <returns>The result</returns>
        public JobResult Harvest()
        {
            // Figure out how many products
            float harvestPercentage = HarvestablePercentage;
            if(harvestPercentage > 1) harvestPercentage = 1;
            float unroundedAmount = harvestPercentage * _plant.Type.maxProductAmount;
            int amount = (int)Math.Round(unroundedAmount, MidpointRounding.AwayFromZero);
            // Use the leftover as a percent chance of getting one more
            float randomNumber = new System.Random().Next();
            if(randomNumber >= unroundedAmount % 1) amount++;
                
            // Reset harvest cycle
            NextMinHarvestAge = Age + _plant.Type.timeToMinHarvest;
            NextFullHarvestAge = Age + _plant.Type.timeToFullHarvest;
            if(_plant.Type.destroyedByHarvest) _plant.DestroySelf();

            return new JobResult() { type = _plant.Type.productType, amount = amount };
        }
    }
}