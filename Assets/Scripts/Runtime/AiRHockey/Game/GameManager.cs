using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
using CENTIS.UnityModuledNet;
using CENTIS.UnityModuledNet.Networking;
using CENTIS.UnityModuledNet.Managing;
using HTW.AiRHockey.Settings;

namespace HTW.AiRHockey.Game
{
    public class GameManager : MonoBehaviour
    {
		#region properties

		public bool IsOnline => ModuledNetManager.IsConnected;
		public bool IsHost => ModuledNetManager.IsHost;

		public bool IsGameRunning => _gameState?.IsGameRunning ?? false;
		public bool IsWaitingForPlayers => _gameState?.IsWaitingForPlayers ?? false;
		public bool IsReady => _gameState?.IsReady ?? false;
		public bool IsOtherPlayerReady => _gameState?.IsOtherPlayerReady ?? false;

		public int Player1Score => _gameState?.Player1Score ?? 0;
		public int Player2Score => _gameState?.Player2Score ?? 0;

		public List<OpenServerInformation> OpenServers => ModuledNetManager.OpenServers;

		public ModuledNetSettings NetworkSettings => ModuledNetSettings.GetOrCreateSettings();

		public GameSettings GameSettings;

		#endregion

		#region fields

		private GameStateModule _gameState;
		private PlayerTransformModule _playerTransform;

		#endregion

		#region lifecycle

		private void OnEnable()
		{
			GameObject go = GameObject.Find("GameManager");
			if (go != gameObject)
				Destroy(gameObject);

			DontDestroyOnLoad(gameObject);
			NetworkSettings.MaxNumberClients = 2;

			ModuledNetManager.OnConnected += Connected;
			ModuledNetManager.OnDisconnected += Disconnected;
			ModuledNetManager.OnClientConnected += ClientConnected;
			ModuledNetManager.OnClientDisconnected += ClientDisconnected;
		}

		private void OnDisable()
		{
			ModuledNetManager.OnConnected -= Connected;
			ModuledNetManager.OnDisconnected -= Disconnected;
			ModuledNetManager.OnClientConnected -= ClientConnected;
			ModuledNetManager.OnClientDisconnected -= ClientDisconnected;
		}

		#endregion

		#region public methods

		public void CreateServer(string name, Action<bool> onConnectionEstablished = null)
		{
			if (string.IsNullOrEmpty(name))
			{
				Debug.LogError("The Servername can't be null or empty!");
				onConnectionEstablished?.Invoke(false);
				return;
			}

			ModuledNetManager.CreateServer(name, onConnectionEstablished);
		}

		public void JoinServer(IPAddress serverIP, Action<bool> onConnectionEstablished = null)
		{
			ModuledNetManager.ConnectToServer(serverIP, onConnectionEstablished);
		}

		public void DisconnectFromServer()
		{
			ModuledNetManager.DisconnectFromServer();
		}

		public void ReadyUp()
		{
			if (!IsOnline || _gameState == null || !_gameState.IsWaitingForPlayers)
				return;

			if (!_gameState.IsReady)
				_gameState.ReadyUp();
			if (IsHost && _gameState.IsOtherPlayerReady)
				_gameState.StartGame();
		}

		public void Unready()
		{
			if (!IsOnline || _gameState == null || !_gameState.IsWaitingForPlayers)
				return;

			if (_gameState.IsReady)
				_gameState.Unready();
		}

		public void EndGame()
		{
			if (!IsOnline || _gameState == null || !_gameState.IsGameRunning)
				return;

			_gameState.EndGame();
		}

		public void ScoreGoal(bool scoringPlayer)
		{
			if (!IsOnline || _gameState == null || !_gameState.IsGameRunning)
				return;

			_gameState.ScoreGoal(scoringPlayer);
			if (scoringPlayer && _gameState.Player2Score >= GameSettings.DecidingScore
				|| !scoringPlayer && _gameState.Player1Score >= GameSettings.DecidingScore)
			{
				_gameState.WinGame(scoringPlayer);
			}
		}

		#endregion

		#region private methods

		private void Connected()
		{
			SceneManager.LoadScene("GameScene");

			_gameState = new();
			_gameState.OnOtherPlayerReadyUp += OtherPlayerReadyUp;
			_gameState.OnGameStart += GameStarted;
			_gameState.OnGameEnd += GameEnded;
			_gameState.OnGameWon += GameWon;
			_gameState.OnGoalScored += GoalScored;

			// create player prefab
			// create player transform module
		}

		private void Disconnected()
		{
			_gameState.OnOtherPlayerReadyUp -= OtherPlayerReadyUp;
			_gameState.OnGameStart -= GameStarted;
			_gameState.OnGameEnd -= GameEnded;
			_gameState.OnGameWon -= GameWon;
			_gameState.OnGoalScored -= GoalScored;
			_gameState.Dispose();
			_gameState = null;

			// destroy player transform module

			SceneManager.LoadScene("MainScene");
		}

		private void OtherPlayerReadyUp(bool isReady)
		{
			if (_gameState.IsWaitingForPlayers && isReady && _gameState.IsOtherPlayerReady)
				_gameState.StartGame();
		}

		private void GameStarted()
		{
			// reset players
			// set beginning player collider
		}

		private void GameEnded()
		{
			// reset players
			// reset score
		}

		private void GameWon(bool winningPlayer)
		{
			string winningPlayerString = winningPlayer ? "Player 2" : "Player 1";
			Debug.Log($"Game won by {winningPlayerString}");
			GameEnded();
		}

		private void GoalScored(bool scoringPlayer)
		{
			string scoringPlayerString = scoringPlayer ? "Player 2" : "Player 1";
			Debug.Log($"Goal scored by {scoringPlayerString}");
		}

		private void ClientConnected(byte clientID)
		{	// update new player on current state
			if (_gameState.IsReady)
				_gameState.ReadyUp();
		}

		private void ClientDisconnected(byte clientID)
		{
			_gameState.ResetState();
			// reset players
			// reset score
		}

		#endregion
	}
}
