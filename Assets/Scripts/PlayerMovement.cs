using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Player _playerNumber;
    [SerializeField] float _speed = 2f;

	private Rigidbody rb => GetComponent<Rigidbody>();

	private void FixedUpdate()
	{
		Vector3 movement = new Vector3(0, 0, 0);

		if (_playerNumber == Player.Player1)
		{
			if (Input.GetKey(KeyCode.W))
				movement.x = -1;
			if (Input.GetKey(KeyCode.A))
				movement.z = -1;
			if (Input.GetKey(KeyCode.S))
				movement.x = 1;
			if (Input.GetKey(KeyCode.D))
				movement.z = 1;
		}
		else if (_playerNumber == Player.Player2)
		{
			if (Input.GetKey(KeyCode.UpArrow))
				movement.x = -1;
			if (Input.GetKey(KeyCode.LeftArrow))
				movement.z = -1;
			if (Input.GetKey(KeyCode.DownArrow))
				movement.x = 1;
			if (Input.GetKey(KeyCode.RightArrow))
				movement.z = 1;
		}

		rb.MovePosition(transform.position + movement * Time.deltaTime * _speed);
	}
}

public enum Player
{
    Player1 = 0,
    Player2 = 1
}
