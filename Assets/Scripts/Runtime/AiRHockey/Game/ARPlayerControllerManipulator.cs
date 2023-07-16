using HTW.AiRHockey.Outline;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SpatialManipulation;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace HTW.AiRHockey.Game
{
    [RequireComponent(typeof(Player), typeof(StatefulInteractable), typeof(OutlineObject))]
    public class ARPlayerControllerManipulator : MonoBehaviour
    {


        [SerializeField] private Player _player;
        [SerializeField] private ObjectManipulator _interactable;
        [SerializeField] private OutlineObject _outline;

        private void Awake()
        {
            if (_player == null)
                _player = GetComponent<Player>();
            if (_interactable == null)
                _interactable = GetComponent<ObjectManipulator>();
            if (_outline == null)
                _outline = GetComponent<OutlineObject>();

        }

        private void OnEnable()
        {
            GameManagerEvents.OnGameStart += ActivateManipulator;
            GameManagerEvents.OnGameEnd += DeactivateManipulator;
            GameManagerEvents.OnGamePaused += DeactivateManipulator;
            GameManagerEvents.OnGameResumed += ActivateManipulator;
        }

        private void OnDisable()
        {
            GameManagerEvents.OnGameStart -= ActivateManipulator;
            GameManagerEvents.OnGameEnd -= DeactivateManipulator;
            GameManagerEvents.OnGamePaused -= DeactivateManipulator;
            GameManagerEvents.OnGameResumed -= ActivateManipulator;
        }

        /// <summary>
        /// Method called when Object is selected via Controller interaction 
        /// </summary>
        /// <param name="Single"></param>
        public void IsSelected(SelectEnterEventArgs evt)
        {
            InstanceFinder.GameManager.ReadyUp();
            _outline.DisableOutlines(true);
        }

        /// <summary>
        /// Method called when Object is deselected via Controller interaction 
        /// </summary>
        /// <param name="Single"></param>
        public void IsDeselected(SelectExitEventArgs evt)
        {
            _outline.DisableOutlines(false);

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
