using UnityEngine;

[CreateAssetMenu(fileName = "JobType", menuName = "ScriptableObjects/JobType")]
public class JobWork : ScriptableObject
{
    public string jobName;
    public int workAmount;
}