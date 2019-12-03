using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectThings : MonoBehaviour
{
    public GameObject effectParticle; 

    int playerLayer;                        


    void Start()
    {
        playerLayer = LayerMask.NameToLayer("Player");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != playerLayer)
        {
            return;
        }

        Instantiate(effectParticle, transform.position, transform.rotation);

        gameObject.SetActive(false);
    }
}

