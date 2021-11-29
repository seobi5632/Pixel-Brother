using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject meshObj;
    //폭발이펙트
    public GameObject effectObj;
    public Rigidbody rigid;

    //폭발 범위
    public float radious;

    void Start()
    {
        StartCoroutine(Explosion());
    }

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3.0f);
        SoundManager.instance.PlaySE("Granede");
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        meshObj.SetActive(false);
        effectObj.SetActive(true);


        RaycastHit[] hit = Physics.SphereCastAll(transform.position, radious, Vector3.up, 0.0f, LayerMask.GetMask("Enemy")); //적에게만 맞도록 레이어마스크 설정

        foreach(RaycastHit hitObj in hit)
        {
            hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);
        }

        Destroy(gameObject, 0.5f);
    }
}
