using System;

using Runtime.Interfaces;
using Runtime.Services.Pool;

using UnityEngine;

namespace Runtime.Enemy
{
    public class UFOView : MonoBehaviour, IMovable, IHitable, IPoolItem<UFOView>
    {
        public event Action<UFOView> OnReturn = delegate {  };
        public event Action<IHitable, IHitable> OnHit = delegate {  };

        public Vector2 GetCurrentPosition()
        {
            return transform.position;
        }

        public void OnOutOfScene()
        {
            ReturnToPool();
        }

        public void ReturnToPool()
        {
            gameObject.SetActive(false);
            OnReturn.Invoke(this);
        }

        public void Hit()
        {
            OnHit.Invoke(this, null);
        }
    }
}