using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttribute : MonoBehaviour
{
    [Header("Reference")]
    public float HP;
    public float Armor;

    // 个性化的设置，暂空

    [Header("Attributes")]
    public float armorDefenceMagnification;

    private float currentHP;
    private float currentArmor;


    public bool isAlive;
    // Start is called before the first frame update
    void Start()
    {
        currentHP = HP;
        currentArmor = Armor;
        isAlive = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnShout(float value)
    {
        if (isAlive)
        {
            if (currentHP > 0)
            {
                if (currentArmor > 0)
                {
                    currentArmor -= armorDefenceMagnification * value;
                    if (currentArmor < 0) currentArmor = 0;
                }
                else
                {
                    currentHP -= value;
                    if (currentHP < 0) currentHP = 0;
                }
                Debug.Log(this.name + " " + currentHP + " " + currentArmor);
            }
            else
            {
                isAlive = false;

                Destroy(GetComponent<BoxCollider>());
            }
        }
    }


}
