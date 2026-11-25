using UnityEngine;

// ค่าแนะนำ: HP 50, Atk 10, Speed 3.5, Range 2, Coin 1-5
public class NoxMelee : EnemyBase
{
    public override void Attack()
    {
        if (target == null) return;
        // ถ้าผู้เล่นหนี (เช่น Dash) ออกนอกระยะระหว่างท่าง้าง จะไม่โดน
        if (Vector3.Distance(transform.position, target.position) > attackRange + 1f) return;
        if (target.TryGetComponent<IDamageable>(out var d)) d.TakeDamage(atk);
    }
}
