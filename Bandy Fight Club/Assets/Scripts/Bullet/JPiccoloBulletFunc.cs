using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JPiccoloBulletFunc : BulletFunc
{
    [SerializeField] float rotationSpeed;
    [SerializeField] Transform rotationTranform; //the target of rotationSpeed

    void Update()
    {
        base.Update();

        float rotSpeed = rotationSpeed;
        rotSpeed *= UFunc.Sign(direction.x);
        rotationTranform.localEulerAngles = rotationTranform.localEulerAngles + new Vector3(0,0,rotSpeed*Time.deltaTime);
    }
}
