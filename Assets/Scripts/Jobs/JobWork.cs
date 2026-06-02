namespace Generations {

    public class JobResult
    {
        public ItemType type;
        public int amount = 0;
    }

    public class JobWork
    {
        public JobTypeData TypeData;
        public EntityData Target;
        public WorkAbility Worker;
        public int WorkLeft;

        public bool Finished { get { return WorkLeft == 0; } }

        public JobWork(JobTypeData typeData, EntityData jobTarget, int workLeft)
        {
            TypeData = typeData;
            Target = jobTarget;
            WorkLeft = workLeft;
        }
    }

}