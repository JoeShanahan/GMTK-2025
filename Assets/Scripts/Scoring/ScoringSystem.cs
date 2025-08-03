using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;

namespace Gmtk2025
{
    public class ScoringSystem : MonoBehaviour
    {
        public static ScoringSystem Instance;
        void Awake()
        {
            if (Instance == null && Instance != this)
                Instance = this;
            else
            Destroy(gameObject);
        }

        [SerializeField] private int scoreTotal = 0;
        [SerializeField] private int scoreTally = 0;

        [SerializeField] private TextMeshProUGUI scoreCounter;

        void Start()
        {
            scoreTotal = 0;
            scoreTally = 0;
        }

        public void IncreaseScore(int scoreAdd)
        {
            scoreTally += scoreAdd;
            //Debug.Log("New Score: " + scoreTally);
            
            scoreCounter.text = "Score: " + scoreTally + " / " + scoreTotal;
        }

        public void IncreaseTotal()
        {
            scoreTotal++;
            Debug.Log("Score Total: " + scoreTotal);

            scoreCounter.text = "Score: " + scoreTally + " / " + scoreTotal;
        }

        public void DecreaseTotal() // this can't be called when the object is destroyed as it'll trigger when collecting the score.. Tie to the delete function?
        {
            scoreTotal--;
            Debug.Log("Score Total: " + scoreTotal);

            scoreCounter.text = "Score: " + scoreTally + " / " + scoreTotal;
        }

        public void ResetScore()
        {
            scoreTally = 0;
        }

        public void UpdateScoreVisuals()
        {

        }
    }
}
