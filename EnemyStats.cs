using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : CharacterStats
{
    public GameObject[] go_item;    //드랍 아이템
    public Transform dropTransform; //드랍 위치
    public override void Die()
    {
        base.Die();

        //확률 드랍
        int i = Random.Range(0, 10);
        int j = Random.Range(0, 10);

        ItemDrop(i, j);

    }

    void ItemDrop(int i, int j) //드랍 확률과 드랍
    {
        switch (i)
        {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
                Instantiate(go_item[0], dropTransform.position, Quaternion.identity);
                break;
            case 7:
            case 8:
                Instantiate(go_item[1], dropTransform.position, Quaternion.identity);
                break;
            case 9:
                Instantiate(go_item[2], dropTransform.position, Quaternion.identity);
                break;
        }

        if (go_item.Length > 3)
        {
            switch (j)
            {
                case 0:
                case 1:
                case 2:
                    Instantiate(go_item[3], dropTransform.position, Quaternion.identity);
                    break;
                case 3:
                case 4:
                case 5:
                    Instantiate(go_item[4], dropTransform.position, Quaternion.identity);
                    break;
                case 6:
                case 7:
                case 8:
                case 9:
                    break;
            }
        }
    }
}
