using UnityEngine;

namespace Runtime.Data
{
    [CreateAssetMenu(fileName = "InputData", menuName = "ScriptableObjects/InputData", order = 1)]
    public class InputData : ScriptableObject
    {
        [field: SerializeField] public KeyCode FireButton{ get; private set; }
        [field: SerializeField] public KeyCode LaserKey { get; private set; }
    }
}