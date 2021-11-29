using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager _instance;
    public static GameManager instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<GameManager>();
            return _instance;
        }
    }
    #endregion

    public static GameObject player;
    public GameObject mainCam;
    public Player play;
    public Boss boss;

    //stage 잠금
    public bool stage1 = false;
    public bool stage2 = false;
    public bool stage3 = false;
    public bool bossstage = false;

    //플레이어상태 및 현스테이지, UI
    public int stage;
    public float playTime;
    public bool isBattle;
    public bool isPlay;
    public bool isPlayerDie = false;
    public bool a = true;
    public bool isSpwan;
    public bool isFinish = false;
    public bool isShop;
    public bool isDialogue = false;

    //적 카운트
    public int enmeyCntA;
    public int enmeyCntB;
    public int enmeyCntC;

    //UI
    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject overPanel;
    public GameObject tutorialPanel;
    public GameObject bossHPGroup;

    public Text stageTxt;
    public Text playtimeTxt;
    public Text besttimeTxt;

    public Text HealthTxt;
    public Text AmmoTxt;
    public Text CoinTxt;

    public Image weapon1Img;
    public Image weapon2Img;
    public Image weapon3Img;
    public Image weapon4Img;

    public Text enemyATxt;
    public Text enemyBTxt;
    public Text enemyCTxt;

    public RectTransform bossHeathGroup;
    public RectTransform bossHeathBar;

    public Transform point;
    public Text curtimeTxt;
    public Text bestTxt;
    //public GameObject dmgText;
    //public GameObject effect;
    void Awake()
    {
        //Best Time
        PlayerPrefs.SetFloat("BestTime", 784.0f);//임의설정
        int hour = (int)(PlayerPrefs.GetFloat("BestTime") / 3600);
        int min = (int)((PlayerPrefs.GetFloat("BestTime") - hour * 3600) / 60);
        int sec = (int)(PlayerPrefs.GetFloat("BestTime") % 60);
        besttimeTxt.text = string.Format("{0:00}", hour) + " : " + string.Format("{0:00}", min) + " : " + string.Format("{0:00}", sec);

        if (PlayerPrefs.HasKey("BestTime")) PlayerPrefs.SetFloat("BestTime", 0.0f);
    }

    public void GameStart()
    {
        SoundManager.instance.PlaySE("Click");
        isDialogue = true;
        menuPanel.SetActive(false);
        gamePanel.SetActive(true);
        if(!isFinish)
        {
            tutorialPanel.SetActive(true);
        }
        isPlay = true;
        play.gameObject.SetActive(true);
    }

    public void GameOver()
    {
        gamePanel.SetActive(false);
        overPanel.SetActive(true);
        curtimeTxt.text = playtimeTxt.text;
    }
    public void GameClear()
    {
        float BestTime = PlayerPrefs.GetFloat("BestTime");
        if (playTime < BestTime)
        {
            bestTxt.gameObject.SetActive(true);
            PlayerPrefs.SetFloat("BestTime", playTime);
        }
    }

    public void Restart()
    {
        SoundManager.instance.PlaySE("Click");
        StageManager stageManager = GetComponent<StageManager>();
        stageManager.MoveToStage(0);
        gamePanel.SetActive(true);
        overPanel.SetActive(false);
        a = true;
        isSpwan = false;
        isPlay = true;
        Time.timeScale = 1.0f;
        play.anim.SetTrigger("isPlay");
    }

    void Update()
    {
        if (isBattle) playTime += Time.deltaTime;
    }

    void LateUpdate()   //UI 값 변화
    {
        if (stage == 0) stageTxt.text = "  BASE";
        else if (stage == 4)
        {
            stageTxt.text = "  BOSS";
            bossHPGroup.SetActive(true);
        }
        else stageTxt.text = "STAGE " + stage;

        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int sec = (int)(playTime % 60);
        playtimeTxt.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", sec); 

        HealthTxt.text = play.GetComponent<CharacterStats>().currentHealth + " / " + play.GetComponent<CharacterStats>().maxHealth;
        CoinTxt.text = string.Format("{0:n0}", play.coin);

        if (play.equipWeapon == null) AmmoTxt.text = "- / " + play.carryAmmo;
        else if (play.equipWeapon.type == Weapon.Type.Melee) AmmoTxt.text = "- / " + play.carryAmmo;
        else AmmoTxt.text = play.equipWeapon.curAmmo + " / " + play.carryAmmo;

        weapon1Img.color = new Color(1, 1, 1, play.hasWeapons[0] ? 1 : 0);
        weapon2Img.color = new Color(1, 1, 1, play.hasWeapons[1] ? 1 : 0);
        weapon3Img.color = new Color(1, 1, 1, play.hasWeapons[2] ? 1 : 0);
        weapon4Img.color = new Color(1, 1, 1, play.hasGrenades > 0 ? 1 : 0);

        enemyATxt.text = enmeyCntA.ToString();
        enemyBTxt.text = enmeyCntB.ToString();
        enemyCTxt.text = enmeyCntC.ToString();

        if(boss != null)
        {
            bossHeathGroup.gameObject.SetActive(true);
            bossHeathBar.localScale = new Vector3((float)boss.GetComponent<CharacterStats>().currentHealth / boss.GetComponent<CharacterStats>().maxHealth, 1, 1);
        }
        if (boss == null) bossHeathGroup.gameObject.SetActive(false);

    }

}
