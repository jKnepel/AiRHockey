using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
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

		public bool IsGameStarted => _gameState?.IsGameStarted ?? false;
		public bool IsWaitingForPlayers => _gameState?.IsWaitingForPlayers ?? false;
		public bool IsReady => _gameState?.IsReady ?? false;
		public bool IsOtherPlayerReady => _gameState?.IsOtherPlayerReady ?? false;

		public int Player1Score => _gameState?.Player1Score ?? 0;
		public int Player2Score => _gameState?.Player2Score ?? 0;

		public float GameTime => _gameState?.GameTime ?? 0;
		public string GameTimeText
		{
			get
			{
				int minutes = Mathf.FloorToInt(GameTime / 60);
				int seconds = Mathf.FloorToInt(GameTime % 60);
				if (minutes >= 100)
					return string.Format("99:99");
				return string.Format("{0:00}:{1:00}", minutes, seconds);
			}
		}

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
			InstanceFinder.GameManager = this;
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

			if (IsOnline)
				DisconnectFromServer();

			if (_gameState != null)
			{
				_gameState.Dispose();
				_gameState = null;
			}

			if (_playerTransform != null)
			{
				_playerTransform.Dispose();
				_playerTransform = null;
			}
		}

		private void OnDestroy()
		{
			OnDisable();
		}

		#endregion

		#region public methods

		/// <summary>
		/// Create and broadcast server for new game
		/// </summary>
		/// <param name="name"></param>
		/// <param name="onConnectionEstablished"></param>
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

		/// <summary>
		/// Join an open server
		/// </summary>
		/// <param name="serverIP"></param>
		/// <param name="onConnectionEstablished"></param>
		public void JoinServer(IPAddress serverIP, Action<bool> onConnectionEstablished = null)
		{
			ModuledNetManager.ConnectToServer(serverIP, onConnectionEstablished);
		}

		/// <summary>
		/// Disconnect from current server
		/// </summary>
		public void DisconnectFromServer()
		{
			ModuledNetManager.DisconnectFromServer();
		}

		/// <summary>
		/// Updates local player position using 2D directional input
		/// </summary>
		/// <param name="movementInput"></param>
		public void UpdatePlayerTransform(Vector2 movementInput)
		{
			if (!IsGameStarted || _playerTransform == null)
				return;

			_playerTransform.UpdatePlayerTransform(movementInput);
		}

		/// <summary>
		/// Ready up local player for game start, starts the game as host once all players are ready
		/// </summary>
		public void ReadyUp()
		{
			if (!IsOnline || !IsWaitingForPlayers)
				return;

			if (!IsHost && !IsReady)
			{
				_gameState.ReadyUp();
				return;
			}
			
			if (IsHost && IsOtherPlayerReady)
			{
				_gameState.ReadyUp();
				_gameState.StartGame();
			}
		}

		/// <summary>
		/// Unready local player before game start
		/// </summary>
		public void Unready()
		{
			if (!IsOnline || !IsWaitingForPlayers)
				return;

			if (_gameState.IsReady)
				_gameState.Unready();
		}

		public void PauseGame()
		{
			if (!IsOnline || !IsGameStarted)
				return;

			_gameState.PauseGame();
		}

		/// <summary>
		/// End game early
		/// </summary>
		public void EndGame()
		{
			if (!IsOnline || !IsGameStarted)
				return;

			_gameState.EndGame();
		}

		/// <summary>
		/// Reset player and puck to initial position
		/// </summary>
		public void ResetPlayers()
		{
			if (!IsOnline || !IsGameStarted)
				return;

			_gameState.ResetPlayers();
		}

		/// <summary>
		/// Scores goal for given player
		/// </summary>
		/// <param name="scoringPlayer"></param>
		public void ScoreGoal(bool scoringPlayer)
		{
			if (!IsOnline || !IsGameStarted)
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
		{   // load gamestate module and player module
			_gameState = new();
			_playerTransform = new(IsHost);

			GameManagerEvents.OnGameStart += GameStarted;
			GameManagerEvents.OnGameEnd += GameEnded;
			GameManagerEvents.OnGameWon += GameWon;
			GameManagerEvents.OnGoalScored += GoalScored;
			GameManagerEvents.OnResetPlayers += PlayersReset;
		}

		private void Disconnected()
		{	// dispose modules and unload scene
			GameManagerEvents.OnGameStart -= GameStarted;
			GameManagerEvents.OnGameEnd -= GameEnded;
			GameManagerEvents.OnGameWon -= GameWon;
			GameManagerEvents.OnGoalScored -= GoalScored;
			GameManagerEvents.OnResetPlayers -= PlayersReset;
			
			if (_gameState != null)
			{
				_gameState.Dispose();
				_gameState = null;
			}

			if (_playerTransform != null)
			{
				_playerTransform.Dispose();
				_playerTransform = null;
			}
		}

		private void GameStarted()
		{   // reset players
			_playerTransform.ResetPlayers();
		}

		private void GameEnded()
		{	// reset state and players, destroy puck
			_gameState.ResetState();
			_playerTransform.ResetPlayers(false);
		}

		private void GameWon(bool winningPlayer)
		{	// end game and announce win
			string winningPlayerString = winningPlayer ? "Client" : "Host";
			Debug.Log($"Game won by {winningPlayerString}");
			GameEnded();
		}

		private void GoalScored(bool scoringPlayer)
		{	// reset players and announce goal
			string scoringPlayerString = scoringPlayer ? "Client" : "Host";
			Debug.Log($"Goal scored by {scoringPlayerString}");
			_playerTransform.ResetPlayers();
		}

		private void PlayersReset()
		{	// reset players and puck
			_playerTransform.ResetPlayers();
		}

		private void ClientConnected(byte clientID)
		{   // update new player on current state, create remote player
			_playerTransform.CreateRemotePlayer();
			if (_gameState.IsReady)
				_gameState.ReadyUp();
		}

		private void ClientDisconnected(byte clientID)
		{	// end game and destroy remote player
			GameEnded();
			_playerTransform.DestroyRemotePlayer();
		}

		#endregion
	}
}
