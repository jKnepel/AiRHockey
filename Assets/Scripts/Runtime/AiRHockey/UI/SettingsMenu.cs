using UnityEngine;
using Microsoft.MixedReality.Toolkit.UX;
using HTW.AiRHockey.Game;

namespace HTW.AiRHockey.UI
{
    public class SettingsMenu : MonoBehaviour
    {
        private const int MAX_ARENA_SIZE = 19;
        private float _volume;
        private bool _muted;

        public void OnArenaSizeChange(SliderEventData data)
        {
            if (InstanceFinder.GameManager)
            {
                InstanceFinder.GameManager.GameSettings.ArenaSizeMultiplier = 1 + (int)(MAX_ARENA_SIZE * data.NewValue);
            }
        }

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
