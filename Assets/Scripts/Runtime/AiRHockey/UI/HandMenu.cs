using HTW.AiRHockey.Game;
using UnityEngine;
using TMPro;

namespace HTW.AiRHockey.UI
{
    public class HandMenu : MonoBehaviour
    {
        [SerializeField] private TMP_Text _player1Score;
        [SerializeField] private TMP_Text _player2Score;

        private void OnEnable()
        {
            _player1Score.text = string.Format("{0:00}", InstanceFinder.GameManager.Player1Score);
            _player2Score.text = string.Format("{0:00}", InstanceFinder.GameManager.Player2Score);
        }
    }
}
