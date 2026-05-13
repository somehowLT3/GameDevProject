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
            // easy
            case 0:
                GameSettings.turretChance = 0.1f;
                GameSettings.segments = 40;
                break;

            // meduim
            case 1:
                GameSettings.turretChance = 0.25f;
                GameSettings.segments = 60;
                break;

            // hard
            case 2:
                GameSettings.turretChance = 0.5f;
                GameSettings.segments = 80;
                break;
        }

        SceneManager.LoadScene("Level");
    }
}