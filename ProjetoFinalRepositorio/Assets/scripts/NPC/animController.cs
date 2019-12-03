using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animController : MonoBehaviour
{
    Animator anim;
    int introID;
    int midID;
    int outroID;
    public bool touching;

    // Start is called before the first frame update
    void Start()
    {
        introID = Animator.StringToHash("intro");
        outroID = Animator.StringToHash("outro");
        midID = Animator.StringToHash("midtro");
        anim = GetComponent<Animator>();
        touching = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!touching)
        {
            anim.SetBool(introID, false);
            anim.SetBool(outroID, false);
            anim.SetBool(midID, false);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Character")
        {
            touching = true;
            anim.SetBool(introID, true);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Character")
        {
            anim.SetBool(midID, true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Character")
        {
            anim.SetBool(outroID, true);
            touching = false;
        }
    }
}
