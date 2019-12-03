using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCharacter : MonoBehaviour
{
    bool run = false;

    Animator m_Animator;

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = -1;
            transform.localScale = scale;
        }
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = 1;
            transform.localScale = scale;
        }

        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            m_Animator.SetBool("run", true);
        }
        else
        {
            m_Animator.SetBool("run", false);
        }
    }
}
