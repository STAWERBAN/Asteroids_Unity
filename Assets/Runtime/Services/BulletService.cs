using System;
using System.Collections.Generic;

using Runtime.Interfaces;
using Runtime.Player;
using Runtime.Pool;

using Unity.VisualScripting;
using UnityEngine;

namespace Runtime.Services
{
    public class BulletSpawnService : IUpdatable, IDisposable
    {
        private readonly BulletView _bullet;
        private readonly PlayerView _playerView;
        private readonly OutOfSceneService _outOfSceneService;

        private PoolService<BulletView> _bulletPool;

        private Dictionary<BulletView, Vector2> _spawnedBullets = new();

        private const float Speed = 10;

        public BulletSpawnService(BulletView bullet, PlayerView playerView, OutOfSceneService outOfSceneService)
        {
            _bullet = bullet;
            _playerView = playerView;
            _outOfSceneService = outOfSceneService;
        }

        public BulletSpawnService Init()
        {
            _bulletPool = new PoolService<BulletView>(_bullet);

            return this;
        }

        public void Dispose()
        {
            foreach(var bullet in _spawnedBullets.Keys)
            {
                bullet.OnReturn -= RemoveBullet;
                bullet.OnHit -= OnHit;
            }

            _spawnedBullets.Clear();
            _bulletPool.ClearPool();
        }

        public void Update()
        {
            foreach (var bullet in _spawnedBullets)
            {
                var bulletTransform = bullet.Key.transform;

                var towardPosition = bulletTransform.position + (Vector3)bullet.Value;

                bulletTransform.position = Vector2.MoveTowards(bulletTransform.position, towardPosition,
                    Time.deltaTime * Speed);
            }
        }

        public void Spawn()
        {
            var bullet = _bulletPool.GetItem();
            var bulletTransform = bullet.transform;
            var direction = _playerView.transform.up;

            bullet.GameObject().SetActive(true);
            bulletTransform.position = _playerView.BulletSpawnPosition.position;
            bulletTransform.rotation = _playerView.transform.rotation;

            _spawnedBullets.Add(bullet, direction);

            _outOfSceneService.AddItem(bullet);

            bullet.OnReturn += RemoveBullet;
            bullet.OnHit += OnHit;
        }

        private void RemoveBullet(BulletView bullet)
        {
            bullet.OnReturn -= RemoveBullet;

            _spawnedBullets.Remove(bullet);
        }

        private void OnHit(IHitable bulletInterface, IHitable target)
        {
            var bullet = (BulletView)bulletInterface;

            bullet.OnHit -= OnHit;

            bullet.ReturnToPool();
            bullet.gameObject.SetActive(false);

            target.Hit();
        }
    }
}