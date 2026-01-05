using UnityEngine;

[CreateAssetMenu(fileName = "FoodType", menuName = "ScriptableObjects/FoodType")]
public class FoodType : ItemType
{
    public int NutritionValue;
    public int TimeToEat;
}