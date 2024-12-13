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
    public GhostNodeStatesEnum startGhostNodeState;
    public GhostNodeStatesEnum respawnState;
    public GhostType ghostType;
    public GameObject ghostNodeLeft;
    public GameObject ghostNodeRight;
    public GameObject ghostNodeCenter;
    public GameObject ghostNodeStart;
    public GameObject startingNode;
    public GameObject[] scatterNodes;
    public MovementController movementController;
    public GameManager gameManager;
    public bool readyToLeaveHome = false;
    public bool testRespawn = false;
    public bool isFreightened = false;
    public bool leftHomeBefore = false;
    public bool isVisible = true;
    public int scatterNodeIndex;
    public SpriteRenderer ghostSprite;
    public SpriteRenderer eyesSprite;
    public Animator animator;
    public Color color;

    void Awake()
    {
        animator = GetComponent<Animator>();
        ghostSprite = GetComponent<SpriteRenderer>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (movementController == null)
        {
            movementController = GetComponent<MovementController>();
            if (movementController == null)
            {
                Debug.LogError("MovementController is missing on the EnemyController GameObject!");
            }
        }
        if (ghostType == GhostType.red)
        {
            startGhostNodeState = GhostNodeStatesEnum.startNode;
            respawnState = GhostNodeStatesEnum.centerNode;
            startingNode = ghostNodeStart;
        }
        else if (ghostType == GhostType.pink) 
        {
            startGhostNodeState = GhostNodeStatesEnum.centerNode;
            startingNode = ghostNodeCenter;
            respawnState = GhostNodeStatesEnum.centerNode;

        }
        else if (ghostType == GhostType.blue) 
        {
            
            startGhostNodeState = GhostNodeStatesEnum.leftNode;
            startingNode = ghostNodeLeft;
            respawnState = GhostNodeStatesEnum.leftNode;
        }
        else if (ghostType == GhostType.orange) 
        {
            startGhostNodeState = GhostNodeStatesEnum.rightNode;
            startingNode = ghostNodeRight;
            respawnState = GhostNodeStatesEnum.rightNode;
        }
    }

    public void SetUp()
    {
        animator = GetComponent<Animator>();
        ghostSprite = GetComponent<SpriteRenderer>();
        animator.SetBool("moving", false);

        ghostNodeState = startGhostNodeState;

        //DO NOT CHANGE ANYTHING BETWEEN THIS AND movementController.currentNode = startingNode;
        //This is the same code as Awake()
        //I cant explain why but i think the Awake() function is not executing -- will try to clean this up in future
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (movementController == null)
        {
            movementController = GetComponent<MovementController>();
            if (movementController == null)
            {
                Debug.LogError("MovementController is missing on the EnemyController GameObject!");
            }
        }
        if (ghostType == GhostType.red)
        {
            startGhostNodeState = GhostNodeStatesEnum.startNode;
            respawnState = GhostNodeStatesEnum.centerNode;
            startingNode = ghostNodeStart;
        }
        else if (ghostType == GhostType.pink) 
        {
            startGhostNodeState = GhostNodeStatesEnum.centerNode;
            startingNode = ghostNodeCenter;
            respawnState = GhostNodeStatesEnum.centerNode;
        }
        else if (ghostType == GhostType.blue) 
        {
            startGhostNodeState = GhostNodeStatesEnum.leftNode;
            startingNode = ghostNodeLeft;
            respawnState = GhostNodeStatesEnum.leftNode;
        }
        else if (ghostType == GhostType.orange) 
        {
            startGhostNodeState = GhostNodeStatesEnum.rightNode;
            startingNode = ghostNodeRight;
            respawnState = GhostNodeStatesEnum.rightNode;
        }       
        //END DO NOT TOUCH CODE 

        readyToLeaveHome = false;
        //Reset our ghost back to home position
        movementController.currentNode = startingNode; 
        transform.position = startingNode.transform.position;
        movementController.direction = "";
        movementController.lastMovingDirection = "";
        //set their sctter  node index back to 0
        scatterNodeIndex = 0;
        //set is frightened and leftHomeBefore to false
        isFreightened = false;
        leftHomeBefore = false;
        //set ready to leave home false if they are blue or pink ghost
        if (ghostType == GhostType.red)
        {
            readyToLeaveHome = true;
            leftHomeBefore = true;
        }
        else if (ghostType == GhostType.pink)
        {
            readyToLeaveHome = true;
        }
        SetVisible(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (ghostNodeState != GhostNodeStatesEnum.movingInNodes || !gameManager.isPowerPelletRunning)
        {
            isFreightened = false;
        }

        //Show our sprites
        if (isVisible)
        {
            if (ghostNodeState != GhostNodeStatesEnum.respawning)
            {
                ghostSprite.enabled = true;
            }
            else
            {
                ghostSprite.enabled = false;
            }
            eyesSprite.enabled = true;
        }
        //hide our sprites
        else
        {
            ghostSprite.enabled = false;
            eyesSprite.enabled = false;
        }

        //start animation, turn off eyes, set color
        if (isFreightened)
        {
            animator.SetBool("frightened", true);
            eyesSprite.enabled = false;
            ghostSprite.color = new Color(255, 255, 255, 255);
        }
        else
        //turn of frightened animation
        //set color back to inspector color
        {
            animator.SetBool("frightened", false);
            animator.SetBool("frightenedBlinking", false);

            ghostSprite.color = color;
        }

        if (!gameManager.gameIsRunning)
        {
            return;
        }

        //make ghosts blink when power pellet is about to stop
        if (gameManager.powerPelletTimer - gameManager.currentPowerPelletTime <= 3)
        {
            animator.SetBool("frightenedBlinking", true);

        }
        else
        {
            animator.SetBool("frightenedBlinking", false);
        }

        animator.SetBool("moving", true);


        if (testRespawn == true)
        {
            readyToLeaveHome = false;
            ghostNodeState = GhostNodeStatesEnum.respawning;
            testRespawn = false;
        }

        //make ghosts slow down through warps
        if (movementController.currentNode.GetComponent<NodeController>().isSideNode)
        {
            movementController.SetSpeed(1);
        }
        else
        {
            //slow down
            if (isFreightened)
            {
                movementController.SetSpeed(1);
            }
            //speed up
            else if (ghostNodeState == GhostNodeStatesEnum.respawning)
            {
                movementController.SetSpeed(7);
            }
            //regular speed
            else
            {
                movementController.SetSpeed(2);
            }
        }
    }

    public void SetFrightened(bool newIsFrightened)
    {
        isFreightened = newIsFrightened;
    }

    public void ReachedCenterOfNode(NodeController nodeController)
    {
        if (ghostNodeState == GhostNodeStatesEnum.movingInNodes)
        {
            leftHomeBefore = true;
            //Scatter Mode
            if (gameManager.currentGhostMode == GameManager.GhostMode.scatter)
            {
                DetermineGhostScatterModeDirection();
            }
            //Freightened Mode
            else if (isFreightened)
            {
                string direction = GetRandomDirection();
                movementController.SetDirection(direction);
            }
            //Chase Mode
            else
            {
                //determine next game node to go to
                if (ghostType == GhostType.red)
                {
                    DetermineRedGhostDirection();
                }
                else if (ghostType == GhostType.pink)
                {
                    DeterminePinkGhostDirection();
                }
                else if (ghostType == GhostType.blue)
                {
                    DetermineBlueGhostDirection();
                }
                else if (ghostType == GhostType.orange)
                {
                    DetermineOrangeGhostDirection();
                }
            }
            
        }
        else if (ghostNodeState == GhostNodeStatesEnum.respawning)
        {
            string direction = "";
            //if we have reached start node, move to centerNode (back to spawn box)
            if (transform.position.x == ghostNodeStart.transform.position.x && transform.position.y == ghostNodeStart.transform.position.y)
            {
                direction = "down";
            }
            //we have reacher our center node, finish respawn or move to the left/right node
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
            
            movementController.SetDirection(direction);
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
                    movementController.SetDirection("right");
                }
                //if we are in the right homeNode => move to the center
                else if (ghostNodeState == GhostNodeStatesEnum.rightNode)
                {
                    ghostNodeState = GhostNodeStatesEnum.centerNode;
                    movementController.SetDirection("left");
                }
                //if we are in the center node => move to startNode
                else if (ghostNodeState == GhostNodeStatesEnum.centerNode)
                {
                    ghostNodeState = GhostNodeStatesEnum.startNode;
                    movementController.SetDirection("up");
                }
                //if we are in the start node, start moving around in the game
                else if (ghostNodeState == GhostNodeStatesEnum.startNode)
                {
                    ghostNodeState = GhostNodeStatesEnum.movingInNodes;
                    movementController.SetDirection("left");
                }
            }
        }
    }

    void DetermineGhostScatterModeDirection()
    {
        //if we reached the scatter node, add 1 to scatternode index
                if (transform.position.x == scatterNodes[scatterNodeIndex].transform.position.x && transform.position.y == scatterNodes[scatterNodeIndex].transform.position.y)
                {
                    scatterNodeIndex++;
                    //reset index to keep loopioing through scatter nodes
                    if (scatterNodeIndex == scatterNodes.Length - 1)
                    {
                        scatterNodeIndex = 0;
                    }
                }

                string direction = GetClosestDirection(scatterNodes[scatterNodeIndex].transform.position);

                movementController.SetDirection(direction);
    }

    //redGhost logic
    void DetermineRedGhostDirection()
    {
        string direction = GetClosestDirection(gameManager.pacman.transform.position);
        movementController.SetDirection(direction);
    }

    //Pink ghost logic
    void DeterminePinkGhostDirection()
    {
        string pacmansDirection = gameManager.pacman.GetComponent<MovementController>().lastMovingDirection;
        float distanceBetweenNodes = .35f;
        Vector2 target = gameManager.pacman.transform.position;
        if (pacmansDirection == "left")
        {
            target.x -= distanceBetweenNodes * 2;
        }
        else if (pacmansDirection == "right")
        {
            target.x += distanceBetweenNodes * 2;
        }
        else if (pacmansDirection == "up")
        {
            target.y += distanceBetweenNodes * 2;
        }
        else if (pacmansDirection == "down")
        {
            target.y -= distanceBetweenNodes * 2;
        }
        string direction = GetClosestDirection(target);
        movementController.SetDirection(direction);
    }

    //Blue ghost logic
    void DetermineBlueGhostDirection()
    {
        string pacmansDirection = gameManager.pacman.GetComponent<MovementController>().lastMovingDirection;
        float distanceBetweenNodes = .35f;
        Vector2 target = gameManager.pacman.transform.position;
        if (pacmansDirection == "left")
        {
            target.x -= distanceBetweenNodes * 2;
        }
        else if (pacmansDirection == "right")
        {
            target.x += distanceBetweenNodes * 2;
        }
        else if (pacmansDirection == "up")
        {
            target.y += distanceBetweenNodes * 2;
        }
        else if (pacmansDirection == "down")
        {
            target.y -= distanceBetweenNodes * 2;
        }

        GameObject redGhost = gameManager.redGhost;
        float xDistance = target.x - redGhost.transform.position.x;
        float yDistance = target.y - redGhost.transform.position.y;

        Vector2 blueTarget = new Vector2(target.x + xDistance, target.y + yDistance);
        string direction = GetClosestDirection(blueTarget);
        movementController.SetDirection(direction);
    }

    //get random direction based off of possible directions for scatter mode
    string GetRandomDirection()
    {
        List<string> possibleDirections = new List<string>();
        NodeController nodeController = movementController.currentNode.GetComponent<NodeController>();

        if (nodeController.canMoveDown && movementController.lastMovingDirection != "up")
        {
            possibleDirections.Add("down");
        }
        else if (nodeController.canMoveUp && movementController.lastMovingDirection != "down")
        {
            possibleDirections.Add("up");
        }
        else if (nodeController.canMoveRight && movementController.lastMovingDirection != "left")
        {
            possibleDirections.Add("right");
        }
        else if (nodeController.canMoveLeft && movementController.lastMovingDirection != "right")
        {
            possibleDirections.Add("left");
        }

        string direction = "";
        int randomDirectionIndex = Random.Range(0, possibleDirections.Count - 1);
        direction = possibleDirections[randomDirectionIndex];
        return direction;
    }

    //orangeGhost Logic
    void DetermineOrangeGhostDirection()
    {
        float distance = Vector2.Distance(gameManager.pacman.transform.position, transform.position);
        float distanceBetweenNodes = .35f;

        if (distance < 0)
        {
            distance *= -1;
        }
        //if we are within 8 nodes, chase using reds logic
        if ( distance <= distanceBetweenNodes * 8)
        {
            DetermineRedGhostDirection();
        }
        //else enter scatter mode
        else
        {
            //scatter mode
            DetermineGhostScatterModeDirection();
        }
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

    public void SetVisible(bool newIsVisible)
    {
        isVisible = newIsVisible;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && ghostNodeState != GhostNodeStatesEnum.respawning)
        {
            //Get eaten
            if (isFreightened)
            {
                gameManager.GhostEaten();
                ghostNodeState = GhostNodeStatesEnum.respawning;
            }
            //Eat player
            else
            {
                StartCoroutine(gameManager.PlayerEaten());
            }
        }
    }
}
