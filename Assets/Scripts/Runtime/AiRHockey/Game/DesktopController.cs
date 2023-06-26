using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HTW.AiRHockey.Game
{
	// temp class until open xr interaction is added
	public class DesktopController : MonoBehaviour
	{
		private Vector2 _movement = new();

		private void Awake()
		{
			Cursor.lockState = CursorLockMode.Locked;
		}

		private void FixedUpdate()
		{
			_movement.x = -Input.GetAxis("Mouse Y");
			_movement.y = Input.GetAxis("Mouse X");

			InstanceFinder.GameManager.UpdatePlayerTransform(_movement);
		}
	}
}