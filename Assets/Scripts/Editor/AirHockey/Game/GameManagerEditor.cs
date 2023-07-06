using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CENTIS.UnityModuledNet.Networking;

namespace HTW.AiRHockey.Game
{
    [CustomEditor(typeof(GameManager))]
    public class GameManagerEditor : Editor
    {
		private GameManager _manager;
		private Editor		_gameSettingsEditor;
		private Editor		_networkSettingsEditor;

		private bool _showGameSettings = false;
		private bool _showNetworkSettings = false;

		private string _newServerName = string.Empty;

		private Vector2 _serversViewPos;

		private void OnEnable()
		{
			_manager = (GameManager)target;
			_gameSettingsEditor = CreateEditor(_manager.GameSettings);
			_networkSettingsEditor = CreateEditor(_manager.NetworkSettings);
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			_showGameSettings = EditorGUILayout.Foldout(_showGameSettings, "Game Settings", EditorStyles.foldoutHeader);
			if (_showGameSettings)
			{
				EditorGUI.indentLevel++;
				_gameSettingsEditor.OnInspectorGUI();
				EditorGUILayout.Space();
				EditorGUI.indentLevel--;
			}

			_showNetworkSettings = EditorGUILayout.Foldout(_showNetworkSettings, "Network Settings", EditorStyles.foldoutHeader);
			if (_showNetworkSettings)
			{
				EditorGUI.indentLevel++;
				_networkSettingsEditor.OnInspectorGUI();
				EditorGUILayout.Space();
				EditorGUI.indentLevel--;
			}

			EditorGUILayout.Space();

			EditorGUILayout.Toggle("Is Online:", _manager.IsOnline);
			EditorGUILayout.Toggle("Is Host:", _manager.IsHost);
			EditorGUILayout.Toggle("Is Game Running:", _manager.IsGameStarted);
			EditorGUILayout.Toggle("Is Waiting For Players:", _manager.IsWaitingForPlayers);
			EditorGUILayout.Toggle("Is Ready:", _manager.IsReady);
			EditorGUILayout.Toggle("Is Other Player Ready:", _manager.IsOtherPlayerReady);

			EditorGUILayout.Space();

			if (!_manager.IsOnline)
			{
				_newServerName = EditorGUILayout.TextField("New Servername:", _newServerName);
				if (GUILayout.Button("Create Server"))
					_manager.CreateServer(_newServerName, (success) => ConnectionEstablished(success));

				EditorGUILayout.Space();

				GUILayout.Label($"Open Servers: {_manager.OpenServers?.Count}");
				_serversViewPos = EditorGUILayout.BeginScrollView(_serversViewPos,
					EditorStyles.helpBox, GUILayout.ExpandWidth(true), GUILayout.MaxHeight(150));
				{
					if (_manager.OpenServers?.Count == 0)
						GUILayout.Label($"No Servers found!");

					for (int i = 0; i < _manager.OpenServers?.Count; i++)
					{
						OpenServerInformation server = _manager.OpenServers[i];
						EditorGUILayout.BeginHorizontal();
						{
							GUILayout.Label(server.Servername);
							GUILayout.Label($"#{server.NumberConnectedClients}/{server.MaxNumberConnectedClients}");
							if (GUILayout.Button(new GUIContent("Connect To Server"), GUILayout.ExpandWidth(false)))
								_manager.JoinServer(server.IP);
						}
						EditorGUILayout.EndHorizontal();
					}
				}
				EditorGUILayout.EndScrollView();
			}
			else if (!_manager.IsGameStarted)
			{
				if (_manager.IsReady)
				{
					if (GUILayout.Button("Unready"))
						_manager.Unready();
				}
				else
				{
					if (GUILayout.Button("ReadyUp"))
						_manager.ReadyUp();
				}

				EditorGUILayout.Space();

				if (GUILayout.Button("Disconnect"))
					_manager.DisconnectFromServer();
			}
			else
			{
				// score
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Score:", GUILayout.Width(EditorGUIUtility.labelWidth - 1));
				EditorGUILayout.SelectableLabel($"{_manager.Player1Score} : {_manager.Player2Score}",
					EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
				EditorGUILayout.EndHorizontal();

				// time
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Time:", GUILayout.Width(EditorGUIUtility.labelWidth - 1));
				EditorGUILayout.SelectableLabel(_manager.GameTimeText,
					EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.Space();

				if (GUILayout.Button("Reset Players"))
					_manager.ResetPlayers();

				if (GUILayout.Button("End Game"))
					_manager.EndGame();

				if (GUILayout.Button("Disconnect"))
					_manager.DisconnectFromServer();
			}
		}

		private void ConnectionEstablished(bool success)
		{
			if (!success)
				Debug.LogError("The Connection to the Server could not be established!");
		}
	}
}