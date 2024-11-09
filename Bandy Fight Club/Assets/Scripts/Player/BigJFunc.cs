using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigJFunc : PlayerFunc, ICharacterFunc
{
    [SerializeField] float timeSinceSlash1;
    [SerializeField] float slash2ComboTime = 0.5f;

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
}

