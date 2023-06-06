using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GunControler : MonoBehaviour
{
    // Start is called before the first frame update

    [Header("References")]
    public Transform cam;


    [Header("Gun Attribute")]
    public float hipShootCooldown;
    public float aimShootCooldown;
    public float continueShootCooldown;
    public float shootDamageValue;
    public int bulletInMagazine;
    public int bulletInBag;
    public float critDamageMagnification;

    [Header("Gun Limit Times")]
    public float reloadTime;
    public float aimStartTime;
    public float aimEndTime;

    [Header("Gun Limit Ranges")]
    public float validShootRange;
    public float maxShootRange;

    [Header("Input")]
    public KeyCode ShootKey = KeyCode.Mouse0;
    public KeyCode AimKey = KeyCode.Mouse1;
    public KeyCode ReloadKey = KeyCode.R;

    public bool noneWeapon;
    public bool normalWeapon;
    public bool shooting;
    public bool aimming;
    public bool reloading;

    private WeaponState state;
    private float shootCdTimer;
    private float continueShootTimer;
    private int currentBulletInMagazine;
    private int currentBulletInBag;

    private Vector3 originalGunPosition;
    private Vector3 originalCamPosition;


    public TextMeshProUGUI text_bullet;
    private enum WeaponState
    {
        noneWeapon,
        normalWeapon,
        shooting,
        reloading
    }
    
    void Start()
    {
        shootCdTimer = hipShootCooldown;
        continueShootTimer = continueShootCooldown;
        originalGunPosition = this.transform.localPosition;
        originalCamPosition = cam.transform.localPosition;

        currentBulletInMagazine = bulletInMagazine;
        currentBulletInBag = bulletInBag - bulletInMagazine;
    }

    // Update is called once per frame
    void Update()
    {
        CdTimer();
        GetMyInput();
        MyInput();

        TextStuff();
    }

    private void GetMyInput()
    {
        if (!noneWeapon && !reloading)
        {
            if (Input.GetKey(ShootKey))
                shooting = true;
            else if (shooting)
            {
                shooting = false;
                continueShootTimer = continueShootCooldown;
            }

            if (Input.GetKeyDown(AimKey))
                StartAimming();
            else if (Input.GetKeyUp(AimKey))
                StopAimming();

            if (Input.GetKeyDown(ReloadKey))
            {
                if (aimming)
                {
                    StopAimming();

                }
                StartReloading();
            }
            
        }
        
    }

    private void MyInput()
    {

        if (noneWeapon)
        {
            state = WeaponState.noneWeapon;
        }else if (shooting)
        {
            state = WeaponState.shooting;
            OnShooting();
        }else
        {
            state = WeaponState.normalWeapon;
        }

    }

    private void OnShooting()
    {
        if (shootCdTimer > 0)
            return; 
        

        if (currentBulletInMagazine == 0)
            return;

        if (currentBulletInMagazine > 0)
        {
            currentBulletInMagazine--;
            //播放射击动画


            ShootRayCast();
            if (aimming)
                shootCdTimer = aimShootCooldown;
            else
                shootCdTimer = hipShootCooldown;
            if (continueShootTimer > 0)
            {
                continueShootTimer = continueShootCooldown;
                shooting = false;
            }
        }
        else
        {
            //播放子弹射不出去动画

            continueShootTimer = continueShootCooldown;
            shooting = false;
        }


    }

    private void ShootRayCast()
    {
        Ray shootRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 测试弹道
        Debug.DrawRay(cam.position, cam.transform.forward, Color.green, 2f, false);

        if (Physics.Raycast(shootRay, out hit, maxShootRange))
        {
            if (hit.transform.tag == "PlayerObject")
            {
                PlayerAttribute pa = hit.transform.GetComponent<PlayerAttribute>();
                float value = shootDamageValue;
                if (hit.distance < validShootRange)
                {
                    int critRate = (int)(100 * hit.distance / validShootRange);
                    int judgeIfCrit = Random.Range(0, 100);
                    if (judgeIfCrit > critRate)
                    {
                        value *= critDamageMagnification;
                    }
                }
                else
                {
                    float x = Mathf.Round(hit.distance - validShootRange) / (maxShootRange - validShootRange);
                    value *= (1 - (Mathf.Pow(x,3) * 0.7f));
                }
                pa.OnShout(value);
            }
        }
    }

    private void StartReloading()
    {
        if (currentBulletInMagazine > bulletInMagazine) return;
        if (currentBulletInBag == 0) return;

        reloading = true;

        bool ifSaveBullet = false;
        int reloadBulletNum = bulletInMagazine - currentBulletInMagazine;
        if (currentBulletInMagazine > 0) ifSaveBullet = true;
        
        if (currentBulletInBag >= reloadBulletNum)
        {
            currentBulletInBag -= reloadBulletNum;
            currentBulletInMagazine = bulletInMagazine;
            if (ifSaveBullet && currentBulletInBag > 0)
            {
                currentBulletInBag--;
                currentBulletInMagazine++;
            }
        }
        else
        {
            currentBulletInMagazine += currentBulletInBag;
            currentBulletInBag = 0;
        }
        // 播放时长为reloadTime的换弹动画
        Invoke(nameof(StopReloading), reloadTime);
    }

    private void StopReloading()
    {
        reloading = false;
    }

    private void StartAimming()
    {
        aimming = true;
        shooting = false;
        this.transform.localPosition = new Vector3(0, originalGunPosition.y + 0.01f, 0f);
    }

    private void StopAimming()
    {
        aimming = false;
        this.transform.localPosition = originalGunPosition;
        cam.transform.localPosition = originalCamPosition;
    }

    private void CdTimer()
    {
        if (shootCdTimer > 0)
        {
            shootCdTimer -= Time.deltaTime;
        }

        if (shooting && continueShootTimer > 0)
        {
            continueShootTimer -= Time.deltaTime;
        }
    }

    private void TextStuff()
    {

        text_bullet.SetText(currentBulletInMagazine + "/" + bulletInMagazine + "    " + currentBulletInBag);
    }

}
