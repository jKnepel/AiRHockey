using System;
using UnityEngine;
using CENTIS.UnityModuledNet.Modules;

namespace HTW.AiRHockey.Game
{
	public class GameStateModule : ReliableModule
	{
		#region properties

		public override string ModuleID => "GameStateModule";

		// TODO : replace with enum/state machine
		public bool IsWaitingForPlayers { get; private set; }
		public bool IsGameStarted { get; private set; }
		public bool IsGamePaused { get; private set; }

		public bool IsReady { get; private set; }
		public bool IsOtherPlayerReady { get; private set; }

		public int Player1Score { get; private set; }
		public int Player2Score { get; private set; }

		public float GameTime { get; private set; }

		#endregion

		#region lifecycle

		public GameStateModule()
		{
			ResetState();
		}

		public override void Update()
		{
			base.Update();
			if (IsGameStarted)
				GameTime += Time.deltaTime;
		}

		#endregion

		#region networking

		public void ResetState()
		{
			IsWaitingForPlayers = true;
			IsGameStarted = false;
			IsGamePaused = false;
			Time.timeScale = 1;
			IsReady = false;
			IsOtherPlayerReady = false;
			Player1Score = 0;
			Player2Score = 0;
			GameTime = 0;
		}

		public void ReadyUp()
		{
			byte[] data = { (byte)GameStatePacketType.ReadyUp };
			SendData(data);
			IsReady = true;
		}

		public void Unready()
		{
			byte[] data = { (byte)GameStatePacketType.Unready };
			SendData(data);
			IsReady = false;
		}

		public void StartGame()
		{
			byte[] data = { (byte)GameStatePacketType.GameStart };
			SendData(data);
			ResetState();
			IsGameStarted = true;
			GameManagerEvents.OnGameStart?.Invoke();
		}

		public void PauseGame()
		{
			byte[] data = { (byte)GameStatePacketType.GamePause };
			SendData(data);
			IsGamePaused = true;
			Time.timeScale = 0;
			GameManagerEvents.OnGamePaused?.Invoke();
		}

		public void UnpauseGame()
		{
			byte[] data = { (byte)GameStatePacketType.GameResume };
			SendData(data);
			IsGamePaused = false;
			Time.timeScale = 1;
			GameManagerEvents.OnGameResumed?.Invoke();
		}

		public void EndGame()
		{
			byte[] data = { (byte)GameStatePacketType.GameEnd };
			SendData(data);
			ResetState();
			GameManagerEvents.OnGameEnd?.Invoke();
		}

		public void WinGame(bool winningPlayer)
		{
			byte[] data = { (byte)GameStatePacketType.GameWon, (byte)(winningPlayer ? 1 : 0) };
			SendData(data);
			ResetState();
			GameManagerEvents.OnGameWon?.Invoke(winningPlayer);
		}

		public void ScoreGoal(bool scoringPlayer)
		{
			byte[] data = { (byte)GameStatePacketType.Goal, (byte)(scoringPlayer ? 1 : 0) };
			SendData(data);
			if (scoringPlayer) Player2Score++; else Player1Score++;
			GameManagerEvents.OnGoalScored?.Invoke(scoringPlayer);
		}

		public void ResetPlayers()
		{
			byte[] data = { (byte)GameStatePacketType.ResetPlayers };
			SendData(data);
			GameManagerEvents.OnResetPlayers?.Invoke();
		}

		public override void OnReceiveData(byte sender, byte[] data)
		{
			GameStatePacketType type = (GameStatePacketType)data[0];
			switch (type)
			{
				case GameStatePacketType.ReadyUp:
					IsOtherPlayerReady = true;
					if (IsReady)
						StartGame();
					break;
				case GameStatePacketType.Unready:
					IsOtherPlayerReady = false;
					break;
				case GameStatePacketType.GameStart:
					IsGameStarted = true;
					IsWaitingForPlayers = false;
					IsReady = false;
					IsOtherPlayerReady = false;
					GameManagerEvents.OnGameStart?.Invoke();
					break;
				case GameStatePacketType.GamePause:
					IsGamePaused = true;
					Time.timeScale = 0;
					GameManagerEvents.OnGamePaused?.Invoke();
					break;
				case GameStatePacketType.GameResume:
					IsGamePaused = false;
					Time.timeScale = 1;
					GameManagerEvents.OnGameResumed?.Invoke();
					break;
				case GameStatePacketType.GameEnd:
					ResetState();
					GameManagerEvents.OnGameEnd?.Invoke();
					break;
				case GameStatePacketType.GameWon:
					ResetState();
					GameManagerEvents.OnGameWon?.Invoke(data[1] != 0);
					break;
				case GameStatePacketType.Goal:
					bool scoringPlayer = data[1] != 0;
					if (scoringPlayer) Player2Score++; else Player1Score++;
					GameManagerEvents.OnGoalScored?.Invoke(scoringPlayer);
					break;
				case GameStatePacketType.ResetPlayers:
					GameManagerEvents.OnResetPlayers?.Invoke();
					break;
			}
		}

		#endregion
	}

	public enum GameStatePacketType : byte
	{
		ReadyUp,
		Unready,
		GameStart,
		GamePause,
		GameResume,
		GameEnd,
		GameWon,
		Goal,
		ResetPlayers
	}
}
