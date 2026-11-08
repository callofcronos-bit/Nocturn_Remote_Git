using UnityEngine;

// ค่าแนะนำ: HP 50, Atk 10, Speed 3.5, Range 2, Coin 1-5
public class NoxMelee : EnemyBase
{
    public override void Attack()
    {
        if (target != null && target.TryGetComponent<IDamageable>(out var d))
            d.TakeDamage(atk);
    }
}
