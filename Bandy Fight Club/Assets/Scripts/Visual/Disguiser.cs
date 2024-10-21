using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disguiser : MonoBehaviour
{
    public int enableLevel; //enable if disguise level is less or equal this
    public bool inverse;

    public void OnEnable()
    {
        CheckDisguise();
    }

    public void CheckDisguise()
    {
        bool activate = DisguiserParent.disguiseLevelStatic <= enableLevel;
        if (inverse) activate = !activate;
        gameObject.SetActive(activate);
    }
}
