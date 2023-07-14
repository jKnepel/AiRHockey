using CENTIS.UnityModuledNet.Managing;
using UnityEngine;

namespace HTW.AiRHockey.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject _current;
        [SerializeField] private GameObject _handMenu;
        [SerializeField] private GameObject _mainMenu;

        private GameObject _previous;

        private void OnEnable()
        {
            ModuledNetManager.OnConnected += MenuOnConnect;
            ModuledNetManager.OnDisconnected += MenuOnDisconnect;
        }

        private void OnDisable()
        {
            ModuledNetManager.OnConnected -= MenuOnConnect;
            ModuledNetManager.OnDisconnected -= MenuOnDisconnect;
        }

        public void TransitionTo(GameObject to)
        {
            _current.SetActive(false);
            _previous = _current;
            _current = to;
            to.SetActive(true);
        }

        public void ReturnToPrevious()
        {
            TransitionTo(_previous);
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
         	Application.Quit();
#endif
        }

        private void MenuOnConnect()
        {
            _handMenu.gameObject.SetActive(true);
            _mainMenu.SetActive(false);
        }

        private void MenuOnDisconnect()
        {
            _handMenu.gameObject.SetActive(false);
            _current.SetActive(false);
            _current = _mainMenu.transform.GetChild(0).gameObject;
            _current.SetActive(true);
            _mainMenu.SetActive(true);
        }
    }
}
