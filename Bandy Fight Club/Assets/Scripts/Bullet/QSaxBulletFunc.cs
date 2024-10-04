using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QSaxBulletFunc : BulletFunc
{
    public int bounceCount = 0;

    public Vector2 bounceVel = new Vector2(10, 25);

    public override void HitDude(IDamageable damageable)
    {
        base.HitDude(damageable);

        doGrav = true;
        direction = new Vector2(-direction.x,direction.y);
        rb.velocity = new Vector2(UFunc.Sign(direction.x),1) * bounceVel;
    }
}
