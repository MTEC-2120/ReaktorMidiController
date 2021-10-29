using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using SgLib;

public enum GameState
{
    Prepare,
    Playing,
    Paused,
    PreGameOver,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static event System.Action<GameState, GameState> GameStateChanged = delegate { };

    public GameState GameState
    {
        get
        {
            return _gameState;
        }
        private set
        {
            if (value != _gameState)
            {
                GameState oldState = _gameState;
                _gameState = value;

                GameStateChanged(_gameState, oldState);
            }
        }
    }

    private GameState _gameState = GameState.Prepare;

    public static int GameCount
    { 
        get { return _gameCount; } 
        private set { _gameCount = value; } 
    }

    private static int _gameCount = 0;

    [Header("Set the target frame rate for this game")]
    public int targetFrameRate = 60;

    [Header("Gameplay Preferences")]
    public UIManager uIManager;
    public GameObject goldPrefab;
    public GameObject parentPlayer;
    public GameObject theGround;
    [Header("Obstacles")]
    public GameObject normalObstacle;
    public GameObject fireObstacle;
    public ParticleSystem fireParticle;
    public GameObject iceObstacle;
    public GameObject iceParticle;
    public GameObject electricObstacle;
    public ParticleSystem electricParticle;

    [Header("Gameplay Config")]   
    public int initialObstacle = 3;
    //How many obstacle you create when the game start
    public int space = 7;
    //Space between 2 obstacle

    /*when the score higher than this value, 
    in this case is 5, that mean player jump over 5 obstacle, 
    the first obstacle will be destroyed (the obstacle you create for the first time) 
    and the ground will be moved to the position of the obstacle that you destroyed. 
    After that, this value will automatically counting. */
    public int obstacleCounter = 4;

    public float maxObstacleFluctuationRange = 4;
    //Max moving flutuation range of obstacle
    public float minObstacleFluctuationRange = 3;
    //Min moving flutuation range of obstacle
    public int scoreToUpdateValue = 10;
    //When you reached this score, onstacle speed will be decrease
    public float decreaseObstacleSpeedValue = 0.05f;
    //Obstacle speed will be minus by this value
    public float minObstacleSpeedFactor = 1f;
    // Min obstacle speed factor
    public float maxObstacleSpeedFactor = 1.5f;
    //Max obstacle speed factor
    public float minimumMinObstacleSpeedFactor = 0.4f;
    //Limited of min obstacle speed factor
    public float minimumMaxObstacleSpeedFactor = 0.7f;
    //Limited of max ofstacle speed factor
    [Range(0f, 1f)]
    public float goldFrequecy;

    private List<GameObject> listObstacle = new List<GameObject>();
    private GameObject obstaclePrefab;
    private GameObject currentObstacle;
    private Vector3 obstaclePosition;
    private Vector3 addedPosition;
    private bool hasCheckedScore = false;
    private int listIndex = 0;
    private int listDestroyIndex = 0;

    void OnEnable()
    {
        PlayerController.PlayerDied += PlayerController_PlayerDied;
    }

