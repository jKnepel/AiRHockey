using System;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Microsoft.MixedReality.Toolkit.UX;

namespace HTW.AiRHockey.UI
{
    public class JoinGameMenu : MonoBehaviour
    {
        private struct GameLobby 
        {
            public string _name;
            public int _decidingScore;
        }

        [SerializeField] private GameObject lobbyPrefab;
        [SerializeField] private GameObject container;
        private List<GameLobby> _lobbies;

        private void OnEnable()
        {
            _lobbies = new List<GameLobby>();
            // TODO: get available game lobbies
            _lobbies.Add(new GameLobby {
                 _name = "Test Lobby", 
                 _decidingScore = 10
            });
            _lobbies.Add(new GameLobby {
                _name = "Another Lobby",
                _decidingScore = 20
            });
            foreach (GameLobby lobby in _lobbies) {
                GameObject entry = Instantiate(lobbyPrefab);
                TextMeshProUGUI text = entry.GetComponentInChildren<TextMeshProUGUI>();
                text.text = $"<size=8>{lobby._name}</size>\n<size=6><alpha=#88>Deciding Score: {lobby._decidingScore}</size>";
                entry.transform.SetParent(container.transform, false);
                // TODO: join game on button click
            }
        }
    }
}
