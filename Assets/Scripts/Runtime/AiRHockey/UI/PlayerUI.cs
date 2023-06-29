using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HTW.AiRHockey.Game
{
    public class PlayerUI : MonoBehaviour
    {

        [SerializeField] private TextMeshPro[] timer;

        [SerializeField] private TextMeshPro[] Player1Score;
        [SerializeField] private TextMeshPro[] Player2Score;

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
            if(player)
            {
                UpdatePlayer2Score();
            } else
            {
                UpdatePlayer1Score();
            }
        }

        public void UpdatePlayer1Score()
        {
            foreach(TextMeshPro textMesh in Player1Score)
            {
                textMesh.text = string.Format("{0:00}", InstanceFinder.GameManager.Player1Score);
            }
        }

        public void UpdatePlayer2Score()
        {
            foreach (TextMeshPro textMesh in Player2Score)
            {
                textMesh.text = string.Format("{0:00}", InstanceFinder.GameManager.Player1Score);
            }
        }

        public void UpdateTimer()
        {
            foreach(TextMeshPro textMesh in timer)
            {
                textMesh.text = InstanceFinder.GameManager.GameTimeText;
            }
        }
    }
}
