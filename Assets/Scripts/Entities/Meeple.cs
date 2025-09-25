using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class Meeple : MonoBehaviour, IEntity
{
    private Rigidbody2D _rigidbody;
    private Animator _animator;

    private readonly Color _noTint = new(0, 0, 0, 0);
    public Color DistressTint = new(1, 0, 0, 0.5f);

    [field: SerializeField]
    public string Name { get; set; }
    [field: SerializeField]
    public IEntityType Type { get; set; }

    // Map movement
    public MapManager Map;
    public Transform MovementTarget;
    private Vector2[] _path;
    private static WaitForSeconds _waitFor25 = new(.25f);
    int targetIndex;

    public Vector2Int MapPosition { get; set; }
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

    private int _food = 10;
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
    private int _sleep = 5;
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
        _rigidbody = GetComponent<Rigidbody2D>();
        MovementTarget = null;
        Food = 10;
    }

    public void Spawn(MapManager map, MapCell cell)
    {
        Map = map;
        if (!_rigidbody) _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.position = cell.WorldPosition;
        StartCoroutine(RefreshPath());
    }

    public void Tick()
    {
        Food--;

        if (Asleep) Sleep++;
        else Sleep--;

        if (CurrentJob == null)
        {
            CurrentJob = Map.JobManager.ReserveJob();
            if (CurrentJob == null) return;
            MovementTarget = CurrentJob.target.Transform;
        }
        // else if(transform.position CurrentJob.target.MapPosition)
        // {
        //     CurrentJob.Work();
        // }

    }

	IEnumerator RefreshPath() {
        Debug.Log("Trying to refresh path");

        while (MovementTarget == null) yield return _waitFor25;

        Debug.Log("Refreshing path");

		Vector2 targetPositionOld = (Vector2)MovementTarget.position + Vector2.up; // ensure != to target.position initially
			
		while (true) {
			if (targetPositionOld != (Vector2)MovementTarget.position) {
				targetPositionOld = (Vector2)MovementTarget.position;

				_path = Map.FindPath(transform.position, MovementTarget.position);
				StopCoroutine(nameof(FollowPath));
				StartCoroutine(nameof(FollowPath));
			}

			yield return _waitFor25;
		}
	}
		
	IEnumerator FollowPath() {
		if (_path.Length > 0)
		{
			targetIndex = 0;
			Vector2 currentWaypoint = _path[0];

			while (true)
			{
				if ((Vector2)transform.position == currentWaypoint)
				{
					targetIndex++;
					if (targetIndex >= _path.Length) yield break;
					currentWaypoint = _path[targetIndex];
				}

                _rigidbody.MovePosition(Vector2.MoveTowards(transform.position, currentWaypoint, Speed * Time.deltaTime));
                _animator.SetFloat("MoveX", currentWaypoint.x * Speed);
                _animator.SetFloat("MoveY", currentWaypoint.y * Speed);
				yield return null;
			}
		}
	}

	public void OnDrawGizmos() {
		if (_path != null) {
			for (int i = targetIndex; i < _path.Length; i ++) {
				Gizmos.color = Color.black;

				if (i == targetIndex) {
					Gizmos.DrawLine(transform.position, _path[i]);
				}
				else {
					Gizmos.DrawLine(_path[i-1], _path[i]);
				}
			}
		}
	}
}
