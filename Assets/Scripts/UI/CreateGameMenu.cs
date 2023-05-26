using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UX;

namespace HTW.AiRHockey.UI
{
    public class CreateGameMenu : MonoBehaviour
    {
        // value has to by in sync with max value on UI slider
        private static int MAX_TIME_MINUTES = 20;
        private string _lobbyName;
        private TimeSpan _gameTime;

        public void OnLobbyNameChange(string lobbyName)
        {
            _lobbyName = lobbyName;
        }

        public void OnGameTimeChange(SliderEventData data)
        {
            _gameTime = new TimeSpan(0, (int) (MAX_TIME_MINUTES * data.NewValue), 0);
        }

        public void OnSubmit()
        {
            Debug.Log(_lobbyName);
            Debug.Log(_gameTime);
            // TODO: create game lobby
            // TODO: transition to "place arena"
        }
    }
}
