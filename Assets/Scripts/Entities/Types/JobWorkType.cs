using UnityEngine;

[CreateAssetMenu(fileName = "JobWorkType", menuName = "ScriptableObjects/JobWorkType")]
public class JobWorkType : ScriptableObject
{
    [field: SerializeField]
    public string Name { get; set; }
    public int workAmount;
    public ItemType productType;
    public int productAmount;
}