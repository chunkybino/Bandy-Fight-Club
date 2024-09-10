using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpriteAngle : MonoBehaviour
{
    public GameObject[] objects;

    public float showAngle = 30;
    public bool activate = true;

    void Update()
    {
        if (activate)
        {
            float closestAngleIndex = 0;
            float closestAngle = 1000;

            for (int i = 0; i < objects.Length; i++)
            {
                GameObject obj = objects[i];

                float xangle = obj.transform.eulerAngles.x;
                if (xangle > 180) xangle -= 360;
                float yangle = obj.transform.eulerAngles.y;
                if (yangle > 180) yangle -= 360;

                float angleAdd = Mathf.Abs(xangle) + Mathf.Abs(yangle);print(angleAdd.ToString() + "   " + closestAngleIndex.ToString());

                if (angleAdd < closestAngle)
                {
                    closestAngleIndex = i;
                    closestAngle = angleAdd;
                }

                /*if (xangle>-showAngle && xangle<showAngle && yangle>-showAngle && yangle<showAngle)
                {
                    obj.SetActive(true);
                }
                else
                {
                    obj.SetActive(false);
                }*/
            }

            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].SetActive(closestAngleIndex == i);
            }
        }
        else
        {
            foreach (GameObject obj in objects)
            {
                obj.SetActive(true);
            }
        }
    }
}
