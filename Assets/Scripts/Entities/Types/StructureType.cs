using UnityEngine;

[CreateAssetMenu(fileName = "StructureType", menuName = "ScriptableObjects/StructureType")]
public class StructureType : ScriptableObject, IEntityType
{
    [field: SerializeField]
    public string Name { get; set; }
    [field: SerializeField]
    public Sprite Sprite { get; set; }
    public StructureCategory category;
    public JobWorkType[] availableJobs;
}