using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public MovementController movementController;
    public SpriteRenderer sprite;
    public Animator animator;
    public GameObject startNode;
    public Vector2 startPos;
    public GameManager gameManager;
    public bool isDead = false;

    // Start is called before the first frame update
    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        startPos = new Vector2(-.06f, -.921f);
        animator = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        movementController = GetComponent<MovementController>();
        startNode = movementController.currentNode;
    }

    //reset pacman animations, speed, and position
    public void SetUp()
    {
        isDead = false;
        animator.SetBool("dead", false);
        animator.SetBool("moving", false);
        movementController.currentNode = startNode;
        movementController.direction = "left";
        movementController.lastMovingDirection = "left";
        sprite.flipX = false;
        transform.position = startPos;
        animator.speed = 1;
    }

    //stop pacmans animation
    public void Stop()
    {
        animator.speed = 0;

    }

    // Update is called once per frame
    void Update()
    {
        //check that game is running
        if (!gameManager.gameIsRunning)
        {
            //stop animation if pacman dies
            if (!isDead)
            {
                animator.speed = 0;
            }
            //return if game is not running
            return;
        }
        //set pacman animation
        animator.speed = 1;
        animator.SetBool("moving", true);
        //check for mevement input
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            movementController.SetDirection("left");
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            movementController.SetDirection("right");
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            movementController.SetDirection("up");
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            movementController.SetDirection("down");
        }

        bool flipX = false;
        bool flipY = false;
        //direction to 0 =left or right
        //if we go right flip left animation horizontally
        if (movementController.lastMovingDirection == "left")
        {
            animator.SetInteger("direction", 0);
        }
        else if (movementController.lastMovingDirection == "right")
        {
            animator.SetInteger("direction", 0);
            flipX = true;
        }
        //direction 1 = up or down
        //if we go down flip animation vertically
        else if (movementController.lastMovingDirection == "up")
        {
            animator.SetInteger("direction", 1);
        }
        else if (movementController.lastMovingDirection == "down")
        {
            animator.SetInteger("direction", 1);
            flipY = true;
        }

        sprite.flipY = flipY;
        sprite.flipX = flipX;
    }

    //pacman is dead
    public void Death()
    {
        isDead = true;
        animator.SetBool("moving", false);
        animator.speed = 1;
        animator.SetBool("dead", true);
    }
}
