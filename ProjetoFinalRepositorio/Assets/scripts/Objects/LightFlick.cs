using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlick : MonoBehaviour
{
    public float quantity;	//amount light flicker
    public float speed;     //speed of flicker

    Light localLight;		//light component
    float intensity;        //intensity of light component
    float offset;           //offset (make all flickers are different)

    void Start()
    {
        localLight = GetComponentInChildren<Light>();

        //record  intensity and pick random offset
        intensity = localLight.intensity;
        offset = Random.Range(0, 10000);
    }

    void Update()
    {
        //determine a random intensity amount
        float amt = Mathf.PerlinNoise(Time.time * speed + offset, Time.time * speed + offset) * quantity;
        localLight.intensity = intensity + amt;
    }
}