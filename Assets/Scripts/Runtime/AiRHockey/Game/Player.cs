using UnityEngine;

namespace HTW.AiRHockey.Game
{
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class Player : MonoBehaviour
    {
        public bool IsLocalPlayer;
        
        [SerializeField] private Rigidbody _rigidbody;
        public Rigidbody Rigidbody => _rigidbody;

        [SerializeField] private CapsuleCollider _collider;
        public CapsuleCollider Collider => _collider;

		private void Awake()
		{
            if (Rigidbody == null)
                _rigidbody = GetComponent<Rigidbody>();
            if (Collider == null)
                _collider = GetComponent<CapsuleCollider>();
		}
	}
}
