using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Meeple : Entity
{
    // Convenience property for getting the correctly typed Data
    public new MeepleData Data { get { return (MeepleData)base.Data; } }
    
    public Animator Animator;
    public Color DistressTint = new(1, 0, 0, 0.5f);

    public override Vector2Int MapPosition
    {
        get { return GameManager.Instance.MapManager.WorldToMap(Transform.position); }
        set { Transform.position = GameManager.Instance.MapManager.MapToWorld(value); }
    }

    void Awake()
    {
        MovementTarget = null;
    }

    void Update()
    {
        Move();
    }

    protected override void OnDataChange()
    {
        base.OnDataChange();
        SpriteRenderer.material.SetColor("_Tint", Data.InDistress ? DistressTint : new(0, 0, 0, 0));
        SpriteRenderer.material.SetInt("_ShowOutline", Data.IsSelected ? 1 : 0);
        transform.rotation = Data.Asleep ? Quaternion.Euler(0, 0, 90) : new();
    }

    public override void Tick()
    {
        base.Tick();
        
        Data.Food--;

        if (Data.Asleep) Data.Sleep += 3;
        else Data.Sleep--;

        Act();
    }

    #region Jobs

    public void Act()
    {
        // If too hungry, make eating your job
        if(!Data.Asleep && Data.Food < 20 && Data.CurrentJob?.TypeData.Type != JobType.Eat)
        {
            bool reservedEating = ReserveFoodJob();
            // If food was found to eat, this tick is done, if not, continue to other ifs
            if(reservedEating) return;
        }
        // If too tired, make sleeping your job
        if(Data.Sleep < 10 && Data.CurrentJob?.TypeData.Type != JobType.Sleep)
        {
            ReserveSleepJob();
        }
        // If you don't have a job, try to reserve one
        else if (Data.CurrentJob == null)
        {
            //if(Data.CurrentJob != null) Debug.Log("Got a job at " + MovementTarget);
            JobManager.Instance.ReserveJob(this);
            MovementTarget = Data.CurrentJob?.Target.MapPosition;
        }
        // If your job is close enough, work it
        else if (Distance.Between(MapPosition, Data.CurrentJob.Target.MapPosition) < 1.5)
        {
            Debug.Log(Data.CurrentJob.TypeData.Name + "ing with " + Data.CurrentJob.WorkLeft + " work left");
            JobManager.Instance.WorkJob(Data.CurrentJob);
            MovementTarget = null;
        }
    }

    public void ReserveSleepJob()
    {
        // Pick a spot to sleep (or just at yourself)
        Entity sleepSpot = (Data.Bed != null) ? Data.Bed : this;
        // Make sleeping your private job
        JobManager.Instance.ReservePrivateJob(JobType.Sleep, sleepSpot, this);
        // Set yourself as asleep
        Data.Asleep = true;

    }

    public bool ReserveFoodJob()
    {
        // Find the best food, then the closest of that best food
        Entity food = null;
        foreach(EntityType foodType in Data.Type.Foods) {
            food = GameManager.Instance.FindNearestUnreservedEntityOfType(foodType, MapPosition);
            if(food != null) break;
        }
        if(food) Debug.Log("Time to eat " + food?.Data.Name);
        if(!food) Debug.Log("Couldn't find anything to eat");
        // If you couldn't find any food, then just give up
        if(!food) return false;
        // Make eating your private job
        JobManager.Instance.ReservePrivateJob(JobType.Eat, food, this);
        return true;
    }

    public JobResult OnJobFinishedBy(JobWork finishedJob, JobResult result)
    {
        if(finishedJob.TypeData.Type == JobType.Eat)
        {
            Data.Food += ((FoodType)result.type).NutritionValue * result.amount;
            return new JobResult();
        }
        else if(finishedJob.TypeData.Type == JobType.Sleep)
        {
            Data.Asleep = false;
            return new JobResult();
        }
        // TODO: increase skills
        return result;
    }

    #endregion

    #region Movement

    private Vector2? _movementTarget;
    private Queue<Vector2> _path; 
    public Vector2? MovementTarget
    {
        get { return _movementTarget; }
        set
        {
            if (_movementTarget.Equals(value)) return;
            _movementTarget = value;
            _path = null;
        }
    }

    public float Speed
    {
        get
        {
            var speed = Data.Type.BaseSpeed;
            if (Data.Asleep) speed = 0;
            else if (Data.Sleep <= 2) speed *= 0.5f;
            if (Data.Food <= 0) speed *= 0.5f;
            return speed;
        }
    }

    private void Move()
    {
        // If there's nowhere to move to, we're done here
        if (MovementTarget == null || _path?.Count == 0) return;

        // Get a new path, if there isn't one (setting _path to null forces re-pathing)
        _path ??= new Queue<Vector2>(GameManager.Instance.MapManager.FindPath(transform.position, (Vector2)MovementTarget));

        // Check if a potentially newly generated path is empty
        if(_path?.Count == 0) return;

        // Get the next point on the path
        Vector2 currentWaypoint = _path.Peek();
        //Debug.Log("Next waypoint " + currentWaypoint);

        // Move towards the point
        transform.position = Vector2.MoveTowards(transform.position, currentWaypoint, Speed * Time.deltaTime);
        Animator.SetFloat("MoveX", currentWaypoint.x * Speed);
        Animator.SetFloat("MoveY", currentWaypoint.y * Speed);

        // If we're at the path point, remove that point from the path
        if ((Vector2)transform.position == currentWaypoint) _path.Dequeue();
    }

    // public void OnDrawGizmos() {
    // 	if (_path != null) {
    // 		for (int i = targetIndex; i < _path.Length; i ++) {
    // 			Gizmos.color = Color.black;

    // 			if (i == targetIndex) {
    // 				Gizmos.DrawLine(transform.position, _path[i]);
    // 			}
    // 			else {
    // 				Gizmos.DrawLine(_path[i-1], _path[i]);
    // 			}
    // 		}
    // 	}
    // }

    #endregion
}
