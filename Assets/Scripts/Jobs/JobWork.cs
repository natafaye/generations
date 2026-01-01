using System;
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
    public IEntity target;
    public int workLeft;
    public JobManager manager;
    public Meeple worker;

    // public event Action OnFinish;

    public bool Finished { get { return workLeft == 0; } }

    public JobWork(JobType jobType, Sprite jobSprite, IEntity jobTarget, JobManager jobManager)
    {
        type = jobType;
        sprite = jobSprite;
        workLeft = jobTarget.GetJobWorkAmount(jobType);
        target = jobTarget;
        manager = jobManager;
    }

    public void Work()
    {
        workLeft--;
        // if (Finished) OnFinish?.Invoke();
    }
}