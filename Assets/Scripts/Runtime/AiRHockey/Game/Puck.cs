using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HTW.AiRHockey.Game
{
	[RequireComponent(typeof(Rigidbody), typeof(AudioSource))]
    public class Puck : MonoBehaviour
    {
		public Vector3 InitialPosition = new();

		private void OnCollisionEnter(Collision collision)
		{
			// player collision sound
		}

		private void OnTriggerEnter(Collider other)
		{
			Goal goal = other.GetComponent<Goal>();
		}
	}
}
