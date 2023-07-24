using UnityEngine;

namespace HTW.AiRHockey.Game
{
    public class Arena : MonoBehaviour
    {
        [SerializeField] private Transform _hostSpawn;
		public Transform HostSpawn => _hostSpawn;

        [SerializeField] private Transform _clientSpawn;
		public Transform ClientSpawn => _clientSpawn;

        [SerializeField] private Transform _puckSpawn;
		public Transform PuckSpawn => _puckSpawn;
	}
}
