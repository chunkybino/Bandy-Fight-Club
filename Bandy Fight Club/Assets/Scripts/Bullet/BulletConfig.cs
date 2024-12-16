using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "BulletSO", menuName = "ScriptableObjects/Bullet")]
public class BulletConfig : ScriptableObject
{
    public BulletFunc bulletObject;
    public AttackInfo enemyAttackInfo;
    public float speed = 10;
    [Tooltip("negative = infitinete peirces")]
    public int pierce;
    public float size = 1;
    public float lifetime = 4;
}
