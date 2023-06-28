using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HTW.AiRHockey.Game
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshPro[] timer;
        [SerializeField]
        private TextMeshPro[] Player1Score;
        [SerializeField]
        private TextMeshPro[] Player2Score;
        private GameManager manager;
        // Start is called before the first frame update
        void Start()
        {
            if(!manager)
            {
                manager = InstanceFinder.GameManager;
                manager.PlayerUI = this;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(manager.IsGameRunning)
            {
                UpdateTimer();
            }
        }

        public void OnGoalScored(bool player)
        {
            if(player)
            {
                UpdatePlayer1Score();
            } else
            {
                UpdatePlayer2Score();
            }
        }

        public void UpdatePlayer1Score()
        {
            foreach(TextMeshPro textMesh in Player1Score)
            {
                textMesh.text = string.Format("{0:00}", manager.Player1Score);
            }
        }

        public void UpdatePlayer2Score()
        {
            foreach (TextMeshPro textMesh in Player2Score)
            {
                textMesh.text = string.Format("{0:00}", manager.Player1Score);
            }
        }

        public void UpdateTimer()
        {
            foreach(TextMeshPro textMesh in timer)
            {
                textMesh.text = manager.GameTimeText;
            }
        }
    }
}
