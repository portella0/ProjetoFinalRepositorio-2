using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSTcontroller : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] OST;
    public float[] timer, maxTimer;
    public bool[] played;
    public float[] volume;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        for (int i = 0; i < played.Length; i++)
        {
            audioSource.PlayOneShot(OST[i], volume[i]);
            played[i] = false;
            timer[i] = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 1; i < played.Length; i++)
        {
            timer[i] += 1* Time.deltaTime;
        }

        for (int i = 1; i < played.Length; i++)
        {
            if (timer[i] > maxTimer[i])
            {
                timer[i] = 0;
                if (OST[i] != null)
                {
                    audioSource.PlayOneShot(OST[i], volume[i]);
                }
            }
        }
    }
}
