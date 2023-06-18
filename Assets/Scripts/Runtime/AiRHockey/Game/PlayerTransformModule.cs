using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CENTIS.UnityModuledNet.Modules;
using HTW.AiRHockey.Settings;

namespace HTW.AiRHockey.Game
{
	public class PlayerTransformModule : UnreliableModule
	{
		#region lifecycle

		public override string ModuleID => "PlayerTransformModule";

		public Rigidbody LocalPlayer;
		public Rigidbody RemotePlayer;

		#endregion

		#region fields

		private readonly bool _isHost;

		private int _layerMask;

		private const int TYPE_LENGTH = 1;
		private const int FLOAT_LENGTH = 4;
		private const int TYPE_OFFSET = TYPE_LENGTH + FLOAT_LENGTH;

		private GameSettings _gameSettings = InstanceFinder.GameManager.GameSettings;

		#endregion

		#region lifecycle

		public PlayerTransformModule(bool isHost)
		{
			_isHost = isHost;
			_layerMask = (TYPE_LENGTH << LayerMask.NameToLayer("Barrier")) 
				| (TYPE_LENGTH << LayerMask.NameToLayer("Player Barrier")) 
				| (TYPE_LENGTH << LayerMask.NameToLayer("Puck"));

			Vector3 position = _isHost ? _gameSettings.InitialPositionPlayer1 : _gameSettings.InitialPositionPlayer2;
			GameObject go = GameObject.Instantiate(_gameSettings.PlayerPrefab, position, Quaternion.identity);
			go.name = "LocalPlayer";
			LocalPlayer = go.GetComponent<Rigidbody>();
		}

		public void CreateRemotePlayer()
		{
			Vector3 position = _isHost ? _gameSettings.InitialPositionPlayer2 : _gameSettings.InitialPositionPlayer1;
			GameObject go = GameObject.Instantiate(_gameSettings.PlayerPrefab, position, Quaternion.identity);
			go.name = "RemotePlayer";
			RemotePlayer = go.GetComponent<Rigidbody>();
		}

		public void DestroyRemotePlayer()
		{
			GameObject.Destroy(RemotePlayer.gameObject);
			RemotePlayer = null;
		}

		public void ResetPlayers()
		{
			LocalPlayer.MovePosition(_isHost ? _gameSettings.InitialPositionPlayer1 : _gameSettings.InitialPositionPlayer2);
			if (RemotePlayer != null)
				RemotePlayer.MovePosition(_isHost ? _gameSettings.InitialPositionPlayer2 : _gameSettings.InitialPositionPlayer1);
		}

		#endregion

		#region networking

		public override void OnReceiveData(byte sender, byte[] data)
		{
			if (_isHost)
			{	// receive input from client and calculate new position
				byte[] movementXBytes = new byte[FLOAT_LENGTH];
				Array.Copy(data, TYPE_LENGTH, movementXBytes, 0, FLOAT_LENGTH);
				float movementX = BitConverter.ToSingle(movementXBytes);
				byte[] movementYBytes = new byte[FLOAT_LENGTH];
				Array.Copy(data, TYPE_OFFSET, movementYBytes, 0, FLOAT_LENGTH);
				float movementY = BitConverter.ToSingle(movementYBytes);

				CalculateTransform(new(movementX, movementY), false);
			}
			else
			{	// receive new position for host and client from host
				PlayerTransformPacketType type = (PlayerTransformPacketType)data[0];
				Rigidbody player = type == PlayerTransformPacketType.ClientTransform ? LocalPlayer : RemotePlayer;

				byte[] positionXBytes = new byte[FLOAT_LENGTH];
				Array.Copy(data, TYPE_LENGTH, positionXBytes, 0, FLOAT_LENGTH);
				float positionX = BitConverter.ToSingle(positionXBytes);
				byte[] positionYBytes = new byte[FLOAT_LENGTH];
				Array.Copy(data, TYPE_OFFSET, positionYBytes, 0, FLOAT_LENGTH);
				float positionY = BitConverter.ToSingle(positionYBytes);
				player.MovePosition(new(positionX, player.position.y, positionY));
			}
		}

		public void CalculateTransform(Vector2 movementInput, bool isLocal = true)
		{
			if (!_isHost)
			{	// send input to host
				byte[] data = new byte[9];
				data[0] = (byte)PlayerTransformPacketType.ClientTransform;
				Array.Copy(BitConverter.GetBytes(movementInput.x), 0, data, TYPE_LENGTH, FLOAT_LENGTH);
				Array.Copy(BitConverter.GetBytes(movementInput.y), 0, data, TYPE_OFFSET, FLOAT_LENGTH);
				SendData(data);
				return;
			}
			else
			{	// calculate position of current player
				Rigidbody player = isLocal ? LocalPlayer : RemotePlayer;
				Vector3 movement = new(movementInput.x, 0, movementInput.y);
				Vector3 newPosition = player.transform.position + InstanceFinder.GameManager.GameSettings.PlayerSpeed * Time.fixedDeltaTime * movement;
				Vector3 direction = newPosition - player.transform.position;
				float delta = Vector3.Distance(player.transform.position, newPosition);

				if (!Physics.Raycast(player.transform.position, direction, delta, _layerMask))
					player.MovePosition(newPosition);

				if (RemotePlayer == null)
					return;

				// update position on client
				byte[] data = new byte[9];
				data[0] = (byte)(isLocal ? PlayerTransformPacketType.HostTransform : PlayerTransformPacketType.ClientTransform);
				Array.Copy(BitConverter.GetBytes(player.position.x), 0, data, TYPE_LENGTH, FLOAT_LENGTH);
				Array.Copy(BitConverter.GetBytes(player.position.z), 0, data, TYPE_OFFSET, FLOAT_LENGTH);
				SendData(data);
			}
		}

		#endregion
	}

	public enum PlayerTransformPacketType : byte
	{
		HostTransform,
		ClientTransform
	}
}
