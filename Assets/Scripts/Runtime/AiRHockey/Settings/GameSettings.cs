using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HTW.AiRHockey.Game;

namespace HTW.AiRHockey.Settings
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/GameSettingsScriptableObject", order = 1)]
    public class GameSettings : ScriptableObject
    {
        public int DecidingScore = 20;

        public float PlayerSpeed = 20f;

        public int TransformSendHertz = 60;

        public GameObject   PlayerPrefab;
        public Puck         PuckPrefab;

        public Vector3 InitialPositionHost   = new();
        public Vector3 InitialPositionClient   = new();
        public Vector3 InitialPuckPosition      = new();
    }
}
