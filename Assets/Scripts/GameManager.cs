using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public AudioSource Siren;
    public AudioSource munch1;
    public AudioSource munch2;
    public AudioSource startGameAudio;
    public AudioSource death;
    public AudioSource powerPelletAudio;
    public AudioSource ghostEatenAudio;
    public Image blackBackground;
    public Text gameOverText;
    public Text livesText;
    public Text scoreText;
    public GameObject pacman;
    public GameObject leftWarpNode;
    public GameObject rightWarpNode;
    public GameObject ghostNodeLeft;
    public GameObject ghostNodeRight;
    public GameObject ghostNodeCenter;
    public GameObject ghostNodeStart;

    public GameObject redGhost;
    public GameObject pinkGhost;
    public GameObject blueGhost;
    public GameObject orangeGhost;
    public EnemyController redGhostController;
    public EnemyController pinkGhostController;
    public EnemyController blueGhostController;
    public EnemyController orangeGhostController;
    public float currentPowerPelletTime = 0;
    public float powerPelletTimer = 8f;
    public float ghostModeTimer;
    public int [] ghostModeTimers = new int[] {7, 20, 7, 20, 5, 20, 5};
    public int ghostModeTimerIndex;
    public int powerPelletMultiplyer = 1;
    public int currentMunch;
    public int score;
    public int totalPellets;
    public int pelletsLeft;
    public int pelletsCollectedOnThisLife;
    public int lives;
    public int currentLevel;
    public bool hadDeathOnThisLevel = false;
    public bool gameIsRunning;
    public bool newGame;
    public bool clearedLevel;
    public bool isPowerPelletRunning = false;
    public bool runningTimer;
    public bool completedTimer;
    public List<NodeController> nodeControllers = new List<NodeController>();
    
    public enum GhostMode
    {
        chase, scatter
    }
    public GhostMode currentGhostMode;

    // Start is called before the first frame update
    void Awake()
    {
        newGame = true;
        clearedLevel = false;
        blackBackground.enabled = false;

        redGhostController = redGhost.GetComponent<EnemyController>();
        pinkGhostController = pinkGhost.GetComponent<EnemyController>();
        blueGhostController = blueGhost.GetComponent<EnemyController>();
        orangeGhostController = orangeGhost.GetComponent<EnemyController>();

        ghostNodeStart.GetComponent<NodeController>().isGhostStartingNode = true;
        //MAYBE DELETE
        pacman = GameObject.Find("Player");


        StartCoroutine(SetUp());
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameIsRunning)
        {
            return;
        }

        if (!completedTimer && runningTimer)
        {
            ghostModeTimer += Time.deltaTime;
            if (ghostModeTimer >= ghostModeTimers[ghostModeTimerIndex])
            {
                ghostModeTimer = 0;
                ghostModeTimerIndex ++;
                if (currentGhostMode == GhostMode.chase)
                {
                    currentGhostMode = GhostMode.scatter;
                }
                else
                {
                    currentGhostMode = GhostMode.chase;
                }

                //if we get to the end of our timer array (switched from chase to scatter max amount of times), stay in chase mode
                if (ghostModeTimerIndex == ghostModeTimers.Length)
                {
                    completedTimer = true;
                    runningTimer = false;
                    currentGhostMode = GhostMode.chase;
                }
            }
        }
        if (isPowerPelletRunning)
        {
            currentPowerPelletTime += Time.deltaTime;
            if (currentPowerPelletTime >= powerPelletTimer)
            {
                isPowerPelletRunning = false;
                currentPowerPelletTime = 0;
                powerPelletAudio.Stop();
                Siren.Play();
                powerPelletMultiplyer = 1;
            }
        }
    }

    public IEnumerator SetUp()
    {
        ghostModeTimerIndex = 0;
        ghostModeTimer = 0;
        completedTimer = false;
        runningTimer = true;
        gameOverText.enabled = false;
        //if pacman clears a level a background will appear covering the level and the game will pause for 0.1 seconds
        if (clearedLevel)
        {
            //Activatebackground
            blackBackground.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
        blackBackground.enabled = false;
        pelletsCollectedOnThisLife = 0;
        currentGhostMode = GhostMode.scatter;
        gameIsRunning = false;
        currentMunch = 0;
        float waitTimer = 1f;

        if (clearedLevel || newGame)
        {
            pelletsLeft = totalPellets;
            waitTimer = 4f;
            //pellets will respawn if pacman clears a level or starts a new game
            for (int i = 0; i < nodeControllers.Count; i++)
            {
                nodeControllers[i].RespawnPellet();
            }
        }

        if (newGame)
        {
            startGameAudio.Play();
            score = 0;
            scoreText.text = "Score: " + score.ToString();
            SetLives(3);
            currentLevel = 1;
        }
        
        //charcters go back to setup positions
        pacman.GetComponent<PlayerController>().SetUp();

        redGhostController.SetUp();
        pinkGhostController.SetUp();
        blueGhostController.SetUp();
        orangeGhostController.SetUp();

        newGame = false;
        clearedLevel = false;
        yield return new WaitForSeconds(waitTimer);

        StartGame();
    }

    void SetLives (int newLives)
    {
        lives = newLives;
        livesText.text = "Lives: " + lives;
    }

    void StartGame()
    {
        gameIsRunning = true;
        Siren.Play();
    }

    void StopGame()
    {
        gameIsRunning = false;
        Siren.Stop();
        powerPelletAudio.Stop();
        pacman.GetComponent<PlayerController>().Stop();
    }

    public void GotPelletFromNodeController(NodeController nodeController)
    {
        nodeControllers.Add(nodeController);
        totalPellets++;
        pelletsLeft++;
    }

    public void AddToScore(int Amount)
    {
        score += Amount;
        scoreText.text = "Score: " + score.ToString();
    }
    public IEnumerator CollectedPellet(NodeController nodeController)
    {
        if (currentMunch == 0)
        {
            munch1.Play();
            currentMunch = 1;
        }
        else if (currentMunch == 1)
        {
            munch2.Play();
            currentMunch = 0;
        }

        pelletsLeft--;
        pelletsCollectedOnThisLife++;

        int requiredBluePellets;
        int requiredOrangePellets;

        if (hadDeathOnThisLevel)
        {
            requiredBluePellets = 12;
            requiredOrangePellets = 32;
        }
        else
        {
            requiredBluePellets = 30;
            requiredOrangePellets = 60;
        }

        if (pelletsCollectedOnThisLife >= requiredBluePellets && !blueGhost.GetComponent<EnemyController>().leftHomeBefore)
        {
            blueGhost.GetComponent<EnemyController>().readyToLeaveHome = true;
        }
        if (pelletsCollectedOnThisLife >= requiredOrangePellets && !orangeGhost.GetComponent<EnemyController>().leftHomeBefore)
        {
            orangeGhost.GetComponent<EnemyController>().readyToLeaveHome = true;
        }

        //add to score
        AddToScore(10);

        //check if there are any pellets left
        if (pelletsLeft == 0)
        {
            currentLevel ++;
            clearedLevel = true;
            StopGame();
            yield return new WaitForSeconds(1);
            StartCoroutine(SetUp());
        }

        //Is this a power pellet
        if (nodeController.isPowerPellet)
        {
            //stop siren
            Siren.Stop();
            //Start power pellet sound
            powerPelletAudio.Play();
            isPowerPelletRunning = true;
            currentPowerPelletTime = 0;

            redGhostController.SetFrightened(true);
            pinkGhostController.SetFrightened(true);
            blueGhostController.SetFrightened(true);
            orangeGhostController.SetFrightened(true);

        }
    }

    public IEnumerator PauseGame(float timeToPause)
    {
        gameIsRunning = false;
        yield return new WaitForSeconds(timeToPause);
        gameIsRunning = true;
    }

    public void GhostEaten()
    {
        ghostEatenAudio.Play();
        AddToScore(400 * powerPelletMultiplyer);
        powerPelletMultiplyer++;
        StartCoroutine(PauseGame(1));

    }

    public IEnumerator PlayerEaten()
    {
        hadDeathOnThisLevel = true;
        //stop animation
        StopGame();
        //pause for 1 second
        yield return new WaitForSeconds(1);

        //ghost disapear
        redGhostController.SetVisible(false);
        pinkGhostController.SetVisible(false);
        blueGhostController.SetVisible(false);
        orangeGhostController.SetVisible(false);

        //play death animation
        pacman.GetComponent<PlayerController>().Death();
        death.Play();
        //wait for 3 seconds so death animation can play
        yield return new WaitForSeconds(3);

        SetLives(lives - 1);

        if (lives <= 0)
        {
            newGame = true;
            //Display gameover text
            gameOverText.enabled = true;
            yield return new WaitForSeconds(3);
        }

        StartCoroutine(SetUp());
    }
}
