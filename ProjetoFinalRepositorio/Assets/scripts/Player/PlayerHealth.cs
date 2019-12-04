using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("timer Properties")]
    public float shieldRegenTimer;// = 0f;
    public float waitRegenShield;// = 0.5f;
    public float colorTimer;

    [Header("bars")]
    public SimpleHealthBar shieldBar;
    //public SimpleHealthBar[] healthBar;
    public SimpleHealthBar healthBar;
    public SimpleHealthBar staminaBar;
    public SimpleHealthBar invBar;
    public SimpleHealthBar magicBar;
    public GameObject shieldSprite;
    public GameObject staminaSprite;
    public GameObject invSprite;
    public GameObject magicSprite;
    //public GameObject[] healthSprite;
    public GameObject healthSprite;
    public GameObject lifeParticleSkel, lifeParticleMage, lifeParticleGolem;

    [Header("bar properties")]
    //public float[] health;
    public float health;
    public float shield;
    public float stamina;
    public float invMana;
    public float magicMana;
    //public float[] maxHealth;
    public float maxHealth;
    public float maxShield;
    public float maxStamina;
    public float maxInvMana;
    public float maxMagicMana;

    public float mathStamina, mathInv, mathMagic;

    [Header("death")]
    public GameObject[] characters;
    public GameObject deathEffect;
    public GameObject particlePos;
    public float particleHeight;
    public bool calledDeath;
    public bool calledControl;
    public bool onTrap;
    public float soundControl;
    public float soundControlMax;
    public bool checkThePoint;

    [Header("player status")]
    public PlayerMovement characterType;
    public bool isAlive = true;
    public bool shouldMove = true;
    public bool touchingGround;

    [Header("sprites")]
    public SpriteRenderer skeletonSprite;
    public SpriteRenderer archerHeadSprite;
    public SpriteRenderer golemSprite;
    public bool damage;

    [Header("respawn")]
    public GameObject checkpoint;

    int trapsLayer;
    int groundLayer;

    [Header("Audio")]
    AudioSource audioSource;
    public AudioClip skelDamage;
    public AudioClip mageDamage;
    public AudioClip skelContinuousDamage;
    public AudioClip mageContinusDamage;
    public AudioClip golemShieldTrapDamage;
    public AudioClip golemHealthTrapDamage;
    public AudioClip golemShieldGuardDamage;
    public AudioClip golemHealthGuardDamage;
    public AudioClip checkpointSound;
    public AudioClip healSound;
    public AudioClip respawnSound;
    public AudioClip skelDeath1, skelDeath2, skelDeath3, skelDeath4;
    public AudioClip splash;

    void Start()
    {
        mathStamina = 100;
        mathInv = 100;
        mathMagic = 100;
        touchingGround = false;
        checkThePoint = true;
        soundControl = 0;
        onTrap = false;
        calledDeath = false;
        calledControl = false;
        audioSource = GetComponent<AudioSource>();
        colorTimer = 0;
        damage = false;
        shouldMove = true;
        health = maxHealth;
        shield = maxShield;
        stamina = maxStamina;
        invMana = maxInvMana;
        magicMana = maxMagicMana;
        shieldSprite = GameObject.Find("ShieldBar");
        healthSprite = GameObject.Find("SkeletonBar");
        staminaSprite = GameObject.Find("staminaBarSprite");
        invSprite = GameObject.Find("invBarSprite");
        magicSprite = GameObject.Find("magicBarSprite");
        healthBar = GameObject.Find("health").GetComponent<SimpleHealthBar>();
        shieldBar = GameObject.Find("shield").GetComponent<SimpleHealthBar>();
        staminaBar = GameObject.Find("stamina").GetComponent<SimpleHealthBar>();
        invBar = GameObject.Find("invisibility").GetComponent<SimpleHealthBar>();
        magicBar = GameObject.Find("magic").GetComponent<SimpleHealthBar>();
        //skeletonSprite = GameObject.Find("skeleton").GetComponent<SpriteRenderer>();
        //archerSprite = GameObject.Find("archer").GetComponent<SpriteRenderer>();
        //golemSprite = GameObject.Find("golem").GetComponent<SpriteRenderer>();
        //healthSprite[0] = GameObject.Find("SkeletonBar");
        //healthSprite[1] = GameObject.Find("ArcherBar");
        //healthSprite[2] = GameObject.Find("GolemBar");
        trapsLayer = LayerMask.NameToLayer("Traps");
        groundLayer = LayerMask.NameToLayer("Plataforms");
        lifeParticleSkel.SetActive(false);
        lifeParticleMage.SetActive(false);
        lifeParticleGolem.SetActive(false);
    }

    void Update()
    {
        if (!shouldMove)
        {
            IsDead();
        }

        soundControl += 1 * Time.deltaTime;

        if (characterType.isSliding)
        {
            mathStamina -= 200 * Time.deltaTime;
        }

        if (characterType.slideTime < characterType.slideTimeMax && characterType.slideTime > 0 && !characterType.isSliding)
        {
            mathStamina += 40f * Time.deltaTime;
        }

        if (characterType.slideTime <= 0)
        {
            mathStamina = 0;
        }

        //cooldown time
        if (characterType.slideCooldown == 3 && !characterType.isSliding)
        {
            mathStamina = 100;
        }

        if (characterType.isMist == true)
        {
            mathInv -= 25 * Time.deltaTime;
            //mistTimer += 1 * Time.deltaTime;
        }

        if (characterType.mistTimer == 0 && !characterType.isMist)
        {
            mathInv = 100;
        }


        if (characterType.didMagic == true && characterType.magicTimer < characterType.magicMaxTimer)
        {
            mathMagic -= 100 * Time.deltaTime;
            //magicTimer += 1 * Time.deltaTime;
        }

        if (characterType.magicTimer == 0 && !characterType.didMagic)
        {
            mathMagic = 100;
        }


        //mathStamina = Mathf.Clamp(characterType.slideTime * 200, 0, 100);
        //mathInv = Mathf.Clamp(characterType.mistTimer * 200, 0, 100);
        //mathMagic = Mathf.Clamp(characterType.magicTimer * 200, 0, 100);

        stamina = mathStamina;
        invMana = mathInv;
        magicMana = mathMagic;

        if (isAlive)
        {
            shieldRegenTimer = shieldRegenTimer + Time.deltaTime;
            colorTimer += 1 * Time.deltaTime;
            if (colorTimer > 0.5f)
            {
                onTrap = false;
                if (skeletonSprite != null)
                {
                    skeletonSprite.color = new Color(1, 1, 1);
                }
                if (archerHeadSprite != null)
                {
                    archerHeadSprite.color = new Color(1, 1, 1);
                }
                if (golemSprite != null)
                {
                    golemSprite.color = new Color(1, 1, 1);
                }
            }
            if (characterType.character == 1)
            {
                skeletonSprite = GameObject.Find("skeleton").GetComponent<SpriteRenderer>();
                //healthSprite[0].SetActive(true);
                //healthSprite[1].SetActive(false);
                //healthSprite[2].SetActive(false);
                shieldSprite.SetActive(false);
                staminaSprite.SetActive(true);
                magicSprite.SetActive(false);
                invSprite.SetActive(false);
            }
            else if (characterType.character == 2)
            {
                archerHeadSprite = GameObject.Find("archer head").GetComponent<SpriteRenderer>();
                //healthSprite[0].SetActive(false);
                //healthSprite[1].SetActive(true);
                //healthSprite[2].SetActive(false);
                shieldSprite.SetActive(false);
                staminaSprite.SetActive(false);
                magicSprite.SetActive(true);
                invSprite.SetActive(true);
            }
            else if (characterType.character == 3)
            {
                golemSprite = GameObject.Find("golem").GetComponent<SpriteRenderer>();
                shieldSprite.SetActive(true);
                //healthSprite[0].SetActive(false);
                //healthSprite[1].SetActive(false);
                //healthSprite[2].SetActive(true);
                staminaSprite.SetActive(false);
                magicSprite.SetActive(false);
                invSprite.SetActive(false);
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

            if (!damage)
            {
                //skeletonSprite.color = new Color(1, 1, 1);
                //archerSprite.color = new Color(1, 1, 1);
                //golemSprite.color = new Color(1, 1, 1);
            }

            healthBar.UpdateBar(health, maxHealth);
            staminaBar.UpdateBar(stamina, maxHealth);
            invBar.UpdateBar(invMana, maxHealth);
            magicBar.UpdateBar(magicMana, maxHealth);

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
            if (checkpoint != null && collision.gameObject != checkpoint)
            {
                checkThePoint = true;
                checkpoint.GetComponentInChildren<Light>().range = 0;
            }
            checkpoint = collision.gameObject;
            checkpoint.GetComponentInChildren<Light>().range = 15;
            if (checkpointSound != null && checkThePoint)
            {
                checkThePoint = false;
                audioSource.PlayOneShot(checkpointSound, 0.2f);
            }
        }
        if (collision.gameObject.layer == trapsLayer && characterType.character == 3)
        {
            if (collision.gameObject.name == "instaLava" || collision.gameObject.name == "River")
            {
                if (splash != null)
                {
                    audioSource.PlayOneShot(splash, 0.5f);
                }
            }
        }
        if (collision.gameObject.layer == trapsLayer && (characterType.character != 3 || shield <= 0))
        {
            onTrap = true;
            if (!characterType.isMist && characterType.character == 2)
            {
                if (collision.gameObject.name == "instaLava" || collision.gameObject.name == "River")
                {
                    if (splash != null)
                    {
                        audioSource.PlayOneShot(splash, 0.5f);
                    }
                }
                health = 0;
                IsDead();
            }
            else if (characterType.character == 1)
            {
                if (collision.gameObject.name == "instaLava" || collision.gameObject.name == "River")
                {
                    if (splash != null)
                    {
                        audioSource.PlayOneShot(splash, 0.5f);
                    }
                }
                health = 0;
                IsDead();
            }
        }
        if (collision.gameObject.tag == "enemyAttack")
        {
            colorTimer = 0;
            if (characterType.character == 3)
            {
                if (shield > 0)
                {
                    shield -= collision.gameObject.GetComponentInParent<EnemyMovement>().damage;
                    damage = true;
                    golemSprite.color = new Color(1f, 0.5f, 0);
                    shieldRegenTimer = 0f;
                    if (golemShieldGuardDamage != null)
                    {
                        audioSource.PlayOneShot(golemShieldGuardDamage);
                    }
                }
                else if (shield <= 0)
                {
                    health -= collision.gameObject.GetComponentInParent<EnemyMovement>().damage;
                    golemSprite.color = new Color(1f, 0.15f, 0.15f);
                    damage = true;
                    if (golemHealthGuardDamage != null)
                    {
                        audioSource.PlayOneShot(golemHealthGuardDamage);
                    }
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
                        skeletonSprite.color = new Color(1f, 0.15f, 0.15f);
                        damage = true;
                        health -= collision.gameObject.GetComponentInParent<EnemyMovement>().damage;
                        if (skelDamage != null)
                        {
                            audioSource.PlayOneShot(skelDamage, 0.5f);
                        }
                    }
                }
                else if (characterType.character == 2)
                {
                    if (!characterType.isMist)
                    {
                        archerHeadSprite.color = new Color(1f, 0.15f, 0.15f);
                        damage = true;
                        health -= collision.gameObject.GetComponentInParent<EnemyMovement>().damage;
                        if (mageDamage != null)
                        {
                            audioSource.PlayOneShot(mageDamage);
                        }
                    }
                }
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Checkpoint")
        {
            if(health < maxHealth/2)
            {
                health += 15 * Time.deltaTime;
                if (characterType.character == 1 && lifeParticleSkel != null && lifeParticleMage != null && lifeParticleGolem != null)
                {
                    lifeParticleSkel.SetActive(true);
                    lifeParticleMage.SetActive(false);
                    lifeParticleGolem.SetActive(false);
                }
                else if (characterType.character == 2 && lifeParticleSkel != null && lifeParticleMage != null && lifeParticleGolem != null)
                {
                    lifeParticleSkel.SetActive(false);
                    lifeParticleMage.SetActive(true);
                    lifeParticleGolem.SetActive(false);
                }
                else if (characterType.character == 3 && lifeParticleSkel != null && lifeParticleMage != null && lifeParticleGolem != null)
                {
                    lifeParticleSkel.SetActive(false);
                    lifeParticleMage.SetActive(false);
                    lifeParticleGolem.SetActive(true);
                }
                if (soundControl > soundControlMax && healSound != null)
                {
                    soundControl = 0;
                    audioSource.PlayOneShot(healSound);
                }
            }
            else if(lifeParticleSkel != null && lifeParticleMage != null && lifeParticleGolem != null)
            {
                lifeParticleSkel.SetActive(false);
                lifeParticleMage.SetActive(false);
                lifeParticleGolem.SetActive(false);
            }
        }
        if (collision.gameObject.tag == "Enemy")
        {
            colorTimer = 0;
            Debug.Log("Encoxando inimigo");
            if (characterType.character == 3)
            {
                if (shield > 0)
                {
                    //golemSprite.color = Color.Lerp(new Color(1, 0.5f, 0), new Color(1, 1, 1), Mathf.PingPong(Time.time, 0.1f));
                    shield -= 10 * Time.deltaTime;
                    damage = true;
                    golemSprite.color = new Color(1f, 0.5f, 0);
                    shieldRegenTimer = 0f;
                    if (soundControl > 1 && golemShieldTrapDamage != null)
                    {
                        soundControl = 0;
                        audioSource.PlayOneShot(golemShieldTrapDamage);
                    }
                }
                else if (shield <= 0)
                {
                    //health[(characterType.character) - 1] -= 10 * Time.deltaTime;
                    health -= 10 * Time.deltaTime;
                    golemSprite.color = new Color(1f, 0.15f, 0.15f);
                    damage = true;
                    if (soundControl > 1 && golemHealthTrapDamage != null)
                    {
                        soundControl = 0;
                        audioSource.PlayOneShot(golemHealthTrapDamage);
                    }
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
                        skeletonSprite.color = new Color(1f, 0.15f, 0.15f);
                        //health[(characterType.character) - 1] -= 10 * Time.deltaTime;
                        damage = true;
                        health -= 10 * Time.deltaTime;
                        if (soundControl > 1 && skelContinuousDamage != null)
                        {
                            soundControl = 0;
                            audioSource.PlayOneShot(skelContinuousDamage);
                        }
                    }
                }
                else if (characterType.character == 2)
                {
                    if (!characterType.isMist)
                    {
                        archerHeadSprite.color = new Color(1f, 0.15f, 0.15f);
                        //health[(characterType.character) - 1] -= 10 * Time.deltaTime;
                        damage = true;
                        health -= 10 * Time.deltaTime;
                    }
                    if (soundControl > 1 && mageContinusDamage != null)
                    {
                        soundControl = 0;
                        audioSource.PlayOneShot(mageContinusDamage);
                    }
                }
            }
        }
        if (collision.gameObject.layer == trapsLayer && characterType.character != 3)
        {
            colorTimer = 0;
            if (!characterType.isMist)
            {
                health = 0;
                IsDead();
            }
        }
        else if (collision.gameObject.layer == trapsLayer && characterType.character == 3)
        {
            onTrap = true;
            colorTimer = 0;
            ////golemSprite.color = Color.Lerp(new Color(1, 0.5f, 0), new Color(1, 1, 1), Mathf.PingPong(Time.time, 0.1f));
            //shield -= 10 * Time.deltaTime;
            //damage = true;
            //golemSprite.color = new Color(1, 0.5f, 0);
            //shieldRegenTimer = 0f;
            //if(shield <=0)
            //{
            //    IsDead();
            //}
            if (characterType.character == 3)
            {
                if (shield > 0)
                {
                    //golemSprite.color = Color.Lerp(new Color(1, 0.5f, 0), new Color(1, 1, 1), Mathf.PingPong(Time.time, 0.1f));
                    shield -= 10 * Time.deltaTime;
                    damage = true;
                    golemSprite.color = new Color(1f, 0.5f, 0);
                    shieldRegenTimer = 0f;
                    if (soundControl > 1 && golemShieldTrapDamage != null)
                    {
                        soundControl = 0;
                        audioSource.PlayOneShot(golemShieldTrapDamage);
                    }
                }
                else if (shield <= 0)
                {
                    //health[(characterType.character) - 1] -= 10 * Time.deltaTime;
                    health -= 10 * Time.deltaTime;
                    golemSprite.color = new Color(1f, 0.15f, 0.15f);
                    damage = true;
                    if (soundControl > 1 && golemHealthTrapDamage != null)
                    {
                        soundControl = 0;
                        audioSource.PlayOneShot(golemHealthTrapDamage);
                    }
                    //if (health[(characterType.character) - 1] <= 0)
                    if (health <= 0)
                    {
                        IsDead();
                    }
                }
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
                archerHeadSprite.color = new Color(1, 1, 1);
            }
            if (golemSprite != null)
            {
                golemSprite.color = new Color(1, 1, 1);
            }
        }
        if (collision.gameObject.tag == "enemyAttack")
        {
            if (skeletonSprite != null)
            {
                skeletonSprite.color = new Color(1, 1, 1);
            }
            if (archerHeadSprite != null)
            {
                archerHeadSprite.color = new Color(1, 1, 1);
            }
            if (golemSprite != null)
            {
                golemSprite.color = new Color(1, 1, 1);
            }
        }
        if (collision.gameObject.layer == trapsLayer && characterType.character == 3)
        {
            onTrap = false;
            golemSprite.color = new Color(1, 1, 1);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == groundLayer)
        {
            touchingGround = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == groundLayer)
        {
            touchingGround = false;
        }
    }

    public void resetStats()
    {
        shouldMove = true;
        calledControl = false;
        calledDeath = false;
        health = maxHealth;
        characterType.mistTimer = 0;
        characterType.isMist = false;
        //health[0] = maxHealth[0];
        //health[1] = maxHealth[1];
        //health[2] = maxHealth[2];
        shield = maxShield;
        characterType.animate.ResetAnimation();
        if (respawnSound != null)
        {
            audioSource.PlayOneShot(respawnSound, 0.5f);
        }
    }

    public void IsDead()
    {
        shouldMove = false;
        if (characterType.character == 2 && calledDeath == false)
        {
            //particlePos = new Vector3(transform.position.x, transform.position.y + particleHeight, transform.position.z);
            calledDeath = true;
            Instantiate(deathEffect, particlePos.transform.position, transform.rotation);
        }

        if(characterType.character==1 && calledDeath == false)
        {
            int rand = Random.Range(0, 4);
            calledDeath = true;

            if (rand == 0)
            {
                if (skelDeath1 != null)
                {
                    audioSource.PlayOneShot(skelDeath1, 0.4f);
                }
            }
            else if (rand == 1)
            {
                if (skelDeath2 != null)
                {
                    audioSource.PlayOneShot(skelDeath2, 0.4f);
                }
            }
            else if (rand == 2)
            {
                if (skelDeath3 != null)
                {
                    audioSource.PlayOneShot(skelDeath3, 0.4f);
                }
            }
            else if (rand == 3)
            {
                if (skelDeath4 != null)
                {
                    audioSource.PlayOneShot(skelDeath4, 0.4f);
                }
            }
        }

        if (!isAlive && calledControl == false)
        {
            calledControl = true;
            // characterType.animate.ResetAnimation();         

            //isAlive = false;          

            //characters[0].SetActive(false);
            // characters[1].SetActive(false);
            //characters[2].SetActive(false);

            GameControl.PlayerDied();
        }
    }
}

