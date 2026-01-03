
public class JobResult
{
    public ItemType type;
    public int amount = 0;
}

public class JobWork
{
    public JobTypeData TypeData;
    public Entity Target;
    public Meeple Worker;
    public int WorkLeft;

    public bool Finished { get { return WorkLeft == 0; } }

    public JobWork(JobTypeData typeData, Entity jobTarget)
    {
        TypeData = typeData;
        Target = jobTarget;
        WorkLeft = jobTarget.GetJobWorkAmount(typeData.Type);
    }
}