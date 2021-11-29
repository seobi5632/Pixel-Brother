using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageController : MonoBehaviour    //스테이지 이동을 위한 스크립트
{
    public RectTransform uiGroup;
    public int[] stageNum;
    public Image[] image;
    public GameObject potal;

    public StageManager stageManager;

    void Update()   //잠금 판정
    {
        if(GameManager.instance.isFinish)
        {
            potal.gameObject.SetActive(true);
            image[0].gameObject.SetActive(false);
        }
        if (GameManager.instance.stage1)
        {
            image[1].gameObject.SetActive(false);
        }
        if (GameManager.instance.stage1 && GameManager.instance.stage2)
        {
            image[2].gameObject.SetActive(false);
        }
        if (GameManager.instance.stage1 && GameManager.instance.stage2 && GameManager.instance.stage3)
        {
            image[3].gameObject.SetActive(false);
        }
    }

    public void Enter()
    {
        uiGroup.anchoredPosition = Vector3.zero;
    }

    public void Exit()
    {
        SoundManager.instance.PlaySE("Click");
        uiGroup.anchoredPosition = Vector3.down * 1000;
    }

    public void Click(int index)
    {
        stageManager.MoveToStage(stageNum[index] + 1);
        uiGroup.anchoredPosition = Vector3.down * 1000;
        SoundManager.instance.PlaySE("Click");
    }

}
