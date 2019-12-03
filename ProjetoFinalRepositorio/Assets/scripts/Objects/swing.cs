using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swing : MonoBehaviour
{

    AudioSource audioSource;
    public AudioClip swingSound;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void swingSoundEffect()
    {
        if (swingSound != null)
        {
            audioSource.PlayOneShot(swingSound, 0.4f);
        }
    }
}
