using System.Collections.Generic;

using Runtime.Data;
using Runtime.Interfaces;
using Runtime.Player;
using Runtime.Pool;
using Runtime.Services;

using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime.Enemy
{
    public class UFOController : IUpdatable
    {
        private readonly OutOfSceneService _outOfSceneObjectService;
        private readonly PlayerView _playerView;
        private readonly GameModel _gameModel;
        private readonly UFOView _ufoView;

        private readonly Vector2 _spawnTimeRange;
        private readonly int _pointsForDestroy;
        private readonly float _speed;

        private PoolService<UFOView> _poolService;

        private Dictionary<UFOView, MoveParameters> _spawnedUfo = new();
        
        private float _timer;

        public UFOController(UFOView ufoView, PlayerView playerView, OutOfSceneService outOfSceneObjectService,
            GameModel gameModel, GameplayData gameplayData)
        {
            _ufoView = ufoView;
            _playerView = playerView;
            _outOfSceneObjectService = outOfSceneObjectService;
            _gameModel = gameModel;

            _speed = gameplayData.UfoSpeed;
            _spawnTimeRange = gameplayData.UfoSpawnTimeRange;
            _pointsForDestroy = gameplayData.PointsForUfoDestroy;
        }

        public UFOController Init()
        {
            _poolService = new PoolService<UFOView>(_ufoView);

            return this;
        }

        public void Dispose()
        {
            foreach (var ufo in _spawnedUfo.Keys)
            {
                ufo.OnReturn -= OnReturnToPool;
                ufo.OnHit -= OnHit;
            }
            
            _poolService.ClearPool();
            _spawnedUfo.Clear();
        }

        public void Update()
        {
            SpawnUfo();
            MoveUfo();
        }

        private void SpawnUfo()
        {
            _timer -= Time.deltaTime;

            if (_timer <= 0)
            {
                var ufo = _poolService.GetItem();
                var position = _outOfSceneObjectService.GetRandomOutOfScenePosition();

                ufo.transform.position = position;
                ufo.gameObject.SetActive(true);

                _spawnedUfo.Add(ufo, new MoveParameters());
                _outOfSceneObjectService.AddItem(ufo);

                ufo.OnReturn += OnReturnToPool;
                ufo.OnHit += OnHit;

                _timer = Random.Range(_spawnTimeRange.x, _spawnTimeRange.y);
            }
        }

        private void MoveUfo()
        {
            foreach (var ufoParam in _spawnedUfo)
            {
                var ufo = ufoParam.Key;
                var parameters = ufoParam.Value;
                var ufoPosition = ufo.GetCurrentPosition();
                var towardDirection = (_playerView.GetCurrentPosition() - ufoPosition).normalized;
                var direction = parameters.Direction + towardDirection * Time.deltaTime;

                ufo.transform.position = Vector3.Lerp(ufoPosition,
                    ufoPosition + direction,
                    _speed * Time.deltaTime);

                parameters.ChangeDirection(direction);
            }
        }

        private void OnHit(IHitable ufoInterface, IHitable empty)
        {
            var ufo = (UFOView)ufoInterface;

            ufo.OnHit -= OnHit;
            ufo.ReturnToPool();

            _gameModel.IncreaseScore(_pointsForDestroy);
        }

        private void OnReturnToPool(UFOView ufo)
        {
            ufo.OnReturn -= OnReturnToPool;

            _spawnedUfo.Remove(ufo);
        }
    }
}