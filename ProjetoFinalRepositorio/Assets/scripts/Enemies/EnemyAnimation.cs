using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    public Animator anim;
    public EnemyMovement enemy;

    int idleID;
    int angeryID;
    int attackID;

    // Start is called before the first frame update
    void Start()
    {
        idleID = Animator.StringToHash("isIdle");
        angeryID = Animator.StringToHash("isAngery");
        attackID = Animator.StringToHash("isAttacking");

        Transform parent = transform.parent;
        anim = GetComponent<Animator>();
        enemy = parent.GetComponent<EnemyMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool(idleID, enemy.isIdle);
        anim.SetBool(angeryID, enemy.OnChase);
        anim.SetBool(attackID, enemy.isAttacking);
    }

    public void ResetAnimation()
    {
        anim.Rebind();
    }
}
