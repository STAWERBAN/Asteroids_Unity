using UnityEngine;

namespace Runtime.Data
{
    [CreateAssetMenu(fileName = "GameplayData", menuName = "ScriptableObjects/GameplayData", order = 1)]
    public class GameplayData : ScriptableObject
    {
        [field: SerializeField] public float UfoSpeed{ get; private set; }
        [field: SerializeField] public int PointsForUfoDestroy { get; private set; }
        [field: SerializeField] public Vector2 UfoSpawnTimeRange { get; private set; }
        
        [field: SerializeField] public float AsteroidsSpeed{ get; private set; }
        [field: SerializeField] public int PointsForAsteroidDestroy { get; private set; }
        [field: SerializeField] public Vector2 AsteroidSpawnTimeRange { get; private set; }
    }
}