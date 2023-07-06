using System.Collections;
using System.Collections.Generic;
using HTW.AiRHockey.Game;
using UnityEngine;
using TMPro;

namespace HTW.AiRHockey.UI
{ 
    public class ArenaUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text[] _timers;
        [SerializeField] private TMP_Text[] _player1Score;
        [SerializeField] private TMP_Text[] _player2Score;

        private void Start()
        {
            GameManagerEvents.OnGoalScored += OnGoalScored;
        }

        private void Update()
        {
            if (InstanceFinder.GameManager.IsGameRunning)
                UpdateTimer();
        }

        public void OnGoalScored(bool player)
        {
            foreach (TMP_Text textMesh in _player1Score)
                textMesh.text = string.Format("{0:00}", InstanceFinder.GameManager.Player1Score);

            foreach (TMP_Text textMesh in _player2Score)
                textMesh.text = string.Format("{0:00}", InstanceFinder.GameManager.Player2Score);
        }

        public void UpdateTimer()
        {
            foreach (TMP_Text textMesh in _timers)
                textMesh.text = InstanceFinder.GameManager.GameTimeText;
        }
    }
}
