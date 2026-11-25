using UnityEngine;

// ค่าแนะนำ: HP 35, Atk 8, Speed 2.5, Range 10, Coin 5-10
public class NoxRange : EnemyBase
{
    [Header("Range")]
    [SerializeField] EnemyProjectile projectilePrefab;
    [SerializeField] Transform firePoint;

    public override void Attack()
    {
        if (projectilePrefab == null || target == null) return;

        FaceTarget();
        Vector3 pos = firePoint != null ? firePoint.position : transform.position + Vector3.up;
        Vector3 aim = target.position - transform.position; aim.y = 0;
        Quaternion rot = aim != Vector3.zero ? Quaternion.LookRotation(aim) : transform.rotation;

        var p = Instantiate(projectilePrefab, pos, rot);
        p.damage = atk;
    }
}
