using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set;  }

    public MapManager MapManager;
    public EntityManager EntityManager;
    public CameraManager CameraManager;

    public int MapWidth = 8;
    public int MapHeight = 8;

    public UIDocument UIDoc;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        MapManager.Init(MapWidth, MapHeight);
        CameraManager.Init(MapWidth, MapHeight);
        EntityManager.Init();
        InvokeRepeating(nameof(Tick), 2f, 2f);
    }

    void Tick()
    {
        EntityManager.Tick();
    }

    void Update()
    {
        
    }
}
