using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    //타입 지정
    public enum Type { A, B, C, D };
    public Type enemyType;

    //타겟과 공격
    public Transform target;
    public BoxCollider meleeArea;
    public GameObject bulletObj;

    //상태 판단
    public bool isChase;
    public bool isAttack;
    public bool isDead;

    //망치 2번 피격 방지 쿨타임
    float curtime;

    
    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshs;
    public NavMeshAgent nav;
    public Animator anim;
    CharacterStats myStats;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        myStats = GetComponent<CharacterStats>();

        if (enemyType != Type.D) Invoke("ChaseStart", 2.0f);
    }

    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }

    void Update()
    {
        if (nav.enabled && enemyType != Type.D && GameManager.instance.isPlay)
        {
            nav.SetDestination(GameManager.player.transform.position);
            nav.isStopped = !isChase;
        }
    }

    void FreezeVelocity()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }
    void Targeting()
    {
        if (!GameManager.instance.isPlay && enemyType != Type.D)
        {
            anim.SetBool("isWalk", false);
            nav.isStopped = true;
        }
        if (!isDead && enemyType != Type.D)
        {
            float targetRadius = 1.5f;
            float targetRange = 3.0f;
            switch (enemyType)
            {
                case Type.A:
                    targetRadius = 1.5f;
                    targetRange = 3.0f;
                    break;
                case Type.B:
                    targetRadius = 1.0f;
                    targetRange = 12.0f;
                    break;
                case Type.C:
                    targetRadius = 0.5f;
                    targetRange = 25.0f;
                    break;
            }

            RaycastHit[] hit = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));

            if (hit.Length > 0 && !isAttack)
            {
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);
        switch (enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(1.0f);
                meleeArea.enabled = false;

                yield return new WaitForSeconds(1.0f);
                break;
            case Type.B:
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(0.3f);
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;

                yield return new WaitForSeconds(2.0f);
                break;
            case Type.C:
                yield return new WaitForSeconds(0.5f);
                GameObject instantBullet = Instantiate(bulletObj, transform.position, transform.rotation);
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20.0f;

                yield return new WaitForSeconds(2.0f);

                break;
        }

        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);
    }

    void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();
        CoolDownManager();
    }

    //망치 타격 쿨타임
    void CoolDownManager()
    {
        if (curtime >= 0.0f)
            curtime -= Time.deltaTime;
    }

    bool IsDamaged()
    {
        if (curtime <= 0.0f)
            return false;

        return true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            if (IsDamaged()) return;

            Weapon weapon = other.GetComponent<Weapon>();
            myStats.TakeDamage(weapon.damage);
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec, false));
            curtime = 0.3f;
        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            myStats.TakeDamage(bullet.damage);
            Vector3 reactVec = transform.position - other.transform.position;
            Destroy(other.gameObject);

            StartCoroutine(OnDamage(reactVec, false));
        }
    }

    public void HitByGrenade(Vector3 explosionPos)
    {
        myStats.TakeDamage(100);
        Vector3 reactVec = transform.position - explosionPos;

        StartCoroutine(OnDamage(reactVec, true));
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)  //무엇에 맞았는지에 따라 날아가는 방식의 변화를 줌
    {
        SoundManager.instance.PlaySE("Hurt");

        foreach(var mesh in meshs)
           mesh.material.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        if(myStats.currentHealth > 0)
        {
            foreach (var mesh in meshs)
                mesh.material.color = Color.white;
        }
        else
        {
            anim.SetBool("isWalk", isChase);
            foreach (var mesh in meshs)
                mesh.material.color = Color.gray;
            gameObject.layer = 14;
            isDead = true;
            isChase = false;

            nav.enabled = false;
            anim.SetTrigger("Die");

            if(isGrenade)
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up * 3.0f;

                rigid.freezeRotation = false;
                rigid.AddForce(reactVec * 5.0f, ForceMode.Impulse);
                rigid.AddTorque(reactVec * 15.0f, ForceMode.Impulse);

            }
            else
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;
                rigid.AddForce(reactVec * 1.0f, ForceMode.Impulse);
            }
            Destroy(gameObject, 2.0f);
        }

    }
}