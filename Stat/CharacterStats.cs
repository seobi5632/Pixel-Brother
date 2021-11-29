using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    #region Singleton
    private static CharacterStats characterStats;
    public static CharacterStats instance
    {
        get
        {
            if (characterStats == null)
                characterStats = FindObjectOfType<CharacterStats>();
            return characterStats;
        }
    }
    #endregion


    //플레이어, 적, 보스 의 체력
    public int maxHealth = 100;
    public int currentHealth { get; private set; }


    //스텟
    public Stats damage;
    public Stats critical_chance;
    public Stats critical_per;

    //HP변화를 판정
    public event System.Action<int, int> OnHealthChanged;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        //Transform cam = Camera.main.transform; ;
        //Quaternion quaternion = cam.rotation;
        //GameObject DmgTextClone = Instantiate(GameManager.instance.dmgText, transform.position, quaternion);
        //GameObject EffectClone = Instantiate(GameManager.instance.effect, transform.position, Quaternion.identity);

        //Destroy(EffectClone, 1.0f);
        //DmgTextClone.GetComponent<DmgText>().DisplayDamage(damage, isCritical);
        

        damage = Mathf.Clamp(damage, 0, int.MaxValue); //데미지값 범위 지정

        currentHealth -= damage;

        if (OnHealthChanged != null)
        {
            OnHealthChanged(maxHealth, currentHealth);
        }

        if (currentHealth <= 0 && GameManager.instance.isPlay)
        {
            Die();
        }
    }

    public void RecoveryHP(int value)   //hp회복
    {
        if (currentHealth + value > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += value;
        }

        if (OnHealthChanged != null)
        {
            OnHealthChanged(maxHealth, currentHealth);
        }
    }
    public virtual void Die()   //자식에서 죽은뒤 효과 설정
    {
        Debug.Log(transform.name + " died. ");
    }
}
