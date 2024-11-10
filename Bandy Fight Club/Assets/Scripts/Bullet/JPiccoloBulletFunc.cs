using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JPiccoloBulletFunc : BulletFunc
{
    [SerializeField] float rotationSpeed;
    [SerializeField] Transform rotationTranform; //the target of rotationSpeed

    public float timeActive;
    public float activateTime = 0.75f;
    public bool isActive {get{return timeActive >= activateTime;}}

    void Update()
    {
        base.Update();

        float rotSpeed = rotationSpeed;
        rotSpeed *= UFunc.Sign(direction.x);
        rotationTranform.localEulerAngles = rotationTranform.localEulerAngles + new Vector3(0,0,rotSpeed*Time.deltaTime);

        timeActive += Time.deltaTime;
    }
}
