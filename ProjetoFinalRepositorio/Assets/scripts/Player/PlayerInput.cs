using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class PlayerInput : MonoBehaviour
{
    public float horizontal;      //Float that stores horizontal input
    public float vertical;
    public float abil1;
    public float abil2;
    public bool jumpHeld;         //Bool that stores jump pressed
    public bool jumpPressed;      //Bool that stores jump held
    public bool crouchHeld;       //Bool that stores crouch pressed
    public bool crouchPressed;    //Bool that stores crouch held
    public bool runHeld;        //Bool that stores run held
    public bool runPressed;       //Bool that stores run held
    public bool interactPressed;       //Bool that stores run held
    public bool respawn;       //Bool that stores run held
    public bool menu;       //Bool that stores run held
    public bool changeLeft;       //Bool that stores run held
    public bool changeRight;       //Bool that stores run held

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

        if (abil1 > 0)
        {
            crouchPressed = true;
            crouchHeld = true;
        }

        if (abil2 > 0)
        {
            runHeld = true;
            runPressed = true;
        }

        //Clamp the horizontal input to be between -1 and 1
        horizontal = Mathf.Clamp(horizontal, -1f, 1f);
        vertical = Mathf.Clamp(vertical, -1f, 1f);
        abil1 = Mathf.Clamp(abil1, -1f, 1f);
        abil2 = Mathf.Clamp(abil2, -1f, 1f);
        //Debug.Log(abil1);
        //Debug.Log(abil2);

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
        vertical = 0f;
        abil1 = 0f;
        abil2 = 0f;
        jumpPressed = false;
        jumpHeld = false;
        crouchPressed = false;
        crouchHeld = false;
        runHeld = false;
        runPressed = false;
        interactPressed = false;
        respawn = false;
        menu = false;
        readyToClear = false;
        changeLeft = false;
        changeRight = false;
    }

    void ProcessInputs()
    {
        //Accumulate horizontal axis input
        horizontal += Input.GetAxis("Horizontal");
        vertical += Input.GetAxis("Vertical");

        abil1 += Input.GetAxis("abil2");
        abil2 += Input.GetAxis("abil1");

        changeLeft = changeLeft || Input.GetButtonDown("changeCharacterLeft");
        changeRight = changeRight || Input.GetButtonDown("changeCharacterRight");

        //Accumulate button inputs
        jumpPressed = jumpPressed || Input.GetButtonDown("Jump");
        jumpHeld = jumpHeld || Input.GetButton("Jump");

        crouchPressed = crouchPressed || Input.GetButtonDown("Crouch");
        crouchHeld = crouchHeld || Input.GetButton("Crouch");

        runHeld = runHeld || Input.GetButton("Run");
        runPressed = runPressed || Input.GetButtonDown("Run"); ;

        interactPressed = interactPressed || Input.GetButtonDown("Interact"); ;
        respawn = respawn || Input.GetButtonDown("Respawn"); ;
        menu = menu || Input.GetButtonDown("Menu"); ;
    }
}
