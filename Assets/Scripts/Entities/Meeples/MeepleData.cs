using System;
using Unity.Properties;
using UnityEngine;

namespace Generations {
    public class MeepleData : EntityData
    {
        public Color DistressTint = new(1, 0, 0, 0.5f);

        readonly MoveAbility _moveAbility;
        readonly WorkAbility _workAbility;

        public bool InDistress;
        public bool Asleep;
        public StructureData Bed;

        // Convenience property for getting the correctly typed Type
        [CreateProperty]
        public new MeepleType Type { get { return (MeepleType)base.Type; } }

        private int _food;
        [CreateProperty]
        public int Food
        {
            get => _food;
            set
            {
                if(value == _food) return;
                _food = Math.Min(value, Type.MaxFood);
                // If we hit negatives on food, we're in distress
                InDistress = _food < 0;
                DataChanged?.Invoke();
            }
        }

        private int _sleep;
        [CreateProperty]
        public int Sleep
        {
            get => _sleep;
            set
            {
                if(value == _sleep) return;
                _sleep = value;
                // Fall asleep immediately when you hit zero
                if (_sleep <= 0) Asleep = true;
                DataChanged?.Invoke();
            }
        }

        public override string Status
        {
            get
            {
                if(Asleep) return "Asleep";
                if(_workAbility.CurrentJob != null) return _workAbility.Status;
                return "Chilling";
            }
        }

        // Constructor
        public MeepleData(MeepleType type) : base(type)
        {
            Food = type.MaxFood;
            Sleep = 100;
            Asleep = false;
            InDistress = false;
            _moveAbility = new MoveAbility(this);
            _workAbility = new WorkAbility(this);
        }

        public override void Update(EntityFrame frame)
        {
            _moveAbility.Move(frame);
        }

        public override void UpdateSprite(EntityFrame frame)
        {
            frame.SpriteRenderer.sprite = Type.Sprite;
            frame.SpriteRenderer.material.SetColor("_Tint", InDistress ? DistressTint : new(0, 0, 0, 0));
            frame.SpriteRenderer.material.SetInt("_ShowOutline", IsSelected ? 1 : 0);
            frame.Transform.rotation = Asleep ? Quaternion.Euler(0, 0, 90) : new();
        }

        public override int GetJobWorkAmount(JobType jobType)
        {
            if (jobType == JobType.Sleep) return 30;
            return base.GetJobWorkAmount(jobType);
        }

        public override void Tick()
        {
            base.Tick();
            
            Food--;

            if (Asleep) Sleep += 3;
            else Sleep--;

            _workAbility.Act();
        }

        public void SetMovementTarget(Vector2? target)
        {
            _moveAbility.MovementTarget = target;
        }
    }
}