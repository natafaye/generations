using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class Meeple : MonoBehaviour, IEntity
{
    private Animator _animator;

    private readonly Color _noTint = new(0, 0, 0, 0);
    public Color DistressTint = new(1, 0, 0, 0.5f);

    [field: SerializeField]
    public string Name { get; set; }
    [field: SerializeField]
    public IEntityType Type { get; set; }

    // Map movement
    public MapManager Map;
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

    public Vector2Int MapPosition
    {
        get { return Map.WorldToMap(Transform.position); }
        set { Transform.position = Map.MapToWorld(value); }
    }

    public Transform Transform { get; set; }
    public SpriteRenderer SpriteRenderer { get; set; }

    // Job
    public JobWork CurrentJob;

    private bool _isSelected = false;
    public bool IsSelected
    {
        get { return _isSelected; }
        set
        {
            _isSelected = value;

            if (!SpriteRenderer) SpriteRenderer = GetComponent<SpriteRenderer>();
            if (_isSelected)
                SpriteRenderer.material.SetInt("_ShowOutline", 1);
            else
                SpriteRenderer.material.SetInt("_ShowOutline", 0);
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

    void Start()
    {
        Transform = transform;
        SpriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        MovementTarget = null;
    }

    public void Spawn(MapManager map, MapCell cell)
    {
        Map = map;
        transform.position = cell.WorldPosition;
    }

    public void Tick()
    {
        // Food
        Food--;

        // Sleep
        if (Asleep) Sleep++;
        else Sleep--;

        Work();
    }

    public void Work()
    {
        // If there is not a job, try to get one
        if (CurrentJob == null)
        {
            Debug.Log("Trying to get a job");
            CurrentJob = Map.JobManager.ReserveJob(this);
            if (CurrentJob == null) return;
            MovementTarget = CurrentJob.target.MapPosition;
            Debug.Log("Got a job at " + MovementTarget);
        }
        // If the job has been fully worked, finish it
        else if (CurrentJob.Finished)
        {
            Debug.Log("Finishing the job");
            Map.JobManager.FinishJob(CurrentJob);
            CurrentJob = null;
        }
        // If the unfinished job is close enough, work it
        else if (DistanceBetween(MapPosition, CurrentJob.target.MapPosition) < 1.5)
        {
            Debug.Log("Working the job with " + CurrentJob.workLeft + " left");
            MovementTarget = null;
            CurrentJob.Work();
        }
    }

    public double DistanceBetween(Vector2 a, Vector2 b)
    {
        return Math.Sqrt(Math.Pow(a.x - b.x, 2) + Math.Pow(a.y - b.y, 2));
    }

    void Update()
    {
        Move();
    }

    public void RemoveCurrentJob()
    {
        if (CurrentJob == null) return;
        CurrentJob.worker = null;
        CurrentJob = null;
        MovementTarget = null;
    }

    private void Move()
    {
        // If there's nowhere to move to, we're done here
        if (MovementTarget == null || _path?.Count == 0) return;

        // Get a new path, if there isn't one (setting _path to null forces re-pathing)
        _path ??= new Queue<Vector2>(Map.FindPath(transform.position, (Vector2)MovementTarget));

        // Get the next point on the path
        Vector2 currentWaypoint = _path.Peek();
        Debug.Log("Next waypoint " + currentWaypoint);

        // Move towards the point
        transform.position = Vector2.MoveTowards(transform.position, currentWaypoint, Speed * Time.deltaTime);
        _animator.SetFloat("MoveX", currentWaypoint.x * Speed);
        _animator.SetFloat("MoveY", currentWaypoint.y * Speed);

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
}
