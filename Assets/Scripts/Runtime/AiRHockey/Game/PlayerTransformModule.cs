using System;
using UnityEngine;
using CENTIS.UnityModuledNet.Modules;
using HTW.AiRHockey.Settings;

namespace HTW.AiRHockey.Game
{
	public class PlayerTransformModule : UnreliableModule
	{
		#region properties

		public override string ModuleID => "PlayerTransformModule";

		#endregion

		#region fields

		private readonly bool _isHost;

		private readonly int _layerMask;

		private Rigidbody	_currentPuck;
		private Rigidbody	_localPlayer;
		private Rigidbody	_remotePlayer;

		private float _time = 0;
		
		private Vector2 _clientsDirectionalInput		= new();
		private Vector2 _clientsLastDirectionalInput	= new();

		private GameSettings _gameSettings = InstanceFinder.GameManager.GameSettings;

		private const int FLOAT_LENGTH	= 4;

		#endregion

		#region lifecycle

		public PlayerTransformModule(bool isHost)
		{
			_isHost = isHost;
			_layerMask = (1 << LayerMask.NameToLayer("Barrier")) 
				| (1 << LayerMask.NameToLayer("Player Barrier")) 
				| (1 << LayerMask.NameToLayer("Puck"));

			Vector3 position = _isHost ? _gameSettings.InitialPositionHost : _gameSettings.InitialPositionClient;
			_localPlayer = GameObject.Instantiate(_gameSettings.PlayerPrefab, position, Quaternion.identity).GetComponent<Rigidbody>();
			_localPlayer.gameObject.name = "LocalPlayer";
		}

		public override void Update()
		{
			base.Update();

			if (_remotePlayer == null)
				return;

			_time += Time.deltaTime;
			if (_time >= (float)(1 / (float)_gameSettings.TransformSendHertz))
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
			Vector3 position = _isHost ? _gameSettings.InitialPositionClient : _gameSettings.InitialPositionHost;
			_remotePlayer = GameObject.Instantiate(_gameSettings.PlayerPrefab, position, Quaternion.identity).GetComponent<Rigidbody>();
			_remotePlayer.gameObject.name = "RemotePlayer";
		}

		public void DestroyRemotePlayer()
		{
			GameObject.Destroy(_remotePlayer.gameObject);
			_remotePlayer = null;
		}

		public void ResetPlayers(bool reinstantiatePuck = true)
		{
			if (_currentPuck != null)
				GameObject.Destroy(_currentPuck.gameObject);

			if (reinstantiatePuck)
			{
				_currentPuck = GameObject.Instantiate(_gameSettings.PuckPrefab, _gameSettings.InitialPuckPosition, Quaternion.identity).GetComponent<Rigidbody>();
				if (!_isHost) _currentPuck.isKinematic = true;
			}
			
			if (!_isHost)
				return;
			
			_localPlayer.position = _isHost ? _gameSettings.InitialPositionHost : _gameSettings.InitialPositionClient;
			if (_remotePlayer != null)
				_remotePlayer.position = _isHost ? _gameSettings.InitialPositionClient : _gameSettings.InitialPositionHost;
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

					_localPlayer.MovePosition(new(positionX, _localPlayer.position.y, positionZ));
				}

				if (_remotePlayer != null)
				{   // update host position
					byte[] positionXBytes = new byte[FLOAT_LENGTH];
					Array.Copy(data, FLOAT_LENGTH * 2, positionXBytes, 0, FLOAT_LENGTH);
					float positionX = BitConverter.ToSingle(positionXBytes);

					byte[] positionZBytes = new byte[FLOAT_LENGTH];
					Array.Copy(data, FLOAT_LENGTH * 3, positionZBytes, 0, FLOAT_LENGTH);
					float positionZ = BitConverter.ToSingle(positionZBytes);

					_remotePlayer.MovePosition(new(positionX, _remotePlayer.position.y, positionZ));
				}

				if (_currentPuck != null)
				{	// update puck position
					byte[] positionXBytes = new byte[FLOAT_LENGTH];
					Array.Copy(data, FLOAT_LENGTH * 4, positionXBytes, 0, FLOAT_LENGTH);
					float positionX = BitConverter.ToSingle(positionXBytes);

					byte[] positionZBytes = new byte[FLOAT_LENGTH];
					Array.Copy(data, FLOAT_LENGTH * 5, positionZBytes, 0, FLOAT_LENGTH);
					float positionZ = BitConverter.ToSingle(positionZBytes);

					_currentPuck.MovePosition(new(positionX, _currentPuck.position.y, positionZ));
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
				Vector3 movement = new(movementInput.x, 0, movementInput.y);
				Vector3 newPosition = player.transform.position + _gameSettings.PlayerSpeed * Time.fixedDeltaTime * movement;
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
			_clientsLastDirectionalInput = _clientsDirectionalInput;
		}

		private void SendTransformsToClient()
		{	// update position of host, client and puck on client
			byte[] data = new byte[FLOAT_LENGTH * 6];
			if (_remotePlayer != null)
			{
				Array.Copy(BitConverter.GetBytes(_remotePlayer.position.x), 0, data, FLOAT_LENGTH * 0, FLOAT_LENGTH);
				Array.Copy(BitConverter.GetBytes(_remotePlayer.position.z), 0, data, FLOAT_LENGTH * 1, FLOAT_LENGTH);
			}

			{
				Array.Copy(BitConverter.GetBytes(_localPlayer.position.x),  0, data, FLOAT_LENGTH * 2, FLOAT_LENGTH);
				Array.Copy(BitConverter.GetBytes(_localPlayer.position.z),  0, data, FLOAT_LENGTH * 3, FLOAT_LENGTH);
			}

			if (_currentPuck != null)
			{
				Array.Copy(BitConverter.GetBytes(_currentPuck.position.x),  0, data, FLOAT_LENGTH * 4, FLOAT_LENGTH);
				Array.Copy(BitConverter.GetBytes(_currentPuck.position.z),  0, data, FLOAT_LENGTH * 5, FLOAT_LENGTH);
			}
			SendData(data);
		}

		#endregion
	}
}
