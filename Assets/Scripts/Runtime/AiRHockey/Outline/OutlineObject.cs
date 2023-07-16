using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HTW.AiRHockey.Outline
{
    public class OutlineObject : MonoBehaviour
    {
        private int defaultLayer = 0;
        private int outlineLayer = 12;
        public bool disabled = false;

        private void Start()
        {
            defaultLayer = gameObject.layer;
        }
        public void DisableOutlines(bool disabled)
        {
            DeactivateOutlines();
            this.disabled = disabled;
        }

        public void ActivateOutlines()
        {
            if(!disabled)
            {
                defaultLayer = gameObject.layer;
                gameObject.layer = outlineLayer;
            }
        }

        public void DeactivateOutlines()
        {
            if(!disabled)
            {
                gameObject.layer = defaultLayer;
            }
        }

        private void OnDisable()
        {
            DeactivateOutlines();
        }
    }
}
