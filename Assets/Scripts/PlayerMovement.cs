using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// temp class until open xr interaction is added
public class PlayerMovement : MonoBehaviour
{
	[SerializeField] Rigidbody _player1;
	[SerializeField] Rigidbody _player2;
    [SerializeField] float _speed = 20f;

	private Rigidbody _currentPlayer;
	private Vector3 _movement = new();
	private int _layerMask;

	private void Awake()
	{
		_currentPlayer = _player1;
		_layerMask = (1 << LayerMask.NameToLayer("Barrier")) | (1 << LayerMask.NameToLayer("Player Barrier")) | (1 << LayerMask.NameToLayer("Puck"));
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.Alpha1))
			_currentPlayer = _player1;
		if (Input.GetKey(KeyCode.Alpha2))
			_currentPlayer = _player2;

		_movement.x = -Input.GetAxis("Mouse Y");
		_movement.z = Input.GetAxis("Mouse X");
	}

	private void FixedUpdate()
	{
		Vector3 newPosition = _currentPlayer.position + _movement * Time.fixedDeltaTime * _speed;
		Vector3 direction = newPosition - _currentPlayer.position;
		float delta = Vector3.Distance(_currentPlayer.position, newPosition);

		if (!Physics.Raycast(_currentPlayer.position, direction, delta, _layerMask))
			_currentPlayer.MovePosition(newPosition);
	}
}