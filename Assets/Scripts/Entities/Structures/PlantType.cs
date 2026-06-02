using UnityEngine;

[CreateAssetMenu(fileName = "PlantType", menuName = "ScriptableObjects/StructureType/PlantType")]
public class PlantType : StructureType
{
    // Sprite to use if this plant is currently harvestable
    public Sprite harvestableSprite;

    // Minimum age (in ticks) to start harvest cycle
    public int ageToStartHarvestCycle;

    // How long from start of harvest cycle to minimum harvestable (low chance of giving any products)
    public int timeToMinHarvest;

    // How long from start of harvest cycle to fully harvestable (high chance of giving max products)
    public int timeToFullHarvest;

    // Is this plant destroyed when it is harvested
    public bool destroyedByHarvest;

    // What type of products are harvested from this plant
    public ItemType productType;

    // What is the maximum number of the product that can be harvested
    public int maxProductAmount;

    // How many ticks to cut
    public int timeToCut;
    
    // How many ticks to harvest
    public int timeToHarvest;
}