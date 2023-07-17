using System;
using UnityEngine;
using CENTIS.UnityModuledNet.Modules;

namespace HTW.AiRHockey.Game
{
	public class PlayerTransformModule : UnreliableModule
	{
		#region properties

		public override string ModuleID => "PlayerTransformModule";

		#endregion

		#region fields

		private readonly bool _isHost;
		private readonly Transform _spawnParent;
		private readonly Transform _hostSpawn;
		private readonly Transform _clientSpawn;
		private readonly Transform _puckSpawn;

		private readonly int _layerMask;

		private Rigidbody	_localPlayer;
		private Rigidbody	_remotePlayer;
		private Rigidbody	_currentPuck;

		private float _time = 0;

		private Vector2 _clientsDirectionalInput = new();

		private const int FLOAT_LENGTH	= 4;

		#endregion

		#region lifecycle

		public PlayerTransformModule(bool isHost, Transform spawnParent, Transform hostSpawn, Transform clientSpawn, Transform puckSpawn)
		{
			_isHost = isHost;
			_spawnParent = spawnParent;
			_hostSpawn = hostSpawn;
			_clientSpawn = clientSpawn;
			_puckSpawn = puckSpawn;
			_layerMask = (1 << LayerMask.NameToLayer("Barrier")) 
				| (1 << LayerMask.NameToLayer("Player Barrier")) 
				| (1 << LayerMask.NameToLayer("Puck"));

			Vector3 position = _isHost ? _hostSpawn.position : _clientSpawn.position;
			Player player = GameObject.Instantiate(InstanceFinder.GameSettings.PlayerPrefab, position, Quaternion.identity, _spawnParent);
			player.IsLocalPlayer = true;
			_localPlayer = player.Rigidbody;
			_localPlayer.transform.GetChild(0).GetComponent<MeshRenderer>().material = _isHost ? InstanceFinder.GameSettings.HostMaterial : InstanceFinder.GameSettings.ClientMaterial;
			_localPlayer.gameObject.name = "LocalPlayer";
		}

		public override void Update()
		{
			base.Update();

			if (_remotePlayer == null)
				return;

			_time += Time.deltaTime;
			if (_time >= (float)(1 / (float)InstanceFinder.GameSettings.TransformSendHertz))
			{	// limit client update to given hertz
				if (_isHost)
					SendTransformsToClient();
				else
					SendInputToHost();
				_time = 0;
			}
		}

		public void CreateRemotePlayer()
		{
			Vector3 position = _isHost ? _clientSpawn.position : _hostSpawn.position;
			Player player = GameObject.Instantiate(InstanceFinder.GameSettings.PlayerPrefab, position, Quaternion.identity, _spawnParent);
			player.IsLocalPlayer = false;
			_remotePlayer = player.Rigidbody;
			_remotePlayer.transform.GetChild(0).GetComponent<MeshRenderer>().material = _isHost ? InstanceFinder.GameSettings.ClientMaterial : InstanceFinder.GameSettings.HostMaterial;
			_remotePlayer.gameObject.name = "RemotePlayer";
		}

		public void DestroyRemotePlayer()
		{
			GameObject.Destroy(_remotePlayer.gameObject);
			_remotePlayer = null;
		}

		public void ResetPlayers(bool deletePuck = true)
		{
			if (deletePuck)
			{
				GameObject.Destroy(_currentPuck.gameObject);
			}
			else if (_currentPuck != null)
			{
				_currentPuck.position = _puckSpawn.position;
				_currentPuck.velocity = Vector3.zero;
				_currentPuck.angularVelocity = Vector3.zero;
				if (!_isHost) _currentPuck.isKinematic = true;
			}
			else
			{
				_currentPuck = GameObject.Instantiate(InstanceFinder.GameSettings.PuckPrefab, _puckSpawn.position, Quaternion.identity, _spawnParent).GetComponent<Rigidbody>();
				if (!_isHost) _currentPuck.isKinematic = true;
			}

			if (!_isHost)
				return;

			_localPlayer.position = _isHost ? _hostSpawn.position : _clientSpawn.position;
			_localPlayer.velocity = Vector3.zero;
			_localPlayer.angularVelocity = Vector3.zero;
			if (_remotePlayer != null)
			{
				_remotePlayer.position = _isHost ? _clientSpawn.position : _hostSpawn.position;
				_remotePlayer.velocity = Vector3.zero;
				_remotePlayer.angularVelocity = Vector3.zero;
			}
		}

		public void UpdatePlayerTransform(Vector2 movementInput)
		{
			CalculateTransform(movementInput, true);
		}

		#endregion

		#region networking

