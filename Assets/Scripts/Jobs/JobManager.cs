using System;
using System.Collections.Generic;
using UnityEngine;

public class JobWork
{
    public JobWorkType type;
    public IEntity target;
    public int workLeft;
    public JobManager manager;
    public Meeple worker;

    public event Action OnFinish;

    public bool Finished { get { return workLeft == 0; } }

    public JobWork(JobWorkType jobType, IEntity jobTarget, JobManager jobManager)
    {
        type = jobType;
        target = jobTarget;
        workLeft = jobType.workAmount;
        manager = jobManager;
    }

    public void Work()
    {
        workLeft--;
        if (Finished) OnFinish?.Invoke();
    }
}

public class JobManager
{
    public List<JobWork> Jobs;
    public List<JobWork> ReservedJobs;
    public MapManager Map;

    public JobManager(MapManager map)
    {
        Jobs = new();
        ReservedJobs = new();
        Map = map;
    }

    public void AddJob(JobWorkType jobType, Structure structure)
    {
        JobWork newJob = new(jobType, structure, this);
        Jobs.Add(newJob);

        // If this replaces another job, get rid of that job
        if (structure.QueuedJob != null)
        {
            // Remove from job queue
            RemoveJob(structure.QueuedJob);
        }

        // Set this as the structure's queued job
        structure.QueuedJob = newJob;
        Debug.Log("Added " + jobType.Name);
    }

    public void RemoveJob(JobWork jobToRemove)
    {
        if (Jobs.Contains(jobToRemove))
        {
            // If it's unreserved
            Jobs.Remove(jobToRemove);
        }
        else if (ReservedJobs.Contains(jobToRemove))
        {
            // If it's reserved
            if (jobToRemove.worker) jobToRemove.worker.RemoveCurrentJob();
            ReservedJobs.Remove(jobToRemove);
        }
    }

    public JobWork ReserveJob(Meeple meeple)
    {
        Debug.Log("Trying to reserve job");
        if (Jobs.Count == 0) return null;
        var reserved = Jobs[0];
        Jobs.RemoveAt(0);
        ReservedJobs.Add(reserved);
        reserved.worker = meeple;
        Debug.Log("Reserved " + reserved.type.Name);
        return reserved;
    }

    public void FinishJob(JobWork finishedJob)
    {
        Debug.Log("Finishing " + finishedJob.type.Name);
        ReservedJobs.Remove(finishedJob);

        // Create any produced items
        if (finishedJob.type.productAmount == 0) return;
        Item product = (Item)Map.EntityManager.Create(finishedJob.type.productType);
        product.ItemsInStack = finishedJob.type.productAmount;
        var cell = Map.FindNearestCell(finishedJob.target.MapPosition, cell => cell.Empty);
        cell.MoveToCell(product);
    }
}