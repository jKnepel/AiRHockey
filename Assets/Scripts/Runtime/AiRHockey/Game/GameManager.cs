using System;
using System.Collections.Generic;
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

		private Puck _currentPuck;
		// TODO : sync puck

		private GameStateModule _gameState;
		private PlayerTransformModule _playerTransform;

		private AsyncOperation _sceneLoadOperation;

		#endregion

		#region lifecycle

		private void OnEnable()
		{
			InstanceFinder.GameManager = this;
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

			if (IsOnline)
				DisconnectFromServer();
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
			if (_gameState == null || !_gameState.IsGameRunning || _playerTransform == null)
				return;

			_playerTransform.CalculateTransform(movementInput);
		}

		/// <summary>
		/// Ready up local player for game start, starts the game as host once all players are ready
		/// </summary>
		public void ReadyUp()
		{
			if (!IsOnline || _gameState == null || !_gameState.IsWaitingForPlayers)
				return;

			if (!IsHost && !_gameState.IsReady)
			{
				_gameState.ReadyUp();
				return;
			}
			
			if (IsHost && _gameState.IsOtherPlayerReady)
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
			if (!IsOnline || _gameState == null || !_gameState.IsWaitingForPlayers)
				return;

			if (_gameState.IsReady)
				_gameState.Unready();
		}

		/// <summary>
		/// End game early
		/// </summary>
		public void EndGame()
		{
			if (!IsOnline || _gameState == null || !_gameState.IsGameRunning)
				return;

			_gameState.EndGame();
		}

		/// <summary>
		/// Reset player and puck to initial position
		/// </summary>
		public void ResetPlayers()
		{
			if (!IsOnline || _gameState == null || !_gameState.IsGameRunning || !IsHost)
				return;

			_playerTransform.ResetPlayers();
			if (_currentPuck != null)
				Destroy(_currentPuck);
			_currentPuck = Instantiate(GameSettings.PuckPrefab, GameSettings.InitialPuckPosition, Quaternion.identity);
		}

		/// <summary>
		/// Scores goal for given player
		/// </summary>
		/// <param name="scoringPlayer"></param>
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
		{   // load scene, gamestate module and player module
			System.Collections.IEnumerator LoadGameScene()
			{
				_sceneLoadOperation = SceneManager.LoadSceneAsync("GameScene");
				while (!_sceneLoadOperation.isDone)
					yield return null;

				_gameState = new();
				_gameState.OnGameStart += GameStarted;
				_gameState.OnGameEnd += GameEnded;
				_gameState.OnGameWon += GameWon;
				_gameState.OnGoalScored += GoalScored;

				_playerTransform = new(IsHost);
			}

			StartCoroutine(LoadGameScene());
		}

		private void Disconnected()
		{	// dispose modules and unload scene
			_gameState.OnGameStart -= GameStarted;
			_gameState.OnGameEnd -= GameEnded;
			_gameState.OnGameWon -= GameWon;
			_gameState.OnGoalScored -= GoalScored;
			_gameState.Dispose();
			_gameState = null;

			_playerTransform.Dispose();
			_playerTransform = null;

			SceneManager.LoadScene("MainScene");
		}

		private void GameStarted()
		{	// reset players
			ResetPlayers();
		}

		private void GameEnded()
		{	// reset state and players, destroy puck
			_gameState.ResetState();
			_playerTransform.ResetPlayers();
			if (_currentPuck != null)
				Destroy(_currentPuck);
		}

		private void GameWon(bool winningPlayer)
		{	// end game and announce win
			string winningPlayerString = winningPlayer ? "Player 2" : "Player 1";
			Debug.Log($"Game won by {winningPlayerString}");
			GameEnded();
		}

		private void GoalScored(bool scoringPlayer)
		{	// reset player and announce goal
			string scoringPlayerString = scoringPlayer ? "Player 2" : "Player 1";
			Debug.Log($"Goal scored by {scoringPlayerString}");
			ResetPlayers();
		}

		private void ClientConnected(byte clientID)
		{   // update new player on current state, create remote player
			void loadClient(AsyncOperation sceneLoad)
			{
				_playerTransform.CreateRemotePlayer();
				if (_gameState.IsReady)
					_gameState.ReadyUp();
			}

			if (_sceneLoadOperation.isDone)
				loadClient(_sceneLoadOperation);
			else
				_sceneLoadOperation.completed += loadClient;
		}

		private void ClientDisconnected(byte clientID)
		{	// end game and destroy remote player
			GameEnded();
			_playerTransform.DestroyRemotePlayer();
		}

		#endregion
	}
}
