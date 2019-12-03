using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TypeWriterEffect : MonoBehaviour {

    public float timeSinceOpened = 0f;
    public float timeToWaitForKeyInput = 0.5f;

    public Image image;
    public Text nameText;
    public Text dialogueText;
    public bool next;

    public Animator animator;

    private Queue<string> sentences;

    // Use this for initialization
    void Start()
    {
        next = false;
        sentences = new Queue<string>();
    }

    public void Update()
    {
        //timer control to avoid a double action
        timeSinceOpened = timeSinceOpened + Time.deltaTime;

        //next text
        if (Input.GetKeyDown(KeyCode.E) && next == true)
        {
            timeSinceOpened = 0f;
            DisplayNextSentence();
        }
    }


    public void StartDialogue(Dialogue dialogue)
    {
        image.sprite = dialogue.image;

        //image.rectTransform.sizeDelta = new Vector2(width, height);

        animator.SetBool("isOpen", true);

        nameText.text = dialogue.name;

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
        next = true;
    }

    public void EndDialogue()
    {
        Debug.Log("Fecha desgraça");
        animator.SetBool("isOpen", false);
        next = false;
        
    }
}

