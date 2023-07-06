using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UX;
using HTW.AiRHockey.Game;

namespace HTW.AiRHockey.UI
{
    public class CreateGameMenu : MonoBehaviour
    {
        private const int MAX_ROUNDS = 20;

        private string _lobbyName;

        public void OnLobbyNameChange(string lobbyName)
        {
            _lobbyName = lobbyName;
        }

        public void OnDecidingScoreChange(SliderEventData data)
        {
            if (InstanceFinder.GameManager)
            {
                InstanceFinder.GameManager.GameSettings.DecidingScore = (int)(MAX_ROUNDS * data.NewValue);
            }
        }

        public void OnSubmit()
        {
            InstanceFinder.GameManager.CreateServer(_lobbyName);
        }
    }
}