using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
}
