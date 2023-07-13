using System;
using Microsoft.MixedReality.GraphicsTools;
using Microsoft.MixedReality.Toolkit;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace HTW.AiRHockey.Game
{
    [RequireComponent(typeof(StatefulInteractable))]
    public class ARPlayerController : MonoBehaviour
    {
        [SerializeField] private InputActionProperty _leftControllerPosition;
        [SerializeField] private InputActionProperty _rightControllerPosition;
        [SerializeField] private StatefulInteractable _interactable;

        private Vector3 _lastPosition;
        private MeshOutline _outline;

        private void Awake()
        {
            if (_interactable == null)
                _interactable = GetComponent<StatefulInteractable>();
            if (_outline == null)
                _outline = GetComponent<MeshOutline>();
        }

        private void OnEnable()
        {
            _interactable.hoverEntered.AddListener(ActivateOutline);
            _interactable.hoverExited.AddListener(DeactivateOutline);
        }

        private void OnDisable()
        {
            _interactable.hoverEntered.RemoveListener(ActivateOutline);
            _interactable.hoverExited.RemoveListener(DeactivateOutline);
        }

        private void Update()
        {
            InputActionProperty positionalProperty = InstanceFinder.GameSettings.IsRightHanded ? _rightControllerPosition : _leftControllerPosition;
            if (InstanceFinder.GameManager.IsGameStarted && _interactable.IsToggled && positionalProperty.action != null)
            {
                Vector3 position = positionalProperty.action.ReadValue<Vector3>();
                if (position.Equals(Vector3.zero))
                    return;

                Vector2 offset = new(position.x - _lastPosition.x, position.z - _lastPosition.z);
                InstanceFinder.GameManager.UpdatePlayerTransform(offset);
                _lastPosition = position;
            }
        }

        /// <summary>
        /// Method called when Object is selected via Controller interaction 
        /// </summary>
        /// <param name="Single"></param>
        public void IsSelected(Single single)
        {
            InstanceFinder.GameManager.ReadyUp();
            DeactivateOutline(null);
            _interactable.hoverEntered.RemoveListener(ActivateOutline);
            _interactable.hoverExited.RemoveListener(DeactivateOutline);
            Debug.Log("Is Selected:" + single);

        }

        /// <summary>
        /// Method called when Object is deselected via Controller interaction 
        /// </summary>
        /// <param name="Single"></param>
        public void IsDeselected(Single single)
        {
            InstanceFinder.GameManager.Unready();
            _interactable.hoverEntered.AddListener(ActivateOutline);
            _interactable.hoverExited.AddListener(DeactivateOutline);
            Debug.Log("Is Deselected: " + single);

        }

        private void ActivateOutline(HoverEnterEventArgs args) {
            _outline.enabled = true;
        }

        private void DeactivateOutline(HoverExitEventArgs args)
        {
            _outline.enabled = false;
        }

    }
}
