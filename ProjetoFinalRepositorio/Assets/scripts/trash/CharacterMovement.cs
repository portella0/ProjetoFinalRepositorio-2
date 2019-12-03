using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float speed = 10f;

    public float sprintSpeed = 0.5f;

    public Rigidbody2D rb2d;
    // Use this for initialization
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (Input.GetAxisRaw("Horizontal") < 0)
        {
            rb2d.velocity = new Vector2(-speed, rb2d.velocity.y);
            if (Input.GetKey(KeyCode.LeftShift))
            {
                rb2d.velocity = new Vector2(-sprintSpeed, rb2d.velocity.y);
            }

        }

        else if (Input.GetAxisRaw("Horizontal") > 0)
        {
            rb2d.velocity = new Vector2(speed, rb2d.velocity.y);
            if (Input.GetKey(KeyCode.LeftShift))
            {
                rb2d.velocity = new Vector2(sprintSpeed, rb2d.velocity.y);
            }
        }
        else { rb2d.velocity = new Vector2(0, rb2d.velocity.y); }
    }
}
