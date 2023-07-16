using HTW.AiRHockey.Game;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HTW.AiRHockey.UI
{
    public class TableText : MonoBehaviour
    {
        [SerializeField]
        private TextMeshPro textMesh;
        private void Start()
        {
            if(textMesh == null)
            {
                textMesh = transform.GetChild(0).GetComponent<TextMeshPro>();
            }
            if(InstanceFinder.GameManager.IsHost)
            {
                textMesh.transform.rotation = Quaternion.Euler(0, 0, 0);
            } else
            {
                textMesh.transform.rotation = Quaternion.Euler(0, 180, 0);
            }

        }

        private void OnEnable()
        {
            GameManagerEvents.OnGameStart += HideText;
            GameManagerEvents.OnGameEnd += DisplayText;
        }

        private void OnDisable()
        {
            GameManagerEvents.OnGameStart -= HideText;
            GameManagerEvents.OnGameEnd -= DisplayText;
        }

        // Update is called once per frame
        public void HideText()
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }

        public void DisplayText()
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }

}
