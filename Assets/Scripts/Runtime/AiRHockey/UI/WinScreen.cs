using UnityEngine;
using HTW.AiRHockey.Game;
using TMPro;

namespace HTW.AiRHockey.UI
{
    public class WinScreen : MonoBehaviour
    {
        private void Awake()
        {
            GameManagerEvents.OnGameWon += GameWon;
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            GameManagerEvents.OnGameWon -= GameWon;
        }

        public void OnRematchClick()
        {
            InstanceFinder.GameManager.ReadyUp();
            gameObject.SetActive(false);
        }

        public void OnLeaveClick()
        {
            InstanceFinder.GameManager.DisconnectFromServer();
            gameObject.SetActive(false);
        }

        private void GameWon(bool winningPlayer)
        {
            TextMeshProUGUI winningText = GetComponentInChildren<TextMeshProUGUI>();
            if (!winningPlayer == InstanceFinder.GameManager.IsHost)
            {
                winningText.text = "You won! Congratulations!";
            }
            else 
            {
                winningText.text = "You lost. Want to try again?";
            }
            gameObject.SetActive(true);
        }
    }
}
