using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapMovement : MonoBehaviour
{
    [Header("timer Properties")]
    public float flipTimer;// = 0f;
    public float waitToFlip;// = 0.5f;

    [Header("Movement Properties")]
    public float speed;// = 5f;                //Player speed

    [Header("Environment Check Properties")]
    public bool OnArea = false;

    [Header("Status Flags")]
    public bool playerOnArea;  //bool to see player
    public bool OnChase;    //is chasing the player
    public int direction;  //Direction player is facing

    [Header("Movement Modes")]
    public bool UpDown;     //should go up and down
    public bool chaseMode;  //should chase the player
    public bool flipMode;   //should flip the sprite

    [Header("Collisions")]
    BoxCollider2D bodyCollider;             
    Rigidbody2D rigidBody;                  

    float enemyHeight;                     //Height of the player

    float originalXScale;   //Original scale on X axis
    public Vector3 targetDirection;

    public Vector3 vel;

    // The target (cylinder) position.
    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Character").transform;

        OnChase = false;

        rigidBody = GetComponent<Rigidbody2D>();

        bodyCollider = GetComponent<BoxCollider2D>();
        enemyHeight = bodyCollider.size.y;


        originalXScale = transform.localScale.x;

        //start not seeing player
        playerOnArea = false;
    }

    void Update()
    {
        vel = rigidBody.velocity;

        flipTimer = flipTimer + Time.deltaTime;

        TrapMovementFuntion();
        if (flipMode)
        {
            FlipTrapDirection();
        }
    }

    void TrapMovementFuntion()
    {
        //Calculate the desired velocity based on inputs
        float xVelocity = speed;

        targetDirection = (target.transform.position - transform.position).normalized;

        if (UpDown)
        {
            if (chaseMode)
            {
                if (playerOnArea == true)
                {
                    speed = 5;
                    //rigidBody.MovePosition(target.transform.position * Time.deltaTime + target.transform.forward * Time.deltaTime);
                    // Debug.Log(targetDirection);
                    Debug.Log(transform.position);
                    rigidBody.MovePosition(new Vector2(0, transform.position.y + targetDirection.y) * speed * Time.deltaTime);
                    OnChase = true;
                }
                //if does not see player just walk
                else if (playerOnArea == false)
                {
                    rigidBody.velocity = new Vector2(0, 1) * speed * direction;
                    OnChase = false;
                }
            }
            //if it is not on chasemode
            else
            {
                rigidBody.velocity = new Vector2(0, 1) * speed * direction;
                OnChase = false;
            }
        }
        else
        {
            if (chaseMode)
            {
                if (playerOnArea == true)
                {
                    speed = 5;
                    //rigidBody.MovePosition(target.transform.position * Time.deltaTime + target.transform.forward * Time.deltaTime);
                    // Debug.Log(targetDirection);
                    Debug.Log(transform.position);
                    rigidBody.MovePosition(transform.position + targetDirection * speed * Time.deltaTime);
                    OnChase = true;
                }
                //if does not see player just walk
                else if (playerOnArea == false)
                {
                    rigidBody.velocity = new Vector2(1, 0) * speed * direction;
                    OnChase = false;
                }
            }
            //if it is not on chasemode
            else
            {
                rigidBody.velocity = new Vector2(1, 0) * speed * direction;
                OnChase = false;
            }
        }
    }

    //flip enemy
    void FlipTrapDirection()
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

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "TrapArea")
        {            
            OnArea = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "TrapArea")
        {
            OnArea = false;
            if (flipTimer >= waitToFlip)
            {
                direction = direction * -1;
                flipTimer = 0f;
            }
        }
    }
}

