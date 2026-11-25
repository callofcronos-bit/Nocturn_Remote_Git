using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("Refs")]
    public Animator animator;                       // ไม่ใส่ก็ได้
    [SerializeField] GameObject skillEffectPrefab;   // เอฟเฟกต์วงคลื่น (ไม่ใส่ก็ได้)
    [SerializeField] LayerMask enemyMask;

    [Header("Combat")]
    public float attackDuration = 0.5f;
    public float attackRange = 1.8f;
    public float skillDuration = 0.7f;
    public float skillCost = 30;
    public float skillClearRadius = 8f;     // รัศมีล้างกระสุนรอบตัว

    [Header("Dash")]
    public float dashSpeed = 18;
    public float dashDuration = 0.18f;
    [SerializeField] int maxDashCharges = 2;
    [SerializeField] float dashRechargeTime = 2f;

    [Header("Mana / Potion")]
    [SerializeField] float maxMana = 100;
    [SerializeField] int potionCount = 3;

    public CharacterController Controller { get; private set; }
    public Vector3 LastMoveDir { get; private set; }
    public bool Invulnerable { get; set; }

    PlayerStatsData stats;
    PlayerState currentState;
    Camera cam;
    float hp, mana, dashTimer;
    int dashCharges;

    public bool IsDead => hp <= 0;
    public float HpPercent => stats == null ? 1 : hp / stats.MaxHp;
    public float ManaPercent => mana / maxMana;
    public int PotionCount => potionCount;
    public int DashCharges => dashCharges;

    void Start()
    {
        Controller = GetComponent<CharacterController>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (animator != null) animator.applyRootMotion = false;
        cam = Camera.main;
        stats = GameManager.Instance.SavedStats;
        hp = stats.MaxHp;
        mana = maxMana;
        dashCharges = maxDashCharges;
        ChangeState(new IdleMoveState(this));
    }

    void Update()
    {
        if (IsDead || stats == null) return;
        currentState?.Update();
        RegenMana();
        RechargeDash();
    }

    public void ChangeState(PlayerState next)
    {
        currentState?.Exit();
        currentState = next;
        currentState.Enter();
    }

    public void PlayTrigger(string trigger)
    {
        if (animator != null) animator.SetTrigger(trigger);
    }

    // ---------- Movement ----------
    public void HandleMovement()
    {
        Vector3 dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        if (dir != Vector3.zero) LastMoveDir = dir;
        Controller.Move((dir * stats.MoveSpeed + Vector3.down * 9.8f) * Time.deltaTime);
        if (animator != null) animator.SetFloat("Speed", dir.magnitude);
    }

    public void LookAtMouse()
    {
        if (cam == null) return;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, transform.position);
        if (ground.Raycast(ray, out float d))
        {
            Vector3 look = ray.GetPoint(d) - transform.position;
            look.y = 0;
            if (look.sqrMagnitude > 0.01f) transform.rotation = Quaternion.LookRotation(look);
        }
    }

    // ---------- Dash ----------
    public bool CanDash() => dashCharges > 0;
    public void ConsumeDash() => dashCharges--;

    void RechargeDash()
    {
        if (dashCharges >= maxDashCharges) return;
        dashTimer += Time.deltaTime;
        if (dashTimer >= dashRechargeTime) { dashCharges++; dashTimer = 0; }
    }

    // ---------- Combat ----------
    public void DoAttackHit()
    {
        Vector3 center = transform.position + transform.forward * 1f + Vector3.up * 0.5f;
        foreach (var h in Physics.OverlapSphere(center, attackRange, enemyMask))
            if (h.TryGetComponent<IDamageable>(out var target) && !target.IsDead)
                target.TakeDamage(stats.Attack);
    }

    // สกิล: ลบกระสุนของมอนสเตอร์และบอสรอบตัวผู้เล่น
    public void CastSkill()
    {
        EnemyProjectile.ClearInRadius(transform.position, skillClearRadius);
        if (skillEffectPrefab != null)
        {
            var fx = Instantiate(skillEffectPrefab, transform.position, Quaternion.identity);
            Destroy(fx, 1.5f);
        }
    }

    // ---------- Mana ----------
    public bool HasMana(float cost) => mana >= cost;
    public void SpendMana(float cost) => mana = Mathf.Max(0, mana - cost);
    void RegenMana() => mana = Mathf.Min(maxMana, mana + stats.ManaRegen * Time.deltaTime);

    // ---------- Potion ----------
    public void UsePotion()
    {
        if (potionCount <= 0 || hp >= stats.MaxHp) return;
        potionCount--;
        hp = Mathf.Min(stats.MaxHp, hp + stats.PotionHeal);
    }

    // ---------- Damage ----------
    public void TakeDamage(float amount)
    {
        if (IsDead || Invulnerable) return;
        hp -= amount;
        if (hp <= 0) { hp = 0; ChangeState(new DeadState(this)); }
    }
}
