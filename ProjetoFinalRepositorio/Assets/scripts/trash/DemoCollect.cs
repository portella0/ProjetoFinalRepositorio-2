using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoCollect : MonoBehaviour
{
    int playerLayer;

    public GameObject lightActive1;
    public GameObject lightActive2;

    void Start()
    {
        playerLayer = LayerMask.NameToLayer("Player");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {

        lightActive1.SetActive(true);
        lightActive2.SetActive(true);

        if (collision.gameObject.layer != playerLayer)
        {
            return;
        }
        gameObject.SetActive(false);
    }
}
