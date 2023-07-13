using UnityEngine;

namespace HTW.AiRHockey.Game
{
    [RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
    public class Player : MonoBehaviour
    {
        public bool IsLocalPlayer;
        
        [SerializeField] private Rigidbody _rigidbody;
        public Rigidbody Rigidbody { get => _rigidbody; private set => _rigidbody = value; }

        [SerializeField] private CapsuleCollider _collider;
        public CapsuleCollider Collider { get => _collider; private set => _collider = value; }

		private void Awake()
		{
            if (Rigidbody == null)
                Rigidbody = GetComponent<Rigidbody>();
            if (Collider == null)
                Collider = GetComponent<CapsuleCollider>();
		}
	}
}
