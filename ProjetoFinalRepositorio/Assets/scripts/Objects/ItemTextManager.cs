using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ItemTextManager : MonoBehaviour
{
    public float timeSinceOpened = 0f;
    public float timeToWaitForKeyInput = 0.5f;
    //public float width, height;

    public Image image;
    public Text nameText;

    public Animator animator;

    public bool next;

    private Queue<string> sentences;

    // Use this for initialization
    void Start()
    {
        next = false;

        sentences = new Queue<string>();
    }

    public void Update()
    {
        timeSinceOpened = timeSinceOpened + Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.E) && timeSinceOpened >= timeToWaitForKeyInput)
        {
            timeSinceOpened = 0f;
            EndDialogue();
        }
    }


    public void StartDialogue(ItemText text)
    {
        timeSinceOpened = 0f;

        image.sprite = text.image;

        //image.rectTransform.sizeDelta = new Vector2(width, height);

        animator.SetBool("isOpen", true);

        nameText.text = text.name;

        next = true;
    }

    public void EndDialogue()
    {
        animator.SetBool("isOpen", false);
        next = false;
    }
}
