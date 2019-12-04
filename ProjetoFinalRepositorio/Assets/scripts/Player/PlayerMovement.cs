using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public bool drawDebugRaycasts = true;  

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
    public GameObject skelUI, mageUI, golemUI;
    public float changetimer;
    public GameObject skelChangeEffectPos;
    public GameObject mageChangeEffectPos;
    public GameObject golemChangeEffectPos;
    public GameObject skelEffect;
    public GameObject mageEffect;
    public GameObject golemEffect;

    [Header("Audio")]
    AudioSource audioSource;
    public AudioClip skelChangeSound;
    public AudioClip mageChangeSound;
    public AudioClip golemChangeSound;
    public AudioClip golemJumpSound;
    public AudioClip skelJump, skelLand;

    [Header("Movement Properties")]
    public float speed = 5f;                //player speed
    public float crouchSpeedDivisor = 3f;   //ppeed reduction when crouching
    public float coyoteDuration =0.05f;     //how long the player can jump after falling
    public float maxFallSpeed = -50f;       //max speed player can fall
    public float slideSpeed = 10f;           //slide speed
    public float slideTime = 0.5f;           //slide timer
    public float slideTimeMax = 0.5f;        //slide maximum time
    public float slideCooldown = 3f;        //cooldown for the slide 
    public float mistTimer;                 //invisiblity cooldown
    public float mistMaxTimer;
    public float golemAttackTimer;
    public float golemMaxAttackTimer;
    public float golemJumpCooldown;
    public float golemMaxJumpCooldown;
    public float magicTimer;
    public float magicMaxTimer;
    public bool didMagic;

    [Header("Jump Properties")]
    public float jumpForce = 90f;          //Initial force of jump
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
    public bool slideSpace;
    //public bool isLookingUp;
    //public bool isLookingDown;

    public PlayerInput input;                      
    BoxCollider2D bodyCollider;             
    Rigidbody2D rigidBody;                  
    public PlayerAnimations animate;               
    public PlayerHealth playerHealth;               

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

    const float hangOffset = .05f;         

    public Vector3 vel;

    void Start()
    {
        //not looking up or down
        //lookingHeight = false;

        audioSource = GetComponent<AudioSource>();

        //set screen shake to false
        GolemShake = false;
        ShakeTimer = 1;
        golemJumpTimer = 0.05f;
        golemAttackCollider.SetActive(false);
        golemJumpCooldown = 0;
        changetimer = 0;

        //reset magic timer
        magicTimer = 0;

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
        slideSpace = true;

        //slide cooldown
        slideTimeMax = slideTime;
        didMagic = false;
        //Calculate crouching collider size and offset
        //colliderCrouchSize = new Vector2(bodyCollider.size.x, bodyCollider.size.y / 2.05f);
        //colliderCrouchOffset = new Vector2(bodyCollider.offset.x, bodyCollider.offset.y / 2.05f);

        colliderCrouchSize = new Vector2(1.5f, 0.7f);
        colliderCrouchOffset = new Vector2(-0.1f, 0.35f);

        colliderSlideSize = new Vector2(1.5f, 0.7f);
        colliderSlideOffset = new Vector2(0.55f, 0.35f);

        //These are the starting sizes for the Collider component
        //skeleton
        colliderSkeletonX = 0.668f;
        colliderSkeletonY = 1.944f;
        skeletonOffsetX = 0.053f;
        skeletonOffsetY = 0.943f;
        //archer
        colliderArcherX = 0.65f;
        colliderArcherY = 1.6f;
        archerOffsetX = 0.06f;
        archerOffsetY = 0.7f;
        //golem
        //colliderGolemX = 1.56f;
        colliderGolemX = 1.9f;
        colliderGolemY = 2.6f;
        //golemOffsetX = 0f;
        golemOffsetX = 0.2f;
        golemOffsetY = 1.26f;

        //Give the PlayerPrefs some values to send over to the next Scene
        if (canChange)
        {
            character = PlayerPrefs.GetInt("Character", 1);
        }
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
        changetimer += 1 * Time.deltaTime;    

        //get speed
        vel = rigidBody.velocity;

        //Check the environment to determine status
        PhysicsCheck();

        //Process skeleton ground and air movements
        if (character == 1)
        {
            SkeletonGroundMovement();
            SkeletonMidAirMovement();
            skelUI.SetActive(true);
            mageUI.SetActive(false);
            golemUI.SetActive(false);
        }
        else if (character == 2)
        {
            ArcherGroundMovement();
            ArcherMidAirMovement();
            skelUI.SetActive(false);
            mageUI.SetActive(true);
            golemUI.SetActive(false);
        }
        else if (character == 3)
        {
            GolemGroundMovement();
            GolemMidAirMovement();
            skelUI.SetActive(false);
            mageUI.SetActive(false);
            golemUI.SetActive(true);
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
        if (Input.GetKey(KeyCode.UpArrow) || input.vertical > 0)
        {
            //if (!isHanging || !isCrouching || !isSliding)
            //{
            //    isLookingUp = true;
            //    lookingHeight = true;
            //}
            follow.transform.position = Vector2.Lerp(follow.transform.position, upCamera.transform.position, 5 * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.DownArrow) || input.vertical < 0)
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

        if (playerHealth.shouldMove)
        {
            if (canChange)
            {
                ////change character
                //if (Input.GetKeyDown(KeyCode.Alpha1) && !isHanging && !isCrouching && character != 1)
                //{
                //    animate.ResetAnimation();
                //    SkeletonActive();
                //}
                //if (Input.GetKeyDown(KeyCode.Alpha2) && !isHanging && !isCrouching && character != 2)
                //{
                //    animate.ResetAnimation();
                //    ArcherActive();
                //}
                //if (Input.GetKeyDown(KeyCode.Alpha3) && !isHanging && !isHeadBlocked && character != 3)
                //{
                //    animate.ResetAnimation();
                //    GolemActive();
                //}

                if (input.changeLeft)
                {
                    if (changetimer > 0.1f)
                    {
                        changetimer = 0;
                        if (character == 1)
                        {
                            animate.ResetAnimation();
                            GolemActive();
                            Debug.Log("leftgolem");
                        }
                        else if (character == 2)
                        {
                            animate.ResetAnimation();
                            SkeletonActive();
                            Debug.Log("leftskel");
                        }
                        else if (character == 3)
                        {
                            animate.ResetAnimation();
                            ArcherActive();
                            Debug.Log("leftArcher");
                        }
                    }
                }
                if (input.changeRight)
                {
                    if (changetimer > 0.1f)
                    {
                        changetimer = 0;
                        if (character == 1)
                        {
                            animate.ResetAnimation();
                            ArcherActive();
                            Debug.Log("RightArcher");
                        }
                        else if (character == 2)
                        {
                            animate.ResetAnimation();
                            GolemActive();
                            Debug.Log("RightGolem");
                        }
                        else if (character == 3)
                        {
                            animate.ResetAnimation();
                            SkeletonActive();
                            Debug.Log("RIghtSkel");
                        }
                    }
                }
            }
        }
    }

    void PhysicsCheck()
    {
        isOnGround = false;
        isHeadBlocked = false;

        if (!playerHealth.shouldMove)
        {
            input.horizontal = 0;
        }

        if (!playerHealth.shouldMove && playerHealth.onTrap && playerHealth.touchingGround)
        {
            rigidBody.bodyType = RigidbodyType2D.Static;
        }
        else if (playerHealth.shouldMove && !isHanging)
        {
            rigidBody.bodyType = RigidbodyType2D.Dynamic;
        }

        RaycastHit2D leftCheck = Raycast(new Vector2(-groundCheckOffset, 0f), Vector2.down, groundDistance);
        RaycastHit2D rightCheck = Raycast(new Vector2(groundCheckOffset, 0f), Vector2.down, groundDistance);

        if (leftCheck || rightCheck)
        {
            if (golemJumpCooldown > 0 && golemJumpCooldown < golemMaxJumpCooldown)
            {
                golemJumpCooldown += 1 * Time.deltaTime;
            }
            if(golemJumpCooldown >= golemMaxJumpCooldown)
            {
                golemJumpCooldown = 0;
            }

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
                    if (golemJumpSound != null)
                    {
                        audioSource.PlayOneShot(golemJumpSound, 0.5F);
                    }
                }

            }

            if (character == 1 && GolemShake == true)
            {
                golemJumpTimer -= Time.deltaTime;
                if (golemJumpTimer < 0)
                {
                    GolemShake = false;
                    if (golemJumpSound != null)
                    {
                        audioSource.PlayOneShot(skelLand, 0.5F);
                    }
                }

            }
        }

        headCheck = Raycast(new Vector2(0f, bodyCollider.size.y), Vector2.up, headAboveSpace);

        if (headCheck)
            isHeadBlocked = true;

        Vector2 grabDir = new Vector2(direction, 0f);

        RaycastHit2D blockedCheck = Raycast(new Vector2(groundCheckOffset * direction, playerHeight), grabDir, grabDistance);
        RaycastHit2D ledgeCheck = Raycast(new Vector2(reachOffset * direction, playerHeight), Vector2.down, grabDistance);
        RaycastHit2D wallCheck = Raycast(new Vector2(groundCheckOffset * direction, visionHeight), grabDir, grabDistance);
        RaycastHit2D slideCheck = Raycast(new Vector2(groundCheckOffset * direction, visionHeight/2), grabDir, grabDistance);

        if (ledgeCheck.collider != null)
        {
            Debug.Log(ledgeCheck.collider.name);
        }

        if(slideCheck)
        {
            slideSpace = false;
        }
        else
        {
            slideSpace = true;
        }

        if(isHeadBlocked)
        {
            isHanging = false;
        }

        if (!isOnGround && !isHanging && rigidBody.velocity.y < 0f &&
        ledgeCheck && wallCheck && !blockedCheck && !isHeadBlocked)
        {
            if (character == 1)
            {
                Vector3 pos = transform.position;
                pos.x += (wallCheck.distance - hangOffset) * direction;
                pos.y -= ledgeCheck.distance;
                transform.position = pos;
                rigidBody.bodyType = RigidbodyType2D.Static;

                isHanging = true;
            }
        }
    }

    public void SkeletonActive()
    {
        animate = skeleton.GetComponent<PlayerAnimations>();
        if (skelEffect != null)
        {
            Instantiate(skelEffect, skelChangeEffectPos.transform.position, transform.rotation);
        }

        if (skelChangeSound != null)
        {
            audioSource.PlayOneShot(skelChangeSound, 0.4f);
        }

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
        speed = 4.5f;                
        crouchSpeedDivisor = 3f;   
        maxFallSpeed = -50f;       
        slideSpeed = 10f;           
        slideTime = 0.5f;           
        slideTimeMax = 0.5f;           
        slideCooldown = 3f;        

        //change jump properties
        jumpForce = 90f;          
        hangingJumpForce = 100f;    
        jumpHoldForce = 0.5f;      
        jumpHoldDuration = 0.1f;    

        //change raycast
        groundCheckOffset = 0.4f;          
        visionHeight = 1.65f;          
        reachOffset = 1f;         
        headAboveSpace = 0.9f;       
        groundDistance = 0.3f;      
        grabDistance = 1f;        
    }

    public void ArcherActive()
    {
        animate = archer.GetComponent<PlayerAnimations>();
        if (mageEffect != null)
        {
            Instantiate(mageEffect, mageChangeEffectPos.transform.position, transform.rotation);
        }

        if (mageChangeSound != null)
        {
            audioSource.PlayOneShot(mageChangeSound, 0.1f);
        }

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
        speed = 5.5f;                
        crouchSpeedDivisor = 3f;   
        maxFallSpeed = -10f;       
        slideSpeed = 10f;           
        slideTime = 1f;           
        slideTimeMax = 1f;          
        slideCooldown = 3f;       

        //change jump properties
        jumpForce = 35f;          
        jumpHoldForce = 0.5f;      
        jumpHoldDuration = 0.1f;    

        //change raycast
        groundCheckOffset = 0.4f;          
        visionHeight = 1.36f;          
        reachOffset = 0.8f;         
        headAboveSpace = 0.9f;       
        groundDistance = 0.3f;      
        grabDistance = 0.4f;
    }

    public void GolemActive()
    {
        golemJumpTimer = 0.05f;

        animate = golem.GetComponent<PlayerAnimations>();
        if (golemEffect != null)
        {
            Instantiate(golemEffect, golemChangeEffectPos.transform.position, transform.rotation);
        }

        if (golemChangeSound != null)
        {
            audioSource.PlayOneShot(golemChangeSound, 0.1f);
        }

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
        speed = 2f;                
        crouchSpeedDivisor = 3f;   
        maxFallSpeed = -10f;       
        slideSpeed = 10f;           
        slideTime = 1f;           
        slideTimeMax = 1f;          
        slideCooldown = 3f;        

        //change jump properties
        jumpForce = 130f;         
        hangingJumpForce = 100f;    
        jumpHoldForce = 0.5f;      
        jumpHoldDuration = 0.1f;    

        //change raycast
        groundCheckOffset = 0.75f;         
        visionHeight = 2.3f;          
        reachOffset = 1.7f;         
        headAboveSpace = 0.5f;       
        groundDistance = 0.1f;      
        grabDistance = 1.5f;       
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
        //If  hanging the player cant move to exit
        if (isHanging)
            return;

        //If holding the crouch button but not crouching, crouch
        if (input.crouchHeld && !isCrouching && isOnGround && !isSliding)
            Crouch();
        //if not sliding and is crouching and pressing the slide button, not slide
        else if (input.runHeld && input.crouchHeld && slideTime > 0 && slideSpace)
            Slide();
        else if (!input.runHeld && isSliding && input.crouchHeld)
            Crouch();
        else if (!input.runHeld && isSliding && input.crouchHeld && slideTime <= 0)
            Crouch();
        //if  sliding and is pressing the crouch button and no more slide time, crouch
        else if (input.runHeld && input.crouchHeld && slideTime <= 0)
            Crouch();
        else if (input.runHeld && isHeadBlocked && slideTime <= 0 && !isStandingUp)
            Crouch();
        else if (!input.runHeld && isHeadBlocked && slideTime <= 0 && !isStandingUp)
            Crouch();
        //if not sliding and head is blocked and pressing the slide button, slide
        else if (input.runHeld && isHeadBlocked && slideTime > 0 && slideSpace)
            Slide();
        //if not holding crouch but currently crouching, stand up
        else if (!input.crouchHeld && isCrouching)
            StandUp();
        //if crouching and no longer on the ground, stand up
        else if (!isOnGround && isCrouching)
            StandUp();
        //if not sliding and holding the slide button, slide
        else if (input.runHeld && !isSliding && slideTime > 0 /*&& isOnGround*/ && slideSpace)
            Slide();
        //if not holding the sliding button but sliding, stand up
        else if (!input.runHeld && isSliding)
            StandUp();
        //if  holding the sliding button but no more time, stand up
        else if (input.runHeld && slideTime <= 0)
            StandUp();
        //if not on ground but sliding, stand up
        //else if (!isOnGround && isSliding)
        //    StandUp();
        //if not holding the crouch button and sliding nut no more time
        //and space to stand, crouch 
        else if (isHeadBlocked && isSliding && slideTime <= 0 && !isStandingUp)
            Crouch();

        float xVelocity = speed * input.horizontal;

        //if (lookingHeight)
        //{
        //    xVelocity = 0;
        //}

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
        //    if (transform.localScale.x == 1)
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
            if (mistTimer >= mistMaxTimer + 1)
            {
                isMist = false;
            }
            mistTimer += 1 * Time.deltaTime;
            if(mistTimer >= mistMaxTimer *3)
            {
                mistTimer = 0;
            }
        }

        if (input.runPressed && magicTimer < magicMaxTimer)
        {
            if (!didMagic)
            {
                if (transform.localScale.x == 1)
                {
                    Instantiate(arrow, arrowShot.transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
                }
                else if (transform.localScale.x == -1)
                {
                    Instantiate(arrow, arrowShot.transform.position, Quaternion.Euler(new Vector3(0, 180, 0)));
                }
            }
            didMagic = true;
        }

        if (didMagic == true && magicTimer < magicMaxTimer)
        {
            magicTimer += 1 * Time.deltaTime;
        }
        else if (magicTimer >= magicMaxTimer)
        {
            didMagic = false;
            magicTimer += 1 * Time.deltaTime;
            if (magicTimer >= magicMaxTimer * 3)
            {
                magicTimer = 0;
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
        if ((input.runPressed || input.runHeld) && golemAttackTimer < golemMaxAttackTimer && isOnGround)
        {
            isGolemAttack = true;
        }

        if (isGolemAttack == true && golemAttackTimer < golemMaxAttackTimer)
        {
            golemAttackTimer += 1 * Time.deltaTime;
            //rigidBody.bodyType = RigidbodyType2D.Static;
            rigidBody.velocity = Vector2.zero;
            //if (golemAttackTimer >= 0.3)
            //{
            //    golemAttackCollider.SetActive(true);
            //}
        }

        else if (golemAttackTimer >= golemMaxAttackTimer)
        {
            isGolemAttack = false;
            golemAttackCollider.SetActive(false);
            //rigidBody.bodyType = RigidbodyType2D.Dynamic;
            golemAttackTimer += 1 * Time.deltaTime;
            if (golemAttackTimer >= golemMaxAttackTimer)
            {
                golemAttackTimer = 0;
            }
        }
    }

    void SkeletonMidAirMovement()
    {

        if (!isOnGround)
        {
            GolemShake = true;
        }

        if (isHanging)
        {
            if (input.crouchPressed)
            {
                isHanging = false;
                rigidBody.bodyType = RigidbodyType2D.Dynamic;
                return;
            }

            if (input.jumpPressed)
            {
                isHanging = false;
                rigidBody.bodyType = RigidbodyType2D.Dynamic;
                rigidBody.AddForce(new Vector2(0f, hangingJumpForce), ForceMode2D.Impulse);
                return;
            }
        }

        if (input.jumpPressed && !isJumping && (isOnGround || coyoteTime > Time.time) && !isHeadBlocked)
        {
            isOnGround = false;
            isJumping = true;

            jumpTime = Time.time + jumpHoldDuration;

            rigidBody.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);

            if (character == 1)
            {
                audioSource.PlayOneShot(skelJump, 0.5f);
            }
            //SoundControl.PlayJumpAudio();
        }
        else if (isJumping)
        {
            if (input.jumpHeld)
                rigidBody.AddForce(new Vector2(0f, jumpHoldForce), ForceMode2D.Impulse);

            if (jumpTime <= Time.time)
                isJumping = false;
        }

        //If player is falling to fast reduce the Y velocity to the max
        if (rigidBody.velocity.y < maxFallSpeed)
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, maxFallSpeed);
    }

    void ArcherMidAirMovement()
    {
        //if space is hold, glide
        if (input.jumpHeld)
        {
            maxFallSpeed = -1;
        }
        else
        {
            maxFallSpeed = -10;
        } 


        if (input.jumpPressed && !isJumping && (isOnGround || coyoteTime > Time.time)&& !isHeadBlocked)
        {
            isOnGround = false;
            isJumping = true;

            jumpTime = Time.time + jumpHoldDuration;

            rigidBody.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);

            //SoundControl.PlayJumpAudio();
        }
        else if (isJumping)
        {
            if (input.jumpHeld)
                rigidBody.AddForce(new Vector2(0f, jumpHoldForce), ForceMode2D.Impulse);

            if (jumpTime <= Time.time)
                isJumping = false;
        }

        //If player is falling to fast reduce the Y velocity to the max
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

        if (input.jumpPressed && !isJumping && (isOnGround || coyoteTime > Time.time) && !isHeadBlocked && golemJumpCooldown == 0)
        {
            isOnGround = false;
            isJumping = true;
            golemJumpCooldown = 0.1f;
            jumpTime = Time.time + jumpHoldDuration;

            rigidBody.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);

            //SoundControl.PlayJumpAudio();
        }
        else if (isJumping)
        {
            if (input.jumpHeld)
                rigidBody.AddForce(new Vector2(0f, jumpHoldForce), ForceMode2D.Impulse);

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
        //If the players head is blocked they cant stand  
        if (isHeadBlocked)
            return;

        isStandingUp = true;

        //The player isnt crouching
        isCrouching = false;

        //The player isnt sliding
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


    RaycastHit2D Raycast(Vector2 offset, Vector2 rayDirection, float length)
    {
        return Raycast(offset, rayDirection, length, groundLayer);
    }

    RaycastHit2D Raycast(Vector2 offset, Vector2 rayDirection, float length, LayerMask mask)
    {
        Vector2 pos = transform.position;

        RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDirection, length, mask);

        if (drawDebugRaycasts)
        {
            Color color = hit ? Color.red : Color.green;
            Debug.DrawRay(pos + offset, rayDirection * length, color);
        }

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
                if (character != 1)
                {
                    animate.ResetAnimation();
                    SkeletonActive();
                    canChange = changer.changeEnable;
                }
            }
            else if (changer.archer)
            {
                if (character != 2)
                {
                    animate.ResetAnimation();
                    ArcherActive();
                    canChange = changer.changeEnable;
                }
            }
            else if (changer.golem)
            {
                if (character != 3)
                {
                    animate.ResetAnimation();
                    GolemActive();
                    canChange = changer.changeEnable;
                }
            }
        }

        if (interact.gameObject.tag == "Changer")
        {
            stepControl stepper = interact.GetComponent<stepControl>();
            animate.stepGround = stepper.grass;
            animate.stepStone = stepper.stone;
            animate.stepWood = stepper.wood;
            animate.stepSnow = stepper.snow;
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
        if (interact.gameObject.tag == "ChangeScene")
        {
            SceneManager.LoadScene(interact.GetComponent<ChangeScene>().sceneNumber);
        }
    }

    void OnTriggerStay2D(Collider2D interact)
    {
        if (interact.gameObject.tag == "OSTCHANGER")
        {
            OST_BackgroundChanger backOSTChanger = interact.GetComponent<OST_BackgroundChanger>();

            if (backOSTChanger.isCave)
            {
                backOSTChanger.caveOST.GetComponent<OSTcontroller>().audioSource.volume = 0.5f;
                //Color color = backOSTChanger.backgroundCave.GetComponent<SpriteRenderer>().material.color;
                //color.a += 1 * Time.deltaTime;
                //if(color.a < 1)
                //{
                //    backOSTChanger.backgroundCave.GetComponent<SpriteRenderer>().material.color = color;
                //}

                //Color color2 = backOSTChanger.backgroundCave.GetComponent<SpriteRenderer>().material.color;
                //color2.a -= 1 * Time.deltaTime;
                //if (color2.a > 0)
                //{
                //    backOSTChanger.backgroundForest.GetComponent<SpriteRenderer>().material.color = color2;
                //}

                backOSTChanger.forestOST.GetComponent<OSTcontroller>().audioSource.volume = 0f;
                backOSTChanger.castleOST.GetComponent<OSTcontroller>().audioSource.volume = 0f;
            }
            if (backOSTChanger.isForest)
            {
                backOSTChanger.caveOST.GetComponent<OSTcontroller>().audioSource.volume = 0f;

                //Color color = backOSTChanger.backgroundForest.GetComponent<SpriteRenderer>().material.color;
                //color.a += 1 * Time.deltaTime;
                //if (color.a < 1)
                //{
                //    backOSTChanger.backgroundForest.GetComponent<SpriteRenderer>().material.color = color;
                //}

                //Color color2 = backOSTChanger.backgroundCave.GetComponent<SpriteRenderer>().material.color;
                //color2.a -= 1 * Time.deltaTime;
                //if (color2.a > 0)
                //{
                //    backOSTChanger.backgroundCave.GetComponent<SpriteRenderer>().material.color = color2;
                //}

                backOSTChanger.forestOST.GetComponent<OSTcontroller>().audioSource.volume = 0.5f;

                backOSTChanger.castleOST.GetComponent<OSTcontroller>().audioSource.volume = 0f;
            }
            if (backOSTChanger.isCastle)
            {
                backOSTChanger.caveOST.GetComponent<OSTcontroller>().audioSource.volume = 0f;
                backOSTChanger.forestOST.GetComponent<OSTcontroller>().audioSource.volume = 0f;
                backOSTChanger.castleOST.GetComponent<OSTcontroller>().audioSource.volume = 0.5f;

            }
        }

        if (interact.gameObject.tag == "Itens" || interact.gameObject.tag == "NPC")
        {
            if (input.interactPressed)
            {
                infoText.SetActive(false);
            }
        }

        if (interact.gameObject.tag == "Lights")
        {
            LightController lightControl = interact.GetComponent<LightController>();
            if (lightControl.lightsOn)
            {
                if (lightControl.light.GetComponent<Light>().intensity <= lightControl.intensity)
                {
                    lightControl.light.GetComponent<Light>().intensity += 0.5f *  Time.deltaTime;
                }
                //lightControl.light.SetActive(true);
            }
            else if (!lightControl.lightsOn)
            {
                if (lightControl.light.GetComponent<Light>().intensity >= lightControl.minimum)
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

