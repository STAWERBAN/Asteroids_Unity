using System.Collections.Generic;
using Runtime.Data;
using Runtime.Enemy;
using Runtime.Interfaces;
using Runtime.Player;
using Runtime.Services;
using Runtime.Services.Input;
using Runtime.UI;
using UnityEngine;

namespace Runtime
{
    public class GameInitializer : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private UIView _uiView;
        [Header("Player")] [SerializeField] private PlayerView _playerView;
        [SerializeField] private BulletView _bulletPrefab;

        [Header("EnemyPrefabs")] [SerializeField]
        private UFOView _ufoPrefab;

        [Header("Data")] [SerializeField] private InputData _inputData;
        [SerializeField] private PlayerData _playerData;
        [SerializeField] private GameplayData _gameplayData;
        [SerializeField] private AsteroidsData _asteroidsData;

        private InputService _inputService;
        private BulletSpawnService _bulletSpawnService;
        private OutOfSceneService _outOfSceneService;
        private LaserService _laserService;

        private AsteroidsController _asteroidsController;
        private PlayerController _playerController;
        private UFOController _ufoController;
        private UIController _uiController;

        private GameModel _gameModel;
        private PlayerModel _playerModel;

        private List<IUpdatable> _updatables = new();

        private void Start()
        {
            _gameModel = new GameModel();
            _playerModel = new PlayerModel();

            _uiController = new UIController(_uiView, _gameModel, _playerModel);
            _uiController.AddActionOnButtonClick(StartNewGame);
        }

        private void Update()
        {
            _updatables?.ForEach(a => a.Update());
        }

        private void StartNewGame()
        {
            Clear();

            var player = Instantiate(_playerView);

            InitializeServices(player);
            InitializeControllers(player);
        }

        private void InitializeControllers(PlayerView player)
        {
            _uiController.Init();
            
            _playerController =
                new PlayerController(player, _inputService, _bulletSpawnService, _playerModel, _playerData,
                    _laserService).Init();

            _ufoController =
                new UFOController(_ufoPrefab, player, _outOfSceneService, _gameModel, _gameplayData).Init();

            _asteroidsController =
                new AsteroidsController(_outOfSceneService, _asteroidsData, _gameModel, _gameplayData).Init();

            _updatables.Add(_playerController);
            _updatables.Add(_ufoController);
            _updatables.Add(_asteroidsController);
        }

        private void InitializeServices(PlayerView playerView)
        {
            _inputService = new InputService(_inputData);
            _outOfSceneService = new OutOfSceneService(_camera).Init();
            _bulletSpawnService = new BulletSpawnService(_bulletPrefab, playerView, _outOfSceneService).Init();
            _laserService = new LaserService(playerView.Laser, _playerData.LaserDuration).Init();

            _outOfSceneService.AddPlayer(playerView);

            _updatables.Add(_inputService);
            _updatables.Add(_bulletSpawnService);
            _updatables.Add(_outOfSceneService);
            _updatables.Add(_laserService);
        }

        private void Clear()
        {
            _bulletSpawnService?.Dispose();
            _outOfSceneService?.Dispose();
            _asteroidsController?.Dispose();
            _playerController?.Dispose();
            _ufoController?.Dispose();
            _uiController?.ClearSubscribers();

            _asteroidsController = null;
            _bulletSpawnService = null;
            _outOfSceneService = null;
            _playerController = null;
            _ufoController = null;
            _inputService = null;
            _laserService = null;

            _updatables.Clear();
            _gameModel.Clear();
        }
    }
}