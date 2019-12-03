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

    void Start()
    {
        player = GameObject.Find("Character");
        dialogueManager = GameObject.Find("DialogueManager");
        showText = false;
        playerCollision = player.GetComponent<PlayerMovement>();
        dialogueText = dialogueManager.GetComponent<TypeWriterEffect>();
    }

    void Update()
    {
        //if is near any text trigger
        if (showText == true)
        {
            //start text
            if (Input.GetKeyDown(KeyCode.E) && dialogueText.next == false && dialogueText.timeSinceOpened >= dialogueText.timeToWaitForKeyInput)
            {
                TriggerDialogue();
            }
        }
    }

    //find script to text
    public void TriggerDialogue()
    {
        FindObjectOfType<TypeWriterEffect>().StartDialogue(dialogue);
    }
}
