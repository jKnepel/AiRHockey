using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HTW.AiRHockey.Game
{
    public class PlayerUI : MonoBehaviour
    {

        [SerializeField] private TMP_Text[] Timers;
        [SerializeField] private TMP_Text[] Player1Score;
        [SerializeField] private TMP_Text[] Player2Score;
        [SerializeField] private Transform HandMenu;

        private void Start()
		{
            GameManagerEvents.OnGoalScored += OnGoalScored;
		}

        private void Update()
        {
            if(InstanceFinder.GameManager.IsGameRunning)
            {
                UpdateTimer();
            }
        }

        public void OnGoalScored(bool player)
        {
            foreach (TMP_Text textMesh in Player1Score)
                textMesh.text = string.Format("{0:00}", InstanceFinder.GameManager.Player1Score);

            foreach (TMP_Text textMesh in Player2Score)
                textMesh.text = string.Format("{0:00}", InstanceFinder.GameManager.Player2Score);
        }

        public void UpdateTimer()
        {
            foreach(TMP_Text textMesh in Timers)
            {
                textMesh.text = InstanceFinder.GameManager.GameTimeText;
            }
        }

        public void DisplayOptions()
        {
            HandMenu.GetChild(0).gameObject.SetActive(false);
            HandMenu.GetChild(1).gameObject.SetActive(true);
        }

        public void HideOptions()
        {
            HandMenu.GetChild(0).gameObject.SetActive(true);
            HandMenu.GetChild(1).gameObject.SetActive(false);
        }

        public void BackToMenu()
        {
            InstanceFinder.GameManager.DisconnectFromServer();
        }

        public void ResetPlayer()
        {
            InstanceFinder.GameManager.ResetPlayers();
        }

        public void Rematch()
        {
            InstanceFinder.GameManager.EndGame();
        }
    }
}
