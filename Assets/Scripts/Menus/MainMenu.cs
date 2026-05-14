using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public TMP_Dropdown difficultyDropdown;
    public Toggle sportsToggle;

    public void StartGame()
    {
        switch (difficultyDropdown.value)
        {
            case 0:
                GameSettings.SetDifficultyEasy();
                break;

            case 1:
                GameSettings.SetDifficultyMedium();
                break;

            case 2:
                GameSettings.SetDifficultyHard();
                break;
        }

        SceneManager.LoadScene("Level");
    }

    void Start()
    {
        sportsToggle.isOn = GameSettings.useSportsCar;

        if (GameSettings.hardCompletions > 0)
        {
            sportsToggle.interactable = true;
            return;
        }
        sportsToggle.interactable = false;
    }

    public void ToggleSportsCar(bool value)
    {
        GameSettings.useSportsCar = value;
    }
}