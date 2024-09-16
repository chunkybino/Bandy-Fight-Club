using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BeanBody : MonoBehaviour
{
    public bool disableAll;
    public bool enableAll;

    public GameObject[] objects;

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

        void DoAll(bool yeah)
        {
            foreach (GameObject g in objects)
            {
                g.SetActive(yeah);
            }
        }
    }
}