    void OnDisable()
    {
        PlayerController.PlayerDied -= PlayerController_PlayerDied;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DestroyImmediate(Instance.gameObject);
            Instance = this;
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    void PlayerController_PlayerDied()
    {
        GameOver();
    }

    void Start()
    {
        GameState = GameState.Prepare;
        Application.targetFrameRate = targetFrameRate;
        ScoreManager.Instance.Reset();

        RandomObstacleType();//Random obstacle's type

        //Create the first obstacle and add to list
        Vector3 firstObstaclePos = theGround.transform.position + new Vector3(0, 13f, 0);
        currentObstacle = Instantiate(obstaclePrefab, firstObstaclePos, Quaternion.identity) as GameObject;
        currentObstacle.GetComponent<ObstacleController>().fluctuationRange = Random.Range(minObstacleFluctuationRange, maxObstacleFluctuationRange);
        currentObstacle.GetComponent<ObstacleController>().movingSpeed = Random.Range(minObstacleSpeedFactor, maxObstacleSpeedFactor);
        currentObstacle.transform.parent = transform;
        listObstacle.Add(currentObstacle);


        addedPosition = new Vector3(0, space, 0);
        //Create position for next obstacle 
        obstaclePosition = currentObstacle.transform.position + addedPosition;

        for (int i = 0; i < initialObstacle; i++)
        {
            CreateObstacle();
        }
            
        StartCoroutine(GenerateObstacle());
    }

    public void StartGame()
    {
        GameState = GameState.Playing;

        if (SoundManager.Instance.background != null)
            SoundManager.Instance.PlayMusic(SoundManager.Instance.background);
    }

    public void GameOver()
    {
        GameState = GameState.GameOver;
        GameCount++;

        if (SoundManager.Instance.background != null)
            SoundManager.Instance.StopMusic();
    }

    public void RestartGame(float delay = 0)
    {
        StartCoroutine(CRRestartGame(delay));
    }

    IEnumerator CRRestartGame(float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //Create obstacle
    void CreateObstacle()
    {
        RandomObstacleType();

        currentObstacle = Instantiate(obstaclePrefab, obstaclePosition, Quaternion.identity) as GameObject;
        currentObstacle.GetComponent<ObstacleController>().fluctuationRange = Random.Range(minObstacleFluctuationRange, maxObstacleFluctuationRange);
        currentObstacle.GetComponent<ObstacleController>().movingSpeed = Random.Range(minObstacleSpeedFactor, maxObstacleSpeedFactor);
        currentObstacle.transform.parent = transform;

        CreateGold();

        listObstacle.Add(currentObstacle);

        obstaclePosition = currentObstacle.transform.position + addedPosition;

    }

    IEnumerator GenerateObstacle()
    {
        while (GameState != GameState.GameOver)
        {
            //Player jump over an obstacle -> add score, create next obstacle
            if (parentPlayer.transform.position.y > listObstacle[listIndex].transform.position.y)
            {
                ScoreManager.Instance.AddScore(1);
                hasCheckedScore = false;
                CreateObstacle();
                listIndex++;
            }

            //Destroy obstacle and move the ground up
            if (ScoreManager.Instance.Score > obstacleCounter)
            {
                theGround.transform.position = listObstacle[listDestroyIndex].transform.position;
                Destroy(listObstacle[listDestroyIndex]);
                listDestroyIndex++;
                obstacleCounter++;
            }


            if (ScoreManager.Instance.Score != 0 && ScoreManager.Instance.Score % scoreToUpdateValue == 0 && !hasCheckedScore)
            {
                hasCheckedScore = true;

                minObstacleSpeedFactor -= decreaseObstacleSpeedValue;
                maxObstacleSpeedFactor -= decreaseObstacleSpeedValue;

                if (minObstacleSpeedFactor <= minimumMinObstacleSpeedFactor)
                {
                    minObstacleSpeedFactor = minimumMinObstacleSpeedFactor;
                }

                if (maxObstacleSpeedFactor <= minimumMaxObstacleSpeedFactor)
                {
                    maxObstacleSpeedFactor = minimumMaxObstacleSpeedFactor;
                }
            }


            yield return null;
        }
    }

    //Create gold
    void CreateGold()
    {
        float goldProbability = Random.Range(0f, 1f);
        if (goldProbability <= goldFrequecy)
        {
            Vector3 goldPosition = currentObstacle.transform.position + new Vector3(0, space / 2f, 0);
            GameObject currentGold = Instantiate(goldPrefab, goldPosition, Quaternion.Euler(0, 0, 45)) as GameObject;

            float goldFluctuationRange = Random.Range(minObstacleFluctuationRange, maxObstacleFluctuationRange);

            currentGold.GetComponent<GoldController>().fluctuationRange = goldFluctuationRange;
            currentGold.GetComponent<GoldController>().movingSpeed = Random.Range(minObstacleSpeedFactor * 2, maxObstacleSpeedFactor * 2);

            int indexPosition = Random.Range(0, 2);
            if (indexPosition == 0)
            {
                currentGold.transform.position += new Vector3(-goldFluctuationRange, 0, 0);
            }
            else
            {
                currentGold.transform.position += new Vector3(goldFluctuationRange, 0, 0);
            }
            currentGold.transform.parent = currentObstacle.transform;
        }
    }

    void RandomObstacleType()
    {
        int index = Random.Range(0, 4);
        if (index == 0) //Normal obstacle
        {
            obstaclePrefab = normalObstacle;
        }
        else if (index == 1) //Fire obstacle
        {
            obstaclePrefab = fireObstacle;
        }
        else if (index == 2) //Ice obstacle
        {
            obstaclePrefab = iceObstacle;
        }
        else //Electric obstacle
        {
            obstaclePrefab = electricObstacle;
        }
    }
}
