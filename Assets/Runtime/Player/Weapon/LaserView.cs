using System;

using Runtime.Interfaces;

using UnityEngine;

namespace Runtime.Player.Weapon
{
    public class LaserView : MonoBehaviour, IHitable
    {
        public event Action<IHitable, IHitable> OnHit = delegate { };

        public void Hit() { }

        private void OnCollisionEnter2D(Collision2D col)
        {
            var isNotPlayer = col.gameObject.GetComponent<PlayerView>() is null;

            if (isNotPlayer)
            {
                var hitItem = col.gameObject.GetComponent<IHitable>();

                if (hitItem == null)
                    return;

                OnHit.Invoke(this, hitItem);
            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            var hitItem = col.gameObject.GetComponent<IHitable>();

            if (hitItem == null)
                return;

            OnHit.Invoke(this, hitItem);
        }
    }
}