using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum GhostNodeStatesEnum{
        respawning,
        leftNode,
        rightNode,
        centerNode,
        startNode,
        movingInNodes,
    }

    public enum GhostType{
        red,
        blue,
        pink,
        orange,
    }

    public GhostNodeStatesEnum ghostNodeState;
    public GhostNodeStatesEnum respawnState;
    public GhostType ghostType;
    public GameObject ghostNodeLeft;
    public GameObject ghostNodeRight;
    public GameObject ghostNodeCenter;
    public GameObject ghostNodeStart;
    public MovementController movementController;
    public GameObject startingNode;
    public bool readyToLeaveHome = false;
    public GameManager gameManager;
    public bool testRespawn = false;

    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        movementController = GetComponent<MovementController>();
        if (ghostType == GhostType.red)
        {
            ghostNodeState = GhostNodeStatesEnum.startNode;
            respawnState = GhostNodeStatesEnum.centerNode;
            startingNode = ghostNodeStart;
            readyToLeaveHome = true;
        }
        else if (ghostType == GhostType.pink) 
        {
            ghostNodeState = GhostNodeStatesEnum.centerNode;
            startingNode = ghostNodeCenter;
            respawnState = GhostNodeStatesEnum.centerNode;
        }
        else if (ghostType == GhostType.blue) 
        {
            ghostNodeState = GhostNodeStatesEnum.leftNode;
            startingNode = ghostNodeLeft;
            respawnState = GhostNodeStatesEnum.leftNode;
        }
        else if (ghostType == GhostType.orange) 
        {
            ghostNodeState = GhostNodeStatesEnum.rightNode;
            startingNode = ghostNodeRight;
            respawnState = GhostNodeStatesEnum.rightNode;
        }
        movementController.currentNode = startingNode;
        transform.position = startingNode.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (testRespawn == true)
        {
            readyToLeaveHome = false;
            ghostNodeState = GhostNodeStatesEnum.respawning;
            testRespawn = false;
        }
    }

    public void ReachedCenterOfNode(NodeController nodeController)
    {
        if (ghostNodeState == GhostNodeStatesEnum.movingInNodes)
        {
            //determine next game node to go to
            if (ghostType == GhostType.red)
            {
                DetermineRedGhostDirection();
            }
        }
        else if (ghostNodeState == GhostNodeStatesEnum.respawning)
        {
            string direction = "";
            //we hjave reached start node, move to centerNode
            if (transform.position.x == ghostNodeStart.transform.position.x && transform.position.y == ghostNodeStart.transform.position.y)
            {
                direction = "down";
            }
            //we have reacher our center node, finish respawn or move tot he left/right node
            else if (transform.position.x == ghostNodeCenter.transform.position.x && transform.position.y == ghostNodeCenter.transform.position.y)
            {
                if (respawnState == GhostNodeStatesEnum.centerNode)
                {
                    ghostNodeState = respawnState;
                }
                else if (respawnState == GhostNodeStatesEnum.leftNode)
                {
                    direction = "left";
                }
                else if (respawnState == GhostNodeStatesEnum.rightNode)
                {
                    direction = "right";
                }
            }
            //if respawn state is either left or right node and we got to that node, leave home again
            else if (
                (transform.position.x == ghostNodeLeft.transform.position.x && transform.position.y == ghostNodeLeft.transform.position.y)
                ||
                (transform.position.x == ghostNodeRight.transform.position.x && transform.position.y == ghostNodeRight.transform.position.y)
                )
            {
                ghostNodeState = respawnState;
            }
            //we are in the gameboard still, locate our start node
            else
            {
                //Determine quickest direction to home
                direction = GetClosestDirection(ghostNodeStart.transform.position);
            }
            
            movementController.setDirection(direction);
        }
        else
        {
            //if ghost is ready to leave home
            if (readyToLeaveHome)
            {
                //if we are in the left homeNode => move to the center
                if (ghostNodeState == GhostNodeStatesEnum.leftNode)
                {
                    ghostNodeState = GhostNodeStatesEnum.centerNode;
                    movementController.setDirection("right");
                }
                //if we are in the right homeNode => move to the center
                else if (ghostNodeState == GhostNodeStatesEnum.rightNode)
                {
                    ghostNodeState = GhostNodeStatesEnum.centerNode;
                    movementController.setDirection("left");
                }
                //if we are in the center node => move to startNode
                else if (ghostNodeState == GhostNodeStatesEnum.centerNode)
                {
                    ghostNodeState = GhostNodeStatesEnum.startNode;
                    movementController.setDirection("up");
                }
                //if we are in the start node, start moving around in the game
                else if (ghostNodeState == GhostNodeStatesEnum.startNode)
                {
                    ghostNodeState = GhostNodeStatesEnum.movingInNodes;
                    movementController.setDirection("left");
                }
            }
        }
    }

    void DetermineRedGhostDirection()
    {
        string direction = GetClosestDirection(gameManager.pacman.transform.position);
        movementController.setDirection(direction);
    }

    void DeterminePinkGhostDirection()
    {

    }

    void DetermineBlueGhostDirection()
    {

    }

    void DetermineOrangeGhostDirection()
    {

    }

    string GetClosestDirection(Vector2 target)
    {
        float shortestDistance = 0;
        string lastMovingDirection = movementController.lastMovingDirection;
        string newDirection = "";
        NodeController nodeController = movementController.currentNode.GetComponent<NodeController>();

        //If we can move up and we aren't reversing
        if (nodeController.canMoveUp && lastMovingDirection != "down")
        {
            //get the node above us
            GameObject nodeUp = nodeController.nodeUp;
            // Get the distance between our top node and pacman
            float distance = Vector2.Distance(nodeUp.transform.position, target);
            //If this is the shortest distance so far, set our direction
            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = "up";
            }
        }

        //If we can move down and we aren't reversing
        if (nodeController.canMoveDown && lastMovingDirection != "up")
        {
            //get the node below us
            GameObject nodeDown = nodeController.nodeDown;
            // Get the distance between our bottom node and pacman
            float distance = Vector2.Distance(nodeDown.transform.position, target);
            //If this is the shortest distance so far, set our direction
            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = "down";
            }
        }

        //If we can move left and we aren't reversing
        if (nodeController.canMoveLeft && lastMovingDirection != "right")
        {
            //get the left node
            GameObject nodeLeft = nodeController.nodeLeft;
            // Get the distance between our left node and pacman
            float distance = Vector2.Distance(nodeLeft.transform.position, target);
            //If this is the shortest distance so far, set our direction
            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = "left";
            }
        }

        //If we can move right and we aren't reversing
        if (nodeController.canMoveRight && lastMovingDirection != "left")
        {
            //get the right node
            GameObject nodeRight = nodeController.nodeRight;
            // Get the distance between our bottom node and pacman
            float distance = Vector2.Distance(nodeRight.transform.position, target);
            //If this is the shortest distance so far, set our direction
            if (distance < shortestDistance || shortestDistance == 0)
            {
                shortestDistance = distance;
                newDirection = "right";
            }
        }

        return newDirection;

    }
}
