using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public RectTransform uiGroup;
    public Animator anim;
    public int[] itemPrice;
    public Text talkText;
    public string[] talkData;

    int count;

    Player enterPlayer;

    public void Enter(Player player)
    {
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero;
    }

    public void Exit()
    {
        SoundManager.instance.PlaySE("Click");
        anim.SetTrigger("doHello");
        uiGroup.anchoredPosition = Vector3.down * 1000;
    }

    public void Buy(int index)
    {
        int price = itemPrice[index];
        count = 0;

        if (price > enterPlayer.coin)
        {
            StopCoroutine(Talk());
            StartCoroutine(Talk());
            return;
        }

        if (index == 0)
        {
            SoundManager.instance.PlaySE("Click");
            enterPlayer.GetComponent<CharacterStats>().maxHealth += 20;
            enterPlayer.GetComponent<CharacterStats>().RecoveryHP(20);
        }

        else if (index == 1)
        {
            SoundManager.instance.PlaySE("Click");
            enterPlayer.carryAmmo += 50;
        }
        else if (index == 2)
        {
            SoundManager.instance.PlaySE("Click");
            count = 1;
            if (enterPlayer.hasGrenades == enterPlayer.maxhasGrenades)
            {
                StopCoroutine(Talk());
                StartCoroutine(Talk());
                return;
            }
            enterPlayer.hasGrenades += 1;
        }
        else if (index == 3)
        {
            SoundManager.instance.PlaySE("Click");
            enterPlayer.hasWeapons[0] = true;
        }
        else if (index == 4)
        {
            SoundManager.instance.PlaySE("Click");
            enterPlayer.hasWeapons[1] = true;
        }
        else if (index == 5)
        {
            SoundManager.instance.PlaySE("Click");
            enterPlayer.hasWeapons[2] = true;
        }
        enterPlayer.coin -= price;
    }

    IEnumerator Talk()
    {
        if (count == 0)
        {
            talkText.text = talkData[1];
        }
        else
        {
            talkText.text = talkData[2];
        }
        yield return new WaitForSeconds(2.0f);
        talkText.text = talkData[0];
    }
}
