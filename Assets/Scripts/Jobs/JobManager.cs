using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JobManager: MonoBehaviour
{
    public static JobManager Instance { get; private set; }

    public JobTypeData[] JobTypes;

    public List<JobWork> Jobs = new();
    public List<JobWork> ReservedJobs = new();
    public List<JobWork> PrivateJobs = new();

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

    private JobWork CreateJob(JobTypeData jobTypeData, Entity target)
    {
        // Get how long the job takes
        int workLeft = target.GetJobWorkAmount(jobTypeData.Type);

        // Create the job object
        JobWork newJob = new(jobTypeData, target, workLeft);

        // Attach to target
        if (target.Data.QueuedJob != null) RemoveJob(target.Data.QueuedJob);
        target.Data.QueuedJob = newJob;

        return newJob;
    }

    public void AddJob(JobTypeData jobTypeData, Entity target)
    {
        // Attach to manager
        Jobs.Add(CreateJob(jobTypeData, target));
    }

    public void RemoveJob(JobWork jobToRemove)
    {
        // Detach from manager
        var _ = Jobs.Remove(jobToRemove)
            || ReservedJobs.Remove(jobToRemove)
            || PrivateJobs.Remove(jobToRemove);
        // Detach from target
        if (jobToRemove.Target.Data.QueuedJob == jobToRemove) 
            jobToRemove.Target.Data.QueuedJob = null;
        // Detach from worker
        if (jobToRemove.Worker) {
            jobToRemove.Worker.Data.CurrentJob = null;
            jobToRemove.Worker.MovementTarget = null;
        }
    }

    // Add and reserve a job only performed by this meeple, not available to other meeples
    public void ReservePrivateJob(JobType jobType, Entity target, Meeple meeple)
    {
        // Create job and attach to target
        JobWork privateJob = CreateJob(GetTypeData(jobType), target);
        // Attach to manager
        PrivateJobs.Add(privateJob);
        // Attach to worker
        privateJob.Worker = meeple;
        meeple.Data.CurrentJob = privateJob;
        privateJob.Worker.MovementTarget = target.MapPosition;
    }

    public JobWork ReserveJob(Meeple meeple)
    {
        if (Jobs.Count == 0) return null;
        // Move to reserved jobs
        var reserved = Jobs[0];
        Jobs.RemoveAt(0);
        if(!ReservedJobs.Contains(reserved)) ReservedJobs.Add(reserved);
        // Attach to worker
        reserved.Worker = meeple;
        meeple.Data.CurrentJob = reserved;
        meeple.MovementTarget = reserved.Target.MapPosition;
        // Return the job
        return reserved;
    }

    public void UnreserveJob(JobWork job)
    {
        if(job == null) return;
        // Move to unreserved jobs (or just remove if private)
        ReservedJobs.Remove(job);
        if(!PrivateJobs.Contains(job) && !Jobs.Contains(job)) Jobs.Add(job);
        PrivateJobs.Remove(job);
        // Detach from worker
        job.Worker.Data.CurrentJob = null;
        job.Worker.MovementTarget = null;
        job.Worker = null;
    }

    public void WorkJob(JobWork job)
    {
        job.WorkLeft--;
        if(job.WorkLeft <= 0) FinishJob(job);
    }

    public void FinishJob(JobWork finishedJob)
    {
        // Tell the target the job is finished
        JobResult result = finishedJob.Target.OnJobFinishedAt(finishedJob);

        // Tell the worker the job is finished
        result = finishedJob.Worker.OnJobFinishedBy(finishedJob, result);

        // Create any resulting items
        if (result.amount > 0) {
            ItemData data = new(result.type, result.amount);
            GameManager.Instance.CreateEntity(data, finishedJob.Target.MapPosition);
        }
        
        // Detach from manager, target, and worker
        RemoveJob(finishedJob);
    }
}