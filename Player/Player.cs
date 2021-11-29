using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Camera cam;
   
    //플레이어의 소지품
    public int carryAmmo;
    public int coin;
    public int hasGrenades;
    public int maxhasGrenades;
    
    //버튼관련
    float hAxis;
    float vAxis;
    bool wDown;
    bool iDown;
    bool fDown;
    bool rDown;
    bool sDown;
    bool eDown;
    bool sDown1;
    bool sDown2;
    bool sDown3;


    //행동판정
    bool isDodge;
    bool isSwap;
    bool isAttackReady = true;
    bool isReload;
    bool isDamage;
    
    
    bool isPause = false;
    bool isBorder;

    public float speed;
    public GameObject[] weapons;
    public GameObject grenadeObj;
    public bool[] hasWeapons;

    float fireDelay;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    public Animator anim;

    //피격시 색변화
    MeshRenderer[] meshs;

    //충돌 오브젝트 저장
    GameObject nearObj;

    //일시정지 UI
    public GameObject uiGroup;

    public Weapon equipWeapon;
    CharacterStats myStats;
    int equipWeaponIndex = -1;

    Shop curShop;
    StageController curstageController;
    public StageManager stageManager;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        GameManager.player = this.gameObject;
        meshs = GetComponentsInChildren<MeshRenderer>();
        myStats = GetComponent<CharacterStats>();
    }

    void Update()
    {
        if (GameManager.instance.isDialogue) return;
        GetInput();
        Move();
        Turn();
        Attack();
        Grenade();
        Reload();
        Dodge();
        Interation();
        Swap();
        EscapekeyDown();
    }
    void GetInput()
    {
        if (GameManager.instance.isPlay)
        {
            hAxis = Input.GetAxisRaw("Horizontal");
            vAxis = Input.GetAxisRaw("Vertical");
            wDown = Input.GetButton("Shift");
            fDown = Input.GetButton("Fire1");
            iDown = Input.GetButtonDown("Interation");
            sDown = Input.GetKeyDown(KeyCode.Space);
            rDown = Input.GetKeyDown(KeyCode.R);
            eDown = Input.GetKeyDown(KeyCode.E);
            sDown1 = Input.GetKeyDown(KeyCode.Alpha1);
            sDown2 = Input.GetKeyDown(KeyCode.Alpha2);
            sDown3 = Input.GetKeyDown(KeyCode.Alpha3);
        }
    }
    void EscapekeyDown()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (curShop != null)
            {
                curShop.Exit();
                GameManager.instance.isShop = false;
            }
            if (curstageController != null)
            {
                curstageController.Exit();
                GameManager.instance.isShop = false;
            }
            if (!GameManager.instance.isShop)
            {
                if (!isPause) CallMenu();
                else CloseMenu();
            }
        }
    }

    void CallMenu()
    {
        isPause = true;
        uiGroup.SetActive(true);
        Time.timeScale = 0.0f;
    }

    void CloseMenu()
    {
        isPause = false;
        uiGroup.SetActive(false);
        Time.timeScale = 1.0f;
    }

    void Move()
    { 
        if (isDodge) moveVec = dodgeVec;

        if (isSwap || !isAttackReady || isReload || !GameManager.instance.isPlay) moveVec = Vector3.zero;
        
        if (!isBorder) transform.position += moveVec * (wDown ? 0.3f : 1f) * Time.deltaTime * speed;

        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        anim.SetBool("isRun", moveVec != Vector3.zero);
        anim.SetBool("isWalk", wDown);
    }
    void Turn()
    {
        transform.LookAt(transform.position + moveVec);

        if (fDown && equipWeapon != null && GameManager.instance.isPlay)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                Vector3 vec = hit.point - transform.position;
                vec.y = 0;
                transform.LookAt(transform.position + vec);
            }
        }
    }

    void Grenade()
    {
        if (hasGrenades == 0) return;

        if(eDown && !isReload && !isSwap && !GameManager.instance.isShop && GameManager.instance.isPlay)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                Vector3 vec = hit.point - transform.position;
                vec.y = 3;

                GameObject instantGrenade = Instantiate(grenadeObj, transform.position, transform.rotation);
                Rigidbody grenadeRigid = instantGrenade.GetComponent<Rigidbody>();
                grenadeRigid.AddForce(vec, ForceMode.Impulse);
                grenadeRigid.AddTorque(Vector3.back * 10.0f, ForceMode.Impulse);

                hasGrenades--;
            }
        }
    }

    void Attack()
    {
        if (equipWeapon == null) return;

        fireDelay += Time.deltaTime;
        isAttackReady = equipWeapon.rate < fireDelay;

        if(fDown && isAttackReady && !isDodge && !isSwap && !GameManager.instance.isShop && GameManager.instance.isPlay)
        {
            equipWeapon.Use();
            Invoke("PlaySwingSound", 0.3f);
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;
        }
    }
    void PlaySwingSound()
    {
        SoundManager.instance.PlaySE(equipWeapon.type == Weapon.Type.Melee ? "Swing" : "Shot");
    }   //사운드 재생위치를 맞추기 위한 함수
    void Reload()
    {
        if (equipWeapon == null) return;
        if (equipWeapon.type == Weapon.Type.Melee) return;
        if (carryAmmo == 0) return;
        if (equipWeapon.curAmmo == equipWeapon.maxAmmo) return;

        if(rDown && !isDodge && !isSwap && isAttackReady && !GameManager.instance.isShop && GameManager.instance.isPlay)
        {
            anim.SetTrigger("Reload");
            isReload = true;

            Invoke("ReloadOut", 0.5f);
        }
    }

    void ReloadOut()
    {
        int curAmmo = equipWeapon.curAmmo;
        int reAmmo = equipWeapon.maxAmmo < carryAmmo ? equipWeapon.maxAmmo : carryAmmo;
        equipWeapon.curAmmo = reAmmo;
        carryAmmo -= (reAmmo-curAmmo);
        isReload = false;
    }

    void Dodge()
    {
        if (sDown && moveVec != Vector3.zero  && !isDodge && !isSwap && !GameManager.instance.isShop && GameManager.instance.isPlay)
        {
            dodgeVec = moveVec;
            speed *= 3.0f;
            anim.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.4f);
        }
    }

    void DodgeOut()
    {
        speed /= 3.0f;
        isDodge = false;
    }

    void Swap()
    {
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0)) return;
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1)) return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2)) return;

        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;

        if ((sDown1 || sDown2 || sDown3) && !isDodge && !GameManager.instance.isShop && GameManager.instance.isPlay)
        { 
            if (equipWeapon != null) equipWeapon.gameObject.SetActive(false);
            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);
            

            anim.SetTrigger("doSwap");

            isSwap = true;
            Invoke("SwapOut", 0.4f);

        }
    }
    void SwapOut()
    {
        isSwap = false;
    }

    void Interation()
    {
        if (iDown && nearObj != null && !isDodge)
        {
            if(nearObj.tag == "Weapon")
            {
                Item item = nearObj.GetComponent<Item>();

                int weponIndex = item.value;
                hasWeapons[weponIndex] = true;

                Destroy(nearObj);
            }
            else if(nearObj.tag == "Shop")
            {
                GameManager.instance.isShop = true;
                Shop shop = nearObj.GetComponent<Shop>();
                curShop = shop;
                shop.Enter(this);
            }
            else if (nearObj.tag == "Stage")
            {
                GameManager.instance.isShop = true;
                StageController stageController = nearObj.GetComponent<StageController>();
                curstageController = stageController;
                stageController.Enter();
            }
            else if (nearObj.tag == "Luna")
            { 
                Clear clear = nearObj.GetComponent<Clear>();
                clear.Enter();
            }
        }
    } // 아이템 습득판정
    void StopToWall()
    {
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
    } // 벽으로 들어가지 않도록 레이를 쏴 더이상 이동하지 못하게 함.

    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;
    }

    void FixedUpdate()
    {
        StopToWall();
        FreezeRotation();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            SoundManager.instance.PlaySE("Item");
            switch(item.type)
            {
                case Item.Type.Ammo:
                    carryAmmo += item.value;
                    break;
                case Item.Type.Heart:
                    myStats.RecoveryHP(item.value);
                    break;
                case Item.Type.Grenade:
                    hasGrenades += item.value;
                    if (hasGrenades > maxhasGrenades) hasGrenades = maxhasGrenades;
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    break;
            }
            Destroy(other.gameObject);
        }
        else if (other.tag == "Cheat")
        {
            Cheat cheat = other.GetComponent<Cheat>();
            cheat.cheat();
            print("cheat on");
            Destroy(other.gameObject);

        }
        else if(other.tag == "EnemyBullet")
        {
            if (!isDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                myStats.TakeDamage(enemyBullet.damage);
                bool isBossAtk = other.name == "Boss Melee Area";
                StartCoroutine(OnDamage(isBossAtk));
            }
            if (other.GetComponent<Rigidbody>() != null) Destroy(other.gameObject);
        }
    }

    IEnumerator OnDamage(bool isBossAtk)
    {
        SoundManager.instance.PlaySE("PlayerHurt");
        isDamage = true;
        foreach(var mesh in meshs)
        {
            mesh.material.color = Color.yellow;
        }
        if (isBossAtk) rigid.AddForce(transform.forward * -40.0f, ForceMode.Impulse);

        yield return new WaitForSeconds(0.5f);

        isDamage = false;

        foreach (var mesh in meshs)
        {
            mesh.material.color = Color.white;    
        }
        rigid.velocity = Vector3.zero;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon" || other.tag == "Shop" || other.tag == "Stage" || other.tag == "Luna") nearObj = other.gameObject;

    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon") nearObj = null;
        else if(other.tag == "Shop")
        {
            Shop shop = nearObj.GetComponent<Shop>();
            shop.Exit();
            GameManager.instance.isShop = false;
            nearObj = null;
        }
        else if (other.tag == "Stage")
        {
            StageController stageController = nearObj.GetComponent<StageController>();
            stageController.Exit();
            GameManager.instance.isShop = false;
            nearObj = null;
        }
    }
}
