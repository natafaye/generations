namespace Generations
{
    public class WorkAbility
    {
        readonly MeepleData _meeple;

        public string Status => CurrentJob.TypeData.Name + "ing " + CurrentJob.Target.Name;

        public int NutritionNeeded => 100 - _meeple.Food;

        private JobWork _currentJob;
        public JobWork CurrentJob
        {
            get { return _currentJob; }
            set
            {
                if (value == _currentJob) return;
                _currentJob = value;
                _meeple.SetMovementTarget(_currentJob?.Target.MapPosition);
                _meeple.DataChanged?.Invoke();
            }
        }

        public WorkAbility(MeepleData meeple)
        {
            _meeple = meeple;
        }

        public void Act()
        {
            // If too hungry, make eating your job
            if (!_meeple.Asleep && _meeple.Food < 20 && CurrentJob?.TypeData.Type != JobType.Eat)
            {
                bool reservedEating = ReserveFoodJob();
                // If food was found to eat, this tick is done, if not, continue to other ifs
                if (reservedEating) return;
            }
            // If too tired, make sleeping your job
            if (_meeple.Sleep < 10 && CurrentJob?.TypeData.Type != JobType.Sleep)
            {
                ReserveSleepJob();
            }
            // If you don't have a job, try to reserve one
            else if (CurrentJob == null)
            {
                JobManager.Instance.ReserveJob(this);
            }
            // If your job is close enough, work it
            else if (MapUtilities.DistanceBetween(_meeple.MapPosition, CurrentJob.Target.MapPosition) < 1.5)
            {
                JobManager.Instance.WorkJob(CurrentJob);
                _meeple.SetMovementTarget(null);
            }
        }

        public JobResult OnJobFinishedBy(JobWork finishedJob, JobResult result)
        {
            if (finishedJob.TypeData.Type == JobType.Eat)
            {
                _meeple.Food += ((FoodType)result.type).NutritionValue * result.amount;
                return new JobResult();
            }
            else if (finishedJob.TypeData.Type == JobType.Sleep)
            {
                _meeple.Asleep = false;
                return new JobResult();
            }
            // TODO: increase skills
            return result;
        }

        private void ReserveSleepJob()
        {
            // Pick a spot to sleep (or just at yourself)
            EntityData sleepSpot = (_meeple.Bed != null) ? _meeple.Bed : _meeple;
            // Make sleeping your private job
            JobManager.Instance.ReservePrivateJob(JobType.Sleep, sleepSpot, this);
            // Set yourself as asleep
            _meeple.Asleep = true;

        }

        private bool ReserveFoodJob()
        {
            // Find the best food, then the closest of that best food
            EntityData food = null;
            foreach (EntityType foodType in _meeple.Type.Foods)
            {
                food = EntityEvents.FindNearestMatchingEntity.Invoke(_meeple.MapPosition, e => e.Type == foodType && e.QueuedJob == null);
                if (food != null) break;
            }
            // If you couldn't find any food, then just give up
            if (food == null) return false;
            // Make eating your private job
            JobManager.Instance.ReservePrivateJob(JobType.Eat, food, this);
            return true;
        }
    }
}