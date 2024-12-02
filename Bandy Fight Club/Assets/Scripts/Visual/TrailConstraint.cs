using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailConstraint : MonoBehaviour
{
    [SerializeField] Transform playerReference; //the point this in achored to
    [SerializeField] Transform renderReference; //the point the constraint is anchored to

    [SerializeField] PlayerFunc playerFunc;
    int facing;

    [SerializeField] TrailRenderer trail;

    public int constraintIndex = -1; //which transfrom to follow
    [SerializeField] List<Transform> constraintTransforms = new List<Transform>();

    float trailNormalTime;
    public float trailTimeMult = 1;

    void Start()
    {
        trailNormalTime = trail.time;
    }

    void Update()
    {
        if (playerFunc) facing = playerFunc.facing;

        if (constraintIndex < constraintTransforms.Count && constraintIndex >= 0)
        {
            if (constraintTransforms[constraintIndex] == null) return;

            Vector3 dif = constraintTransforms[constraintIndex].position - renderReference.position;
            dif.x *= facing;
            transform.position = dif + playerReference.position;
        }

        trail.time = trailNormalTime * trailTimeMult;
    }
}
