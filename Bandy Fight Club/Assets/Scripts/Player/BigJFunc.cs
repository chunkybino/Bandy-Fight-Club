using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigJFunc : PlayerFunc, ICharacterFunc
{
    void Awake()
    {
        if (characterFunc == null) characterFunc = this;
    }
    void Update()
    {
        base.Update();

        if (characterFunc == null) characterFunc = this;
    }

    public string FindAttackName()
    {
        string attackName = "nope";

        attackName = "slash1";

        return attackName;
    }
}

