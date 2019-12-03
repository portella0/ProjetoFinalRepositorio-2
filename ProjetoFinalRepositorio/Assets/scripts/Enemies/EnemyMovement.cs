using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("timer Properties")]
    public float flipTimer;// = 0f;
    public float waitToFlip;// = 0.5f;

    [Header("Movement Properties")]
    public float speed;// = 5f;                //Player speed

    [Header("Environment Check Properties")]
    public float groundCheckOffset;// = 0.4f;          //X Offset of feet raycast
    public float visionHeight;// = -0.5f;          //Height of wall checks
    public float headAboveSpace;// = 1f;       //Space needed above the player's head
    public float groundDistance;// = 0.8f;      //Distance player is considered to be on the ground
    public float grabDistance;// = 0.4f;        //The reach distance for wall grabs
    public float visionDistance;// = 0.4f;       //distance to see the player 
    public float soundDistance;
    public LayerMask groundLayer;           //Layer of the ground
    public bool drawDebugRaycasts = true;   //Should the environment checks be visualized
    public bool OnArea = false;
    public float playerDistance;
    public float checkTimer;
    public bool cliff;

    [Header("Status Flags")]
    public bool isOnGround;                 //Is the player on the ground?
    public bool isHeadBlocked;
    public int direction = 1;  //Direction player is facing
    public bool playerOnArea;  //bool to see player
    public bool OnChase;
    public bool isIdle;
    public bool isHearing;
    public bool isSeeing;
    public float enemyAttackTimer;
    public float enemyMaxAttackTimer;
    public bool nearPlayer;
    public bool isAttacking;

    [Header("Collisions")]
    BoxCollider2D bodyCollider;             //The collider component
    Rigidbody2D rigidBody;                  //The rigidbody component
    public GameObject attackCollider;

    float enemyHeight;                     //Height of the player
    float enemyLenght;

    float originalXScale;   //Original scale on X axis
    public Vector3 targetDirection;

    public Vector3 vel;

    // The target (cylinder) position.
    public Transform target;
    public PlayerMovement playerScript;

    // Start is called before the first frame update
    void Start()
    {
        isIdle = true;

        cliff = false;

        attackCollider.SetActive(false);

        target = GameObject.Find("Character").transform;

        playerScript = GameObject.Find("Character").GetComponent<PlayerMovement>();

        //not near player
        isAttacking = false;
        OnChase = false;
        nearPlayer = false;

        //Start by assuming the player isn't on the ground and the head isn't blocked
        isOnGround = false;
        isHeadBlocked = false;

        //Get a reference to the required components
        rigidBody = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<BoxCollider2D>();

        //Record the original x scale of the player
        originalXScale = transform.localScale.x;

        //start not seeing or hearing player
        playerOnArea = false;
        isHearing = false;
        isSeeing = false;

        //Record the player's height from the collider
        enemyHeight = bodyCollider.size.y;
        enemyLenght = bodyCollider.size.x;
        headAboveSpace = 2*bodyCollider.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        playerDistance = Vector2.Distance(target.position, transform.position);

        if(playerDistance < visionDistance)
        {
            isSeeing = true;
        }
        else
        {
            isSeeing = false;
        }

        if(playerScript.character == 1)
        {
            if (playerDistance < soundDistance/2)
            {
                isHearing = true;
            }
            else
            {
                isHearing = false;
            }
        }
        else if (playerScript.character == 2)
        {
            if (playerDistance < soundDistance / 4)
            {
                isHearing = true;
            }
            else
            {
                isHearing = false;
            }
        }
        else if (playerScript.character == 3)
        {
            if (playerDistance < soundDistance)
            {
                isHearing = true;
            }
            else
            {
                isHearing = false;
            }
        }

        flipTimer = flipTimer + Time.deltaTime;

        //get speed
        vel = rigidBody.velocity;

        if (OnChase)
        {
            isIdle = false;
        }
        else
        {
            isIdle = true;
        }

        EnemyMovementFuntion();
        PhysicsCheck();
        FlipEnemyDirection();
        EnemyAttackFunction();
    }

    void PhysicsCheck()
    {
        //Cast rays for the left and right foot
        //RaycastHit2D leftCheck = Raycast(new Vector2(-groundCheckOffset, -0.5f), Vector2.down, groundDistance);
        //RaycastHit2D rightCheck = Raycast(new Vector2(groundCheckOffset, -0.5f), Vector2.down, groundDistance);

        RaycastHit2D leftCheck = Raycast(new Vector2(-enemyLenght, -enemyHeight), Vector2.down, enemyHeight);
        RaycastHit2D rightCheck = Raycast(new Vector2(enemyLenght, -enemyHeight), Vector2.down, enemyHeight);

        //If either ray hit the ground, the player is on the ground
        if (leftCheck || rightCheck)
        {
            isOnGround = true;
        }

        if (!rightCheck)
        {
            //direction = -direction;;
        }

        if ((!leftCheck && rightCheck)|| (leftCheck && !rightCheck))
        {
            if (cliff)
            {
                checkTimer = 0;
                direction = -direction;
                cliff = false;
            }         
        }

        if (!cliff)
        {
            checkTimer += Time.deltaTime;
            if (checkTimer > 1)
            {
                cliff = true;
            }
        }

        //Determine the direction of the wall grab attempt
        Vector2 grabDir = new Vector2(direction, 0f);

        //Cast the ray to check above the player's head
        RaycastHit2D headCheck = Raycast(new Vector2(0f, bodyCollider.size.y), Vector2.up, headAboveSpace);

        //Cast three rays to look for a wall grab
        RaycastHit2D wallCheck = Raycast(new Vector2(groundCheckOffset * direction, visionHeight), grabDir, grabDistance);     

        if (wallCheck)
        {
            direction = -direction;
        }
    }

    void EnemyMovementFuntion()
    {
        //Calculate the desired velocity based on inputs
        float xVelocity = speed;

        //Apply the desired velocity 
        //rigidBody.velocity = new Vector2(xVelocity, rigidBody.velocity.y);

        targetDirection = (target.transform.position - transform.position).normalized;

        targetDirection.y = 0;

        if (playerOnArea == true)
        { 
            if (targetDirection.x < 0f)
            {
                //if player is in front chase
                if (direction == -1)
                {
                    if (isSeeing == true)
                    {
                        speed = 10;
                        Debug.Log(transform.position);
                        rigidBody.MovePosition(transform.position + targetDirection * speed * Time.deltaTime);
                        OnChase = true;
                    }
                    else if (isSeeing == false)
                    {
                        speed = 5;
                        rigidBody.velocity = new Vector2(1, 0) * speed * direction;
                    }
                }
                //if player is behind just walk
                else if (direction == 1)
                {
                    if (isHearing == true)
                    {
                        speed = 10;
                        Debug.Log(transform.position);
                        rigidBody.MovePosition(transform.position + targetDirection * speed * Time.deltaTime);
                        OnChase = true;
                    }
                    else if (isHearing == false)
                    {
                        speed = 5;
                        rigidBody.velocity = new Vector2(1, 0) * speed * direction;
                    }
                }
            }
            else if (targetDirection.x > 0f)
            {
                //if player is in front chase
                if (direction == 1)
                {
                    if (isSeeing == true)
                    {
                        speed = 10;
                        Debug.Log(transform.position);
                        rigidBody.MovePosition(transform.position + targetDirection * speed * Time.deltaTime);
                        OnChase = true;
                    }
                    else if (isSeeing == false)
                    {
                        speed = 5;
                        rigidBody.velocity = new Vector2(1, 0) * speed * direction;
                    }
                }
                //if player is behind just walk
                else if (direction == -1)
                {
                    if (isHearing == true && !nearPlayer)
                    {
                        speed = 10;
                        Debug.Log(transform.position);
                        rigidBody.MovePosition(transform.position + targetDirection * speed * Time.deltaTime);
                        OnChase = true;
                    }
                    else if (isHearing == false && !nearPlayer)
                    {
                        speed = 5;
                        rigidBody.velocity = new Vector2(1, 0) * speed * direction;
                    }
                }
            }            
        }

        //if does not see player just walk
        else if (playerOnArea == false)
        {
            if (isOnGround)
            {
                speed = 5;
                rigidBody.velocity = new Vector2(1, 0) * speed * direction;
                OnChase = false;
            }
        }
    }

    //flip enemy
    void FlipEnemyDirection()
    {
        if (OnChase)
        {
            //if (xVelocity * direction < 0f)
            if (targetDirection.x < 0f)
            {
                direction = -1;
            }
            else if (targetDirection.x > 0f)
            {
                direction = 1;
            }
        }

        if (direction == 1)
        {
            //Record the current scale
            Vector3 scale = transform.localScale;

            //Set the X scale to be the original times the direction
            scale.x = originalXScale * 1;

            //Apply the new scale
            transform.localScale = scale;
        }
        else if (direction == -1)
        {
            //Record the current scale
            Vector3 scale = transform.localScale;

            //Set the X scale to be the original times the direction
            scale.x = originalXScale * -1;

            //Apply the new scale
            transform.localScale = scale;
        }      
    }

    public void EnemyAttackFunction()
    {
        //golem attack
        if (enemyAttackTimer < enemyMaxAttackTimer && nearPlayer)
        {
            isAttacking = true;
        }

        if (isAttacking == true && enemyAttackTimer < enemyMaxAttackTimer)
        {
            enemyAttackTimer += 1 * Time.deltaTime;
            if (enemyAttackTimer >= 0.15)
            {
                attackCollider.SetActive(true);
            }
        }

        else if (enemyAttackTimer >= enemyMaxAttackTimer)
        {
            isAttacking = false;
            attackCollider.SetActive(false);
            enemyAttackTimer += 1 * Time.deltaTime;
            if (enemyAttackTimer >= enemyMaxAttackTimer)
            {
                enemyAttackTimer = 0;
            }
        }
    }

    //These two Raycast methods wrap the Physics2D.Raycast() and provide some extra
    //functionality
    RaycastHit2D Raycast(Vector2 offset, Vector2 rayDirection, float length)
    {
        //Call the overloaded Raycast() method using the ground layermask and return 
        //the results
        return Raycast(offset, rayDirection, length, groundLayer);
    }

    RaycastHit2D Raycast(Vector2 offset, Vector2 rayDirection, float length, LayerMask mask)
    {
        //Record the player's position
        Vector2 pos = transform.position;

        //Send out the desired raycasr and record the result
        RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDirection, length, mask);

        //If we want to show debug raycasts in the scene...
        if (drawDebugRaycasts)
        {
            //...determine the color based on if the raycast hit...
            Color color = hit ? Color.red : Color.green;
            //...and draw the ray in the scene view
            Debug.DrawRay(pos + offset, rayDirection * length, color);
        }

        //Return the results of the raycast
        return hit;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Character")
        {
            nearPlayer = true;
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        //check if enemy is inside his area
        if (collision.gameObject.name == "EnemyArea")
        {
            OnArea = true;
        }
        if (collision.gameObject.name == "Character")
        {
            nearPlayer = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        //check if enemy exits his area
        if (collision.gameObject.name == "EnemyArea")
        {
            OnArea = false;
            if (flipTimer >= waitToFlip)
            {
                direction = direction * -1;
                flipTimer = 0f;
            }
        }

        if (collision.gameObject.name == "Character")
        {
            nearPlayer = false;
        }
    }
}
