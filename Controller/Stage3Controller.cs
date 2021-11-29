using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage3Controller : StageManager
{
    public Transform[] spawnPoint;
    public GameObject Enemys;
    public GameObject parentObj;
    bool Check;

    void Start()
    {
        InvokeRepeating("Spawn", 2.0f, 1.0f);
    }
    void Update()
    {
        GameManager.instance.enmeyCntC = parentObj.transform.childCount;
    }
    void Spawn()
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
        if (!GameManager.instance.isSpwan && !Check && GameManager.instance.isPlay && GameManager.instance.stage == 3)
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
                GameManager.instance.stage3 = true; //스테이지 잠금해제
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
