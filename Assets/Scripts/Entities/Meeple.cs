using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Meeple : Entity
{
    public Animator Animator;
    
    private readonly Color _noTint = new(0, 0, 0, 0);
    public Color DistressTint = new(1, 0, 0, 0.5f);

    private bool _isSelected = false;
    public override bool IsSelected
    {
        get { return _isSelected; }
        set
        {
            _isSelected = value;
            if (_isSelected)
                SpriteRenderer.material.SetInt("_ShowOutline", 1);
            else
                SpriteRenderer.material.SetInt("_ShowOutline", 0);
        }
    }

    public override Vector2Int MapPosition
    {
        get { return GameManager.Instance.MapManager.WorldToMap(Transform.position); }
        set { Transform.position = GameManager.Instance.MapManager.MapToWorld(value); }
    }

    private bool _inDistress;
    public bool InDistress
    {
        get { return _inDistress; }
        private set
        {
            _inDistress = value;
            if (_inDistress)
            {
                SpriteRenderer.material.SetColor("_Tint", DistressTint);
            }
            else
            {
                SpriteRenderer.material.SetColor("_Tint", _noTint);
            }
        }
    }

    private int _food = 100;
    public int Food
    {
        get { return _food; }
        set
        {
            _food = value;
            InDistress = _food < 0;
        }
    }

    private bool _asleep = false;
    public bool Asleep
    {
        get { return _asleep; }
        set
        {
            _asleep = value;
            if (_asleep)
            {
                transform.rotation = Quaternion.Euler(0, 0, 90);
            }
            else
            {
                transform.rotation = new();
            }
        }
    }
    private int _sleep = 100;
    public int Sleep
    {
        get { return _sleep; }
        set
        {
            _sleep = value;
            // Wake up when you're filled up on sleep
            if (Asleep && _sleep >= 5) Asleep = false;
            // Fall asleep when you hit zero
            if (_sleep <= 0) Asleep = true;
        }
    }


    void Awake()
    {
        MovementTarget = null;
    }

    void Update()
    {
        Move();
    }

    public override void Tick()
    {
        base.Tick();
        
        Debug.Log("Meeple Tick");
        Food--;

        if (Asleep) Sleep++;
        else Sleep--;

        if(!Asleep) Work();
    }

    #region Jobs

    public JobWork CurrentJob;

    public void Work()
    {
        // If there is not a job, try to get one
        if (CurrentJob == null)
        {
            Debug.Log("Trying to get a job");
            CurrentJob = JobManager.Instance.ReserveJob(this);
            if (CurrentJob == null) return;
            MovementTarget = CurrentJob.target.MapPosition;
            Debug.Log("Got a job at " + MovementTarget);
        }
        // If the job has been fully worked, finish it
        else if (CurrentJob.Finished)
        {
            Debug.Log("Finishing the job");
            JobManager.Instance.FinishJob(CurrentJob);
            CurrentJob = null;
        }
        // If the unfinished job is close enough, work it
        else if (DistanceBetween(MapPosition, CurrentJob.target.MapPosition) < 1.5)
        {
            Debug.Log("Working the job with " + CurrentJob.workLeft + " left");
            MovementTarget = null;
            JobManager.Instance.WorkJob(CurrentJob);
        }
    }

    public void RemoveCurrentJob()
    {
        if (CurrentJob == null) return;
        CurrentJob.worker = null;
        CurrentJob = null;
        MovementTarget = null;
    }

    #endregion

    #region Movement

    // Map movement
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

    private float _baseSpeed = 4;
    public float Speed
    {
        get
        {
            var speed = _baseSpeed;
            if (Asleep) speed = 0;
            else if (Sleep <= 2) speed *= 0.5f;
            if (Food <= 0) speed *= 0.5f;
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

        // Get the next point on the path
        Vector2 currentWaypoint = _path.Peek();
        Debug.Log("Next waypoint " + currentWaypoint);

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
