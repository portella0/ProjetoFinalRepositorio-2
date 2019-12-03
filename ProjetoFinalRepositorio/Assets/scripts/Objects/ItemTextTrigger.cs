using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTextTrigger : MonoBehaviour
{
    public ItemText text;

    //public string name;

    //[TextArea(3, 10)]
    //public string[] sentences;

    public bool showText;
    public PlayerMovement playerCollision;
    public ItemTextManager itemText;
    public GameObject player;
    public GameObject ItemTextManager;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Character");
        ItemTextManager = GameObject.Find("ItemManager");
        showText = false;
        playerCollision = player.GetComponent<PlayerMovement>();
        itemText = ItemTextManager.GetComponent<ItemTextManager>();
    }

    void Update()
    {
        if (showText == true)
        {
            if (Input.GetKeyDown(KeyCode.E) && itemText.next == false && itemText.timeSinceOpened >= itemText.timeToWaitForKeyInput)
            {
                TriggerDialogue();
            }
        }
    }

    public void TriggerDialogue()
    {
        FindObjectOfType<ItemTextManager>().StartDialogue(text);
    }
}
