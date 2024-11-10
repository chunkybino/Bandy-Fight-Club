using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JCloneFunc : MonoBehaviour
{
    public Vector2 direction;

    [SerializeField] Rigidbody2D rb;
    [SerializeField] PlayerAnimation playerAnimation;
    [SerializeField] PlayerMeshParent meshParent;

    public void Spawn(int renderNum)
    {
        meshParent.num = renderNum;
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void SetTeam(int team)
    {
        meshParent.team = team;
    }
}
