using Generations;
using UnityEngine;

namespace Generations
{
    public class GameManager : MonoBehaviour
    {
        // Data
        public int MapWidth = 8;
        public int MapHeight = 8;
        public MeepleType Human;

        // References
        public MapManager MapManager;
        public EntityManager EntityManager;
        public CameraManager CameraManager;

        public StructureType[] Recipes;

        void Start()
        {
            MapManager.Init(MapWidth, MapHeight);
            CameraManager.Init(MapWidth, MapHeight);

            MeepleData human = new(Human);
            human.MapPosition = new Vector2Int(2, 2);
            EntityEvents.RequestCreateEntity?.Invoke(human);

            BuildEvents.RecipesUpdated?.Invoke(Recipes);

            InvokeRepeating(nameof(Tick), 1f, 1f);
        }

        void Tick()
        {
            EntityManager.Tick();
        }
    }
}
