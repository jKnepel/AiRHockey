using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HTW.AiRHockey.Settings;

namespace HTW.AiRHockey.Game
{
    public class GameManager : MonoBehaviour
    {
		#region fields

		[SerializeField] protected GameSettings _settings;

		[SerializeField]  protected Puck _puck;
		[SerializeField]  protected List<Player> _players;

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

		public virtual void ResetPositions()
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

		#endregion

		#region private methods

		private void GameWon(Player winningPlayer)
		{
			Debug.Log($"Game Won by Player {winningPlayer}");
		}

		#endregion
	}
}
