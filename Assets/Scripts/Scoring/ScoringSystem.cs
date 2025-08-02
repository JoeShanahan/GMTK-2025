using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace Gmtk2025
{
    public class ScoringSystem : MonoBehaviour
    {
        public static int scoreTotal = 0;
        void Start()
        {
            scoreTotal = 0;
        }

        public static void IncreaseScore(int scoreAdd)
        {
            scoreTotal += scoreAdd;
            Debug.Log("New Score: " + scoreTotal);
            // update on-screen text to add score
        }

        public static void ResetScore()
        {
            scoreTotal = 0;
        }

        public void UpdateScoreVisuals()
        {
            // update text on screen with total amount of scoring objects
        }
    }
}
