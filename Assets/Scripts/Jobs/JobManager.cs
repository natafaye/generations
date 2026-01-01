using System.Collections.Generic;
using UnityEngine;

public class JobManager
{
    public List<JobWork> Jobs;
    public List<JobWork> ReservedJobs;
    public MapManager Map;
    public JobTypesData JobTypesData;

    public JobManager(MapManager map, JobTypesData jobTypesData)
    {
        Jobs = new();
        ReservedJobs = new();
        Map = map;
        JobTypesData = jobTypesData;
    }

    public void AddJob(JobType jobType, Structure structure)
    {
        JobWork newJob = new(
            jobType, 
            JobTypesData.GetSprite(jobType), 
            structure, 
            this
        );
        Jobs.Add(newJob);

        // If this replaces another job, get rid of that job
        if (structure.QueuedJob != null) RemoveJob(structure.QueuedJob);

        // Set this as the structure's queued job
        structure.QueuedJob = newJob;
        Debug.Log("Added " + jobType);
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
        Debug.Log("Reserved " + reserved.type);
        return reserved;
    }

    public void FinishJob(JobWork finishedJob)
    {
        Debug.Log("Finishing " + finishedJob.type);
        ReservedJobs.Remove(finishedJob);

        // Create any produced items
        JobResult result = finishedJob.target.FinishJob(finishedJob.type);
        if (result.amount == 0) return;
        Item product = (Item)Map.EntityManager.Create(result.type);
        product.ItemsInStack = result.amount;
        var cell = Map.FindNearestCell(finishedJob.target.MapPosition, cell => cell.Empty);
        cell.MoveToCell(product);
    }
}