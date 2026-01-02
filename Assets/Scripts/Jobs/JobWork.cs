using UnityEngine;

public class JobResult
{
    public ItemType type;
    public int amount = 0;
}

public class JobWork
{
    public JobType type;
    public Sprite sprite;
    public Entity target;
    public Meeple worker;
    public int workLeft;

    public bool Finished { get { return workLeft == 0; } }

    public JobWork(JobType jobType, Sprite jobSprite, Entity jobTarget)
    {
        type = jobType;
        sprite = jobSprite;
        target = jobTarget;
        workLeft = jobTarget.GetJobWorkAmount(jobType);
    }
}