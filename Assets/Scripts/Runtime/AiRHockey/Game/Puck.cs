using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HTW.AiRHockey.Game
{
	[RequireComponent(typeof(Rigidbody), typeof(AudioSource))]
    public class Puck : MonoBehaviour
    {
		private void OnCollisionEnter(Collision collision)
		{
			// TODO : player collision sound
		}

		private void OnTriggerEnter(Collider other)
		{
			Goal goal = other.GetComponent<Goal>();
			if (goal != null)
				InstanceFinder.GameManager.ScoreGoal(goal.Player);
		}
	}
}
