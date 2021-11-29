using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{
    public Text[] texts;
    public Image[] buttons;
    public int[] sounds;
    bool isClick = true;

    public void Click(int index)
    {
        if (isClick)
        {
            SoundOff(index);
            isClick = false;
        }
        else
        {
            SoundOn(index);
            isClick = true;
        }
    }

    void SoundOff(int index)
    {
        buttons[index].color = Color.gray;
        if (index == 0)
        {
            texts[index].text = "SOUND Off";
            SoundManager.instance.SetSoundVolume(0);
        }
        else
        {
            texts[index].text = "BGM Off";
            SoundManager.instance.SetMusicVolume(0);
        }
    }

    void SoundOn(int index)
    {
        buttons[index].color = Color.white;
        if (index == 0)
        {
            SoundManager.instance.PlaySE("Click");
            texts[index].text = "SOUND ON";
            SoundManager.instance.SetSoundVolume(1);
        }
        else
        {
            SoundManager.instance.PlaySE("Click");
            texts[index].text = "BGM ON";
            SoundManager.instance.SetMusicVolume(1);
        }
    }

    public void Exit()
    {
        SoundManager.instance.PlaySE("Click");
        GameManager.instance.play.uiGroup.SetActive(false);
    }

    public void GameOver()
    {
        SoundManager.instance.PlaySE("Click");
        Application.Quit();
        print("게임 종료");
    }
}
