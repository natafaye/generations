using System.Collections.Generic;
using UnityEngine;

namespace Generations
{
    public class MoveAbility
    {
        private static readonly int MoveXHash = Animator.StringToHash("MoveX");
        private static readonly int MoveYHash = Animator.StringToHash("MoveY");

        readonly MeepleData _meeple;

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
                var speed = _meeple.Type.BaseSpeed;
                if (_meeple.Asleep) speed = 0;
                else if (_meeple.Sleep <= 2) speed *= 0.5f;
                if (_meeple.Food <= 0) speed *= 0.5f;
                return speed;
            }
        }

        public MoveAbility(MeepleData meeple)
        {
            _meeple = meeple;
        }

        public void Move(EntityFrame frame)
        {
            // If there's nowhere to move to, we're done here
            if (MovementTarget == null || _path?.Count == 0) return;

            // Get a new path, if there isn't one (setting _path to null forces re-pathing)
            _path ??= new Queue<Vector2>(MapEvents.GetPath(frame.Transform.position, (Vector2)MovementTarget));

            // Check if a potentially newly generated path is empty
            if (_path?.Count == 0) return;

            // Get the next point on the path
            Vector2 currentWaypoint = _path.Peek();
            //Debug.Log("Next waypoint " + currentWaypoint);

            // Move towards the point
            frame.Transform.position = Vector2.MoveTowards(frame.Transform.position, currentWaypoint, Speed * Time.deltaTime);
            if(frame.Animator != null) {
                frame.Animator.SetFloat(MoveXHash, currentWaypoint.x * Speed);
                frame.Animator.SetFloat(MoveYHash, currentWaypoint.y * Speed);
            }

            // If we're at the path point, remove that point from the path
            if ((Vector2)frame.Transform.position == currentWaypoint) _path.Dequeue();
        }
    }
}