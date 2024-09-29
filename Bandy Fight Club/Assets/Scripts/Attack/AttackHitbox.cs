using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    [SerializeField] TeamHolder team;

    [SerializeField] float rehitTime = 0.5f;
    List<Collider2D> rehitList = new List<Collider2D>();
    IEnumerator rehitCorountine;

    [SerializeField] AttackInfo attackInfo;

    void OnTriggerStay2D(Collider2D item)
    {
        IDamageable damageable = item.GetComponent<IDamageable>();

        if (damageable == null) return; //if we find a hitbox
        if (rehitList.Contains(item)) return; //avoid rehits

        print(team.team);
        print(damageable.team.team);
        if (team) if (team.team == damageable.team.team) return; //if we on the same team, if this hitbox isnt assigned to a team, hit everyone

        bool hitTarget = damageable.GetHit(attackInfo);

        print(hitTarget);

        if (hitTarget) 
        {
            HitDude();

            rehitCorountine = removeFromHitListCorountine(item);
            StartCoroutine(rehitCorountine);
        }
    }

    void HitDude()
    {

    }

    IEnumerator removeFromHitListCorountine(Collider2D target)
    {
        rehitList.Add(target);
        yield return new WaitForSeconds(rehitTime);
        rehitList.Remove(target);
    }
}
