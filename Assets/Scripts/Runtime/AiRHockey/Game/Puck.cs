using UnityEngine;

namespace HTW.AiRHockey.Game
{
	[RequireComponent(typeof(Rigidbody))]
    public class Puck : MonoBehaviour
    {
		[SerializeField] private AudioSource _puckPlayerSound;
		[SerializeField] private AudioSource _puckBoardSound;

		private void OnCollisionEnter(Collision collision)
		{
			Debug.Log(collision.gameObject.layer);
			if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
			{
				_puckPlayerSound.Play();
			} 
			else if (collision.gameObject.layer == LayerMask.NameToLayer("Barrier"))
			{
				_puckBoardSound.Play();
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			Goal goal = other.GetComponent<Goal>();
			if (goal != null)
				InstanceFinder.GameManager.ScoreGoal(goal.Player);
		}
	}
}
