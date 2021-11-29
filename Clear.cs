using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Tutorial 응용
[System.Serializable]
public class Dialogue2
{
    [TextArea]
    public string dialogue2;
}
public class Clear : MonoBehaviour
{
    [SerializeField] private CanvasRenderer sprite_DialogueBox;
    [SerializeField] private Text txt_Dialogue;

    public GameObject uiGroup;

    public Text text;

    private int count = 0;
    private bool isClick = true;
    Transform target;

    public Dialogue2[] dialogue2;
    void Awake()
    {
        target = GameManager.instance.play.transform;
    }

    private void OnOff(bool _flag)
    {
        sprite_DialogueBox.gameObject.SetActive(_flag);
        txt_Dialogue.gameObject.SetActive(_flag);
        GameManager.instance.isDialogue = _flag;
        isClick = _flag;
    }

    void Update()
    {
        transform.LookAt(target.position);
        if (Input.GetKeyDown(KeyCode.Space) && isClick)
        {
            if (count < dialogue2.Length) NextDialogue();
            else
            {
                if (count > dialogue2.Length) return;
                OnOff(false);
                GameManager.instance.isPlay = false;
                Time.timeScale = 0.0f;
                text.text = "Game Clear!";
                GameManager.instance.GameOver();
                GameManager.instance.GameClear();
            }
        }
    }

    public void Enter()
    {
        uiGroup.SetActive(true);
    }

    public void Exit()
    {
        SoundManager.instance.PlaySE("Click");
        Application.Quit();
        print("게임종료");
    }

    public void NextDialogue()
    {
        txt_Dialogue.text = dialogue2[count].dialogue2;
        count++;
    }
}
