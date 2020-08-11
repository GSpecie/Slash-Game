using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreGameManager : MonoBehaviour
{
    private int currentScore = 0;
    [SerializeField] private int scoreToAddEnemy;
    private int currentScoreToAddEnemy;
    public static int highScore;

    [SerializeField] private Text scoreText;

    [SerializeField] private Transform[] spawnPoints;

    [SerializeField] private List<BasicEnemy> basicEnemies = new List<BasicEnemy>();
    [SerializeField] private List<BasicEnemy> strongerBasicEnemies = new List<BasicEnemy>();
    [SerializeField] private List<EnemyChaser> chaserEnemies = new List<EnemyChaser>();

    private int indexBasicEnemies = 0;
    private int indexStrongerBasicEnemies = 0;
    private int indexChaserEnemies = 0;

    [SerializeField] private float timeToRessurectionBasicEnemy;
    [SerializeField] private float timeToRessurectionChaserEnemy;

    private float comboPercentage;
    private float currentComboTime;
    [SerializeField] private float maximumComboTime;
    [SerializeField] private Image currentComboTimingBar;
    [SerializeField] private GameObject comboCountInterface;
    [SerializeField] private Text comboCountText;
    private bool isCombo = false;
    private int comboValue = 0;

    [SerializeField] private Player player;
    [SerializeField] private float bonusHealthToPlayer;

    [SerializeField] private ContinueMenu m_continueMenu;



    //[System.Serializable] public struct BasicEnemyPoolData
    //{
    //    public GameObject prefab;
    //    public int size;
    //}

    //private Dictionary<string, Queue<GameObject>> poolBasicEnemyDictionary;
    //[SerializeField] private List<BasicEnemyPoolData> basicEnemyPool = new List<BasicEnemyPoolData>();


    // Start is called before the first frame update
    void Start()
    {
        //poolBasicEnemyDictionary = new Dictionary<string, Queue<GameObject>>();
        scoreText.text = currentScore.ToString("0");
    }

    // Update is called once per frame
    void Update()
    {
        ComboManager();
    }

    public void StartGameplay()
    {
        //invoke 1 basic enemy
        basicEnemies[0].transform.position = spawnPoints[0].transform.position;
        basicEnemies[0].transform.rotation = spawnPoints[0].transform.rotation;
        basicEnemies[0].gameObject.SetActive(true);

        //invoke 1 chaser enemy
        chaserEnemies[0].transform.position = spawnPoints[7].transform.position;
        chaserEnemies[0].transform.rotation = spawnPoints[7].transform.rotation;
        chaserEnemies[indexChaserEnemies].gameObject.SetActive(true);
    }

    public void ComboManager()
    {
        if (currentComboTime <= 0) ComboBreak();

        if (isCombo)
        {
            comboPercentage = currentComboTime / maximumComboTime;
            currentComboTimingBar.rectTransform.localScale = new Vector3(comboPercentage, 1, 1);

            currentComboTime -= Time.deltaTime;
        }
        else if (!isCombo)
        {
            comboCountInterface.SetActive(false);
            comboValue = 0;
            player.ChangeFury(false, 0);
        }
    }

    public void ComboBreak()
    {
        isCombo = false;
    }

    public void AddScore()
    {
        //combo
        currentComboTime = maximumComboTime;
        isCombo = true;
        comboCountInterface.SetActive(true);
        comboValue++;
        comboCountText.text = comboValue.ToString("0" +" X");

        //score
        currentScore++;
        currentScoreToAddEnemy++;
        scoreText.text = currentScore.ToString("0");

        //active score UI
        if (currentScore > 0) scoreText.gameObject.SetActive(true);

        if (comboValue == 10) player.ChangeFury(true, bonusHealthToPlayer);
    }

    public void EnableContinueScreen()
    {
        SetMatchScore(currentScore);
        if (currentScore > highScore)
        {
            SetHighScore(currentScore);
        }

        m_continueMenu.PlayCountdown();
    }

    public void ContinueReviveGame()
    {
        player.Revive();

        //kill all enemies in screen
        for(int i = 0; i < basicEnemies.Count; i++)
        {
            if (basicEnemies[i].isActiveAndEnabled)
            {
                basicEnemies[i].TakeDamage(10f, Vector3.zero);
            }
        }

        for (int i = 0; i < chaserEnemies.Count; i++)
        {
            if (chaserEnemies[i].isActiveAndEnabled)
            {
                chaserEnemies[i].TakeDamage(10f, Vector3.zero);
            }
        }
    }

    public void RemoveScore()
    {
        currentScore--;
    }

    public void RespawnEnemy(BasicEnemy enemy)
    {
        CheckNewEnemiesBorning();
        StartCoroutine(RebornBasicEnemies(enemy));
    }

    public void CheckNewEnemiesBorning()
    {
        int raffle = Random.Range(0, 1);

        if(raffle == 0)
        {
            if(indexBasicEnemies < basicEnemies.Count - 1) StartCoroutine(AddBasicEnemies());
            else StartCoroutine(AddChaserEnemies());

        }
        else if(raffle == 1)
        {
            if(indexChaserEnemies < chaserEnemies.Count - 1) StartCoroutine(AddChaserEnemies());
            else StartCoroutine(AddBasicEnemies());
        }

        //add stronger basic enemies
        if (currentScore >= 50 && indexStrongerBasicEnemies < strongerBasicEnemies.Count)
        {
            int spawnIndex = Random.Range(0, spawnPoints.Length);
            strongerBasicEnemies[indexStrongerBasicEnemies].transform.position = spawnPoints[spawnIndex].transform.position;
            strongerBasicEnemies[indexStrongerBasicEnemies].transform.rotation = spawnPoints[spawnIndex].transform.rotation;
            strongerBasicEnemies[indexStrongerBasicEnemies].gameObject.SetActive(true);
            indexStrongerBasicEnemies++;
        }
    }

    IEnumerator RebornBasicEnemies(BasicEnemy enemy)
    {
        yield return new WaitForSeconds(timeToRessurectionBasicEnemy);

        int mspawnIndex = Random.Range(0, spawnPoints.Length);
        enemy.transform.position = spawnPoints[mspawnIndex].transform.position;
        enemy.transform.rotation = spawnPoints[mspawnIndex].transform.rotation;
        enemy.Reborn();
    }

    IEnumerator AddBasicEnemies()
    {
        yield return new WaitForSeconds(timeToRessurectionBasicEnemy);
        if (indexBasicEnemies < basicEnemies.Count - 1 && currentScoreToAddEnemy >= scoreToAddEnemy)
        {
            currentScoreToAddEnemy = 0;
            indexBasicEnemies++;
            int spawnIndex = Random.Range(0, spawnPoints.Length);
            basicEnemies[indexBasicEnemies].transform.position = spawnPoints[spawnIndex].transform.position;
            basicEnemies[indexBasicEnemies].transform.rotation = spawnPoints[spawnIndex].transform.rotation;
            basicEnemies[indexBasicEnemies].gameObject.SetActive(true);
        }
    }


    public void RespawnEnemy(EnemyChaser enemy)
    {
        CheckNewEnemiesBorning();
        StartCoroutine(RebornChaserEnemies(enemy));
    }

    IEnumerator RebornChaserEnemies(EnemyChaser enemy)
    {
        yield return new WaitForSeconds(timeToRessurectionChaserEnemy);

        int mspawnIndex = Random.Range(0, spawnPoints.Length);
        enemy.transform.position = spawnPoints[mspawnIndex].transform.position;
        enemy.transform.rotation = spawnPoints[mspawnIndex].transform.rotation;
        enemy.Reborn();
    }

    IEnumerator AddChaserEnemies()
    {
        yield return new WaitForSeconds(timeToRessurectionChaserEnemy);
        if (indexChaserEnemies < chaserEnemies.Count - 1 && currentScoreToAddEnemy >= scoreToAddEnemy)
        {
            currentScoreToAddEnemy = 0;
            indexChaserEnemies++;
            int spawnIndex = Random.Range(0, spawnPoints.Length);
            chaserEnemies[indexChaserEnemies].transform.position = spawnPoints[spawnIndex].transform.position;
            chaserEnemies[indexChaserEnemies].transform.rotation = spawnPoints[spawnIndex].transform.rotation;
            chaserEnemies[indexChaserEnemies].gameObject.SetActive(true);
        }
    }

    #region playerPrefsThings
    public void SetHighScore(int value)
    {
        highScore = value;
        PlayerPrefs.SetInt("HighScore", value);
    }

    public static int GetHighScore()
    {
        return PlayerPrefs.GetInt("HighScore");
    }

    public void SetMatchScore(int value)
    {
        PlayerPrefs.SetInt("MatchScore", value);
    }

    public static int GetMatchScore()
    {
        return PlayerPrefs.GetInt("MatchScore");
    }
    #endregion
}
