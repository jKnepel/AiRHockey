using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HTW.AiRHockey.Game
{
    public class Goal : MonoBehaviour
    {
        [SerializeField] private Player _player;

        public Player Player => _player;
    }
}