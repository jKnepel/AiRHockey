using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HTW.AiRHockey.Settings
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/GameSettingsScriptableObject", order = 1)]
    public class GameSettings : ScriptableObject
    {
        public int DecidingScore = 20;

        public float PlayerSpeed = 20f;
        public GameObject PlayerPrefab;
        public Vector3 InitialPositionPlayer1 = new();
        public Vector3 InitialPositionPlayer2 = new();

        // volume

        // keybinds
        public KeyCode ResetPositions = KeyCode.Backspace;

        // speed
    }
}
