using System;
using System.Collections.Generic;

using Runtime.Interfaces;
using Runtime.Player;

using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime.Services
{
    public class OutOfSceneService : IUpdatable, IDisposable
    {
        private readonly Camera _camera;

        private Vector2 _sceneRange;

        private List<IMovable> _movableItems = new();
        private IMovable _player;
        
        public OutOfSceneService(Camera camera)
        {
            _camera = camera;
        }

        public OutOfSceneService Init()
        {
            var height = Display.main.renderingHeight;
            var width = Display.main.renderingWidth;

            _sceneRange = _camera.ScreenToWorldPoint(new Vector3(width, height, 0));

            return this;
        }

        public void Dispose()
        {
            _movableItems.Clear();
            _player = null;
        }

        public Vector2 GetRandomOutOfScenePosition()
        {
            return GetRandomPointOnEdge(_sceneRange);
        }

        public Vector2 GetRandomPositionInCenter()
        {
            return GetRandomPointOnEdge(_sceneRange / 2);
        }

        public void AddItem(IMovable movable)
        {
            _movableItems.Add(movable);
        }

        public void AddPlayer(IMovable movable)
        {
            _player = movable;
        }

        public void Update()
        {
            CheckItemsPosition();
            CheckPlayerPosition();
        }

        private Vector2 GetRandomPointOnEdge(Vector2 anglePosition)
        {
            var isHorizontal = Random.Range(0, 100) < 50;
            var isNegativeX = Random.Range(0, 100) < 50 ? -1 : 1;
            var isNegativeY = Random.Range(0, 100) < 50 ? -1 : 1;
            var x = isHorizontal ? Random.Range(0, anglePosition.x) * isNegativeX : anglePosition.x * isNegativeX;
            var y = !isHorizontal ? Random.Range(0, anglePosition.y) * isNegativeY : anglePosition.y * isNegativeY;

            return new Vector2(x, y);
        }

        private void CheckItemsPosition()
        {
            var items = new List<IMovable>(_movableItems);
            
            foreach (var movableItem in items)
            {
                var position = movableItem.GetCurrentPosition();

                if (!(position.x > _sceneRange.x) && !(position.x < -_sceneRange.x) &&
                    !(position.y > _sceneRange.y) && !(position.y < -_sceneRange.y)) continue;
                
                _movableItems.Remove(movableItem);
                movableItem.OnOutOfScene();
            }
        }

        private void CheckPlayerPosition()
        {
            var position = _player.GetCurrentPosition();
            var isHorizontalInvert = position.x > _sceneRange.x || position.x < -_sceneRange.x;
            var isVerticalInvert = position.y > _sceneRange.y || position.y < -_sceneRange.y;

            if (isHorizontalInvert || isVerticalInvert)
            {
                var player = (PlayerView)_player;
                var x = isHorizontalInvert ? -position.x : position.x;
                var y = isVerticalInvert ? -position.y : position.y;

                player.transform.position = new Vector3(x, y, 0);
            }
        }
    }
}