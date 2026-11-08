using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    // static: เก็บกระสุนศัตรู/บอสทั้งหมดที่ยังอยู่ในฉาก
    static readonly List<EnemyProjectile> active = new List<EnemyProjectile>();

    public float damage = 8;
    public float speed = 8;
    [SerializeField] float lifetime = 6;

    void OnEnable() => active.Add(this);
    void OnDisable() => active.Remove(this);

    void Start() => Destroy(gameObject, lifetime);
    void Update() => transform.position += transform.forward * speed * Time.deltaTime;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent<IDamageable>(out var d))
        {
            d.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    // ลบกระสุนทุกนัดในรัศมีรอบจุดที่กำหนด (ระนาบ XZ) คืนค่าจำนวนที่ลบ
    public static int ClearInRadius(Vector3 center, float radius)
    {
        int count = 0;
        float sqr = radius * radius;
        for (int i = active.Count - 1; i >= 0; i--)
        {
            var p = active[i];
            if (p == null) { active.RemoveAt(i); continue; }
            Vector3 d = p.transform.position - center; d.y = 0;
            if (d.sqrMagnitude <= sqr)
            {
                Destroy(p.gameObject);
                count++;
            }
        }
        return count;
    }
}
