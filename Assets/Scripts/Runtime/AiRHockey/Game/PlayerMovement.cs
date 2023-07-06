using UnityEngine;

namespace HTW.AiRHockey.Game
{
    public class PlayerMovement : MonoBehaviour
    {
		[SerializeField] float _speed = 3f;

		private Rigidbody rb => GetComponent<Rigidbody>();

		private void FixedUpdate()
		{
			Vector3 movement = new Vector3(0, 0, 0);
			if (Input.GetKey(KeyCode.W))
				movement.x = -1;
			if (Input.GetKey(KeyCode.A))
				movement.z = -1;
			if (Input.GetKey(KeyCode.S))
				movement.x = 1;
			if (Input.GetKey(KeyCode.D))
				movement.z = 1;
			
			rb.MovePosition(transform.position + movement * Time.deltaTime * _speed);
		}
	}
}
