using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    PlayerMovement movement;    //Reference to the PlayerMovement script component
    Rigidbody2D rigidBody;      //Reference to the Rigidbody2D component
    PlayerInput input;          //Reference to the PlayerInput script component
    Animator anim;              //Reference to the Animator component

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
    //int lookUpID;
    //int lookDownID;

    void Start()
    {
        //Get the integer hashes of the parameters. This is much more efficient
        //than passing strings into the animator
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
        //skeletonBlockID = Animator.StringToHash("skeletonBlock");
        //golemBlockID = Animator.StringToHash("golemBlock");
        //nearWallID = Animator.StringToHash("nearWall");
        //lookUpID = Animator.StringToHash("lookUp");
        //lookDownID = Animator.StringToHash("lookDown");
        //Grab a reference to this object's parent transform
        Transform parent = transform.parent;

        //Get references to the needed components
        movement = parent.GetComponent<PlayerMovement>();
        rigidBody = parent.GetComponent<Rigidbody2D>();
        input = parent.GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();

        //If any of the needed components don't exist...
        if (movement == null || rigidBody == null || input == null || anim == null)
        {
            //...log an error and then remove this component
            Debug.LogError("A needed component is missing from the player");
            Destroy(this);
        }
    }

    void Update()
    {
        //Update the Animator with the appropriate values
        anim.SetBool(hangID, movement.isHanging);
        anim.SetBool(groundID, movement.isOnGround);
        anim.SetBool(crouchID, movement.isCrouching);
        anim.SetBool(slideID, movement.isSliding);
        anim.SetBool(isHeadBlockedID, movement.isHeadBlocked);
        anim.SetBool(crouchID, movement.isCrouching);
        anim.SetBool(mistID, movement.isMist);
        anim.SetBool(golemAttackID, movement.isGolemAttack);
        anim.SetBool(movingID, movement.isMoving);
        //anim.SetBool(skeletonBlockID, movement.isBlockingGolem);
        //anim.SetBool(golemBlockID, movement.isBlockingSkeleton);
        //anim.SetBool(nearWallID, movement.isNearWall);
        //anim.SetBool(lookUpID, movement.isLookingUp);
        //anim.SetBool(lookDownID, movement.isLookingDown);
        anim.SetFloat(fallID, rigidBody.velocity.y);
        //Use the absolute value of speed so that we only pass in positive numbers
        anim.SetFloat(speedID, Mathf.Abs(input.horizontal));
    }

    //This method is called from events in the animation itself. This keeps the footstep
    //sounds in sync with the visuals
    public void StepAudio()
    {
        //Tell the Audio Manager to play a footstep sound
        //SoundControl.PlayFootstepAudio();
    }

    //This method is called from events in the animation itself. This keeps the footstep
    //sounds in sync with the visuals
    public void CrouchStepAudio()
    {
        //Tell the Audio Manager to play a crouching footstep sound
        //SoundControl.PlayCrouchFootstepAudio();
    }

    public void ResetAnimation()
    {
        anim.Rebind();
    }
}
