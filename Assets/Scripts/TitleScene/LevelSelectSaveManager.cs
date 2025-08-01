using UnityEngine;
using TMPro;

public class LevelSelectSaveManager : MonoBehaviour {
    [SerializeField] private int numberOfLevels;
    [SerializeField] private GameObject levelSelectButtonPrefab;
    [SerializeField] private Transform gridLayoutGroupParent;

    private void Start() {
        GenerateLevelButtons();
    }

    private void GenerateLevelButtons() {
        for (int i = 0;i < numberOfLevels;i++) {
            GameObject buttonInstance = Instantiate(levelSelectButtonPrefab, gridLayoutGroupParent);
            TextMeshProUGUI textComponent = buttonInstance.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null) {
                textComponent.text = (i + 1).ToString();
            }
        }
    }
}