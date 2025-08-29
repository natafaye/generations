using System;
using System.Collections.Generic;

public enum JobType
{
    Haul,
    Harvest,
    Build
}

public class Job
{
    public JobType Type;
}

public class JobManager
{
    public List<Job> Jobs;

    public JobManager()
    {
        Jobs = new();
    }
}