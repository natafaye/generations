using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Generations {
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

        private JobWork CreateJob(JobTypeData jobTypeData, EntityData target)
        {
            // Get how long the job takes
            int workLeft = target.GetJobWorkAmount(jobTypeData.Type);

            // Create the job object
            JobWork newJob = new(jobTypeData, target, workLeft);

            // Attach to target
            if (target.QueuedJob != null) RemoveJob(target.QueuedJob);
            target.QueuedJob = newJob;

            return newJob;
        }

        public void AddJob(JobTypeData jobTypeData, EntityData target)
        {
            // Attach to manager
            Jobs.Add(CreateJob(jobTypeData, target));
        }

        public void RemoveJob(JobWork jobToRemove)
        {
            if(jobToRemove == null) return;
            // Detach from manager
            var _ = Jobs.Remove(jobToRemove)
                || ReservedJobs.Remove(jobToRemove)
                || PrivateJobs.Remove(jobToRemove);
            // Detach from target
            if (jobToRemove.Target.QueuedJob == jobToRemove) 
                jobToRemove.Target.QueuedJob = null;
            // Detach from worker
            if (jobToRemove.Worker != null) {
                jobToRemove.Worker.CurrentJob = null;
            }
        }

        // Add and reserve a job only performed by this meeple, not available to other meeples
        public void ReservePrivateJob(JobType jobType, EntityData target, WorkAbility worker)
        {
            // Create job and attach to target
            JobWork privateJob = CreateJob(GetTypeData(jobType), target);
            // Attach to manager
            PrivateJobs.Add(privateJob);
            // Attach to worker
            privateJob.Worker = worker;
            worker.CurrentJob = privateJob;
        }

        public void ReserveJob(WorkAbility worker)
        {
            if (Jobs.Count == 0) return;
            // Move to reserved jobs
            var reserved = Jobs[0];
            Jobs.RemoveAt(0);
            if(!ReservedJobs.Contains(reserved)) ReservedJobs.Add(reserved);
            // Attach worker and job
            reserved.Worker = worker;
            worker.CurrentJob = reserved;
        }

        public void UnreserveJob(JobWork job)
        {
            if(job == null) return;
            // Move to unreserved jobs (or just remove if private)
            ReservedJobs.Remove(job);
            if(!PrivateJobs.Contains(job) && !Jobs.Contains(job)) Jobs.Add(job);
            PrivateJobs.Remove(job);
            // Detach from worker
            job.Worker.CurrentJob = null;
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
                ItemData data = new(result.type, result.amount) {
                    MapPosition = finishedJob.Target.MapPosition
                };
                EntityEvents.RequestCreateEntity.Invoke(data);
            }
            
            // Detach from manager, target, and worker
            RemoveJob(finishedJob);
        }
    }
}