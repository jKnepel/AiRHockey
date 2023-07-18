using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HTW.AiRHockey.QR;
using HTW.AiRHockey.Settings;

namespace HTW.AiRHockey.Game
{
    public static class InstanceFinder
    {
        private static GameManager _gameManager;
        public static GameManager GameManager
		{
            get => _gameManager;
            set
			{
                if (_gameManager != null)
				{
 #if UNITY_EDITOR
                    GameObject.DestroyImmediate(_gameManager.gameObject);
#else
                    GameObject.Destroy(_gameManager.gameObject);
#endif
				}
                
                _gameManager = value;
			}
		}

        private static QRCodesManager _qrManager;
        public static QRCodesManager QRCodesManager
		{
            get => _qrManager;
            set
			{
                if (_qrManager != null)
                {
#if UNITY_EDITOR
                    GameObject.DestroyImmediate(_qrManager.gameObject);
#else
                    GameObject.Destroy(_qrManager.gameObject);
#endif
                }

                _qrManager = value;
            }
		}

        private static SpatialGraphNodeTracker _nodeTracker;
        public static SpatialGraphNodeTracker NodeTracker
		{
            get => _nodeTracker;
            set => _nodeTracker = value;
		}

        public static GameSettings GameSettings => _gameManager.GameSettings;
    }
}
