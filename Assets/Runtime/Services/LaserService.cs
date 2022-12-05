using Runtime.Interfaces;
using Runtime.Player.Weapon;

using UnityEngine;

namespace Runtime.Services
{
    public class LaserService: IUpdatable
    {
        private readonly LaserView _view;
        private readonly float _laserDuration;

        public bool IsEmitted => _isEmitted;

        private bool _isEmitted;
        private float _laserTimer;

        public LaserService(LaserView view, float laserDuration)
        {
            _view = view;
            _laserDuration = laserDuration;
        }

        public LaserService Init()
        {
            _view.OnHit += OnLaserHit;

            return this;
        }

        public void Update()
        {
            if(!_isEmitted)
                return;

            _laserTimer -= Time.deltaTime;

            if (_laserTimer <= 0)
            {
                _isEmitted = false;
                _view.gameObject.SetActive(false);
            }
        }

        public bool LaserFire()
        {
            if(_isEmitted)
                return false;

            _isEmitted = true;
            _laserTimer = _laserDuration;
            _view.gameObject.SetActive(true);

            return true;
        }

        private void OnLaserHit(IHitable laser, IHitable target)
        {
            target.Hit();
        }
    }
}