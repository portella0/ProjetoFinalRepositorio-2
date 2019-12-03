using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("timer Properties")]
    public float shieldRegenTimer;// = 0f;
    public float waitRegenShield;// = 0.5f;

    [Header("bars")]
    public SimpleHealthBar shieldBar;
    //public SimpleHealthBar[] healthBar;
    public SimpleHealthBar healthBar;
    public GameObject shieldSprite;
    //public GameObject[] healthSprite;
    public GameObject healthSprite;

    [Header("bar properties")]
    //public float[] health;
    public float health;
    public float shield;
    //public float[] maxHealth;
    public float maxHealth;
    public float maxShield;

    [Header("death")]
    public GameObject[] characters;
    public GameObject deathEffect;
    Vector3 particlePos;
    public float particleHeight;

    [Header("player status")]
    public PlayerMovement characterType;
    public bool isAlive = true;   

    [Header("sprites")]
    public SpriteRenderer skeletonSprite;
    public SpriteRenderer archerHeadSprite;
    public SpriteRenderer archerBodySprite;
    public SpriteRenderer golemSprite;
    public bool damage;

    [Header("respawn")]
    public GameObject checkpoint;

    int trapsLayer;

    void Start()
    {
        damage = false;
        health = maxHealth;
        shield = maxShield;
        shieldSprite = GameObject.Find("ShieldBar");
        healthSprite = GameObject.Find("SkeletonBar");
        //skeletonSprite = GameObject.Find("skeleton").GetComponent<SpriteRenderer>();
        //archerSprite = GameObject.Find("archer").GetComponent<SpriteRenderer>();
        //golemSprite = GameObject.Find("golem").GetComponent<SpriteRenderer>();
        //healthSprite[0] = GameObject.Find("SkeletonBar");
        //healthSprite[1] = GameObject.Find("ArcherBar");
        //healthSprite[2] = GameObject.Find("GolemBar");
        trapsLayer = LayerMask.NameToLayer("Traps");
    }

    void Update()
    {
        if (isAlive)
        {
            shieldRegenTimer = shieldRegenTimer + Time.deltaTime;

            if (characterType.character == 1)
            {
                skeletonSprite = GameObject.Find("skeleton").GetComponent<SpriteRenderer>();
                //healthSprite[0].SetActive(true);
                //healthSprite[1].SetActive(false);
                //healthSprite[2].SetActive(false);
                shieldSprite.SetActive(false);
            }
            else if (characterType.character == 2)
            {
                archerHeadSprite = GameObject.Find("archer head").GetComponent<SpriteRenderer>();
                archerBodySprite = GameObject.Find("archer body").GetComponent<SpriteRenderer>();
                //healthSprite[0].SetActive(false);
                //healthSprite[1].SetActive(true);
                //healthSprite[2].SetActive(false);
                shieldSprite.SetActive(false);
            }
            else if (characterType.character == 3)
            {
                golemSprite = GameObject.Find("golem").GetComponent<SpriteRenderer>();
                shieldSprite.SetActive(true);
                //healthSprite[0].SetActive(false);
                //healthSprite[1].SetActive(false);
                //healthSprite[2].SetActive(true);
            }
            // (health[(characterType.character) - 1] <= 0)
            if (health <= 0)
            {
                IsDead();
            }            

            if (shieldRegenTimer >= waitRegenShield && shield < maxShield)
            {
                shield += 0.5f * Time.deltaTime;
            }

            transform.position += Vector3.zero;

            damage = false;

            if(!damage)
            {
               //skeletonSprite.color = new Color(1, 1, 1);
               //archerSprite.color = new Color(1, 1, 1);
               //golemSprite.color = new Color(1, 1, 1);
            }

            healthBar.UpdateBar(health, maxHealth);

            //healthBar[0].UpdateBar(health[0], maxHealth[0]);
            //healthBar[1].UpdateBar(health[1], maxHealth[1]);
            //healthBar[2].UpdateBar(health[2], maxHealth[2]);

            shieldBar.UpdateBar(shield, maxShield);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Checkpoint") 
        {
            if (checkpoint != null)
            {
                checkpoint.GetComponentInChildren<Light>().range = 0;
            }
            checkpoint = collision.gameObject;
            checkpoint.GetComponentInChildren<Light>().range = 5;
        }
        if (collision.gameObject.layer == trapsLayer && (characterType.character != 3 || shield <= 0))
        {
            if (!characterType.isMist && characterType.character == 2)
            {
                IsDead();
            }
            else if (characterType.character == 1)
            {
                IsDead();
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Debug.Log("Encoxando inimigo");
            if (characterType.character == 3)
            {
                if (shield > 0)
                {
                    //golemSprite.color = Color.Lerp(new Color(1, 0.5f, 0), new Color(1, 1, 1), Mathf.PingPong(Time.time, 0.1f));
                    shield -= 10 * Time.deltaTime;
                    damage = true;
                    golemSprite.color = new Color(1, 0.5f, 0);
                    shieldRegenTimer = 0f;
                }
                else if (shield <= 0)
                {
                    //health[(characterType.character) - 1] -= 10 * Time.deltaTime;
                    health -= 10 * Time.deltaTime;
                    golemSprite.color = new Color(1, 0, 0);
                    damage = true;
                    //if (health[(characterType.character) - 1] <= 0)
                    if (health <= 0)
                    {
                        IsDead();
                    }
                }
            }
            else if (characterType.character != 3)
            {
                if (characterType.character == 1)
                {
                    if (!characterType.isSliding)
                    {
                        skeletonSprite.color = new Color(1, 0, 0);
                        //health[(characterType.character) - 1] -= 10 * Time.deltaTime;
                        damage = true;
                        health -= 10 * Time.deltaTime;
                    }
                }
                else if (characterType.character == 2)
                {
                    if (!characterType.isMist)
                    {
                        archerHeadSprite.color = new Color(1, 0, 0);
                        archerBodySprite.color = new Color(1, 0, 0);
                        //health[(characterType.character) - 1] -= 10 * Time.deltaTime;
                        damage = true;
                        health -= 10 * Time.deltaTime;
                    }
                }
            }
        }
        if (collision.gameObject.layer == trapsLayer && characterType.character != 3)
        {
            if (!characterType.isMist)
            {
                IsDead();
            }
        }
        else if (collision.gameObject.layer == trapsLayer && characterType.character == 3)
        {
            //golemSprite.color = Color.Lerp(new Color(1, 0.5f, 0), new Color(1, 1, 1), Mathf.PingPong(Time.time, 0.1f));
            shield -= 10 * Time.deltaTime;
            damage = true;
            golemSprite.color = new Color(1, 0.5f, 0);
            shieldRegenTimer = 0f;
            if(shield <=0)
            {
                IsDead();
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            if (skeletonSprite != null)
            {
                skeletonSprite.color = new Color(1, 1, 1);
            }
            if (archerHeadSprite != null)
            {
                archerBodySprite.color = new Color(1, 1, 1);
                archerHeadSprite.color = new Color(1, 1, 1);
            }
            if (golemSprite != null)
            {
                golemSprite.color = new Color(1, 1, 1);
            }
        }
        if (collision.gameObject.layer == trapsLayer && characterType.character == 3)
        {
            golemSprite.color = new Color(1, 1, 1);
        }
    }

    public void resetStats()
    {
        health = maxHealth;
        characterType.mistTimer = 0;
        characterType.isMist = false;
        //health[0] = maxHealth[0];
        //health[1] = maxHealth[1];
        //health[2] = maxHealth[2];
        shield = maxShield;
    }

    void IsDead()
    {
        if (isAlive)
        {
            characterType.animate.ResetAnimation();

            isAlive = false;

            particlePos = new Vector3(transform.position.x, transform.position.y + particleHeight, transform.position.z);

            Instantiate(deathEffect, particlePos, transform.rotation);

            characters[0].SetActive(false);
            characters[1].SetActive(false);
            characters[2].SetActive(false);

            GameControl.PlayerDied();
        }
    }
}

