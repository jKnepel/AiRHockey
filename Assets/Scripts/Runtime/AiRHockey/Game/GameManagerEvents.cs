using System;

namespace HTW.AiRHockey.Game
{
	public static class GameManagerEvents
	{
		public static Action OnGameStart;
		public static Action OnGameEnd;
		public static Action<bool> OnGameWon;
		public static Action<bool> OnGoalScored;
		public static Action OnResetPlayers;
	}
}
