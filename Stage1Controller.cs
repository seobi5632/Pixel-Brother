using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1Controller : StageManager
{
    public Transform[] spawnPoint;  //스폰 포인트
    public GameObject Enemys;       //소환되는 적 프리펩
    public GameObject parentObj;    //소환되는 적의 수을 알기위한 부모

    bool Check;

    void Start()
    {
        InvokeRepeating("Spawn", 2.0f , 1.0f);
    }

    void Update()
    {
        GameManager.instance.enmeyCntA = parentObj.transform.childCount;    //남은 몬스터 수 확인.
    }

    void Spawn()    //stage1,2,3,boss 내용이 같음.
    {
        if (GameManager.instance.isPlayerDie)
        {
            Transform child = parentObj.GetComponentInChildren<Transform>();
            foreach (Transform iter in child)
            {
                if (iter != this.transform)
                {
                    Destroy(iter.gameObject);
                }
                GameManager.instance.a = false;
            }
        }
        if (!GameManager.instance.isSpwan && !Check && GameManager.instance.isPlay && GameManager.instance.stage == 1 )
        {
            for (int i = 0; i < spawnPoint.Length; i++)
            {
                GameObject Enemy = Instantiate(Enemys, spawnPoint[i].position, Quaternion.identity);
                Enemy.transform.parent = parentObj.transform;          
            }
            GameManager.instance.isSpwan = true;
            Check = true;
        }
        if (Check && EndStage() && GameManager.instance.a)
        {
            Clear();
            if (GameManager.instance.play.GetComponent<CharacterStats>().currentHealth > 0 && !GameManager.instance.isPlay)
                GameManager.instance.stage1 = true;     //스테이지 잠금해제
            GameManager.instance.isSpwan = false;
        }
    }

    bool EndStage()
    {
        if (parentObj.transform.childCount == 0)
        {
            Check = false;
            return true;
        }
        else return false;
    }

    public override void Clear()
    {
        base.Clear();
    }
    public override void Next()
    {
        base.Next();
    }
}
