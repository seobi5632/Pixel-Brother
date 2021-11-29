using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStageController : StageManager
{
    public GameObject Luna;     //보스를 잡을시 나타나기 위한 게임오브젝트
    public GameObject boss;     //프리펩에있는 보스몬스터를 소환시키기위한 게임오브젝트
    public Transform[] spawnPoint; // 스폰 위치
    public GameObject parentObj; //보스가 살아있는지 확인을 위한 변수
    bool Check = false;     //반복 호출되는 함수에서 무한 호출 방지를 위한 bool 값
    bool isSpwan = false;   //스폰 확인
    void Start()
    {
        InvokeRepeating("Spawn", 2.0f, 1.0f); // Start 함수 호출 후 2초뒤 실행되며 1초마다 반복
    }



    void Spawn()
    {
        //플레이어가 죽어 게임을 종료하거나 베이스로 돌아갈 때 남아있는 적을 삭제시키기위한 함수, 반복호출을 막기위해 return 사용
        if (GameManager.instance.isPlayerDie)
        {
            Transform child = parentObj.GetComponentInChildren<Transform>();
            foreach (Transform iter in child)
            {
                if (iter != this.transform)
                {
                    Destroy(iter.gameObject);
                }
            }
            return;
        }
        //스테이지에 스폰장소를 지정해 스폰 판정
        if (!isSpwan && !Check && GameManager.instance.isPlay && GameManager.instance.stage == 4)
        {
            for (int i = 0; i < spawnPoint.Length; i++)
            {
                GameObject Enemy = Instantiate(boss, spawnPoint[i].position, Quaternion.identity);
                Enemy.transform.parent = parentObj.transform;
                GameManager.instance.boss = Enemy.GetComponent<Boss>();
                GameManager.instance.boss.target = GameManager.instance.play.transform;
            }
            isSpwan = true;
            Check = true;
        }
        //클리어 확인
        if (Check && EndStage())
        {
            Luna.SetActive(true);
            GameManager.instance.bossstage = true; //스테이지 잠금해제
            GameManager.instance.isBattle = false;
        }
    }

    //남아있는 적 확인
    bool EndStage()
    {
        if (parentObj.transform.childCount == 0)
        {
            Check = false;
            return true;
        }
        else return false;
    }


}
