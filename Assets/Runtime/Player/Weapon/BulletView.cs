using System;

using Runtime.Interfaces;
using Runtime.Services.Pool;

using UnityEngine;

namespace Runtime.Player
{
    public class BulletView : MonoBehaviour, IMovable, IHitable, IPoolItem<BulletView>
    {
        public event Action<BulletView> OnReturn = delegate {  };
        public event Action<IHitable, IHitable> OnHit = delegate {  };

        public void ReturnToPool()
        {
            gameObject.SetActive(false);
            OnReturn.Invoke(this);
        }

        public Vector2 GetCurrentPosition()
        {
            return transform.position;
        }

        public void OnOutOfScene()
        {
            ReturnToPool();
        }

        public void Hit()
        {
            ReturnToPool();
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            var hitItem = col.GetComponent<IHitable>();

            if(hitItem == null)
                return;
            
            OnHit.Invoke(this, hitItem);
        }
    }
}