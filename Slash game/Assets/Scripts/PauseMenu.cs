using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;

    [SerializeField] private GameObject pauseMenuUI;

    [SerializeField] private Text countdownToResume;
    private float countdownValue;
    [SerializeField] private float countdownValueOriginal;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseButton();
        }

        //if (isCountingDown)
        //{
        //    countdownToResume.text = countdownValue.ToString("0");

        //    if (countdownValue <= 0)
        //    {
        //        isCountingDown = false;
        //        Resume();
        //    }
        //}

    }

    public void PauseButton()
    {
        if (gameIsPaused)
        {
            StartCoroutine(StartCountDown());
        }
        else
        {
            Pause();
        }
    }

    public void ReloadSceneButton()
    {
        SceneManager.LoadScene("Gameplay");
        Resume();
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
        Resume();
    }

    void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
        countdownToResume.gameObject.SetActive(false);
    }
    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    IEnumerator StartCountDown()
    {
        countdownValue = countdownValueOriginal;
        pauseMenuUI.SetActive(false);
        countdownToResume.gameObject.SetActive(true);
        countdownToResume.text = countdownValue.ToString("0");
        countdownValue--;
        yield return new WaitForSecondsRealtime(1f);
        countdownToResume.text = countdownValue.ToString("0");
        countdownValue--;
        yield return new WaitForSecondsRealtime(1f);
        countdownToResume.text = countdownValue.ToString("0");
        countdownValue--;
        yield return new WaitForSecondsRealtime(1f);
        countdownToResume.text = countdownValue.ToString("0");
        countdownValue--;
        yield return new WaitForSecondsRealtime(0.1f);
        Resume();
    }
}
