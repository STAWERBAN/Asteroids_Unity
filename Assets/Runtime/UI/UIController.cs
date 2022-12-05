using System;
using Runtime.Player;
using UnityEngine;

namespace Runtime.UI
{
    public class UIController
    {
        private readonly UIView _view;
        private readonly GameModel _model;
        private readonly PlayerModel _playerModel;

        public UIController(UIView view, GameModel model, PlayerModel playerModel)
        {
            _view = view;
            _model = model;
            _playerModel = playerModel;
        }

        public void Init()
        {
            _model.ScoreChanged += ChangeScoreText;

            _playerModel.PlayerChangePosition += OnPlayerChangePosition;
            _playerModel.PlayerChangeEuler += OnPlayerChangeEuler;
            _playerModel.PlayerChangeSpeed += OnPlayerChangeSpeed;
            _playerModel.PlayerLaserCooldownChanged += OnPlayerLaserCooldownChanged;
            _playerModel.PlayerLaserCountChanged += OnPlayerLaserCountChanged;
            _playerModel.PlayerDead += OnGameOver;
        }

        public void ClearSubscribers()
        {
            Clear();

            _model.ScoreChanged -= ChangeScoreText;

            _playerModel.PlayerChangePosition -= OnPlayerChangePosition;
            _playerModel.PlayerChangeEuler -= OnPlayerChangeEuler;
            _playerModel.PlayerChangeSpeed -= OnPlayerChangeSpeed;
            _playerModel.PlayerLaserCooldownChanged -= OnPlayerLaserCooldownChanged;
            _playerModel.PlayerLaserCountChanged -= OnPlayerLaserCountChanged;
            _playerModel.PlayerDead += OnGameOver;
        }

        public void OnGameStart()
        {
            _view.StartGamePanel.SetActive(false);
            _view.GameplayPanel.SetActive(true);
            _view.EndGamePanel.SetActive(false);
        }

        public void AddActionOnButtonClick(Action onClick)
        {
            _view.StartGameButton.onClick.AddListener(onClick.Invoke);
            _view.StartGameButton.onClick.AddListener(OnGameStart);

            _view.RestartGameButton.onClick.AddListener(onClick.Invoke);
            _view.RestartGameButton.onClick.AddListener(OnGameStart);
        }

        private void OnPlayerLaserCountChanged(int count)
        {
            _view.PlayerLaserCountText.text = "Lasers: ";

            for (var i = 0; i < count; i++)
            {
                _view.PlayerLaserCountText.text += "▩";
            }
        }

        private void OnPlayerLaserCooldownChanged(float value)
        {
            var cooldown = Math.Round(value, 1);
            _view.PlayerLaserCooldownText.text = cooldown + "s";
        }

        private void OnPlayerChangeSpeed(float value)
        {
            var speed = Math.Round(value, 1);
            _view.PlayerSpeedText.text = "Speed = " + speed;
        }

        private void OnPlayerChangeEuler(float value)
        {
            var angle = Math.Round(value, 1);
            _view.PlayerRotationText.text = "Rotation = " + angle;
        }

        private void OnPlayerChangePosition(Vector2 value)
        {
            var x = Math.Round(value.x, 1);
            var y = Math.Round(value.y, 1);
            _view.PlayerPositionText.text = "x = " + x + " y = " + y;
        }

        private void OnGameOver()
        {
            _view.EndGamePanel.SetActive(true);
            _view.EndGameScoreText.text = _view.ScoreText.text;
        }

        private void Clear()
        {
            _view.ScoreText.text = "0";
        }

        private void ChangeScoreText(int currentScore)
        {
            _view.ScoreText.text = currentScore.ToString();
        }
    }
}