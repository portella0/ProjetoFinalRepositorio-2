using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[System.Serializable]
public class Dialogue 
{
    [Header("DialogueBox")]
    public string name;
    public Sprite image;
    public float width, height;
    public float offsetX, offsetY;
    public Color Textcolor;

    [Header("Audio")]
    public AudioClip NPCsound;
    public float volume;
    //[Header("animation")]
    //public Animator anim;
    //int animateID;
    //public bool shouldAnimate;

    [TextArea(3, 10)]
    public string[] sentences;

    void Start()
    {
        //shouldAnimate = false;
        //animateID = Animator.StringToHash("shouldAnimate");
        //anim = GetComponent<Animator>();
    }

    void Update()
    {
        //anim.SetBool(animateID, shouldAnimate);
    }
}
