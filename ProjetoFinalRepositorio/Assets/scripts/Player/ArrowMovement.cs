using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowMovement : MonoBehaviour
{
    Rigidbody2D rigidBody;                  //The rigidbody component
    public bool move;
    public float speed;
    public GameObject sphere;
    public GameObject light1;
    public GameObject particlesTrail;
    public GameObject endParticle;
    [Header("Audio")]
    AudioSource audioSource;
    public AudioClip hitSound;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
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
    }

    void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        Instantiate(endParticle, transform.position, transform.rotation);
        move = false;
        Debug.Log(collisionInfo.gameObject.name);
        Destroy(sphere);
        Destroy(light1);
        particlesTrail.GetComponent<ParticleSystem>().Stop();
        Destroy(gameObject, 5);
        audioSource.PlayOneShot(hitSound, 1f);
        //audioSource.Stop();
    }

    void OnTriggerEnter2D(Collider2D collisionInfo)
    {
        if (collisionInfo.gameObject.tag == "magicColideEnemy")
        {
            move = false;
            Instantiate(endParticle, transform.position, transform.rotation);
            Destroy(sphere);
            Destroy(light1);
            particlesTrail.GetComponent<ParticleSystem>().Stop();
            Destroy(gameObject, 5);
        }        
    }
}
