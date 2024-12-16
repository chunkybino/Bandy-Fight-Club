using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHandler : MonoBehaviour
{
    public static BulletHandler singleton;

    public BulletConfig defaultConfig;
 
    public List<BulletFunc> currentBullets = new List<BulletFunc>();

    public Transform bulletParent; //spawn bullets unde rthis object to keep things orgainized

    void Awake()
    {
        if (!singleton)
        {
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(this);
        }
    }
    void Start()
    {
        GameManager.singleton.onRespawn.AddListener(OnRespawn);
    }
    void Update()
    {
        if (!singleton) singleton = this;
    }

    /*public BulletFunc ShootBulletWallCheck(BulletConfig config, Vector2 pos, Vector2 direction, Vector2 firerPos) //raycasts between the firepoint annd firerpos and fire a shot from there, for making sure we dont shoot through walls
    {
        RaycastHit2D hit = HitscanHandler.RaycastBetween2Points(pos, firerPos, GameManager.singleton.groundLayermask);
        Vector2 newFirePos = pos; 
        if (hit.collider != null) newFirePos = hit.point;
        return ShootBullet(config, newFirePos, direction);
    }*/
    public static BulletFunc ShootBulletStatic(BulletConfig config, Vector2 pos, Vector2 direction, int team = 0)
    {
        return singleton.ShootBullet(config, pos, direction, team);
    }
    public BulletFunc ShootBullet(BulletConfig config, Vector2 pos, Vector2 direction, int team = 0)
    {
        BulletFunc thisBullet = Instantiate(config.bulletObject, pos, Quaternion.identity, bulletParent);
        thisBullet.config = config;
        thisBullet.direction = direction;
        thisBullet.attackInfo = config.enemyAttackInfo.dataCopy;
        thisBullet.SetTeam(team);

        currentBullets.Add(thisBullet);

        return thisBullet;
    }

    public void DestroyAllBullets()
    {
        foreach (BulletFunc bullet in currentBullets)
        {
            Destroy(bullet.gameObject);
        }
        currentBullets.Clear();
    }

    public void RemoveBulletFromList(BulletFunc bullet)
    {
        currentBullets.Remove(bullet);
    }

    void OnRespawn()
    {
        DestroyAllBullets();
    }
}

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