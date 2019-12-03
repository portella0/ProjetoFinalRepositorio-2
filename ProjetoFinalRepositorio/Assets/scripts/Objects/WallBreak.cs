using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBreak : MonoBehaviour
{
    Animator anim;
    public GameObject[] walls;
    public GameObject wallCollider;
    public bool isBroken;
    int wallAnimationID;

    // Start is called before the first frame update
    void Start()
    {
        isBroken = false;
        wallAnimationID = Animator.StringToHash("breakWall");
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool(wallAnimationID, isBroken);

        if (isBroken)
        {
            DestroyObjectDelayed();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "golemArm")
        {
            isBroken = true;
        }
    }

    void DestroyObjectDelayed()
    {
        // Kills the game object in 5 seconds after loading the object
        Destroy(wallCollider);

        for (int i = 0; i < walls.Length; i++)
        {
            Destroy(walls[i], 2);
            isBroken = false;
        }
    }
}
