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

    [SerializeField] private List<SpawnPoint> meleeSpawnPoints = new List<SpawnPoint>();
    [SerializeField] private List<SpawnPoint> rangedSpawnPoints = new List<SpawnPoint>();

    [SerializeField] private List<BasicEnemy> basicEnemies = new List<BasicEnemy>();
    [SerializeField] private List<BasicEnemy> strongerBasicEnemies = new List<BasicEnemy>();
    [SerializeField] private List<EnemyChaser> chaserEnemies = new List<EnemyChaser>();
    [SerializeField] private List<EnemyShooter> shooterEnemies = new List<EnemyShooter>();
    [SerializeField] private List<GameObject> firstWaveEnemies = new List<GameObject>();

    private int indexFirstWaveEnemies = 0;
    private int indexFirstWaveTankers = 0;
    private int indexFirstWaveShooters = 0;


    private int currentWaveValue;
    [SerializeField] private int firstWaveValue = 100;
    [SerializeField] private int secondWaveValue = 100;
    private int currentWave = 1;
    private float currentWaveTimeToRessurection;
    [SerializeField] private float timeToRessurectionFirstWaveEnemies;
    [SerializeField] private float timeToRessurectionSpecialEnemy;

    //combo variables
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


    //overlap variables
    [SerializeField] private LayerMask overlapSpawnLayer;

    //hit stop variables
    private float timeRestorationSpeed;
    private bool restoreTime = false;

    //cam shake variables
    [SerializeField] private Transform mCameraCoordinates;
    private Vector3 storedCamPosition;

    // Start is called before the first frame update
    void Start()
    {
        currentWaveTimeToRessurection = timeToRessurectionFirstWaveEnemies;
        indexFirstWaveEnemies = 1;
        currentWaveValue = firstWaveValue;
        storedCamPosition = mCameraCoordinates.position;
    }

    // Update is called once per frame
    void Update()
    {
        ComboManager();
        CheckTimeRestoration();
    }

    public void StartGameplay()
    {
        //invoke 1 basic enemy
        firstWaveEnemies[0].transform.position = meleeSpawnPoints[0].transform.position;
        firstWaveEnemies[0].transform.rotation = meleeSpawnPoints[0].transform.rotation;
        firstWaveEnemies[0].gameObject.SetActive(true);

        //invoke 1 chaser enemy
        firstWaveEnemies[1].transform.position = meleeSpawnPoints[7].transform.position;
        firstWaveEnemies[1].transform.rotation = meleeSpawnPoints[7].transform.rotation;
        firstWaveEnemies[1].gameObject.SetActive(true);

        //shooterEnemies[0].transform.position = rangedSpawnPoints[0].transform.position;
        //shooterEnemies[0].transform.rotation = rangedSpawnPoints[0].transform.rotation;
        //shooterEnemies[0].gameObject.SetActive(true);
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
        comboCountText.text = comboValue.ToString("0" + " X");

        ////wave
        //currentWaveValue--;
        //WaveManager();

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
        for (int i = 0; i < basicEnemies.Count; i++)
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

        for (int i = 0; i < strongerBasicEnemies.Count; i++)
        {
            if (strongerBasicEnemies[i].isActiveAndEnabled)
            {
                strongerBasicEnemies[i].TakeDamage(10f, Vector3.zero);
            }
        }
        for (int i = 0; i < shooterEnemies.Count; i++)
        {
            if (shooterEnemies[i].isActiveAndEnabled)
            {
                shooterEnemies[i].TakeDamage(10f, Vector3.zero);
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
        if (currentWaveValue > 0) StartCoroutine(RebornBasicEnemies(enemy));
    }

    public void RespawnEnemy(EnemyChaser enemy)
    {
        CheckNewEnemiesBorning();
        if (currentWaveValue > 0) StartCoroutine(RebornChaserEnemies(enemy));
    }

    public void RespawnEnemy(EnemyShooter enemy)
    {
        CheckNewEnemiesBorning();
        if(currentWaveValue > 0) StartCoroutine(RebornShooterEnemies(enemy));

    }

    IEnumerator RebornBasicEnemies(BasicEnemy enemy)
    {
        yield return new WaitForSeconds(currentWaveTimeToRessurection);
        int spawnIndex = Random.Range(0, meleeSpawnPoints.Count);

        enemy.transform.position = meleeSpawnPoints[spawnIndex].transform.position;
        enemy.transform.rotation = meleeSpawnPoints[spawnIndex].transform.rotation;
        enemy.Reborn();

    }

    IEnumerator RebornChaserEnemies(EnemyChaser enemy)
    {
        yield return new WaitForSeconds(currentWaveTimeToRessurection);

        int spawnIndex = Random.Range(0, meleeSpawnPoints.Count);

        enemy.transform.position = meleeSpawnPoints[spawnIndex].transform.position;
        enemy.transform.rotation = meleeSpawnPoints[spawnIndex].transform.rotation;
        enemy.Reborn();

    }

    IEnumerator RebornShooterEnemies(EnemyShooter enemy)
    {
        yield return new WaitForSeconds(timeToRessurectionSpecialEnemy);

        int spawnIndex = Random.Range(0, rangedSpawnPoints.Count);

        enemy.transform.position = rangedSpawnPoints[spawnIndex].transform.position;
        enemy.transform.rotation = rangedSpawnPoints[spawnIndex].transform.rotation;
        enemy.Reborn();

    }
    
    public void CheckNewEnemiesBorning()
    {
        //add normal enemies
        StartCoroutine(AddEnemies());
    }

    public void WaveManager()
    {
        if(currentWaveValue == 0 && currentWave == 1)
        {
            currentWave++;
            currentWaveValue = secondWaveValue;
        }
        else if (currentWaveValue == 0 && currentWave == 2)
        {
            currentWave++;
            //currentWaveValue = secondWaveValue;
        }
    }

    IEnumerator AddEnemies()
    {
        yield return new WaitForSeconds(currentWaveTimeToRessurection);

        if(currentWave == 1)
        {
            //first wave borning enemies
            if (indexFirstWaveEnemies < firstWaveEnemies.Count - 1 && currentScoreToAddEnemy >= scoreToAddEnemy)
            {
                int spawnIndex = Random.Range(0, meleeSpawnPoints.Count);

                currentScoreToAddEnemy = 0;
                indexFirstWaveEnemies++;
                firstWaveEnemies[indexFirstWaveEnemies].transform.position = meleeSpawnPoints[spawnIndex].transform.position;
                firstWaveEnemies[indexFirstWaveEnemies].transform.rotation = meleeSpawnPoints[spawnIndex].transform.rotation;
                firstWaveEnemies[indexFirstWaveEnemies].gameObject.SetActive(true);
            }
            //add stronger basic enemies
            if (currentScore >= 40 && indexFirstWaveTankers < strongerBasicEnemies.Count)
            {
                int spawnIndex = Random.Range(0, meleeSpawnPoints.Count);
                strongerBasicEnemies[indexFirstWaveTankers].transform.position = meleeSpawnPoints[spawnIndex].transform.position;
                strongerBasicEnemies[indexFirstWaveTankers].transform.rotation = meleeSpawnPoints[spawnIndex].transform.rotation;
                strongerBasicEnemies[indexFirstWaveTankers].gameObject.SetActive(true);
                indexFirstWaveTankers++;

            }
            //add ranged
            if (currentScore >= 25 && indexFirstWaveShooters < shooterEnemies.Count)
            {
                int spawnIndex = Random.Range(0, rangedSpawnPoints.Count);
                shooterEnemies[indexFirstWaveShooters].transform.position = rangedSpawnPoints[spawnIndex].transform.position;
                shooterEnemies[indexFirstWaveShooters].transform.rotation = rangedSpawnPoints[spawnIndex].transform.rotation;
                shooterEnemies[indexFirstWaveShooters].gameObject.SetActive(true);
                indexFirstWaveShooters++;
            }
        }
    }

    //IEnumerator AddChaserEnemies()
    //{
    //    yield return new WaitForSeconds(timeToRessurectionChaserEnemy);
    //    if (indexChaserEnemies < chaserEnemies.Count - 1 && currentScoreToAddEnemy >= scoreToAddEnemy)
    //    {
    //        currentScoreToAddEnemy = 0;
    //        indexChaserEnemies++;
    //        int spawnIndex = Random.Range(0, spawnPoints.Length);
    //        chaserEnemies[indexChaserEnemies].transform.position = spawnPoints[spawnIndex].transform.position;
    //        chaserEnemies[indexChaserEnemies].transform.rotation = spawnPoints[spawnIndex].transform.rotation;
    //        chaserEnemies[indexChaserEnemies].gameObject.SetActive(true);
    //    }
    //}

    public void ManageSpawnPoint(bool isAdding, SpawnPoint spawnObject, int enemyType)
    {
        if (isAdding)
        {
            if(enemyType == 0) if (!meleeSpawnPoints.Contains(spawnObject)) meleeSpawnPoints.Add(spawnObject);
                else if (enemyType == 1) if(!rangedSpawnPoints.Contains(spawnObject)) rangedSpawnPoints.Add(spawnObject);
        }
        else
        {
            if (enemyType == 0) if (meleeSpawnPoints.Contains(spawnObject)) meleeSpawnPoints.Remove(spawnObject);
            else if (enemyType == 1) if (rangedSpawnPoints.Contains(spawnObject)) rangedSpawnPoints.Remove(spawnObject);
        }
    }

    //public void OverlapTest()
    //{

    //    for (int index = 0; index < firstWaveEnemies.Count; index++)
    //    {
    //        int spawnIndex = Random.Range(0, spawnPoints.Count);

    //        firstWaveEnemies[index].transform.position = spawnPoints[spawnIndex].transform.position;
    //        firstWaveEnemies[index].transform.rotation = spawnPoints[spawnIndex].transform.rotation;
    //        firstWaveEnemies[index].gameObject.SetActive(true);
    //        //bool enemySpawned = false;

    //        //while (enemySpawned == false)
    //        //{
    //        //    int spawnIndex = Random.Range(0, spawnPoints.Count);


    //        //    Collider[] overlapSpawnCollider = Physics.OverlapBox(spawnPoints[spawnIndex].transform.position, new Vector3(2.5f, 1, 5.5f), spawnPoints[spawnIndex].transform.rotation, overlapSpawnLayer);

    //        //    if (overlapSpawnCollider.Length != 0)
    //        //    {
    //        //        continue;
    //        //    }
    //        //    else
    //        //    {
    //        //        firstWaveEnemies[index].transform.position = spawnPoints[spawnIndex].transform.position;
    //        //        firstWaveEnemies[index].transform.rotation = spawnPoints[spawnIndex].transform.rotation;
    //        //        firstWaveEnemies[index].gameObject.SetActive(true);
    //        //        enemySpawned = true;
    //        //    }
    //        //}
    //    }
    //}

    public void CheckTimeRestoration()
    {
        if (restoreTime)
        {
            if (Time.timeScale < 1f)
            {
                Time.timeScale += Time.deltaTime * timeRestorationSpeed;
            }
            else
            {
                Time.timeScale = 1f;
                restoreTime = false;
            }
        }
    }

    public void StopTime(float changeTime, int restoreSpeed, float delayToRestore)
    {
        timeRestorationSpeed = restoreSpeed;

        if (delayToRestore > 0)
        {
            StopCoroutine(StartTimeAgain(delayToRestore));
            StartCoroutine(StartTimeAgain(delayToRestore));
        }
        else
        {
            restoreTime = true;
        }

        Time.timeScale = changeTime;
    }

    IEnumerator StartTimeAgain(float amt)
    {
        yield return new WaitForSecondsRealtime(amt);
        if (PauseMenu.gameIsPaused == false) restoreTime = true;
    }

    public IEnumerator CamShake(float shakeValue)
    {
        mCameraCoordinates.position = new Vector3(mCameraCoordinates.position.x + Random.Range(-shakeValue, shakeValue), mCameraCoordinates.position.y + Random.Range(-shakeValue, shakeValue), mCameraCoordinates.position.z + Random.Range(-shakeValue, shakeValue));

        yield return new WaitForSecondsRealtime(0.05f);

        mCameraCoordinates.position = new Vector3(mCameraCoordinates.position.x + Random.Range(-shakeValue, shakeValue), mCameraCoordinates.position.y + Random.Range(-shakeValue, shakeValue), mCameraCoordinates.position.z + Random.Range(-shakeValue, shakeValue));

        yield return new WaitForSecondsRealtime(0.05f);

        mCameraCoordinates.position = storedCamPosition;
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
