using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    //미사일 프리펩과 소환위치
    public GameObject missile;
    public Transform missilePortA;
    public Transform missilePortB;
    
    //타겟추적
    Vector3 lookVec;
    Vector3 tauntVec;
    public bool isLook;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        nav.isStopped = true;
        StartCoroutine(Pattern());
    }

    void Update()
    {
        if(isDead)
        {
            StopAllCoroutines();
            return;
        }

        if (isLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 5.0f;
            transform.LookAt(target.position + lookVec);
        }
        else nav.SetDestination(tauntVec);
    }

    //패턴 실행
    IEnumerator Pattern()
    {
        yield return new WaitForSeconds(0.7f);

        int ranAction = Random.Range(0, 5);

        switch(ranAction)
        {
            case 0:
            case 1:
                StartCoroutine(MissileShot());
                break;
            case 2:
            case 3:
                StartCoroutine(RockShot());
                break;
            case 4:
                StartCoroutine(Taunt());
                break;
        }
    }

    //패턴
    IEnumerator MissileShot()
    {
        anim.SetTrigger("doShot");
        yield return new WaitForSeconds(0.2f);
        GameObject instantMissileA = Instantiate(missile, missilePortA.position, missilePortA.rotation);
        BossMissile bossMissileA = instantMissileA.GetComponent<BossMissile>();
        bossMissileA.target = target;

        yield return new WaitForSeconds(0.3f);
        GameObject instantMissileB = Instantiate(missile, missilePortB.position, missilePortB.rotation);
        BossMissile bossMissileB = instantMissileB.GetComponent<BossMissile>();
        bossMissileB.target = target;

        yield return new WaitForSeconds(2.0f);

        StartCoroutine(Pattern());
    }
    IEnumerator RockShot()
    {
        isLook = false;
        anim.SetTrigger("doBigshot");
        Instantiate(bulletObj, transform.position, transform.rotation);
        yield return new WaitForSeconds(3.0f);

        isLook = true;
        StartCoroutine(Pattern());
    }
    IEnumerator Taunt()
    {
        tauntVec = target.position + lookVec;

        isLook = false;
        nav.isStopped = false;
        boxCollider.enabled = false;
        anim.SetTrigger("doTaunt");

        yield return new WaitForSeconds(1.5f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(1.0f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1.0f);
        isLook = true;
        boxCollider.enabled = true;
        nav.isStopped = true;

        StartCoroutine(Pattern());
    }
    //패턴
}
