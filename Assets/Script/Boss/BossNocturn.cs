using System.Collections;
using UnityEngine;

public class BossNocturn : MonoBehaviour, IDamageable, IDroppable
{
    [SerializeField] float maxHp = 1000;
    [SerializeField] float atk = 12;
    [SerializeField] int coinDrop = 50;
    [SerializeField] EnemyProjectile bulletPrefab;
    [SerializeField] float bulletSpeed = 8;
    [SerializeField] float idleBetweenMoves = 1.5f;

    [Header("Bullet Spawn")]
    [SerializeField] Transform muzzle;            // จุดที่กระสุนออก (วางไว้ที่ตัวบอส/ปากบอส) ถ้าไม่ใส่จะใช้ Bullet Height
    [SerializeField] float bulletHeight = 1.5f;   // ความสูงจุดออกกระสุนจากตำแหน่งบอส (ใช้เมื่อไม่มี Muzzle)
    [SerializeField] float bulletPlaneHeight = 1f; // ความสูงที่กระสุนบินจริง เหนือพื้นผู้เล่น (ต้องอยู่ในช่วงตัวผู้เล่น)
    [SerializeField] float descendTime = 0.4f;     // เวลาที่กระสุนลดระดับจากตัวบอสลงมาระนาบผู้เล่น

    [Header("Implode (ท่ารวบเข้ากลาง)")]
    [SerializeField] float implodeRadius = 8f;   // รัศมีวงกระสุนก่อนรวบ
    [SerializeField] float implodeHold = 0.8f;   // เวลาที่วงกระสุนค้างให้ผู้เล่นเห็นก่อนพุ่งเข้ากลาง

    [Header("Animation")]
    [SerializeField] Animator animator;          // ไม่ใส่ = หาในลูกให้เอง
    [SerializeField] float deathDelay = 2.5f;    // รอเล่นท่าตายก่อนทำลายตัว

    float hp;
    Transform player;

    // ระดับความสูงที่กระสุนใช้บินจริง (ผูกกับความสูงพื้นของผู้เล่น)
    float PlaneY => (player != null ? player.position.y : transform.position.y) + bulletPlaneHeight;

    public float ImplodeRadius => implodeRadius;
    public float ImplodeHold => implodeHold;
    public float BulletSpeed => bulletSpeed;

    // จุดศูนย์กลางของบอส (ตำแหน่ง Muzzle ถ้ามี) ที่ระนาบกระสุน
    public Vector3 CenterPoint
    {
        get
        {
            Vector3 c = muzzle != null ? muzzle.position : transform.position;
            c.y = PlaneY;
            return c;
        }
    }

    public bool IsDead => hp <= 0;
    public float HpPercent => Mathf.Clamp01(hp / maxHp);

    void Start()
    {
        hp = maxHp;
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (animator != null) animator.applyRootMotion = false;
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        StartCoroutine(BossLoop());
    }

    IEnumerator BossLoop()
    {
        BossState[] moves = { new ConeState(this), new RingState(this), new ImplodeState(this) };
        int index = 0;
        while (!IsDead)
        {
            yield return new WaitForSeconds(idleBetweenMoves);
            PlayTrigger("Attack");
            yield return StartCoroutine(moves[index].Execute());
            index = (index + 1) % moves.Length;
        }
    }

    public void FacePlayer()
    {
        if (player == null) return;
        Vector3 d = player.position - transform.position; d.y = 0;
        if (d != Vector3.zero) transform.rotation = Quaternion.LookRotation(d);
    }

    public void Shoot(float angleOffset)
    {
        Quaternion rot = transform.rotation * Quaternion.Euler(0, angleOffset, 0);
        Vector3 origin = muzzle != null ? muzzle.position : transform.position + Vector3.up * bulletHeight;
        Spawn(origin, rot, true);
    }

    public void ShootFrom(Vector3 pos, Vector3 dir)
    {
        // วงกระสุนท่า Implode อยู่ห่างบอส จึงเกิดที่ระนาบผู้เล่นโดยตรง
        pos.y = PlaneY;
        Spawn(pos, Quaternion.LookRotation(dir), false);
    }

    // กระสุนท่า Implode: เกิดที่ pos ค้าง hold วินาที แล้วพุ่งเข้าหา center และบินทะลุต่อ
    public void ShootConverging(Vector3 pos, Vector3 center, float hold)
    {
        pos.y = PlaneY;
        Vector3 dir = center - pos; dir.y = 0;
        if (dir == Vector3.zero) return;
        var b = Instantiate(bulletPrefab, pos, Quaternion.LookRotation(dir));
        b.damage = atk;
        b.speed = bulletSpeed;
        b.Hold(hold);   // ค้างก่อนพุ่ง แล้วบินทะลุผ่านกลางไปอีกฝั่ง
    }

    void Spawn(Vector3 pos, Quaternion rot, bool descendToPlane)
    {
        var b = Instantiate(bulletPrefab, pos, rot);
        b.damage = atk;
        b.speed = bulletSpeed;
        if (descendToPlane) b.DescendTo(PlaneY, descendTime);
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;
        hp -= amount;
        if (hp <= 0) Die();
    }

    void Die()
    {
        hp = 0;
        StopAllCoroutines();
        DropLoot();
        foreach (var c in GetComponentsInChildren<Collider>()) c.enabled = false;
        PlayTrigger("Die");
        GameManager.Instance.CompleteLevel(1.5f);
        Destroy(gameObject, deathDelay);
    }

    void PlayTrigger(string trigger)
    {
        if (animator != null) animator.SetTrigger(trigger);
    }

    public void DropLoot() => GameManager.Instance.AddCoin(coinDrop);
}
