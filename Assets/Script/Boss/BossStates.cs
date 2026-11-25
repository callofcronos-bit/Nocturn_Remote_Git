using System.Collections;
using UnityEngine;

// ท่า 1: Bullet Cone 6 นัด x3
public class ConeState : BossState
{
    public ConeState(BossNocturn b) : base(b) { }
    public override IEnumerator Execute()
    {
        for (int wave = 0; wave < 3; wave++)
        {
            boss.FacePlayer();
            const float spread = 60f;
            for (int i = 0; i < 6; i++)
                boss.Shoot(-spread / 2 + spread / 5 * i);
            yield return new WaitForSeconds(0.7f);
        }
    }
}

// ท่า 2: ปล่อยรอบตัว 12 นัด x2
public class RingState : BossState
{
    public RingState(BossNocturn b) : base(b) { }
    public override IEnumerator Execute()
    {
        for (int wave = 0; wave < 2; wave++)
        {
            float offset = wave * 15f;
            for (int i = 0; i < 12; i++)
                boss.Shoot(offset + i * 30f);
            yield return new WaitForSeconds(1f);
        }
    }
}

// ท่า 3: ปล่อยวงกระสุนรอบตัว ค้างไว้ครู่หนึ่ง แล้วพุ่งเข้าหากลางบอสพร้อมกัน (บินทะลุต่อ)
public class ImplodeState : BossState
{
    public ImplodeState(BossNocturn b) : base(b) { }
    public override IEnumerator Execute()
    {
        const int count = 16;
        Vector3 center = boss.CenterPoint;
        float radius = boss.ImplodeRadius;
        float hold = boss.ImplodeHold;

        for (int i = 0; i < count; i++)
        {
            Vector3 offset = Quaternion.Euler(0, i * (360f / count), 0) * Vector3.forward * radius;
            boss.ShootConverging(center + offset, center, hold);
        }

        // รอจนกระสุนค้างครบเวลา + พุ่งถึงกลาง
        yield return new WaitForSeconds(hold + radius / boss.BulletSpeed + 0.5f);
    }
}
