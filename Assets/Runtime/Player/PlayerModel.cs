using System;

using UnityEngine;

namespace Runtime.Player
{
    public class PlayerModel
    {
        public event Action<Vector2> PlayerChangePosition = delegate { };
        public event Action<float> PlayerChangeSpeed = delegate { };
        public event Action<float> PlayerChangeEuler = delegate { };
        public event Action<int> PlayerLaserCountChanged = delegate { };
        public event Action<float> PlayerLaserCooldownChanged = delegate { };
        public event Action PlayerDead = delegate { };

        public bool IsPlayerAlive { get; private set; }

        public void Init()
        {
            IsPlayerAlive = true;
        }
        
        public void ChangePlayerPosition(Vector2 currentPosition)
        {
            PlayerChangePosition.Invoke(currentPosition);
        }

        public void ChangePlayerSpeed(float speed)
        {
            PlayerChangeSpeed.Invoke(speed);
        }

        public void ChangePlayerRotation(float angle)
        {
            PlayerChangeEuler.Invoke(angle);
        }

        public void ChangePlayerLaserCount(int count)
        {
            PlayerLaserCountChanged.Invoke(count);
        }

        public void ChangePlayerLaserCooldown(float cooldown)
        {
            PlayerLaserCooldownChanged.Invoke(cooldown);
        }

        public void OnPlayerDead()
        {
            IsPlayerAlive = false;
            PlayerDead.Invoke();
        }
    }
}