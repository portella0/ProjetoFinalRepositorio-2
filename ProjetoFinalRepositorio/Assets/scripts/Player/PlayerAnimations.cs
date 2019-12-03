using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    PlayerMovement movement;
    PlayerHealth health;
    Rigidbody2D rigidBody;     
    PlayerInput input;          
    public Animator anim;

    int hangID;         
    int groundID;          
    int crouchID;          
    int slideID;           
    int isHeadBlockedID;           
    int speedID;           
    int fallID;            
    int mistID;
    int skeletonBlockID;
    int golemBlockID;
    int nearWallID;
    int golemAttackID;
    int movingID;
    int skelDeadID;
    int golemDeadID;
    int mageDeadID;
    //int lookUpID;
    //int lookDownID;

    [Header("Audio")]
    AudioSource audioSource;
    public AudioClip skelStepGround, skelStepStone, skelStepWood, skelStepSnow;
    public AudioClip skelCrawl;
    public AudioClip skelSlide;
    public AudioClip SkelJump;
    public AudioClip SkelFallGround, SkelFallStone, SkelFallWood, SkelFallSnow;
    public AudioClip skelDeath;
    public AudioClip skelDamage;
    public AudioClip skelGrab;
    public AudioClip skelSound;
    public AudioClip golemStep;
    public AudioClip golemJump;
    public AudioClip golemFall;
    public AudioClip golemDamage;
    public AudioClip golemDeath;
    public AudioClip golemAttack;
    public AudioClip golemSound;
    public AudioClip golemMove;
    public AudioClip mageMove;
    public AudioClip mageJump; 
    public AudioClip mageGlide;
    public AudioClip mageInvOn;
    public AudioClip mageInvOff;
    public AudioClip mageMagicShoot;
    public AudioClip mageDamage;
    public AudioClip mageDeath;
    public AudioClip mageSound;
    public bool stepStone;
    public bool stepWood;
    public bool stepGround;
    public bool stepSnow;
    public float skeletonStepSoundControl;
    public float skeletonCrawlSoundControl;
    public float golemMoveTimer;

    void Start()
    {
        stepGround = false;
        stepSnow = false;
        stepStone = false;
        stepWood = false;
        audioSource = GetComponent<AudioSource>();
        hangID = Animator.StringToHash("isHang");
        groundID = Animator.StringToHash("isGround");
        crouchID = Animator.StringToHash("isCrouch");
        slideID = Animator.StringToHash("isSlide");
        isHeadBlockedID = Animator.StringToHash("isHeadBlocked");
        speedID = Animator.StringToHash("speed");
        fallID = Animator.StringToHash("VerticalSpeed");
        mistID = Animator.StringToHash("isMist");
        golemAttackID = Animator.StringToHash("golemAttack");
        movingID = Animator.StringToHash("moving");
        skelDeadID = Animator.StringToHash("skelDead");
        golemDeadID = Animator.StringToHash("golemDead");
        mageDeadID = Animator.StringToHash("mageDead");
        //skeletonBlockID = Animator.StringToHash("skeletonBlock");
        //golemBlockID = Animator.StringToHash("golemBlock");
        //nearWallID = Animator.StringToHash("nearWall");
        //lookUpID = Animator.StringToHash("lookUp");
        //lookDownID = Animator.StringToHash("lookDown");
        //Grab a reference to this object's parent transform
        Transform parent = transform.parent;

        movement = parent.GetComponent<PlayerMovement>();
        health = parent.GetComponent<PlayerHealth>();
        rigidBody = parent.GetComponent<Rigidbody2D>();
        input = parent.GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();

        if (movement == null || rigidBody == null || input == null || anim == null)
        {
            Debug.LogError("A needed component is missing from the player");
            Destroy(this);
        }

        skeletonStepSoundControl = 0;
        skeletonCrawlSoundControl = 0;
        golemMoveTimer = 0;
    }

    void Update()
    {
        anim.SetBool(hangID, movement.isHanging);
        anim.SetBool(groundID, movement.isOnGround);
        anim.SetBool(crouchID, movement.isCrouching);
        anim.SetBool(slideID, movement.isSliding);
        anim.SetBool(isHeadBlockedID, movement.isHeadBlocked);
        anim.SetBool(crouchID, movement.isCrouching);
        anim.SetBool(mistID, movement.isMist);
        anim.SetBool(golemAttackID, movement.isGolemAttack);
        anim.SetBool(movingID, movement.isMoving);

        if (movement.character == 1)
        {
            anim.SetBool(skelDeadID, health.shouldMove);
        }
        if (movement.character == 2)
        {
            anim.SetBool(mageDeadID, health.shouldMove);
        }
        if (movement.character == 3)
        {
            anim.SetBool(golemDeadID, health.shouldMove);
        }


        if (input.horizontal != 0 && movement.character == 1 && movement.isOnGround && !movement.isCrouching && !movement.isSliding && health.shouldMove)
        {
            skeletonStepSoundControl += 1 * Time.deltaTime;

            if (skeletonStepSoundControl > 0.4f)
            {
                skeletonStepSoundControl = 0;
                audioSource.PlayOneShot(skelStepStone, 0.05f);
            }
        }

        if (input.horizontal != 0 && movement.character == 1 && movement.isOnGround && movement.isCrouching && health.shouldMove)
        {
            skeletonCrawlSoundControl += 1 * Time.deltaTime;

            if (skeletonCrawlSoundControl > 0.4f)
            {
                skeletonCrawlSoundControl = 0;
                audioSource.PlayOneShot(skelCrawl, 0.5f);
            }
        }

        golemMoveTimer += 1 * Time.deltaTime;

        if (input.horizontal != 0 && movement.character == 3)
        {
            if (golemMoveTimer > 3.4f)
            {
                golemMoveTimer = 0;
                audioSource.PlayOneShot(golemMove, 0.1f);
            }
        }

        //anim.SetBool(skeletonBlockID, movement.isBlockingGolem);
        //anim.SetBool(golemBlockID, movement.isBlockingSkeleton);
        //anim.SetBool(nearWallID, movement.isNearWall);
        //anim.SetBool(lookUpID, movement.isLookingUp);
        //anim.SetBool(lookDownID, movement.isLookingDown);
        anim.SetFloat(fallID, rigidBody.velocity.y);
        //Use the absolute value of speed so that we only pass in positive numbers
        anim.SetFloat(speedID, Mathf.Abs(input.horizontal));
    }


    public void GolemAttackCollider()
    {
        movement.golemAttackCollider.SetActive(true);
    }

    public void isNotAlive()
    {
        health.isAlive = false;
    }

    public void ResetAnimation()
    {
        anim.Rebind();
    }

    public void skelStepAudio()
    {
        //if(stepGround)
        //{
        //    if (skelStepGround != null)
        //    {
        //        audioSource.PlayOneShot(skelStepGround, 0.1f);
        //    }
        //}
        //else if (stepSnow)
        //{
        //    if (skelStepSnow != null)
        //    {
        //        audioSource.PlayOneShot(skelStepSnow, 0.1f);
        //    }
        //}
        //else if (stepStone)
        //{
        //    if (skelStepStone != null)
        //    {
        //        audioSource.PlayOneShot(skelStepStone, 0.1f);
        //    }
        //}
        //else if (stepWood)
        //{
        //    if (skelStepWood != null)
        //    {
        //        audioSource.PlayOneShot(skelStepWood, 0.1f);
        //    }
        //}
    }

    public void skelJumpAudio()
    {
         if (SkelJump != null)
         {
            audioSource.PlayOneShot(SkelJump, 0.2f);
        }
    }
    public void skelFallAudio()
    {
        if (stepGround)
        {
            if (SkelFallGround != null)
            {
                audioSource.PlayOneShot(SkelFallGround, 0.1f);
            }
        }
        else if (stepSnow)
        {
            if (SkelFallSnow != null)
            {
                audioSource.PlayOneShot(SkelFallSnow, 0.1f);
            }
        }
        else if (stepStone)
        {
            if (SkelFallStone != null)
            {
                audioSource.PlayOneShot(SkelFallStone, 0.1f);
            }
        }
        else if (SkelFallGround)
        {
            if (SkelFallWood != null)
            {
                audioSource.PlayOneShot(SkelFallWood, 0.1f);
            }
        }
    }
    public void skelCrawlAudio()
    {
         if (skelCrawl != null)
         {
            audioSource.PlayOneShot(skelCrawl, 0.4f);
        }
    }
    public void skelSlideAudio()
    {
         if (skelSlide != null)
         {
            audioSource.PlayOneShot(skelSlide, 0.9f);
         }
    }
    public void skelDeathAudio()
    {
         if (skelDeath != null)
        {
            audioSource.PlayOneShot(skelDeath, 0.5f);
        }
    }
    public void skelDamageAudio()
    {
         if (skelDamage != null)
        {
            audioSource.PlayOneShot(skelDamage, 0.5f);
        }
    }
    public void skelSoundAudio()
    {
         if (skelSound != null)
        {
            audioSource.PlayOneShot(skelSound, 0.2f);
        }
    }
    public void golemStepAudio()
    {
         if (golemStep != null)
        {
            audioSource.PlayOneShot(golemStep, 0.4f);
        }
    }
    public void golemJumpAudio()
    {
         if (golemJump != null)
        {
            audioSource.PlayOneShot(golemJump, 0.2f);
        }
    }
    public void golemFallAudio()
    {
         if (golemFall != null)
        {
            audioSource.PlayOneShot(golemFall, 0.2f);
        }
    }
    public void golemDamageAudio()
    {
         if (golemDamage != null)
        {
            audioSource.PlayOneShot(golemDamage, 0.5f);
        }
    }
    public void golemDeathAudio()
    {
         if (golemDeath != null)
        {
            audioSource.PlayOneShot(golemDeath, 0.5f);
        }
    }
    public void golemSoundAudio()
    {
         if (golemSound != null)
        {
            audioSource.PlayOneShot(golemSound, 0.2f);
        }
    }
    public void mageMoveAudio()
    {
         if (mageMove != null)
        {
            audioSource.PlayOneShot(mageMove, 0.2f);
        }
    }
    public void mageJumpAudio()
    {
         if (mageJump != null)
        {
            audioSource.PlayOneShot(mageJump, 0.2f);
        }
    }
    public void mageGliadeAudio()
    {
         if (mageGlide != null)
        {
            audioSource.PlayOneShot(mageGlide, 0.4f);
        }
    }
    public void mageInvOnAudio()
    {
         if (mageInvOn != null)
        {
            audioSource.PlayOneShot(mageInvOn, 0.3f);
        }
    }
    public void mageInvOffAudio()
    {
         if (mageInvOff != null)
        {
            audioSource.PlayOneShot(mageInvOff, 0.3f);
        }
    }
    public void mageMagicAudio()
    {
         if (mageMagicShoot != null)
        {
            audioSource.PlayOneShot(mageMagicShoot, 0.5f);
        }
    }
    public void mageDamageAudio()
    {
         if (mageDamage != null)
        {
            audioSource.PlayOneShot(mageDamage, 0.5f);
        }
    }
    public void mageDeathAudio()
    {
         if (mageDeath != null)
         {
            audioSource.PlayOneShot(mageDeath, 0.5f);
        }
    }
    public void mageSoundAudio()
    {
        if (mageSound != null){}
        {
            audioSource.PlayOneShot(mageSound, 0.2f);
        }
    }

}
