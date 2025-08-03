using UnityEngine;
using UnityEngine.UI;

namespace Gmtk2025
{
    public class GameEditorUI : MonoBehaviour
    {
        [SerializeField] private Text _scoreText;

        public void UpdateScore(int cur, int tot)
        {
            _scoreText.text = $"{cur}/{tot}";
        }
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
