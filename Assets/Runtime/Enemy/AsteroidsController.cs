using System;
using System.Collections.Generic;

using Runtime.Data;
using Runtime.Pool;
using Runtime.Services;
using Runtime.Interfaces;

using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime.Enemy
{
    public class AsteroidsController : IUpdatable, IDisposable
    {
        private readonly OutOfSceneService _outOfSceneObjectService;
        private readonly AsteroidsData _asteroidsData;
        private readonly GameModel _gameModel;

        private readonly Vector2 _spawnTimeRange;
        private readonly int _pointsForDestroy;
        private readonly float _speed;

        private PoolService<AsteroidView> _littleAsteroidsPool;
        private PoolService<AsteroidView> _mediumAsteroidsPool;
        private PoolService<AsteroidView> _largeAsteroidsPool;

        private Dictionary<AsteroidView, MoveParameters> _spawnedAsteroids = new();

        private float _timer;

        private const float DestroyedAsteroidChangeAngle = 30;

        public AsteroidsController(OutOfSceneService outOfSceneObjectService, AsteroidsData asteroidsData,
            GameModel gameModel, GameplayData gameplayData)
        {
            _outOfSceneObjectService = outOfSceneObjectService;
            _asteroidsData = asteroidsData;
            _gameModel = gameModel;

            _speed = gameplayData.AsteroidsSpeed;
            _spawnTimeRange = gameplayData.AsteroidSpawnTimeRange;
            _pointsForDestroy = gameplayData.PointsForAsteroidDestroy;
        }

        public void Dispose()
        {
            foreach (var asteroid in _spawnedAsteroids.Keys)
            {
                asteroid.OnReturn -= OnReturnToPool;
                asteroid.OnHit -= OnHit;
            }

            _spawnedAsteroids.Clear();

            _littleAsteroidsPool.ClearPool();
            _mediumAsteroidsPool.ClearPool();
            _largeAsteroidsPool.ClearPool();
        }

        public AsteroidsController Init()
        {
            _littleAsteroidsPool = new PoolService<AsteroidView>(_asteroidsData.LittleAsteroid);
            _mediumAsteroidsPool = new PoolService<AsteroidView>(_asteroidsData.MediumAsteroid);
            _largeAsteroidsPool = new PoolService<AsteroidView>(_asteroidsData.LargeAsteroid);

            return this;
        }

        public void Update()
        {
            SpawnAsteroids();
            MoveAsteroids();
        }

        private void SpawnAsteroids()
        {
            _timer -= Time.deltaTime;

            if (_timer <= 0)
            {
                var pool = GetRandomPoolService();
                var asteroid = pool.GetItem();
                var position = _outOfSceneObjectService.GetRandomOutOfScenePosition();
                var randomPositionInCenter = _outOfSceneObjectService.GetRandomPositionInCenter();
                var direction = (randomPositionInCenter - position).normalized * _speed;
                var moveParameter = new MoveParameters(direction);

                asteroid.transform.position = position;
                asteroid.gameObject.SetActive(true);

                _spawnedAsteroids.Add(asteroid, moveParameter);
                _outOfSceneObjectService.AddItem(asteroid);

                asteroid.OnReturn += OnReturnToPool;
                asteroid.OnHit += OnHit;

                _timer = Random.Range(_spawnTimeRange.x, _spawnTimeRange.y);
            }
        }

        private void MoveAsteroids()
        {
            foreach (var ufoParam in _spawnedAsteroids)
            {
                var ufo = ufoParam.Key;
                var parameters = ufoParam.Value;
                var ufoPosition = ufo.GetCurrentPosition();

                ufo.transform.position = Vector3.Lerp(ufoPosition,
                    ufoPosition + parameters.Direction,
                    _speed * Time.deltaTime);
            }
        }

        private PoolService<AsteroidView> GetRandomPoolService()
        {
            var random = Random.Range(0, 100);

            return random switch
            {
                < 33 => _littleAsteroidsPool,
                < 66 => _mediumAsteroidsPool,
                _ => _largeAsteroidsPool
            };
        }

        private void OnHit(IHitable asteroidInterface, IHitable empty)
        {
            var asteroid = (AsteroidView)asteroidInterface;
            var asteroidIndex = asteroid.AsteroidLevel;
            var pool = GetPoolByIndex(asteroidIndex);
            var direction = _spawnedAsteroids[asteroid].Direction;
            var position = asteroid.GetCurrentPosition();

            asteroid.ReturnToPool();

            _gameModel.IncreaseScore(_pointsForDestroy);

            if (pool is null)
                return;

            for (var i = -1; i < 2; i += 2)
            {
                var item = pool.GetItem();
                var newDirection = GetNewRotatedVector(direction, DestroyedAsteroidChangeAngle * i);
                var moveParameter = new MoveParameters(newDirection);

                item.transform.position = position;
                item.gameObject.SetActive(true);

                _spawnedAsteroids.Add(item, moveParameter);
                _outOfSceneObjectService.AddItem(item);

                item.OnReturn += OnReturnToPool;
                item.OnHit += OnHit;
            }
        }

        private void OnReturnToPool(AsteroidView asteroid)
        {
            asteroid.OnReturn -= OnReturnToPool;
            asteroid.OnHit -= OnHit;

            _spawnedAsteroids.Remove(asteroid);
        }

        private PoolService<AsteroidView> GetPoolByIndex(int index)
        {
            return index switch
            {
                < 1 => null,
                < 2 => _littleAsteroidsPool,
                < 3 => _mediumAsteroidsPool,
                _ => _largeAsteroidsPool
            };
        }

        private Vector2 GetNewRotatedVector(Vector2 vector, float angle)
        {
            var piAngle = Mathf.PI * angle / 180f;

            var x = vector.x * Mathf.Cos(piAngle) - vector.y * Mathf.Sin(piAngle);
            var y = vector.x * Mathf.Sin(piAngle) + vector.y * Mathf.Cos(piAngle);

            return new Vector2(x, y);
        }
    }
}