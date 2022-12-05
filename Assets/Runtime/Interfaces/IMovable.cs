using UnityEngine;

namespace Runtime.Interfaces
{
    public interface IMovable
    {
        Vector2 GetCurrentPosition();
        void OnOutOfScene();
    }
}