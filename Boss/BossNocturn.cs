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

    float hp;
    Transform player;

    public bool IsDead => hp <= 0;
    public float HpPercent => Mathf.Clamp01(hp / maxHp);

    void Start()
    {
        hp = maxHp;
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
        Spawn(transform.position + Vector3.up, rot);
    }

    public void ShootFrom(Vector3 pos, Vector3 dir)
    {
        Spawn(pos + Vector3.up, Quaternion.LookRotation(dir));
    }

    void Spawn(Vector3 pos, Quaternion rot)
    {
        var b = Instantiate(bulletPrefab, pos, rot);
        b.damage = atk;
        b.speed = bulletSpeed;
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
        GameManager.Instance.CompleteLevel(1.5f);
        Destroy(gameObject, 0.5f);
    }

    public void DropLoot() => GameManager.Instance.AddCoin(coinDrop);
}
