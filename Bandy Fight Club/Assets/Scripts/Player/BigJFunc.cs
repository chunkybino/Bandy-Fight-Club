using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigJFunc : PlayerFunc, ICharacterFunc
{
    [SerializeField] float timeSinceSlash1;
    [SerializeField] float slash2ComboTime = 1.2f;

    public BulletConfig piccoloBulletConfig;

    void Awake()
    {
        if (characterFunc == null) characterFunc = this;
    }
    void Update()
    {
        base.Update();

        if (characterFunc == null) characterFunc = this;

        timeSinceSlash1 += Time.deltaTime;
    }

    public string FindAttackName()
    {
        string attackName = "nope";

        if (timeSinceSlash1 <= slash2ComboTime)
        {
            attackName = "slash2";
            timeSinceSlash1 = 1;
        }
        else
        {
            attackName = "slash1";
            timeSinceSlash1 = 0;
        }

        return attackName;
    }
    public string FindAttackHeavyName()
    {
        string attackName = "PiccoloThrow";

        return attackName;
    }

    public void PiccoloThrowAttack()
    {
        ShootPiccolo();
    }
    public void ShootPiccolo()
    {
        BulletFunc bullet = BulletHandler.ShootBulletStatic(piccoloBulletConfig, transform.position, new Vector2(facing, 1), team);
    }
}

