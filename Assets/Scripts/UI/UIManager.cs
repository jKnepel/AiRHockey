using UnityEngine;

namespace HTW.AiRHockey.UI
{
    public class UIManager : MonoBehaviour
    {
        private GameObject _current;
        private GameObject _previous;

        private void Start()
        {
            _current = GameObject.FindObjectOfType<Canvas>().gameObject;
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
    }
}
