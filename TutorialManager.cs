using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]   // 대화 내용을 직접 적기위해 선언
public class Dialogue
{
    [TextArea]
    public string dialogue;
}
public class TutorialManager : MonoBehaviour
{
    //Dialogue 변화
    [SerializeField] private CanvasRenderer sprite_DialogueBox;
    [SerializeField] private Text txt_Dialogue;
    private int count = 0;

    //튜토리얼 몬스터
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject[] Enemys;
    [SerializeField] private GameObject parentObj;
    private bool isSpwan = false;
    private bool isClick = true;

    [SerializeField] private Dialogue[] dialogue;

    private void NextDialogue() //다음 대화이동
    {
        txt_Dialogue.text = dialogue[count].dialogue;
        count++;
    }

    private void Quest()    //대화에 중간중간 플레이어의 퀘스트
    {
        if (GameManager.instance.isPlay)
        {
            if (count == 5 || count == 6)
            {
                OnOff(false);
                if (count == 6 && !isSpwan)
                {
                    GameObject Enemy = Instantiate(Enemys[0], spawnPoint.position, Quaternion.identity);
                    Enemy.transform.parent = parentObj.transform;
                    isSpwan = true;
                }
            }
            if(count == 5 && GameManager.instance.play.hasWeapons[0] && !GameManager.instance.isShop)
            {
                OnOff(true);
            }
            if(count == 6)
            {
                if (parentObj.transform.childCount == 0)
                {
                    GameManager.player.GetComponent<CharacterStats>().RecoveryHP(100);
                    OnOff(true);
                }
            }
        }
    }
    private void OnOff(bool _flag)  //대화창 onoff
    {
        sprite_DialogueBox.gameObject.SetActive(_flag);
        txt_Dialogue.gameObject.SetActive(_flag);
        GameManager.instance.isDialogue = _flag;
        isClick = _flag;
    }

    void Update()
    {
        Quest();
        if (!GameManager.instance.isFinish)
        {
            if (Input.GetKeyDown(KeyCode.Space) && isClick)
            {
                if (count < dialogue.Length) NextDialogue();
                else
                {
                    OnOff(false);
                    GameManager.instance.isFinish = true;
                }
            }
        }
    }
}
