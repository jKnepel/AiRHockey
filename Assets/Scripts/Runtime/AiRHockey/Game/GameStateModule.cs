using System;
using UnityEngine;
using CENTIS.UnityModuledNet.Modules;

namespace HTW.AiRHockey.Game
{
	public class GameStateModule : ReliableModule
	{
		#region properties

		public override string ModuleID => "GameStateModule";

		public bool IsGameRunning { get; private set; }
		public bool IsWaitingForPlayers { get; private set; }

		public bool IsReady { get; private set; }
		public bool IsOtherPlayerReady { get; private set; }

		public int Player1Score { get; private set; }
		public int Player2Score { get; private set; }

		public Action<bool> OnReadyUp;
		public Action		OnGameStart;
		public Action		OnGameEnd;
		public Action<bool> OnGameWon;
		public Action<bool> OnGoalScored;

		#endregion

		#region lifecycle

		public GameStateModule()
		{
			IsWaitingForPlayers = true;
			Player1Score = 0;
			Player2Score = 0;
		}

		#endregion

		#region networking

		public void ReadyUp()
		{
			IsReady = true;
			OnReadyUp?.Invoke(true);
			byte[] data = { (byte)GameStatePacketType.ReadyUp };
			SendData(data);
		}

		public void Unready()
		{
			IsReady = false;
			OnReadyUp?.Invoke(false);
			byte[] data = { (byte)GameStatePacketType.Unready };
			SendData(data);
		}

		public void StartGame()
		{
			byte[] data = { (byte)GameStatePacketType.GameStart };
			SendData(data);
			IsGameRunning = true;
			IsWaitingForPlayers = false;
			OnGameStart?.Invoke();
		}

		public void GamePause()
		{

		}

		public void EndGame()
		{
			byte[] data = { (byte)GameStatePacketType.GameEnd };
			SendData(data);
			IsGameRunning = false;
			IsWaitingForPlayers = true;
			OnGameEnd?.Invoke();
		}

		public void GameWon(bool winningPlayer)
		{
			byte[] data = { (byte)GameStatePacketType.GameWon, (byte)(winningPlayer ? 1 : 0) };
			SendData(data);
			IsGameRunning = false;
			IsWaitingForPlayers = true;
			OnGameWon?.Invoke(winningPlayer);
		}

		public void GoalScored(bool scoringPlayer)
		{
			byte[] data = { (byte)GameStatePacketType.Goal, (byte)(scoringPlayer ? 1 : 0) };
			SendData(data);
			if (scoringPlayer)
				Player2Score++;
			else
				Player1Score++;
			OnGoalScored?.Invoke(scoringPlayer);
		}

		public override void OnReceiveData(byte sender, byte[] data)
		{
			GameStatePacketType type = (GameStatePacketType)data[0];
			switch (type)
			{
				case GameStatePacketType.ReadyUp:
					IsOtherPlayerReady = true;
					break;
				case GameStatePacketType.Unready:
					IsOtherPlayerReady = false;
					break;
				case GameStatePacketType.GameStart:
					IsGameRunning = true;
					IsWaitingForPlayers = false;
					OnGameStart?.Invoke();
					break;
				case GameStatePacketType.GamePause:

					break;
				case GameStatePacketType.GameEnd:
					IsGameRunning = false;
					IsWaitingForPlayers = true;
					OnGameStart?.Invoke();
					break;
				case GameStatePacketType.GameWon:
					IsGameRunning = false;
					IsWaitingForPlayers = true;
					OnGameWon?.Invoke(data[1] != 0);
					break;
				case GameStatePacketType.Goal:
					bool scoringPlayer = data[1] != 0;
					if (scoringPlayer)
						Player2Score++;
					else
						Player1Score++;
					OnGoalScored?.Invoke(scoringPlayer);
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
		GameWon,
		GameEnd,
		Goal
	}
}
