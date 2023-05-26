using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HTW.AiRHockey.Settings;

namespace HTW.AiRHockey.Game
{
    public class GameManager : MonoBehaviour
    {
		#region fields

		[SerializeField] private GameSettings _settings;

		[SerializeField] private Puck _puck;
		[SerializeField] private List<Player> _players;

        #endregion

        #region properties

        public ScriptableObject Settings => _settings;

		#endregion

		#region lifecycle

		private void Awake()
		{
			// set initial positions
			// start game
		}

		private void Update()
		{
			if (Input.GetKey(_settings.ResetPositions))
				ResetPositions();
		}

		#endregion

		#region public methods

		// start game

		// pause game

		public void ResetPositions()
		{
			foreach (Player player in _players)
				player.transform.position = player.InitialPosition;

			Rigidbody puckRB = _puck.gameObject.GetComponent<Rigidbody>();
			puckRB.velocity = new();
			puckRB.angularVelocity = new();
			_puck.transform.position = _puck.InitialPosition;
		}

		public void IncreaseScore(Player scoringPlayer)
		{
			scoringPlayer.Score++;
			if (scoringPlayer.Score >= _settings.DecidingScore)
				GameWon(scoringPlayer);
			else
				ResetPositions();
		}

		public void PauseGame()
		{
			// TODO: pause the game
		}

		#endregion

		#region private methods

		private void GameWon(Player winningPlayer)
		{
			Debug.Log($"Game Won by Player {winningPlayer}");
		}

		#endregion
	}
}
