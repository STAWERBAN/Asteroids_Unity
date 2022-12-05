using System;

using Runtime.Interfaces;
using Runtime.Services.Pool;

using UnityEngine;

namespace Runtime.Enemy
{
    public class AsteroidView : MonoBehaviour, IMovable, IHitable, IPoolItem<AsteroidView>
    {
        [field: SerializeField] public int AsteroidLevel { get; private set; } // 0 - small, 1 - medium, 2 - large
        
        public event Action<AsteroidView> OnReturn = delegate {  };
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
        {;
            OnHit.Invoke(this, null);
        }
    }
}