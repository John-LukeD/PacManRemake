using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public GameObject pacman;
    public GameObject leftWarpNode;
    public GameObject rightWarpNode;

    public AudioSource Siren;
    public AudioSource munch1;
    public AudioSource munch2;
    public int currentMunch;
    public int score;
    public Text scoreText;
    public GameObject ghostNodeLeft;
    public GameObject ghostNodeRight;
    public GameObject ghostNodeCenter;
    public GameObject ghostNodeStart;

    public GameObject redGhost;
    public GameObject pinkGhost;
    public GameObject blueGhost;
    public GameObject orangeGhost;

    public int totalPellets;
    public int pelletsLeft;
    public int pelletsCollectedOnThisLife;
    public bool hadDeathOnThisLevel = false;
    public enum GhostMode
    {
        chase, scatter
    }
    public GhostMode currentGhostMode;

    // Start is called before the first frame update
    void Awake()
    {
        pinkGhost.GetComponent<EnemyController>().readyToLeaveHome = true;
        currentGhostMode = GhostMode.chase;
        ghostNodeStart.GetComponent<NodeController>().isGhostStartingNode = true;
        score = 0;
        currentMunch = 0;
        Siren.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GotPelletFromNodeController()
    {
        totalPellets++;
        pelletsLeft++;
    }

    public void AddToScore(int Amount)
    {
        score += Amount;
        scoreText.text = "Score: " + score.ToString();
    }
    public void CollectedPellet(NodeController nodeController)
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

        //check how many pellets were eaten

        //Is this a power pellet
    }
}
