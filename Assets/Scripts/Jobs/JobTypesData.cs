using System;
using System.Collections.Generic;
using UnityEngine;

public enum JobType
{
    Destroy,
    Cut,
    Harvest
}

// Scriptable Object for connecting sprites to job types

[Serializable]
public class JobTypeData
{
    public JobType Type;
    public Sprite Sprite;
}

[CreateAssetMenu(fileName = "JobTypeData", menuName = "ScriptableObjects/JobTypes")]
public class JobTypesData: ScriptableObject
{
    public List<JobTypeData> JobTypes;

    public Sprite GetSprite(JobType type)
    {
        return JobTypes.Find(j => j.Type == type).Sprite;
    }
}