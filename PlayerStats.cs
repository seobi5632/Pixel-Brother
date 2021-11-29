using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    public override void Die()  //플레이어가 죽었을때 판정
    {
        base.Die();
        GameManager.instance.play.anim.SetTrigger("Die");
        GameManager.instance.isPlay = false;
        GameManager.instance.isBattle = false;
        GameManager.instance.isPlayerDie = true;
        GameManager.instance.GameOver();
    }
}
