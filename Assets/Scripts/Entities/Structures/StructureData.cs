using System;
using Unity.Properties;
using UnityEngine;

namespace Generations {
    public class StructureData : EntityData
    {
        // Convenience property for getting the correctly typed Type
        public new StructureType Type { get { return (StructureType)base.Type; } }

        private int _rotation;
        [CreateProperty]
        public int Rotation
        {
            // 0 faces right, 1 faces down, 2 faces left, 3 faces up
            get => _rotation;
            set
            {
                if(value == _rotation) return;
                _rotation = value;
                DataChanged?.Invoke();
            }
        }

        public override Sprite Sprite => (Type.RotatedSprite == null || Rotation == 0 || Rotation == 2) ? Type.Sprite : Type.RotatedSprite;

        // Constructor

        public StructureData(StructureType type, int rotation = 0) : base(type)
        {
            Rotation = rotation;
        }

        // Overriden Methods

        public override void UpdateSprite(EntityFrame frame)
        {
            base.UpdateSprite(frame);
            
            frame.SpriteRenderer.sprite = Sprite;
            frame.SpriteRenderer.flipX = Rotation == 2;
            frame.SpriteRenderer.flipY = Rotation == 3;

            frame.Overlay.sprite = QueuedJob?.TypeData.Sprite;
            if(IsBlueprint) frame.SpriteRenderer.color = new Color(1, 1, 1, 0.3f);
            else frame.SpriteRenderer.color = new Color(1, 1, 1, 1);
        }

        public override int GetJobWorkAmount(JobType type)
        {
            return type switch
            {
                // Destroying takes 1/10th the health in ticks
                JobType.Destroy => (int)Math.Round(Health / (double)10),
                _ => base.GetJobWorkAmount(type),
            };
        }

        // Type Specific Methods

        public JobResult DestroySelf()
        {
            Destroy();
            return new JobResult() { 
                type = Type.destroyProductType, 
                amount = Type.destroyProductAmount 
            };
        }
    }
}