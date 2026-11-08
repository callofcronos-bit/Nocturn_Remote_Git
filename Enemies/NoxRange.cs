using UnityEngine;

// ค่าแนะนำ: HP 35, Atk 8, Speed 2.5, Range 10, Coin 5-10
public class NoxRange : EnemyBase
{
    [SerializeField] EnemyProjectile projectilePrefab;
    [SerializeField] Transform firePoint;

    public override void Attack()
    {
        if (projectilePrefab == null) return;
        Vector3 pos = firePoint != null ? firePoint.position : transform.position + Vector3.up;
        var p = Instantiate(projectilePrefab, pos, transform.rotation);
        p.damage = atk;
    }
}
