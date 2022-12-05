using Runtime.Data;
using Runtime.Interfaces;
using Runtime.Services;
using Runtime.Services.Input;

using UnityEngine;

namespace Runtime.Player
{
    public class PlayerController : IUpdatable
    {
        private readonly PlayerView _playerView;
        private readonly InputService _inputService;
        private readonly BulletSpawnService _bulletSpawnService;
        private readonly PlayerModel _playerModel;
        private readonly PlayerData _playerData;
        private readonly LaserService _laserService;

        private Vector2 _moveDirection;
        private float _bulletSpawnTimer;
        private float _laserCooldownTimer;

        private float _fireCooldown;
        private float _sensitivity;
        private float _speed;
        private int _laserCount;


        public PlayerController(PlayerView playerView, InputService inputService,
            BulletSpawnService bulletSpawnService, PlayerModel playerModel, PlayerData playerData,
            LaserService laserService)
        {
            _playerView = playerView;
            _inputService = inputService;
            _bulletSpawnService = bulletSpawnService;
            _playerModel = playerModel;
            _playerData = playerData;
            _laserService = laserService;
        }

        public PlayerController Init()
        {
            _sensitivity = _playerData.Sensitivity;
            _speed = _playerData.Speed;
            _laserCount = _playerData.LaserCount;
            _fireCooldown = _playerData.FireCooldown;

            _playerModel.Init();
            _playerModel.ChangePlayerLaserCount(_laserCount);

            _inputService.FireButtonPress += OnFireButtonPress;
            _inputService.LaserButtonPress += OnLaserButtonPress;

            _playerView.OnHit += OnPlayerHit;

            return this;
        }

        public void Dispose()
        {
            _inputService.FireButtonPress -= OnFireButtonPress;
            _inputService.LaserButtonPress -= OnLaserButtonPress;

            _playerView.OnHit -= OnPlayerHit;
            
            Object.Destroy(_playerView.gameObject);
        }

        public void Update()
        {
            if(!_playerModel.IsPlayerAlive)
                return;
            
            var inputDirection = _inputService.GetInputAxis();

            CheckBulletTimer();
            CheckLaserCount();
            Move(inputDirection.y);
            Rotate(inputDirection.x);
        }

        private void CheckLaserCount()
        {
            if (_laserCount >= _playerData.LaserCount || _laserService.IsEmitted)
                return;

            _laserCooldownTimer -= Time.deltaTime;

            _playerModel.ChangePlayerLaserCooldown(_laserCooldownTimer);

            if (!(_laserCooldownTimer <= 0))
                return;

            _laserCount++;
            _laserCooldownTimer = 0;

            _playerModel.ChangePlayerLaserCooldown(_laserCooldownTimer);
            _playerModel.ChangePlayerLaserCount(_laserCount);

            _laserCooldownTimer = _playerData.LaserCooldown;
        }

        private void CheckBulletTimer()
        {
            if (_bulletSpawnTimer <= 0 || _laserService.IsEmitted)
                return;

            _bulletSpawnTimer -= Time.deltaTime;
        }

        private void OnLaserButtonPress()
        {
            if (_laserCount <= 0)
            {
                return;
            }

            if (_laserService.LaserFire())
            {
                _laserCount--;
                _playerModel.ChangePlayerLaserCount(_laserCount);
                _laserCooldownTimer = _playerData.LaserCooldown;
            }
        }

        private void OnFireButtonPress()
        {
            if (_bulletSpawnTimer > 0)
            {
                return;
            }

            _bulletSpawnService.Spawn();
            _bulletSpawnTimer = _fireCooldown;
        }

        private void Move(float moveValue)
        {
            var playerTransform = _playerView.transform;
            var playerPosition = playerTransform.position;

            if (moveValue != 0)
            {
                var towardDirection = playerTransform.up * moveValue;

                _moveDirection += (Vector2)towardDirection * Time.deltaTime;
            }

            playerTransform.position = Vector3.Lerp(playerPosition,
                playerPosition + (Vector3)_moveDirection,
                _speed * Time.deltaTime);

            _playerModel.ChangePlayerPosition(playerTransform.position);
            _playerModel.ChangePlayerSpeed(_moveDirection.magnitude);
        }

        private void Rotate(float rotateAngle)
        {
            _playerView.transform.eulerAngles -= Vector3.forward * rotateAngle * _sensitivity * Time.deltaTime;
            _playerModel.ChangePlayerRotation(_playerView.transform.eulerAngles.z);
        }

        private void OnPlayerHit(IHitable player, IHitable target)
        {
            _playerView.gameObject.SetActive(false);
            _playerModel.OnPlayerDead();

            target.Hit();
        }
    }
}