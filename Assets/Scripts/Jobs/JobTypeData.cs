using System;
using UnityEngine;

public enum JobType
{
    Destroy,
    Cut,
    Harvest
}

[Serializable]
public class JobTypeData
{
    public JobType Type;
    public string Name;
    public Sprite Sprite;
}