using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UX;

namespace HTW.AiRHockey.UI
{
    public class CreateGameMenu : MonoBehaviour
    {
        // value has to by in sync with max value on UI slider
        private static int MAX_ROUNDS = 20;
        private string _lobbyName;
        private int _decidingScore;

        public void OnLobbyNameChange(string lobbyName)
        {
            _lobbyName = lobbyName;
        }

        public void OnDecidingScoreChange(SliderEventData data)
        {
            _decidingScore = (int) (MAX_ROUNDS * data.NewValue);
        }

        public void OnSubmit()
        {
            Debug.Log(_lobbyName);
            Debug.Log(_decidingScore);
            // TODO: create game lobby
            // TODO: transition to "place arena"
        }
    }
}
