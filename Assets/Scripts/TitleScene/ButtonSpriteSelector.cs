using UnityEngine;
using UnityEngine.UI;

public class ButtonSpriteSelector : MonoBehaviour {
    [SerializeField] private Button[] buttons;
    [SerializeField] private Sprite[] whiteSprites;
    [SerializeField] private Sprite[] yellowSprites;
    [SerializeField] private Sprite[] blackSprites;

    private int currentThemeIndex = 0;

    public void CycleTheme() {
        currentThemeIndex = (currentThemeIndex + 1) % 3;
        ApplyTheme(currentThemeIndex);
    }

    private void ApplyTheme(int themeIndex) {
        Sprite[] selectedSprites = themeIndex switch
        {
            0 => whiteSprites,
            1 => yellowSprites,
            2 => blackSprites,
            _ => whiteSprites
        };

        for (int i = 0;i < buttons.Length && i < selectedSprites.Length;i++) {
            buttons[i].image.sprite = selectedSprites[i];
        }
    }
}