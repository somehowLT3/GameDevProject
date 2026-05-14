using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject failPanel;
    public GameObject successPanel;

    [Header("Death Timing")]
    public float slowMotionScale = 0.2f;
    public float failScreenDelay = 1f;
    public float restoreTimeDelay = 1f;

    public void ShowFailScreen()
    {
        StartCoroutine(FailSequence());
    }

    public void ShowSuccessScreen()
    {
        successPanel.SetActive(true);
    }

    System.Collections.IEnumerator FailSequence()
    {
        // slow motion
        Time.timeScale = slowMotionScale;

        // delay ui
        yield return new WaitForSecondsRealtime(failScreenDelay);

        failPanel.SetActive(true);

        yield return new WaitForSecondsRealtime(restoreTimeDelay);

        Time.timeScale = 1f;
    }

    public void Retry()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }
}