using LudumDare55;
using UnityEngine;

namespace Gmtk2025
{
    public class TitleScreenManager : MonoBehaviour
    {
        [SerializeField] private RectTransform _quitButton;
        
        public void ButtonPressPlay()
        {
            TransitionManager.Instance.GoToGame();
        }

        public void ButtonPressEdit()
        {
            TransitionManager.Instance.GoToEditor();
        }

        public void ButtonPressQuit()
        {
            Application.Quit();
        }
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Application.targetFrameRate = 60;

            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                _quitButton.gameObject.SetActive(false);
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
