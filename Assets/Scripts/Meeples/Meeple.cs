using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class Meeple : MonoBehaviour, IPointerClickHandler
{
    public event Action<Meeple> OnClick;

    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    private readonly Color _noTint = new(0, 0, 0, 0);
    public Color DistressTint = new(1, 0, 0, 0.5f);

    public string Name;

    // Map movement
    public MapManager Map;
    public Transform MovementTarget;
    private Vector2[] _path;
    private static WaitForSeconds _waitFor25 = new(.25f);
    int targetIndex;

    private float _baseSpeed = 1;
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
                _spriteRenderer.material.SetColor("_Tint", DistressTint);
            }
            else
            {
                _spriteRenderer.material.SetColor("_Tint", _noTint);
            }
        }
    }

    private int _food = 10;
    private static readonly int hungryLevel = 2;
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

    private bool _selected = false;
    public bool Selected
    {
        get { return _selected; }
        set
        {
            _selected = value;
            if (_selected)
                _spriteRenderer.material.SetInt("_ShowOutline", 1);
            else
                _spriteRenderer.material.SetInt("_ShowOutline", 0);
        }
    }

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
        Food = 10;
    }

    public void Spawn(MapManager map, MapCell cell)
    {
        Map = map;
        if (!_rigidbody) _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.position = cell.WorldPosition;
        StartCoroutine(RefreshPath());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke(this);
    }

    public void Tick()
    {
        Food--;

        if (Asleep) Sleep++;
        else Sleep--;
    }

	IEnumerator RefreshPath() {
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
