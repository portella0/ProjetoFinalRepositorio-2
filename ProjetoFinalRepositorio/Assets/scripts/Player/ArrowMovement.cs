using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowMovement : MonoBehaviour
{
    Rigidbody2D rigidBody;                  //The rigidbody component
    public bool move;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        move = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (move == true)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
        Destroy(gameObject, 5);
    }
    void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        move = false;
        Debug.Log(collisionInfo.gameObject.name);
    }
}
