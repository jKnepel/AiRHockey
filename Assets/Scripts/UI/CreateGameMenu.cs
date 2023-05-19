using UnityEngine;
using Microsoft.MixedReality.Toolkit.UX;

namespace HTW.AiRHockey.UI
{
    public class CreateGameMenu : MonoBehaviour
    {
        private string _lobbyName;
        private float _gameTime;

        public void OnLobbyNameChange(string lobbyName)
        {
            _lobbyName = lobbyName;
        }

        public void OnGameTimeChange(SliderEventData data)
        {
            _gameTime = data.NewValue;
        }

        public void OnSubmit()
        {
            Debug.Log(_lobbyName);
            Debug.Log(_gameTime);
            // create lobby
            // transition to "place arena"            
        }
    }
}
