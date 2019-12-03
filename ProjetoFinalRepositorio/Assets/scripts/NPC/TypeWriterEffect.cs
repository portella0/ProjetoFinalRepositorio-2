using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TypeWriterEffect : MonoBehaviour {

    public float timeSinceOpened = 0f;
    public float timeToWaitForKeyInput = 0.1f;
    public Image image;
    public Text nameText;
    public Text dialogueText;
    public bool next;
    public GameObject frame;
    public Vector2 standardPos;
    public Animator animator;
    public PlayerMovement playerCollision;
    public GameObject player;
    public bool startedDialogue;
    AudioSource audioSource;
    public AudioClip NPCsound;
    public bool shouldAnimate;
    public float volume;
    private Queue<string> sentences;

    // Use this for initialization
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        startedDialogue = false;
        next = false;
        sentences = new Queue<string>();
        player = GameObject.Find("Character");
        playerCollision = player.GetComponent<PlayerMovement>();
        shouldAnimate = false;
    }

    public void Update()
    {
        //timer control to avoid a double action
        timeSinceOpened = timeSinceOpened + Time.deltaTime;

        //next text
        if (playerCollision.input.interactPressed && next == true)
        {
            timeSinceOpened = 0f;
            DisplayNextSentence();
        }
    }


    public void StartDialogue(Dialogue dialogue)
    {
        startedDialogue = true;
        //image.transform.position = new Vector2(0, 0); 
        shouldAnimate = true;
        //frame.GetComponent<Image>().color = dialogue.Textcolor;    
        volume = dialogue.volume;
        standardPos = new Vector2(dialogue.offsetX, dialogue.offsetY);

        image.GetComponent<Canvas>().sortingOrder = -2;
        image.GetComponent<Canvas>().sortingOrder = -3;
        image.rectTransform.sizeDelta = new Vector2(dialogue.width, dialogue.height);
        image.transform.position = new Vector2(image.transform.position.x + dialogue.offsetX, image.transform.position.y + dialogue.offsetY);

        image.sprite = dialogue.image;

        animator.SetBool("isOpen", true);

        nameText.text = dialogue.name;
        nameText.color = dialogue.Textcolor;
        //dialogueText.color = dialogue.Textcolor;

        if (dialogue.NPCsound != null)
        {
            NPCsound = dialogue.NPCsound;
            NPCAudio();
        }

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count <= 0)
        {
            Debug.Log("Fecha desgraça");
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
            if(NPCsound != null)
            {
                //audioSource.Play();
            }
        }
        next = true;
    }

    public void EndDialogue()
    {
        if (startedDialogue)
        {
            image.transform.position = new Vector2(image.transform.position.x - standardPos.x, image.transform.position.y - standardPos.y);
            startedDialogue = false;
        }
        animator.SetBool("isOpen", false);
        next = false;
        shouldAnimate = false;
    }

    public void NPCAudio()
    {
        audioSource.PlayOneShot(NPCsound, volume);
    }
}

