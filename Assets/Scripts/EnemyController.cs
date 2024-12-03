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
    public GhostType ghostType;
    public GameObject ghostNodeLeft;
    public GameObject ghostNodeRight;
    public GameObject ghostNodeCenter;
    public GameObject ghostNodeStart;
    public MovementController movementController;
    public GameObject startingNode;
    public bool readyToLeaveHome = false;
    public GameManager gameManager;

    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        movementController = GetComponent<MovementController>();
        if (ghostType == GhostType.red)
        {
            ghostNodeState = GhostNodeStatesEnum.startNode;
            startingNode = ghostNodeStart;
        }
        else if (ghostType == GhostType.pink) 
        {
            ghostNodeState = GhostNodeStatesEnum.centerNode;
            startingNode = ghostNodeCenter;
        }
        else if (ghostType == GhostType.blue) 
        {
            ghostNodeState = GhostNodeStatesEnum.leftNode;
            startingNode = ghostNodeLeft;
        }
        else if (ghostType == GhostType.orange) 
        {
            ghostNodeState = GhostNodeStatesEnum.rightNode;
            startingNode = ghostNodeRight;
        }
        movementController.currentNode = startingNode;
        transform.position = startingNode.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
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
            //Determine quickest direction to home
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
