using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BeanBody : MonoBehaviour
{
    public bool disableAll;
    public bool enableAll;

    [SerializeField] bool doDisguise;
    [SerializeField] float disguiseEnableLevel;

    public GameObject[] objects;

    public bool shouldFlip = true;

    void Update()
    {
        if (disableAll)
        {
            disableAll = false;
            DoAll(false);
        }
        if (enableAll)
        {
            enableAll = false;
            DoAll(true);
        }

        if (doDisguise)
        {
            bool activate = DisguiserParent.disguiseLevelStatic <= disguiseEnableLevel;
            DoAll(!activate);
            shouldFlip = activate;
        }

        void DoAll(bool yeah)
        {
            foreach (GameObject g in objects)
            {
                g.SetActive(yeah);
            }
        }
    }
}
