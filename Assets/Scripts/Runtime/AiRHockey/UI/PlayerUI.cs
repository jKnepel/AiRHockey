using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HTW.AiRHockey.Game
{
    public class PlayerUI : MonoBehaviour
    {

        [SerializeField] private TMP_Text[] Timers;
        [SerializeField] private TMP_Text[] Player1Score;
        [SerializeField] private TMP_Text[] Player2Score;
        [SerializeField] private Transform HandMenu;

        private float p1Rotation = 0f;
        private float p1TimerRotation = -90f;
        private float pRotation = 180;
        private bool host = false;

        private void Start()
		{
            Timers[0].text = "12:99";
            GameManagerEvents.OnGoalScored += OnGoalScored;
            //Set rotation according to player perspective, but only the ones on the table.
            if(InstanceFinder.GameManager.IsHost)
            {
                foreach(TextMeshPro text in Player2Score)
                {
                    if(text.gameObject.tag.Equals("Table"))
                    {
                        Vector3 initRotation = text.transform.parent.rotation.eulerAngles;
                        text.transform.parent.rotation = Quaternion.Euler(initRotation.x, p1Rotation, initRotation.z);
                    }
                }

                foreach(TextMeshPro timer in Timers)
                {
                    if (timer.gameObject.tag.Equals("Table"))
                    {
                        Vector3 initRotation = timer.transform.rotation.eulerAngles;
                        timer.transform.rotation = Quaternion.Euler(initRotation.x, initRotation.y, p1TimerRotation);
                    }
                }

            } else
            {
                foreach (TextMeshPro text in Player1Score)
                {
                    if (text.gameObject.tag.Equals("Table"))
                    {
                        Vector3 initRotation = text.transform.parent.rotation.eulerAngles;
                        text.transform.parent.rotation = Quaternion.Euler(initRotation.x, p1Rotation + pRotation, initRotation.z);
                    }
                }

                foreach (TextMeshPro timer in Timers)
                {
                    if (timer.gameObject.tag.Equals("Table"))
                    {
                        Vector3 initRotation = timer.transform.rotation.eulerAngles;
                        timer.transform.rotation = Quaternion.Euler(initRotation.x, p1TimerRotation + pRotation, initRotation.z);
                    }
                }
            }
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
            foreach (TextMeshPro textMesh in Player1Score)
                textMesh.text = string.Format("{0:00}:{1:00}", InstanceFinder.GameManager.Player1Score, InstanceFinder.GameManager.Player2Score);

            foreach (TextMeshPro textMesh in Player2Score)
                textMesh.text = string.Format("{0:00}:{1:00}", InstanceFinder.GameManager.Player2Score, InstanceFinder.GameManager.Player1Score);
        }

        public void UpdateTimer()
        {
            foreach(TextMeshPro textMesh in Timers)
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
