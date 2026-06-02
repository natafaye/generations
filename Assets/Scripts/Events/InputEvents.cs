using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Generations
{
    public static class InputEvents
    {
        public static Action<InputValue> Navigate;

        public static Action<InputValue> ScrollWheel;

        public static Action<Vector2> PointerMove;

        public static Action<RaycastHit2D[]> LeftClick;

        public static Action<Key> KeyDown;
    }
}