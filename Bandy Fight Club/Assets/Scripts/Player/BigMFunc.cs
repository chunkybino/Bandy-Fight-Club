using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigMFunc : PlayerFunc, ICharacterFunc
{
    void Awake()
    {
        if (characterFunc == null) characterFunc = this;
    }

    public string FindAttackName()
    {
        string attackName = "nope";

        attackName = "slash1";

        return attackName;
    }
}
