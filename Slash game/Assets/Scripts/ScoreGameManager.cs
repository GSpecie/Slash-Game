using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreGameManager : MonoBehaviour
{
    private int currentScore = 0;
    private int highScore;

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
        scoreText.text = currentScore.ToString("0");

        if (comboValue == 10) player.ChangeFury(true, bonusHealthToPlayer);
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
        if (currentScore >= 15 && indexStrongerBasicEnemies < strongerBasicEnemies.Count)
        {
            strongerBasicEnemies[indexStrongerBasicEnemies].transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
            strongerBasicEnemies[indexStrongerBasicEnemies].gameObject.SetActive(true);
            indexStrongerBasicEnemies++;
        }
    }

    IEnumerator RebornBasicEnemies(BasicEnemy enemy)
    {
        yield return new WaitForSeconds(timeToRessurectionBasicEnemy);
        if (indexBasicEnemies < basicEnemies.Count - 1)
        {
            indexBasicEnemies++;
            basicEnemies[indexBasicEnemies].transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
            basicEnemies[indexBasicEnemies].gameObject.SetActive(true);
        }

        enemy.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
        enemy.Reborn();
    }

    public void RespawnEnemy(EnemyChaser enemy)
    {
        CheckNewEnemiesBorning();
        StartCoroutine(RebornChaserEnemies(enemy));
    }

    IEnumerator RebornChaserEnemies(EnemyChaser enemy)
    {
        yield return new WaitForSeconds(timeToRessurectionChaserEnemy);
        if (indexChaserEnemies < chaserEnemies.Count - 1)
        {
            indexChaserEnemies++;
            chaserEnemies[indexChaserEnemies].transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
            chaserEnemies[indexChaserEnemies].gameObject.SetActive(true);
        }
        enemy.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
        enemy.Reborn();
    }
}
