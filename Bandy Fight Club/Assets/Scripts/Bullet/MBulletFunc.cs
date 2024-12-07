using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MBulletFunc : BulletFunc
{
    [SerializeField] bool hasBounced = false;

    [SerializeField] float bounceDamageMult = 1.5f;

    [SerializeField] string bounceSound;
    [SerializeField] float bounceSoundVol = 1;
    [SerializeField] float bounceSoundPitch = 1;

    public override void HitWall(Collider2D wall)
    {
        hasBounced = true;

        attackInfo.damage = Mathf.FloorToInt(attackInfo.damage * bounceDamageMult);

        Vector2 wallPoint = wall.ClosestPoint(transform.position);
        Vector2 wallDistance = wallPoint - new Vector2(transform.position.x,transform.position.y);

        Vector2 bounceVec = UFunc.BounceVector(rb.velocity.normalized, wallDistance);

        rb.velocity = bounceVec.normalized*rb.velocity.magnitude;

        Vector2 translationVec = wallDistance.normalized*1.1f*(wallDistance.magnitude-(circleCollider.radius*transform.localScale.x));//print(wallPoint.x);print(transform.position.x);print(wallPoint.x-transform.position.x);print(wallDistance);print(translationVec);
        transform.position += new Vector3(translationVec.x,translationVec.y,0);

        direction = new Vector2(UFunc.Sign(rb.velocity.x),direction.y);

        GameManager.singleton.audio.Play(bounceSound, bounceSoundVol, bounceSoundPitch);
    }
}
