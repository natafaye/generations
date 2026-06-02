using UnityEngine;

namespace Generations {
    public class EntityFrame : MonoBehaviour
    {
        public Transform Transform;
        public SpriteRenderer SpriteRenderer;
        public GameObject GameObject;
        public SpriteRenderer Overlay;
        public Animator Animator;

        protected EntityData _data;
        public EntityData Data
        {
            get { return _data; }
            set
            {
                _data = value;
                _data.DataChanged += OnDataChange;
                _data.RequestDestroyFrame += DestroyFrame;
                OnDataChange();
            }
        }

        public void Update()
        {
            Data.Update(this);
        }

        private void OnDataChange()
        {
            Data.UpdateSprite(this);
        }

        private void DestroyFrame()
        {
            Destroy(this);
        }
    }
}