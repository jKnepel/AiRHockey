using System;
using Microsoft.MixedReality.Toolkit;
using UnityEngine;
using UnityEngine.InputSystem;
using HTW.AiRHockey.Outline;

namespace HTW.AiRHockey.Game
{
    [RequireComponent(typeof(Player), typeof(StatefulInteractable), typeof(OutlineObject))]
    public class ARPlayerController : MonoBehaviour
    {
        [SerializeField] private InputActionProperty    _leftControllerPosition;
        [SerializeField] private InputActionProperty    _rightControllerPosition;

        [SerializeField] private Player                 _player;
        [SerializeField] private StatefulInteractable   _interactable;
        [SerializeField] private OutlineObject          _outline;

        private bool _isSelected;

        private void Awake()
        {
            if (_player == null)
                _player = GetComponent<Player>();
            if (_interactable == null)
                _interactable = GetComponent<StatefulInteractable>();
            if (_outline == null)
                _outline = GetComponent<OutlineObject>();
        }

        private void OnEnable()
        {
            GameManagerEvents.OnGamePaused += DeactivateManipulator;
            GameManagerEvents.OnGameResumed += ActivateManipulator;
        }

        private void OnDisable()
        {
            GameManagerEvents.OnGamePaused -= DeactivateManipulator;
            GameManagerEvents.OnGameResumed -= ActivateManipulator;
        }

        private void Update()
        {
            InputActionProperty positionalProperty = InstanceFinder.GameSettings.IsRightHanded ? _rightControllerPosition : _leftControllerPosition;
            if (_player.IsLocalPlayer && _isSelected && InstanceFinder.GameManager.IsGameStarted && positionalProperty.action != null)
            {
                Vector3 position = positionalProperty.action.ReadValue<Vector3>();
                InstanceFinder.GameManager.UpdatePlayerTransform(new(position.x, position.z));
            }
        }

        /// <summary>
        /// Method called when Object is selected via Controller interaction 
        /// </summary>
        /// <param name="Single"></param>
        public void IsSelected(Single single)
        {
            InstanceFinder.GameManager.ReadyUp();
            _outline.ActivateOutlines();
            _isSelected = true;
            Debug.Log("Is Selected");

        }

        /// <summary>
        /// Method called when Object is deselected via Controller interaction 
        /// </summary>
        /// <param name="Single"></param>
        public void IsDeselected(Single single)
        {
            InstanceFinder.GameManager.Unready();
            _isSelected = false;
            _outline.DeactivateOutlines();
        }

        public void ActivateOutlines(Single single)
		{ 
            _outline.ActivateOutlines();
		}

        public void DeactivateOutlines(Single single)
		{
            if (!_isSelected)
                _outline.DeactivateOutlines();
		}

        private void ActivateManipulator()
        {
            _interactable.enabled = true;
        }

        private void DeactivateManipulator()
        {
            _interactable.enabled = false;
        }
    }
}
