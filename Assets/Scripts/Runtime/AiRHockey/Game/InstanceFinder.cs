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
                    GameObject.Destroy(value.gameObject);
                else
                    _gameManager = value;
			}
		}
    }
}
