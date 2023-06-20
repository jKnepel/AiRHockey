using UnityEngine;
using Microsoft.MixedReality.Toolkit.UX;

namespace HTW.AiRHockey.UI
{
    public class SettingsMenu : MonoBehaviour
    {
        private float _volume;
        private bool _muted;

        public void OnVolumeChange(SliderEventData data)
        {
            _volume = data.NewValue;
            if (!_muted)
            {
                AudioListener.volume = _volume;
            }
        }

        public void OnMute()
        {
            _muted = !_muted;
            if (_muted)
            {
                AudioListener.volume = 0;
            }
            else
            {
                AudioListener.volume = _volume;
            }
        }
    }
}
