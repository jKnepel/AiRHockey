using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.UX;
using CENTIS.UnityModuledNet.Networking;
using HTW.AiRHockey.Game;

namespace HTW.AiRHockey.UI
{
    public class JoinGameMenu : MonoBehaviour
    {
        private const int UPDATE_LOBBIES_TIMEOUT = 5000;

        [SerializeField] private GameObject lobbyPrefab;
        [SerializeField] private GameObject container;

        private bool _isRunning = true;
        
        private void OnEnable()
        {
            _ = UpdateOpenLobbies();
        }

		private void OnDisable()
		{
            _isRunning = false;
		}

		private async Task UpdateOpenLobbies()
		{
            if (!_isRunning)
                return;

            // destroy old prefabs
            while (container.transform.childCount > 0)
			{
#if UNITY_EDITOR
                DestroyImmediate(container.transform.GetChild(0).gameObject);
#else
                Destroy(container.transform.GetChild(0).gameObject);
#endif
            }

            // create new prefab buttons for each lobby
            List<OpenServerInformation> openServers = InstanceFinder.GameManager.OpenServers;
            foreach (OpenServerInformation lobby in openServers)
            {
                GameObject entry = Instantiate(lobbyPrefab, container.transform);
                TextMeshProUGUI text = entry.GetComponentInChildren<TextMeshProUGUI>();
                text.text = $"<size=8>{lobby.Servername}</size>\n<size=6><alpha=#88>Is Open: {!lobby.IsServerFull}</size>";
                entry.GetComponent<PressableButton>().OnClicked.AddListener(() => InstanceFinder.GameManager.JoinServer(lobby.IP));
            }

            await Task.Delay(UPDATE_LOBBIES_TIMEOUT);
        }
    }
}
