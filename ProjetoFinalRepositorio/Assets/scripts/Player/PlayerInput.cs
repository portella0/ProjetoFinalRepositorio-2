using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class PlayerInput : MonoBehaviour
{
    public float horizontal;      //Float that stores horizontal input
    public bool jumpHeld;         //Bool that stores jump pressed
    public bool jumpPressed;      //Bool that stores jump held
    public bool crouchHeld;       //Bool that stores crouch pressed
    public bool crouchPressed;    //Bool that stores crouch held
    public bool runHeld;        //Bool that stores run held
    public bool runPressed;       //Bool that stores run held

    bool readyToClear;                              //Bool used to keep input in sync


    void Update()
    {
        //Clear out existing input values
        ClearInput();

        //If the Game Manager says the game is over, exit
        if (GameControl.IsGameOver())
        {
            return;
        }

        //Process keyboard, mouse, gamepad (etc) inputs
        ProcessInputs();

        //Clamp the horizontal input to be between -1 and 1
        horizontal = Mathf.Clamp(horizontal, -1f, 1f);
    }

    void FixedUpdate()
    {
        //In FixedUpdate() we set a flag that lets inputs to be cleared out during the 
        //next Update(). This ensures that all code gets to use the controller inputs
        readyToClear = true;
    }

    void ClearInput()
    {
        //If we're not ready to clear input, exit
        if (!readyToClear)
            return;

        //Reset all inputs
        horizontal = 0f;
        jumpPressed = false;
        jumpHeld = false;
        crouchPressed = false;
        crouchHeld = false;
        runHeld = false;
        runPressed = false;

        readyToClear = false;
    }

    void ProcessInputs()
    {
        //Accumulate horizontal axis input
        horizontal += Input.GetAxis("Horizontal");

        //Accumulate button inputs
        jumpPressed = jumpPressed || Input.GetButtonDown("Jump");
        jumpHeld = jumpHeld || Input.GetButton("Jump");

        crouchPressed = crouchPressed || Input.GetButtonDown("Crouch");
        crouchHeld = crouchHeld || Input.GetButton("Crouch");

        runHeld = runHeld || Input.GetButton("Run");
        runPressed = runPressed || Input.GetButtonDown("Run"); ;
    }
}
