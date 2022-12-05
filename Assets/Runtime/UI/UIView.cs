using TMPro;

using UnityEngine;

namespace Runtime.UI
{
    public class UIView : MonoBehaviour
    {
        [field: SerializeField] public GameObject StartGamePanel { get; private set; }
        [field: SerializeField] public UnityEngine.UI.Button StartGameButton { get; private set; }

        [field: SerializeField] public GameObject GameplayPanel { private set; get; }
        [field: SerializeField] public TMP_Text EndGameScoreText { get; private set; }
        [field: SerializeField] public GameObject EndGamePanel { get; private set; }
        [field: SerializeField] public UnityEngine.UI.Button RestartGameButton { get; private set; }
        
        [field: SerializeField] public TMP_Text ScoreText { get; private set; }
        [field: SerializeField] public TMP_Text PlayerPositionText { get; private set; }
        [field: SerializeField] public TMP_Text PlayerSpeedText { get; private set; }
        [field: SerializeField] public TMP_Text PlayerRotationText { get; private set; }
        [field: SerializeField] public TMP_Text PlayerLaserCountText { get; private set; }
        [field: SerializeField] public TMP_Text PlayerLaserCooldownText { get; private set; }
    }
}