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

    /*void Awake()
    {
        if (DisguiserParent.disguiserList.Contains(this)) DisguiserParent.disguiserList.Add(this);
    }
    void OnDestroy()
    {
        DisguiserParent.disguiserList.Remove(this);
    }*/

    public void CheckDisguise()
    {
        bool activate = DisguiserParent.disguiseLevelStatic <= enableLevel;
        if (inverse) activate = !activate;
        gameObject.SetActive(activate);
    }
}
