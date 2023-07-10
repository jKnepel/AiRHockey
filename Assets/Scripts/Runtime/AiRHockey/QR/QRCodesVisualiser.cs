using System;
using System.Collections.Concurrent;
using Microsoft.MixedReality.QR;
using UnityEngine;
using HTW.AiRHockey.Game;

namespace HTW.AiRHockey.QR
{
    public class QRCodesVisualiser : MonoBehaviour
    {
		#region attributes

		[SerializeField] private SpatialGraphNodeTracker _nodeTracker;
        [SerializeField] private string _content = string.Empty;

        private readonly ConcurrentQueue<Action> _actions = new();

		#endregion

		#region lifecycle

		private void Start()
        {
            InstanceFinder.QRCodesManager.QRCodesTrackingStateChanged += Instance_QRCodesTrackingStateChanged;
            InstanceFinder.QRCodesManager.QRCodeAdded += Instance_QRCodeAdded;
            InstanceFinder.QRCodesManager.QRCodeUpdated += Instance_QRCodeUpdated;
            InstanceFinder.QRCodesManager.QRCodeRemoved += Instance_QRCodeRemoved;
        }

		private void Update()
		{
			while (_actions.Count > 0)
			{
                if (_actions.TryDequeue(out Action action))
                    action.Invoke();
			}
		}

		#endregion

		#region private methods

		private void Instance_QRCodesTrackingStateChanged(object sender, bool status)
        {
            if (!status)
            {
                _actions.Enqueue(() => _nodeTracker.gameObject.SetActive(false));
            }
        }

        private void Instance_QRCodeAdded(object sender, QRCodeEventArgs<QRCode> e)
        {
            if (string.IsNullOrEmpty(_content) || !e.Data.Data.Equals(_content)) return;

            _actions.Enqueue(() => UpdateNodeTracker(e.Data.SpatialGraphNodeId, e.Data.PhysicalSideLength));
        }

        private void Instance_QRCodeUpdated(object sender, QRCodeEventArgs<QRCode> e)
        {
            if (string.IsNullOrEmpty(_content) || !e.Data.Data.Equals(_content)) return;

            _actions.Enqueue(() => UpdateNodeTracker(e.Data.SpatialGraphNodeId, e.Data.PhysicalSideLength));
        }

        private void Instance_QRCodeRemoved(object sender, QRCodeEventArgs<QRCode> e)
        {
            if (string.IsNullOrEmpty(_content) || !e.Data.Data.Equals(_content)) return;

            _actions.Enqueue(() => _nodeTracker.gameObject.SetActive(false));
        }

        private void UpdateNodeTracker(Guid id, float sideLength)
		{
            _nodeTracker.Id = id;
            _nodeTracker.PhysicalSideLength = sideLength;
            if (!_nodeTracker.gameObject.activeSelf)
                _nodeTracker.gameObject.SetActive(true);
		}
    }

	#endregion
}
