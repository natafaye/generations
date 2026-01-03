using System;
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

        if (Data.Asleep) Data.Sleep++;
        else Data.Sleep--;

        if(!Data.Asleep) Work();
    }

    #region Jobs

    public void Work()
    {
        // If there is not a job, try to get one
        if (Data.CurrentJob == null)
        {
            Data.CurrentJob = JobManager.Instance.ReserveJob(this);
            if (Data.CurrentJob == null) return;
            MovementTarget = Data.CurrentJob.Target.MapPosition;
            Debug.Log("Got a job at " + MovementTarget);
        }
        // If the job has been fully worked, finish it
        else if (Data.CurrentJob.Finished)
        {
            Debug.Log("Finishing the job");
            JobManager.Instance.FinishJob(Data.CurrentJob);
            Data.CurrentJob = null;
        }
        // If the unfinished job is close enough, work it
        else if (DistanceBetween(MapPosition, Data.CurrentJob.Target.MapPosition) < 1.5)
        {
            Debug.Log("Working the job with " + Data.CurrentJob.WorkLeft + " left");
            MovementTarget = null;
            JobManager.Instance.WorkJob(Data.CurrentJob);
        }
    }

    public void RemoveCurrentJob()
    {
        if (Data.CurrentJob == null) return;
        Data.CurrentJob.Worker = null;
        Data.CurrentJob = null;
        MovementTarget = null;
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

    public double DistanceBetween(Vector2 a, Vector2 b)
    {
        return Math.Sqrt(Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2));
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
