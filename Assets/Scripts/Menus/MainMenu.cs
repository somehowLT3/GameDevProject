using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TMP_Dropdown difficultyDropdown;

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
}