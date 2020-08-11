using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ContinueMenu : MonoBehaviour
{
    [SerializeField] private GameObject continueMenuUI;
    [SerializeField] private Text countdownToLose;
    [SerializeField] private Text chancesToContinue;
    private float countdownValue;
    [SerializeField] private float countdownValueOriginal;
    [SerializeField] ScoreGameManager gameManager;

    [SerializeField] private int chances = 1;

    private Coroutine countdownCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayCountdown()
    {
        countdownCoroutine = StartCoroutine(StartCountDown());
    }

    public void ContinueButton()
    {
        if(chances > 0)
        {
            StopCoroutine(countdownCoroutine);
            chances--;
            continueMenuUI.SetActive(false);
            Time.timeScale = 1f;
            gameManager.ContinueReviveGame();
        }
    }

    public void GiveUpButton()
    {
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1f;
    }

    IEnumerator StartCountDown()
    {
        Time.timeScale = 0f;
        countdownValue = countdownValueOriginal;
        continueMenuUI.SetActive(true);
        countdownToLose.text = countdownValue.ToString("0");
        countdownValue--;

        chancesToContinue.text = "Chances: " + chances.ToString("0");
        yield return new WaitForSecondsRealtime(1f);
        countdownToLose.text = countdownValue.ToString("0");
        countdownValue--;
        yield return new WaitForSecondsRealtime(1f);
        countdownToLose.text = countdownValue.ToString("0");
        countdownValue--;
        yield return new WaitForSecondsRealtime(1f);
        countdownToLose.text = countdownValue.ToString("0");
        countdownValue--;
        yield return new WaitForSecondsRealtime(1f);
        countdownToLose.text = countdownValue.ToString("0");
        countdownValue--;
        yield return new WaitForSecondsRealtime(1f);
        countdownToLose.text = countdownValue.ToString("0");
        countdownValue--;
        yield return new WaitForSecondsRealtime(0.1f);

        SceneManager.LoadScene("Menu");
        Time.timeScale = 1f;
    }
}
