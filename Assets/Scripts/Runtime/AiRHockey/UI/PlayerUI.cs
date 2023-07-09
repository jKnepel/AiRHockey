using System.Collections;
using System.Collections.Generic;
using HTW.AiRHockey.Game;
using UnityEngine;

namespace HTW.AiRHockey.UI
{
    public class PlayerUI : MonoBehaviour
    {
        private Transform HandMenu;

        private void Start()
        {
            HandMenu = transform.GetChild(0);
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
