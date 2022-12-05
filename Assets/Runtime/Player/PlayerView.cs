using System;

using Runtime.Interfaces;
using Runtime.Player.Weapon;

using UnityEngine;

namespace Runtime.Player
{
    public class PlayerView : MonoBehaviour, IMovable, IHitable
    {
        [SerializeField] private Transform _bulletSpawnPosition;
        [SerializeField] private LaserView _laserView;

        public event Action<IHitable, IHitable> OnHit = delegate {  };

        public Transform BulletSpawnPosition => _bulletSpawnPosition;
        public LaserView Laser => _laserView;


        public Vector2 GetCurrentPosition()
        {
            return transform.position;
        }

        public void OnOutOfScene() { }

        public void Hit() { }

        private void OnTriggerEnter2D(Collider2D col)
        {
            var hitItem = col.GetComponent<IHitable>();

            if(hitItem == null)
                return;
            
            OnHit.Invoke(this, hitItem);
        }
    }
}