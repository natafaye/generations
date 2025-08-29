using UnityEngine;

[CreateAssetMenu(fileName = "EntityType", menuName = "ScriptableObjects/EntityType")]
public class EntityType : ScriptableObject
{
    public string typeName;

    public JobWork buttonJobs;
}