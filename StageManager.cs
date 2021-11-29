using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    public Transform[] stagePoint;      //이동할 스테이지 위치 저장
    public Image loadImage;             //이동하는 동안 화면을 바꿀 이미지
    public GameObject[] stages;         //stage onoff 판정
    public GameObject uiGroup;          //clear UI 그룹
    GameObject curStage;                //현재 스테이지 판정
    public Text txt;                    //보상 텍스트

    public void MoveToStage(int i)  //스테이지 이동
    {
        StartCoroutine(FadeIn());
        if (curStage != null) curStage.SetActive(false);
        curStage = stages[i];
        curStage.SetActive(true);
        SetPlayerPosition(i);
        GameManager.instance.stage = i;
        GameManager.instance.a = true;
        StartCoroutine(FadeOut());
        if (i == 0)
        {
            uiGroup.SetActive(false);
            GameManager.player.GetComponent<CharacterStats>().RecoveryHP(GameManager.player.GetComponent<CharacterStats>().maxHealth);
            GameManager.instance.isBattle = false;
            GameManager.instance.isPlayerDie = false;
        }
        else GameManager.instance.isBattle = true;

    }

    IEnumerator FadeIn()
    {
        loadImage.gameObject.SetActive(true);
        GameManager.instance.isPlay = false;
        float fadeCount = 0.0f;
        while (fadeCount < 1.0f)
        {
            fadeCount += 0.01f;
            yield return new WaitForSeconds(0.01f);
            loadImage.color = new Color(0, 0, 0, fadeCount);
        }

        yield return new WaitForSeconds(4.0f);
    }

    IEnumerator FadeOut()
    {
        float fadeCount = 1.0f;
        while (fadeCount > 0.0f)
        {
            fadeCount -= 0.01f;
            yield return new WaitForSeconds(0.01f);
            loadImage.color = new Color(0, 0, 0, fadeCount);
        }
        loadImage.gameObject.SetActive(false);
        GameManager.instance.isPlay = true;
    }
    void SetPlayerPosition(int i)   //스테이지 플레이어 포지션 변경
    {
        if (i == 0)
        {
            GameManager.player.GetComponent<CharacterStats>().RecoveryHP(GameManager.player.GetComponent<CharacterStats>().maxHealth);
        }
        GameManager.player.transform.position = stagePoint[i].position;
    }

    public virtual void Clear()     //스테이지 클리어
    {
        if (GameManager.instance.stage == 0) return;
        uiGroup.SetActive(true);
        txt.text = " + " + (GameManager.instance.stage * 1000);
        GameManager.instance.play.coin += GameManager.instance.stage * 1000;
        Time.timeScale = 0.0f;
        GameManager.instance.isPlay = false;
    }
    public void Back()      //베이스 복귀
    {
        SoundManager.instance.PlaySE("Click");
        uiGroup.SetActive(false);
        MoveToStage(0);
        Time.timeScale = 1.0f;
        GameManager.instance.isPlay = true;
    }
    public virtual void Next()  //다음 스테이지
    {
        SoundManager.instance.PlaySE("Click");
        uiGroup.SetActive(false);
        MoveToStage(GameManager.instance.stage+1);
        Time.timeScale = 1.0f;
        GameManager.instance.isPlay = true;
    }
}
