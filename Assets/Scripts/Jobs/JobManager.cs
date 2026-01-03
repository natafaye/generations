using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JobManager: MonoBehaviour
{
    public static JobManager Instance { get; private set; }

    public JobTypeData[] JobTypes;

    public List<JobWork> Jobs = new();
    public List<JobWork> ReservedJobs = new();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public JobTypeData GetTypeData(JobType type)
    {
        return JobTypes.First(j => j.Type == type);
    }

    public void AddJob(JobTypeData jobTypeData, Entity target)
    {
        JobWork newJob = new(jobTypeData, target);
        Jobs.Add(newJob);

        // Entities can only have one job queued at a time
        if (target.Data.QueuedJob != null) RemoveJob(target.Data.QueuedJob);

        // Set this as the entity's queued job
        target.Data.QueuedJob = newJob;

        Debug.Log(target.Data.QueuedJob);
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
            if (jobToRemove.Worker) jobToRemove.Worker.RemoveCurrentJob();
            ReservedJobs.Remove(jobToRemove);
        }
    }

    public JobWork ReserveJob(Meeple meeple)
    {
        //Debug.Log("Trying to reserve job");
        if (Jobs.Count == 0) return null;
        var reserved = Jobs[0];
        Jobs.RemoveAt(0);
        ReservedJobs.Add(reserved);
        reserved.Worker = meeple;
        Debug.Log("Reserved " + reserved.TypeData.Type);
        return reserved;
    }

    public void WorkJob(JobWork job)
    {
        job.WorkLeft--;
    }

    public void FinishJob(JobWork finishedJob)
    {
        //Debug.Log("Finishing " + finishedJob.type);
        ReservedJobs.Remove(finishedJob);

        // Create any produced items
        JobResult result = finishedJob.Target.FinishJob(finishedJob.TypeData.Type);
        if (result.amount > 0) {
            ItemData data = new(result.type, result.amount);
            GameManager.Instance.CreateEntity(data, finishedJob.Target.MapPosition);
        }
    }
}