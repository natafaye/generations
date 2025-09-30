using System.Collections.Generic;
using UnityEngine;

public class JobWork
{
    public JobWorkType type;
    public IEntity target;
    public int workLeft;
    public JobManager manager;

    public bool Finished { get { return workLeft == 0; } }

    public JobWork(JobWorkType jobType, IEntity jobTarget, JobManager jobManager)
    {
        type = jobType;
        target = jobTarget;
        workLeft = jobType.workAmount;
        manager = jobManager;
    }

    public void Work() {
        workLeft--;
    }
}

public class JobManager
{
    public Queue<JobWork> Jobs;
    public List<JobWork> ReservedJobs;
    public MapManager Map;

    public JobManager(MapManager map)
    {
        Jobs = new();
        ReservedJobs = new();
        Map = map;
    }

    public void AddJob(JobWorkType jobType, IEntity target)
    {
        JobWork newJob = new(jobType, target, this);
        Jobs.Enqueue(newJob);
        Debug.Log("Added " + jobType.Name);
    }

    public JobWork ReserveJob()
    {
        Debug.Log("Trying to reserve job");
        if (Jobs.Count == 0) return null;
        var reserved = Jobs.Dequeue();
        ReservedJobs.Add(reserved);
        Debug.Log("Reserved " + reserved.type.Name);
        return reserved;
    }

    public void FinishJob(JobWork finishedJob)
    {
        Debug.Log("Finishing " + finishedJob.type.Name);
        ReservedJobs.Remove(finishedJob);
        if (finishedJob.type.productAmount == 0) return;
        var position = Map.GetNearestPassableCell(finishedJob.target.MapPosition);

        Item product = new(finishedJob.type.productType, finishedJob.type.productAmount);
        Map.Spawn(product, position.MapPosition);
    }
}