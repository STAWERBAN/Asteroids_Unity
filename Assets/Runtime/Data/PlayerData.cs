using UnityEngine;

namespace Runtime.Data
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 1)]
    public class PlayerData : ScriptableObject
    {
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public float Sensitivity { get; private set; }
        [field: SerializeField] public float FireCooldown { get; private set; }
        [field: SerializeField] public float LaserDuration { get; private set; }
        [field: SerializeField] public int LaserCount { get; private set; }
        [field: SerializeField] public float LaserCooldown { get; private set; }
    }
}