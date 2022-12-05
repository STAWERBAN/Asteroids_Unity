using UnityEngine;

namespace Runtime
{
    public class MoveParameters
    {
        public Vector2 Direction { get; private set; }

        public MoveParameters() { }

        public MoveParameters(Vector2 direction)
        {
            Direction = direction;
        }

        public void ChangeDirection(Vector2 direction)
        {
            Direction = direction;
        }
    }
}