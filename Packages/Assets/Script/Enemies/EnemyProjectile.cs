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
    bool descending;
    float targetY, descendSpeed;

    float holdTimer;
    bool hasConvergePoint;
    Vector3 convergePoint;

    void Update()
    {
        // ค้างอยู่กับที่ก่อน (ใช้กับท่า Implode เพื่อให้เห็นวงกระสุนก่อนรวบเข้ากลาง)
        if (holdTimer > 0f) { holdTimer -= Time.deltaTime; return; }

        transform.position += transform.forward * speed * Time.deltaTime;

        // ถึงจุดศูนย์กลางแล้วให้หายไป
        if (hasConvergePoint)
        {
            Vector3 to = convergePoint - transform.position; to.y = 0;
            if (Vector3.Dot(to, transform.forward) <= 0f) { Destroy(gameObject); return; }
        }

        if (descending)
        {
            Vector3 p = transform.position;
            p.y = Mathf.MoveTowards(p.y, targetY, descendSpeed * Time.deltaTime);
            transform.position = p;
            if (Mathf.Approximately(p.y, targetY)) descending = false;
        }
    }

    public void Hold(float seconds) => holdTimer = seconds;

    public void DestroyAtPoint(Vector3 point)
    {
        convergePoint = point;
        hasConvergePoint = true;
    }

    // ให้กระสุนค่อยๆ ลดระดับ/ยกระดับไปที่ความสูง y ภายในเวลา time วินาที
    public void DescendTo(float y, float time)
    {
        float dy = Mathf.Abs(transform.position.y - y);
        if (dy < 0.01f) return;
        if (time <= 0.01f)
        {
            var p = transform.position; p.y = y; transform.position = p;
            return;
        }
        targetY = y;
        descendSpeed = dy / time;
        descending = true;
    }

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
