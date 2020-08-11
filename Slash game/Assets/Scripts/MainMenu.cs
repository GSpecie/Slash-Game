using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Text scoreText;
    [SerializeField] private Text highScoreText;

    private void Start()
    {
        if (ScoreGameManager.GetMatchScore() != 0)
        {
            //scoreText.gameObject.SetActive(true);
            scoreText.text = "Last Try: " + ScoreGameManager.GetMatchScore().ToString("0");
        }
        else scoreText.gameObject.SetActive(false);

        if (ScoreGameManager.GetHighScore() != 0)
        {
            //scoreText.gameObject.SetActive(true);
            highScoreText.text = "Highscore: " + ScoreGameManager.GetHighScore().ToString("0");
        }
        else highScoreText.gameObject.SetActive(false);
    }

    public void LoadGameplay()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
}
