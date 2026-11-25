using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class EnemyBase : MonoBehaviour, IDamageable, IDroppable, IAttacker
{
    public static int AliveCount { get; private set; }   // static: นับมอนที่ยังมีชีวิต
    public static void ResetCount() => AliveCount = 0;

    [Header("Stats")]
    [SerializeField] protected float maxHp = 50;
    [SerializeField] protected float atk = 10;
    [SerializeField] protected float moveSpeed = 3;
    [SerializeField] protected float attackRange = 2;
    [SerializeField] protected float attackCooldown = 1.5f;
    [SerializeField] protected int minCoin = 1;
    [SerializeField] protected int maxCoin = 5;

    [Header("Animation")]
    [SerializeField] protected Animator animator;          // ไม่ใส่ = หาในลูกให้เอง
    [SerializeField] protected float attackHitDelay = 0.4f; // หน่วงก่อนดาเมจออก ให้ตรงจังหวะท่า
    [SerializeField] protected float deathDelay = 2f;       // รอเล่นท่าตายก่อนทำลายตัว

    protected NavMeshAgent agent;
    protected Transform target;
    protected float hp;
    float lastAttackTime;
    bool counted;

    public bool IsDead => hp <= 0;

    protected virtual void Start()
    {
        AliveCount++;
        counted = true;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        hp = maxHp;

        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (animator != null) animator.applyRootMotion = false;

        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) target = p.transform;
    }

    void OnDestroy() => Uncount();

    void Uncount()
    {
        if (!counted) return;
        counted = false;
        AliveCount = Mathf.Max(0, AliveCount - 1);
    }

    protected virtual void Update()
    {
        if (IsDead || target == null || !agent.isOnNavMesh) return;

        float dist = Vector3.Distance(transform.position, target.position);
        if (dist > attackRange)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
        else
        {
            agent.isStopped = true;
            FaceTarget();
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;
                StartCoroutine(AttackRoutine());
            }
        }

        if (animator != null)
            animator.SetFloat("Speed", agent.isStopped ? 0f : agent.velocity.magnitude / Mathf.Max(0.1f, moveSpeed));
    }

    IEnumerator AttackRoutine()
    {
        PlayTrigger("Attack");
        yield return new WaitForSeconds(attackHitDelay);
        if (!IsDead && target != null) Attack();
    }

    protected void PlayTrigger(string trigger)
    {
        if (animator != null) animator.SetTrigger(trigger);
    }

    protected void FaceTarget()
    {
        if (target == null) return;
        Vector3 d = target.position - transform.position; d.y = 0;
        if (d != Vector3.zero) transform.rotation = Quaternion.LookRotation(d);
    }

    public abstract void Attack();   // Abstraction: ลูกต้อง implement เอง

    public virtual void TakeDamage(float amount)
    {
        if (IsDead) return;
        hp -= amount;
        if (hp <= 0) Die();
    }

    protected virtual void Die()
    {
        hp = 0;
        Uncount();
        DropLoot();
        GameManager.Instance.RegisterKill();

        if (agent.isOnNavMesh) agent.isStopped = true;
        agent.enabled = false;
        foreach (var c in GetComponentsInChildren<Collider>()) c.enabled = false;

        PlayTrigger("Die");
        Destroy(gameObject, deathDelay);
    }

    public virtual void DropLoot() => GameManager.Instance.AddCoin(Random.Range(minCoin, maxCoin + 1));
}