		public override void OnReceiveData(byte sender, byte[] data)
		{
			if (_isHost)
			{	// receive input from client and calculate new position
				byte[] movementXBytes = new byte[FLOAT_LENGTH];
				Array.Copy(data, FLOAT_LENGTH * 0, movementXBytes, 0, FLOAT_LENGTH);
				float movementX = BitConverter.ToSingle(movementXBytes);

				byte[] movementYBytes = new byte[FLOAT_LENGTH];
				Array.Copy(data, FLOAT_LENGTH * 1, movementYBytes, 0, FLOAT_LENGTH);
				float movementY = BitConverter.ToSingle(movementYBytes);

				CalculateTransform(new(movementX, movementY), false);
			}
			else
			{   // receive new position for host, client and puck from host
				{	// update client position
					byte[] positionXBytes = new byte[FLOAT_LENGTH];
					Array.Copy(data, FLOAT_LENGTH * 0, positionXBytes, 0, FLOAT_LENGTH);
					float positionX = BitConverter.ToSingle(positionXBytes);

					byte[] positionZBytes = new byte[FLOAT_LENGTH];
					Array.Copy(data, FLOAT_LENGTH * 1, positionZBytes, 0, FLOAT_LENGTH);
					float positionZ = BitConverter.ToSingle(positionZBytes);

					_localPlayer.transform.localPosition = new(positionX, _localPlayer.transform.position.y, positionZ);
				}

				if (_remotePlayer != null)
				{   // update host position
					byte[] positionXBytes = new byte[FLOAT_LENGTH];
					Array.Copy(data, FLOAT_LENGTH * 2, positionXBytes, 0, FLOAT_LENGTH);
					float positionX = BitConverter.ToSingle(positionXBytes);

					byte[] positionZBytes = new byte[FLOAT_LENGTH];
					Array.Copy(data, FLOAT_LENGTH * 3, positionZBytes, 0, FLOAT_LENGTH);
					float positionZ = BitConverter.ToSingle(positionZBytes);

					_remotePlayer.transform.localPosition = new(positionX, _remotePlayer.transform.position.y, positionZ);
				}

				if (_currentPuck != null)
				{	// update puck position
					byte[] positionXBytes = new byte[FLOAT_LENGTH];
					Array.Copy(data, FLOAT_LENGTH * 4, positionXBytes, 0, FLOAT_LENGTH);
					float positionX = BitConverter.ToSingle(positionXBytes);

					byte[] positionZBytes = new byte[FLOAT_LENGTH];
					Array.Copy(data, FLOAT_LENGTH * 5, positionZBytes, 0, FLOAT_LENGTH);
					float positionZ = BitConverter.ToSingle(positionZBytes);

					_currentPuck.transform.localPosition = new(positionX, _currentPuck.transform.position.y, positionZ);
				}
			}
		}

		private void CalculateTransform(Vector2 movementInput, bool isLocal = true)
		{
			if (!_isHost)
			{   // save directional input as client
				_clientsDirectionalInput = movementInput;
			}
			else
			{	// calculate position of current player as host
				Rigidbody player = isLocal ? _localPlayer : _remotePlayer;
				Vector3 movement = new(movementInput.x, player.transform.position.y, movementInput.y);
				Vector3 newPosition = movement;
				Vector3 direction = newPosition - player.transform.position;
				float delta = Vector3.Distance(player.transform.position, newPosition);

				if (!Physics.Raycast(player.transform.position, direction, delta, _layerMask))
					player.MovePosition(newPosition);
			}
		}

		private void SendInputToHost()
		{	// update clients directional input on host
			byte[] data = new byte[FLOAT_LENGTH * 2];
			Array.Copy(BitConverter.GetBytes(_clientsDirectionalInput.x), 0, data, FLOAT_LENGTH * 0, FLOAT_LENGTH);
			Array.Copy(BitConverter.GetBytes(_clientsDirectionalInput.y), 0, data, FLOAT_LENGTH * 1, FLOAT_LENGTH);
			SendData(data);
		}

		private void SendTransformsToClient()
		{	// update position of host, client and puck on client
			byte[] data = new byte[FLOAT_LENGTH * 6];
			if (_remotePlayer != null)
			{
				Array.Copy(BitConverter.GetBytes(_remotePlayer.transform.localPosition.x), 0, data, FLOAT_LENGTH * 0, FLOAT_LENGTH);
				Array.Copy(BitConverter.GetBytes(_remotePlayer.transform.localPosition.z), 0, data, FLOAT_LENGTH * 1, FLOAT_LENGTH);
			}

			{
				Array.Copy(BitConverter.GetBytes(_localPlayer.transform.localPosition.x),  0, data, FLOAT_LENGTH * 2, FLOAT_LENGTH);
				Array.Copy(BitConverter.GetBytes(_localPlayer.transform.localPosition.z),  0, data, FLOAT_LENGTH * 3, FLOAT_LENGTH);
			}

			if (_currentPuck != null)
			{
				Array.Copy(BitConverter.GetBytes(_currentPuck.transform.localPosition.x),  0, data, FLOAT_LENGTH * 4, FLOAT_LENGTH);
				Array.Copy(BitConverter.GetBytes(_currentPuck.transform.localPosition.z),  0, data, FLOAT_LENGTH * 5, FLOAT_LENGTH);
			}
			SendData(data);
		}

		#endregion
	}
}
