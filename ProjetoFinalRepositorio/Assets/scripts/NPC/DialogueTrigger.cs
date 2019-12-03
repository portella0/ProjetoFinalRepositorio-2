using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    public bool showText;
    public PlayerMovement playerCollision;
    public TypeWriterEffect dialogueText;
    public GameObject player;
    public GameObject dialogueManager;

    [Header("animation")]
    public Animator anim;
    int animateID;

    void Start()
    {
        player = GameObject.Find("Character");
        dialogueManager = GameObject.Find("DialogueManager");
        showText = false;
        playerCollision = player.GetComponent<PlayerMovement>();
        dialogueText = dialogueManager.GetComponent<TypeWriterEffect>();

        animateID = Animator.StringToHash("shouldAnimate");
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        //if is near any text trigger
        if (showText == true)
        {
            //start text
            if (playerCollision.input.interactPressed && dialogueText.next == false && dialogueText.timeSinceOpened >= dialogueText.timeToWaitForKeyInput 
            && dialogueText.startedDialogue == false)
            {
                TriggerDialogue();
            }
        }

        if (anim != null)
        {
            anim.SetBool(animateID, dialogueText.shouldAnimate);
        }
    }

    //find script to text
    public void TriggerDialogue()
    {
        FindObjectOfType<TypeWriterEffect>().StartDialogue(dialogue);
    }
}
