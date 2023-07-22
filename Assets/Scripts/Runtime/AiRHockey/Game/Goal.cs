using UnityEngine;

namespace HTW.AiRHockey.Game
{
    public class Goal : MonoBehaviour
    {
        [SerializeField] private bool _player;
        public bool Player => _player;
    }
}
