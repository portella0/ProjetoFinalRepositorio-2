using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    public Animator anim;
    public EnemyMovement enemy;
    public GameObject attackCollider;

    int idleID;
    int angeryID;
    int attackID;
    int stunID;
    //int attackFowardID;

    AudioSource audioSource;
    public AudioClip enemyWalk;
    public AudioClip enemyAttack1, enemyAttack2, enemyAttack3;
    public AudioClip swordAttack1, swordAttack2;
    public AudioClip slowed;
    public AudioClip punch;
    public AudioClip fall;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        idleID = Animator.StringToHash("isIdle");
        angeryID = Animator.StringToHash("isAngery");
        attackID = Animator.StringToHash("isAttacking");
        stunID = Animator.StringToHash("isStun");
        //attackFowardID = Animator.StringToHash("isAttackingFoward");
        attackCollider.SetActive(false);
        Transform parent = transform.parent;
        anim = GetComponent<Animator>();
        enemy = parent.GetComponent<EnemyMovement>();
    }

    void Update()
    {
        anim.SetBool(idleID, enemy.isIdle);
        anim.SetBool(angeryID, enemy.OnChase);
        anim.SetBool(attackID, enemy.isAttacking);
        anim.SetBool(stunID, enemy.isStunned);
        //anim.SetBool(attackFowardID, enemy.isAttackingFoward);
    }

    public void StepAudio()
    {
        if (enemyWalk != null)
        {
            audioSource.PlayOneShot(enemyWalk, 0.3f);
        }
    }
    public void RunAudio()
    {
        if (enemyWalk != null)
        {
            audioSource.PlayOneShot(enemyWalk, 0.4f);
        }
    }
    public void swordAudio()
    {

            if (swordAttack1 != null)
            {
                audioSource.PlayOneShot(swordAttack1, 0.4f);
            }
        
    }
    public void attackAudio()
    {
        int rand = Random.Range(0, 3);

        if (rand == 0)
        {
            if (enemyAttack1 != null)
            {
                audioSource.PlayOneShot(enemyAttack1, 0.4f);
            }
        }
        else if (rand == 1)
        {
            if (enemyAttack2 != null)
            {
                audioSource.PlayOneShot(enemyAttack2, 0.4f);
            }
        }
        else if (rand == 2)
        {
            if (enemyAttack3 != null)
            {
                audioSource.PlayOneShot(enemyAttack3, 0.4f);
            }
        }

    }
    public void slowAudio()
    {
        if (slowed != null)
        {
            audioSource.PlayOneShot(slowed, 0.5f);
        }
    }
    public void stunAudio()
    {
        if (punch != null)
        {
            audioSource.PlayOneShot(punch, 0.5f);
        }
    }
    public void fallAudio()
    {
        if (fall != null)
        {
            audioSource.PlayOneShot(fall, 0.5f);
        }
    }

    public void EndStun()
    {
        enemy.isStunned = false;
    }

    public void ResetAnimation()
    {
        anim.Rebind();
    }

    public void makeAttack()
    {
        attackCollider.SetActive(true);
    }

    public void stopAttack()
    {
        attackCollider.SetActive(false);
    }
}
