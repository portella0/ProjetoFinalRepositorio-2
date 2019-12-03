using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public bool drawDebugRaycasts = true;   //Should the environment checks be visualized

    [Header("Camera")]
    public CinemachineImpulseSource cameraShake;
    public GameObject follow, upCamera, downCamera, centerCamera;
    //public bool lookingHeight;

    [Header("Interaction")]
    public GameObject infoText;
    Vector3 scaleText;

    [Header("Characters")]
    public int character;
    public GameObject skeleton;
    public GameObject archer;
    public GameObject arrowShot;
    public Transform arrow;
    public GameObject golem;
    public bool GolemShake;
    public float ShakeTimer;
    public float golemJumpTimer;
    public GameObject golemAttackCollider;
    public float colliderSkeletonX, colliderSkeletonY, skeletonOffsetX, skeletonOffsetY, colliderArcherX, colliderArcherY, archerOffsetX, archerOffsetY, colliderGolemX, colliderGolemY,  golemOffsetX, golemOffsetY;
    public bool canChange;

    [Header("Movement Properties")]
    public float speed = 5f;                //Player speed
    public float crouchSpeedDivisor = 3f;   //Speed reduction when crouching
    public float coyoteDuration =0.05f;     //How long the player can jump after falling
    public float maxFallSpeed = -50f;       //Max speed player can fall
    public float slideSpeed = 10f;           //Slide speed
    public float slideTime = 0.5f;           //Slide timer
    public float slideTimeMax = 0.5f;        //Slide maximum time
    public float slideCooldown = 3f;        //cooldown for the slide 
    public float mistTimer;
    public float mistMaxTimer;
    public float golemAttackTimer;
    public float golemMaxAttackTimer;

    [Header("Jump Properties")]
    public float jumpForce = 90f;          //Initial force of jump
    public float crouchJumpBoost = 20f;    //Jump boost when crouching
    public float hangingJumpForce = 100f;    //Force of wall hanging jumo
    public float jumpHoldForce = 0.5f;      //Incremental force when jump is held
    public float jumpHoldDuration = 0.1f;    //How long the jump key can be held

    [Header("Environment Check Properties")]
    public float groundCheckOffset = 0.4f;          //X Offset of feet raycast
    public float visionHeight = 1.6f;          //Height of wall checks
    public float reachOffset = 0.8f;         //X offset for wall grabbing
    public float headAboveSpace = 1f;       //Space needed above the player's head
    public float groundDistance = 0.3f;      //Distance player is considered to be on the ground
    public float grabDistance = 0.8f;        //The reach distance for wall grabs
    public LayerMask groundLayer;           //Layer of the ground
    public LayerMask traplayer;           //Layer of the ground
    RaycastHit2D headCheck;

    [Header("Status Flags")]
    public bool isOnGround;                 //Is the player on the ground?
    public bool isJumping;                  //Is player jumping?
    public bool isHanging;                  //Is player hanging?
    public bool isCrouching;                //Is player crouching?
    public bool isHeadBlocked;
    public bool isSliding;
    public bool isStandingUp;
    public bool isBlockingGolem;
    public bool isBlockingSkeleton;
    public bool isMist;
    public bool isNearWall;
    public bool isGolemAttack;
    public bool isMoving;
    //public bool isLookingUp;
    //public bool isLookingDown;

    PlayerInput input;                      //The current inputs for the player
    BoxCollider2D bodyCollider;             //The collider component
    Rigidbody2D rigidBody;                  //The rigidbody component
    public PlayerAnimations animate;               //The animation script
    public PlayerHealth playerHealth;               //The animation script

    float jumpTime;                         //Variable to hold jump duration
    float coyoteTime;                       //Variable to hold coyote duration
    float playerHeight;                     //Height of the player

    float originalXScale;                   //Original scale on X axis
    int direction = 1;                      //Direction player is facing

    [Header("Movement Colliders")]
    public Vector2 colliderStandSize;              //Size of the standing collider
    public Vector2 colliderStandOffset;            //Offset of the standing collider
    public Vector2 colliderCrouchSize;             //Size of the crouching collider
    public Vector2 colliderCrouchOffset;           //Offset of the crouching collider
    public Vector2 colliderSlideSize;             //Size of the sliding collider
    public Vector2 colliderSlideOffset;           //Offset of the sliding collider

    const float smallAmount = .05f;         //A small amount used for hanging position

    public Vector3 vel;

    void Start()
    {
        //not looking up or down
        //lookingHeight = false;

        //set screen shake to false
        GolemShake = false;
        ShakeTimer = 1;
        golemJumpTimer = 0.05f;
        golemAttackCollider.SetActive(false);

        //get impulse script
        cameraShake = GetComponent<CinemachineImpulseSource>();

        //Get a reference to the required components
        input = GetComponent<PlayerInput>();
        rigidBody = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<BoxCollider2D>();
        playerHealth = GetComponent<PlayerHealth>();

        //Record the original x scale of the player
        originalXScale = transform.localScale.x;
        scaleText = infoText.transform.localScale;
        infoText.SetActive(false);

        //Record the player's height from the collider
        playerHeight = bodyCollider.size.y;

        //Record initial collider size and offset
        colliderStandSize = bodyCollider.size;
        colliderStandOffset = bodyCollider.offset;

        //character is not in any specific state
        isHanging = false;
        isBlockingGolem = false;
        isBlockingSkeleton = false;
        isMist = false;
        isNearWall = false;

        //slide cooldown
        slideTimeMax = slideTime;

        //Calculate crouching collider size and offset
        //colliderCrouchSize = new Vector2(bodyCollider.size.x, bodyCollider.size.y / 2.05f);
        //colliderCrouchOffset = new Vector2(bodyCollider.offset.x, bodyCollider.offset.y / 2.05f);

        colliderCrouchSize = new Vector2(1.5f, 0.7f);
        colliderCrouchOffset = new Vector2(-0.1f, 0.35f);

        colliderSlideSize = new Vector2(1.5f, 0.7f);
        colliderSlideOffset = new Vector2(0.55f, 0.35f);

        //These are the starting sizes for the Collider component
        //skeleton
        colliderSkeletonX = 0.6684278f;
        colliderSkeletonY = 1.944759f;
        skeletonOffsetX = 0.05300087f;
        skeletonOffsetY = 0.9430555f;
        //archer
        colliderArcherX = 0.8f;
        colliderArcherY = 1.9f;
        archerOffsetX = 0.05f;
        archerOffsetY = 0.9f;
        //golem
        //colliderGolemX = 1.56f;
        colliderGolemX = 1.9f;
        colliderGolemY = 2.6f;
        //golemOffsetX = 0f;
        golemOffsetX = 0.2f;
        golemOffsetY = 1.26f;

        //Give the PlayerPrefs some values to send over to the next Scene
        character = PlayerPrefs.GetInt("Character", 1);

        //character use
        if (character == 1)
        {
            SkeletonActive();
        }
        else if (character == 2)
        {
            ArcherActive();
        }
        if (character == 3)
        {
            GolemActive();
        }
    }

    void FixedUpdate()
    {
        //get speed
        vel = rigidBody.velocity;

        //Check the environment to determine status
        PhysicsCheck();

        //Process skeleton ground and air movements
        if (character == 1)
        {
            SkeletonGroundMovement();
            SkeletonMidAirMovement();
        }
        else if (character == 2)
        {
            ArcherGroundMovement();
            ArcherMidAirMovement();
        }
        else if (character == 3)
        {
            GolemGroundMovement();
            GolemMidAirMovement();
        }

        if (input.horizontal != 0)
        {
            isMoving = true;
        }
        else if (input.horizontal == 0)
        {
            isMoving = false;
        }

        //move camera
        if (Input.GetKey(KeyCode.UpArrow))
        {
            //if (!isHanging || !isCrouching || !isSliding)
            //{
            //    isLookingUp = true;
            //    lookingHeight = true;
            //}
            follow.transform.position = Vector2.Lerp(follow.transform.position, upCamera.transform.position, 5 * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            //if (!isHanging || !isCrouching || !isSliding)
            //{
            //    isLookingDown = true;
            //    lookingHeight = true;
            //}
            follow.transform.position = Vector2.Lerp(follow.transform.position, downCamera.transform.position, 5 * Time.deltaTime);
        }
        else
        {
            //isLookingUp = false;
            //isLookingDown = false;
            //lookingHeight = false;
            follow.transform.position = Vector2.Lerp(follow.transform.position, centerCamera.transform.position, 5 * Time.deltaTime);
        }

        if (playerHealth.isAlive)
        {
            if (canChange)
            {
                //change character
                if (Input.GetKeyDown(KeyCode.Alpha1) && !isHanging && !isCrouching && character != 1)
                {
                    animate.ResetAnimation();
                    SkeletonActive();
                }
                if (Input.GetKeyDown(KeyCode.Alpha2) && !isHanging && !isCrouching && character != 2)
                {
                    animate.ResetAnimation();
                    ArcherActive();
                }
                if (Input.GetKeyDown(KeyCode.Alpha3) && !isHanging && !isHeadBlocked && character != 3)
                {
                    animate.ResetAnimation();
                    GolemActive();
                }
            }
        }
    }

    void PhysicsCheck()
    {
        //Start by assuming the player isn't on the ground and the head isn't blocked
        isOnGround = false;
        isHeadBlocked = false;

        if(!playerHealth.isAlive)
        {
            rigidBody.bodyType = RigidbodyType2D.Static;
        }
        else if (playerHealth.isAlive && !isHanging)
        {
            rigidBody.bodyType = RigidbodyType2D.Dynamic;
        }

        //Cast rays for the left and right foot
        RaycastHit2D leftCheck = Raycast(new Vector2(-groundCheckOffset, 0f), Vector2.down, groundDistance);
        RaycastHit2D rightCheck = Raycast(new Vector2(groundCheckOffset, 0f), Vector2.down, groundDistance);

        //If either ray hit the ground, the player is on the ground
        if (leftCheck || rightCheck)
        {
            isOnGround = true;
            if (character == 3 && GolemShake == true)
            {
                rigidBody.bodyType = RigidbodyType2D.Static;
                golemJumpTimer -= Time.deltaTime;
                if (golemJumpTimer < 0)
                {
                    rigidBody.bodyType = RigidbodyType2D.Dynamic;
                    golemJumpTimer = 0.05f;

                    cameraShake.GenerateImpulse();
                    ShakeTimer = 1;
                    GolemShake = false;
                }

            }
        }

        //Cast the ray to check above the player's head
        headCheck = Raycast(new Vector2(0f, bodyCollider.size.y), Vector2.up, headAboveSpace);

        //If that ray hits, the player's head is blocked
        if (headCheck)
            isHeadBlocked = true;

        //Determine the direction of the wall grab attempt
        Vector2 grabDir = new Vector2(direction, 0f);

        //Cast three rays to look for a wall grab
        RaycastHit2D blockedCheck = Raycast(new Vector2(groundCheckOffset * direction, playerHeight), grabDir, grabDistance);
        RaycastHit2D ledgeCheck = Raycast(new Vector2(reachOffset * direction, playerHeight), Vector2.down, grabDistance);
        RaycastHit2D wallCheck = Raycast(new Vector2(groundCheckOffset * direction, visionHeight), grabDir, grabDistance);

        if (ledgeCheck.collider != null)
        {
            Debug.Log(ledgeCheck.collider.name);
        }

        //If the player is off the ground AND is not hanging AND is falling AND
        //found a ledge AND found a wall AND the grab is NOT blocked...
        if (!isOnGround && !isHanging && rigidBody.velocity.y < 0f &&
        ledgeCheck && wallCheck && !blockedCheck)
        {
            if (character == 1)
            {
                //...we have a ledge grab. Record the current position...
                Vector3 pos = transform.position;
                //...move the distance to the wall (minus a small amount)...
                pos.x += (wallCheck.distance - smallAmount) * direction;
                //...move the player down to grab onto the ledge...
                pos.y -= ledgeCheck.distance;
                //...apply this position to the platform...
                transform.position = pos;
                //...set the rigidbody to static...
                rigidBody.bodyType = RigidbodyType2D.Static;
                //...finally, set isHanging to true

                isHanging = true;
            }
        }
    }

    public void SkeletonActive()
    {
        animate = skeleton.GetComponent<PlayerAnimations>();

        //save character
        PlayerPrefs.SetInt("Character", 1);
        character = 1;

        //acitivate sprite
        skeleton.SetActive(true);
        archer.SetActive(false);
        golem.SetActive(false);

        //change collider
        bodyCollider.size = new Vector2(colliderSkeletonX, colliderSkeletonY);
        bodyCollider.offset = new Vector2(skeletonOffsetX, skeletonOffsetY);
        updateCollider();

        //change weight
        rigidBody.mass = 5;

        //change movement properties
        speed = 5f;                //Player speed
        crouchSpeedDivisor = 3f;   //Speed reduction when crouching
        maxFallSpeed = -50f;       //Max speed player can fall
        slideSpeed = 10f;           //Slide speed
        slideTime = 0.5f;           //Slide time
        slideTimeMax = 0.5f;           //Slide time
        slideCooldown = 3f;        //cooldown for the slide 

        //change jump properties
        jumpForce = 90f;          //Initial force of jump
        hangingJumpForce = 100f;    //Force of wall hanging jumo
        jumpHoldForce = 0.5f;      //Incremental force when jump is held
        jumpHoldDuration = 0.1f;    //How long the jump key can be held

        //change raycast
        groundCheckOffset = 0.4f;          //X Offset of feet raycast
        visionHeight = 1.65f;          //Height of wall checks
        reachOffset = 1f;         //X offset for wall grabbing
        headAboveSpace = 0.9f;       //Space needed above the player's head
        groundDistance = 0.3f;      //Distance player is considered to be on the ground
        grabDistance = 0.8f;        //The reach distance for wall grabs
    }

    public void ArcherActive()
    {
        animate = archer.GetComponent<PlayerAnimations>();

        //save character
        PlayerPrefs.SetInt("Character", 2);
        character = 2;

        //acitivate sprite
        skeleton.SetActive(false);
        archer.SetActive(true);
        golem.SetActive(false);

        //change collider
        bodyCollider.size = new Vector2(colliderArcherX, colliderArcherY);
        bodyCollider.offset = new Vector2(archerOffsetX, archerOffsetY);
        updateCollider();

        //change weight
        rigidBody.mass = 2;

        //change movement properties
        speed = 7f;                //Player speed
        crouchSpeedDivisor = 3f;   //Speed reduction when crouching
        maxFallSpeed = -10f;       //Max speed player can fall
        slideSpeed = 10f;           //Slide speed
        slideTime = 1f;           //Slide time
        slideTimeMax = 1f;           //Slide time
        slideCooldown = 3f;        //cooldown for the slide 

        //change jump properties
        jumpForce = 35f;          //Initial force of jump
        jumpHoldForce = 0.5f;      //Incremental force when jump is held
        jumpHoldDuration = 0.1f;    //How long the jump key can be held

        //change raycast
        groundCheckOffset = 0.4f;          //X Offset of feet raycast
        visionHeight = 1.36f;          //Height of wall checks
        reachOffset = 0.8f;         //X offset for wall grabbing
        headAboveSpace = 0.9f;       //Space needed above the player's head
        groundDistance = 0.3f;      //Distance player is considered to be on the ground
        grabDistance = 0.4f;        //The reach distance for wall grabs
    }

    public void GolemActive()
    {
        animate = golem.GetComponent<PlayerAnimations>();

        //save character
        PlayerPrefs.SetInt("Character", 3);
        character = 3;

        //activate sprite
        skeleton.SetActive(false);
        archer.SetActive(false);
        golem.SetActive(true);

        //change collider
        bodyCollider.size = new Vector2(colliderGolemX, colliderGolemY);
        bodyCollider.offset = new Vector2(golemOffsetX, golemOffsetY);
        updateCollider();
        golemAttackCollider.SetActive(false);

        //change weight
        rigidBody.mass = 10;

        //change movement properties
        speed = 1.5f;                //Player speed
        crouchSpeedDivisor = 3f;   //Speed reduction when crouching
        maxFallSpeed = -10f;       //Max speed player can fall
        slideSpeed = 10f;           //Slide speed
        slideTime = 1f;           //Slide time
        slideTimeMax = 1f;           //Slide time
        slideCooldown = 3f;        //cooldown for the slide 

        //change jump properties
        jumpForce = 130f;          //Initial force of jump
        crouchJumpBoost = 20f;    //Jump boost when crouching
        hangingJumpForce = 100f;    //Force of wall hanging jumo
        jumpHoldForce = 0.5f;      //Incremental force when jump is held
        jumpHoldDuration = 0.1f;    //How long the jump key can be held

        //change raycast
        groundCheckOffset = 0.75f;          //X Offset of feet raycast
        visionHeight = 2.3f;          //Height of wall checks
        reachOffset = 1.7f;         //X offset for wall grabbing
        headAboveSpace = 0.5f;       //Space needed above the player's head
        groundDistance = 0.1f;      //Distance player is considered to be on the ground
        grabDistance = 1.5f;        //The reach distance for wall grabs
    }

    //updates the collider size
    void updateCollider()
    {
        colliderStandSize = bodyCollider.size;
        colliderStandOffset = bodyCollider.offset;

        //colliderCrouchSize = new Vector2(bodyCollider.size.x, bodyCollider.size.y / 2.05f);
        //colliderCrouchOffset = new Vector2(bodyCollider.offset.x, bodyCollider.offset.y / 2.05f);
        colliderCrouchSize = new Vector2(1.5f, 0.7f);
        colliderCrouchOffset = new Vector2(-0.1f, 0.35f);

        colliderSlideSize = new Vector2(1.5f, 0.7f);
        colliderSlideOffset = new Vector2(0.55f, 0.35f);

        playerHeight = bodyCollider.size.y;
    }

    void SkeletonGroundMovement()
    {
        //If currently hanging, the player can't move to exit
        if (isHanging)
            return;

        //Handle crouching input. If holding the crouch button but not crouching, crouch
        if (input.crouchHeld && !isCrouching && isOnGround && !isSliding)
            Crouch();
        //Otherwise, if not sliding and is crouching and pressing the slide button, not slide
        else if (input.runHeld && input.crouchHeld && slideTime > 0)
            Slide();
        else if (!input.runHeld && isSliding && input.crouchHeld)
            Crouch();
        else if (!input.runHeld && isSliding && input.crouchHeld && slideTime <= 0)
            Crouch();
        //Otherwise, if  sliding and is pressing the crouch button and no more slide time, crouch
        else if (input.runHeld && input.crouchHeld && slideTime <= 0)
            Crouch();
        else if (input.runHeld && isHeadBlocked && slideTime <= 0 && !isStandingUp)
            Crouch();
        else if (!input.runHeld && isHeadBlocked && slideTime <= 0 && !isStandingUp)
            Crouch();
        //Otherwise, if not sliding and head is blocked and pressing the slide button, slide
        else if (input.runHeld && isHeadBlocked && slideTime > 0)
            Slide();
        //Otherwise, if not holding crouch but currently crouching, stand up
        else if (!input.crouchHeld && isCrouching)
            StandUp();
        //Otherwise, if crouching and no longer on the ground, stand up
        else if (!isOnGround && isCrouching)
            StandUp();
        //Otherwise, if not sliding and holding the slide button, slide
        else if (input.runHeld && !isSliding && slideTime > 0 && isOnGround)
            Slide();
        //Otherwise, if not holding the sliding button but sliding, stand up
        else if (!input.runHeld && isSliding)
            StandUp();
        //Otherwise, if  holding the sliding button but no more time, stand up
        else if (input.runHeld && slideTime <= 0)
            StandUp();
        //Otherwise, if not on ground but sliding, stand up
        else if (!isOnGround && isSliding)
            StandUp();
        //Otherwise, if not holding the crouch button and sliding nut no more time
        //and space to stand, crouch 
        else if (isHeadBlocked && isSliding && slideTime <= 0 && !isStandingUp)
            Crouch();

        //Calculate the desired velocity based on inputs
        float xVelocity = speed * input.horizontal;

        //if (lookingHeight)
        //{
        //    xVelocity = 0;
        //}

        //If the sign of the velocity and direction don't match, flip the character
        if (xVelocity * direction < 0f)
            FlipCharacterDirection();

        //If the player is crouching, reduce the velocity
        if (isCrouching)
        {
            xVelocity /= crouchSpeedDivisor;
        }

        //If the player is sliding, increase the velocity
        if (isSliding)
        {
            if (slideTime > slideTimeMax/2)
            {
                xVelocity = slideSpeed * input.horizontal;
            }
            else if (slideTime < slideTimeMax / 2 && slideTime > slideTimeMax / 3)
            {
                xVelocity = (slideSpeed - 3) * input.horizontal;
            }
            else if (slideTime < slideTimeMax / 3)
            {
                xVelocity = (slideSpeed - 5) * input.horizontal;
            }
        }

        //stop sliding after time
        if (isSliding)
        {
            slideTime -= 1 * Time.deltaTime;
            headCheck = Raycast(new Vector2(-bodyCollider.size.x * -direction, bodyCollider.size.y), Vector2.up, headAboveSpace + 1);
        }

        if (slideTime < slideTimeMax && slideTime > 0 && !isSliding)
        {
            slideTime += 0.2f * Time.deltaTime;
        }

        //if slide is over enter cooldown
        if (slideTime <= 0)
        {
            slideCooldown -= 1 * Time.deltaTime;
        }

        //cooldown time
        if (slideCooldown <= 0)
        {
            slideCooldown = 3;
            slideTime = slideTimeMax;
        }


        //Apply the desired velocity 
        rigidBody.velocity = new Vector2(xVelocity, rigidBody.velocity.y);
        
        //If the player is on the ground, extend the coyote time window
        if (isOnGround)
            coyoteTime = Time.time + coyoteDuration;
    }

    void ArcherGroundMovement()
    {
        //If currently hanging, the player can't move to exit
        if (isHanging)
            return;

        //shoot arrow
        //if (input.runPressed)
        //{
        //    if(transform.localScale.x == 1)
        //    {
        //        Instantiate(arrow, arrowShot.transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
        //    }
        //    else if (transform.localScale.x == -1)
        //    {
        //        Instantiate(arrow, arrowShot.transform.position, Quaternion.Euler(new Vector3(0, 180, 0)));
        //    }
        //}

        //becomes invisible
        if (input.crouchPressed && mistTimer < mistMaxTimer)
        {
            isMist = true;
        }
        if (isMist == true && mistTimer < mistMaxTimer)
        {
            mistTimer += 1 * Time.deltaTime;
        }
        else if (mistTimer >= mistMaxTimer)
        {
            isMist = false;
            mistTimer += 1 * Time.deltaTime;
            if(mistTimer >= mistMaxTimer *3)
            {
                mistTimer = 0;
            }
        }

        //Calculate the desired velocity based on inputs
        float xVelocity = speed * input.horizontal;

        //If the sign of the velocity and direction don't match, flip the character
        if (xVelocity * direction < 0f)
            FlipCharacterDirection();

        //Apply the desired velocity 
        rigidBody.velocity = new Vector2(xVelocity, rigidBody.velocity.y);

        //If the player is on the ground, extend the coyote time window
        if (isOnGround)
            coyoteTime = Time.time + coyoteDuration;
    }

    void GolemGroundMovement()
    {
        //If currently hanging, the player can't move to exit
        if (isHanging)
            return;

        //Calculate the desired velocity based on inputs
        float xVelocity = speed * input.horizontal;

        //If the sign of the velocity and direction don't match, flip the character
        if (xVelocity * direction < 0f)
            FlipCharacterDirection();

        //Apply the desired velocity 
        rigidBody.velocity = new Vector2(xVelocity, rigidBody.velocity.y);

        //If the player is on the ground, extend the coyote time window
        if (isOnGround)
        {
            coyoteTime = Time.time + coyoteDuration;
        }

        //golem attack
        if ((input.runPressed || input.runHeld) && golemAttackTimer < golemMaxAttackTimer)
        {
            isGolemAttack = true;
        }

        if (isGolemAttack == true && golemAttackTimer < golemMaxAttackTimer)
        {
            golemAttackTimer += 1 * Time.deltaTime;
            rigidBody.bodyType = RigidbodyType2D.Static;
            if(golemAttackTimer >= 0.3)
            {
                golemAttackCollider.SetActive(true);
            }
        }

        else if (golemAttackTimer >= golemMaxAttackTimer)
        {
            isGolemAttack = false;
            golemAttackCollider.SetActive(false);
            rigidBody.bodyType = RigidbodyType2D.Dynamic;
            golemAttackTimer += 1 * Time.deltaTime;
            if (golemAttackTimer >= golemMaxAttackTimer)
            {
                golemAttackTimer = 0;
            }
        }
    }

    void SkeletonMidAirMovement()
    {
        //If the player is currently hanging...
        if (isHanging)
        {
            //If crouch is pressed...
            if (input.crouchPressed)
            {
                //...let go...
                isHanging = false;
                //...set the rigidbody to dynamic and exit
                rigidBody.bodyType = RigidbodyType2D.Dynamic;
                return;
            }

            //If jump is pressed...
            if (input.jumpPressed)
            {
                //...let go...
                isHanging = false;
                //...set the rigidbody to dynamic and apply a jump force...
                rigidBody.bodyType = RigidbodyType2D.Dynamic;
                rigidBody.AddForce(new Vector2(0f, hangingJumpForce), ForceMode2D.Impulse);
                //...and exit
                return;
            }
        }

        //If the jump key is pressed AND the player isn't already jumping AND EITHER
        //the player is on the ground or within the coyote time window...
        if (input.jumpPressed && !isJumping && (isOnGround || coyoteTime > Time.time) && !isHeadBlocked)
        {
            //...check to see if crouching AND not blocked. If so...
            //if (isCrouching && !isHeadBlocked)
            //{
            //    //...stand up and apply a crouching jump boost
            //    StandUp();
            //    rigidBody.AddForce(new Vector2(0f, crouchJumpBoost), ForceMode2D.Impulse);
            //}

            //...The player is no longer on the groud and is jumping...
            isOnGround = false;
            isJumping = true;

            //...record the time the player will stop being able to boost their jump...
            jumpTime = Time.time + jumpHoldDuration;

            //...add the jump force to the rigidbody...
            rigidBody.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);

            //...and tell the Audio Manager to play the jump audio
            //SoundControl.PlayJumpAudio();
        }
        //Otherwise, if currently within the jump time window...
        else if (isJumping)
        {
            //...and the jump button is held, apply an incremental force to the rigidbody...
            if (input.jumpHeld)
                rigidBody.AddForce(new Vector2(0f, jumpHoldForce), ForceMode2D.Impulse);

            //...and if jump time is past, set isJumping to false
            if (jumpTime <= Time.time)
                isJumping = false;
        }

        //If player is falling to fast, reduce the Y velocity to the max
        if (rigidBody.velocity.y < maxFallSpeed)
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, maxFallSpeed);
    }

    void ArcherMidAirMovement()
    {
        //if space is hold, glide
        if (Input.GetKey(KeyCode.Space))
        {
            maxFallSpeed = -1;
        }
        else
        {
            maxFallSpeed = -10;
        } 

        //If the jump key is pressed AND the player isn't already jumping AND EITHER
        //the player is on the ground or within the coyote time window...
        if (input.jumpPressed && !isJumping && (isOnGround || coyoteTime > Time.time)&& !isHeadBlocked)
        {
            //...check to see if crouching AND not blocked. If so...
            //if (isCrouching && !isHeadBlocked)
            //{
            //    //...stand up and apply a crouching jump boost
            //    StandUp();
            //    rigidBody.AddForce(new Vector2(0f, crouchJumpBoost), ForceMode2D.Impulse);
            //}

            //...The player is no longer on the groud and is jumping...
            isOnGround = false;
            isJumping = true;

            //...record the time the player will stop being able to boost their jump...
            jumpTime = Time.time + jumpHoldDuration;

            //...add the jump force to the rigidbody...
            rigidBody.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);

            //...and tell the Audio Manager to play the jump audio
            //SoundControl.PlayJumpAudio();
        }
        //Otherwise, if currently within the jump time window...
        else if (isJumping)
        {
            //...and the jump button is held, apply an incremental force to the rigidbody...
            if (input.jumpHeld)
                rigidBody.AddForce(new Vector2(0f, jumpHoldForce), ForceMode2D.Impulse);

            //...and if jump time is past, set isJumping to false
            if (jumpTime <= Time.time)
                isJumping = false;
        }

        //If player is falling to fast, reduce the Y velocity to the max
        if (rigidBody.velocity.y < maxFallSpeed)
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, maxFallSpeed);
    }

    void GolemMidAirMovement()
    {
        //shake ground jump control
        if (!isOnGround)
        {
            GolemShake = true;
            ShakeTimer += 1 * Time.deltaTime;
            isGolemAttack = false;
        }

        //If the jump key is pressed AND the player isn't already jumping AND EITHER
        //the player is on the ground or within the coyote time window...
        if (input.jumpPressed && !isJumping && (isOnGround || coyoteTime > Time.time) && !isHeadBlocked)
        {
            //...check to see if crouching AND not blocked. If so...
            //if (isCrouching && !isHeadBlocked)
            //{
            //    //...stand up and apply a crouching jump boost
            //    StandUp();
            //    rigidBody.AddForce(new Vector2(0f, crouchJumpBoost), ForceMode2D.Impulse);
            //}

            //...The player is no longer on the groud and is jumping...
            isOnGround = false;
            isJumping = true;

            //...record the time the player will stop being able to boost their jump...
            jumpTime = Time.time + jumpHoldDuration;

            //...add the jump force to the rigidbody...
            rigidBody.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);

            //...and tell the Audio Manager to play the jump audio
            //SoundControl.PlayJumpAudio();
        }
        //Otherwise, if currently within the jump time window...
        else if (isJumping)
        {
            //...and the jump button is held, apply an incremental force to the rigidbody...
            if (input.jumpHeld)
                rigidBody.AddForce(new Vector2(0f, jumpHoldForce), ForceMode2D.Impulse);

            //...and if jump time is past, set isJumping to false
            if (jumpTime <= Time.time)
                isJumping = false;
        }

        //If player is falling to fast, reduce the Y velocity to the max
        if (rigidBody.velocity.y < maxFallSpeed)
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, maxFallSpeed);
    }

    void FlipCharacterDirection()
    {
        //Turn the character by flipping the direction
        direction *= -1;

        //Record the current scale
        Vector3 scale = transform.localScale;


        Vector3 textPos = infoText.transform.position;


        //Set the X scale to be the original times the direction
        scale.x = originalXScale * direction;

        //Apply the new scale
        transform.localScale = scale;

        if (scale.x < 0)
        {
            scaleText.x = -1;
        }
        else
        {
            scaleText.x = 1;
        }

        infoText.transform.position = textPos;

        infoText.transform.localScale = scaleText;

    }

    void Crouch()
    {
        isStandingUp = false;

        //The player is crouching
        isCrouching = true;

        isSliding = false;

        if (character == 1)
        {
            //Apply the crouching collider size and offset
            bodyCollider.size = colliderCrouchSize;
            bodyCollider.offset = colliderCrouchOffset;
        }
    }

    void StandUp()
    {
        //If the player's head is blocked, they can't stand so exit
        if (isHeadBlocked)
            return;

        isStandingUp = true;

        //The player isn't crouching
        isCrouching = false;

        //The player isn't sliding
        isSliding = false;

        //Apply the standing collider size and offset
        bodyCollider.size = colliderStandSize;
        bodyCollider.offset = colliderStandOffset;
    }

    void Slide()
    {
        isStandingUp = false;

        //The player is sliding
        isSliding = true;

        //isCrouching = false;

        if (character == 1)
        {
            //Apply the sliding collider size and offset
            bodyCollider.size = colliderSlideSize;
            bodyCollider.offset = colliderSlideOffset;
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

    //trigger controller
    void OnTriggerEnter2D(Collider2D interact)
    {
        if (interact.gameObject.tag == "Changer")
        {
            ForcePlayerChange changer = interact.GetComponent<ForcePlayerChange>();
            if (changer.skeleton)
            {
                SkeletonActive();
                canChange = changer.changeEnable;
            }
            else if (changer.archer)
            {
                ArcherActive();
                canChange = changer.changeEnable;
            }
            else if (changer.golem)
            {
                GolemActive();
                canChange = changer.changeEnable;
            }
        }

            //is near item
            if (interact.gameObject.tag == "Itens")
        {
            ItemTextTrigger itemText = interact.GetComponent<ItemTextTrigger>();
            itemText.showText = true;
            infoText.SetActive(true);
        }
        //is near npc
        if (interact.gameObject.tag == "NPC")
        {
            DialogueTrigger dialogue = interact.GetComponent<DialogueTrigger>();
            dialogue.showText = true;
            infoText.SetActive(true);          
        }
        //entered enemy area
        if (interact.gameObject.tag == "EnemyArea")
        {
            EnemyMovement enemy = interact.GetComponentInChildren<EnemyMovement>();
            enemy.playerOnArea = true;
            enemy.target = gameObject.transform;
        }
        if (interact.gameObject.tag == "DemoExit")
        {
            Application.Quit();
            Debug.Log("Jogo Fechado");
        }
    }

    void OnTriggerStay2D(Collider2D interact)
    {
        if (interact.gameObject.tag == "Itens" || interact.gameObject.tag == "NPC")
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                infoText.SetActive(false);
            }
        }

        if (interact.gameObject.tag == "Lights")
        {
            LightController lightControl = interact.GetComponent<LightController>();
            if (lightControl.lightsOn)
            {
                if (lightControl.light.GetComponent<Light>().intensity <= 1)
                {
                    lightControl.light.GetComponent<Light>().intensity += 0.5f *  Time.deltaTime;
                }
                //lightControl.light.SetActive(true);
            }
            else if (!lightControl.lightsOn)
            {
                if (lightControl.light.GetComponent<Light>().intensity >= 0)
                {
                    lightControl.light.GetComponent<Light>().intensity -= 0.5f * Time.deltaTime;
                }
                //lightControl.light.SetActive(false);
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D interact)
    {
        if (interact.gameObject.tag == "Itens")
        {
            ItemTextTrigger itemText = interact.GetComponent<ItemTextTrigger>();
            itemText.showText = false;
            FindObjectOfType<ItemTextManager>().EndDialogue();
            infoText.SetActive(false);
        }
        if (interact.gameObject.tag == "NPC")
        {
            DialogueTrigger dialogue = interact.GetComponent<DialogueTrigger>();
            dialogue.showText = false;
            FindObjectOfType<TypeWriterEffect>().EndDialogue();
            infoText.SetActive(false);
        }
        if (interact.gameObject.tag == "EnemyArea")
        {
            EnemyMovement enemy = interact.GetComponentInChildren<EnemyMovement>();
            enemy.playerOnArea = false;
        }
    }
}

