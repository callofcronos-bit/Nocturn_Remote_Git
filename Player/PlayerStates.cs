using UnityEngine;

public class IdleMoveState : PlayerState
{
    public IdleMoveState(PlayerController p) : base(p) { }

    public override void Update()
    {
        player.HandleMovement();
        player.LookAtMouse();

        if (Input.GetKeyDown(KeyCode.R)) player.UsePotion();

        if (Input.GetKeyDown(KeyCode.Space) && player.CanDash())
            player.ChangeState(new DashState(player));
        else if (Input.GetMouseButtonDown(0))
            player.ChangeState(new AttackState(player));
        else if (Input.GetMouseButtonDown(1) && player.HasMana(player.skillCost))
            player.ChangeState(new SkillState(player));
    }
}

// ท่าที่ล็อกการเคลื่อนที่ จนกว่าจะจบ หรือกด Dash ยกเลิก
public abstract class LockedActionState : PlayerState
{
    float timer;
    protected LockedActionState(PlayerController p) : base(p) { }

    protected abstract float Duration { get; }
    protected abstract void OnStart();

    public override void Enter()
    {
        timer = Duration;
        player.LookAtMouse();
        OnStart();
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && player.CanDash())
        {
            player.ChangeState(new DashState(player));
            return;
        }
        timer -= Time.deltaTime;
        if (timer <= 0) player.ChangeState(new IdleMoveState(player));
    }
}

public class AttackState : LockedActionState
{
    public AttackState(PlayerController p) : base(p) { }
    protected override float Duration => player.attackDuration;
    protected override void OnStart()
    {
        player.PlayTrigger("Attack");
        player.DoAttackHit();
    }
}

public class SkillState : LockedActionState
{
    public SkillState(PlayerController p) : base(p) { }
    protected override float Duration => player.skillDuration;
    protected override void OnStart()
    {
        player.PlayTrigger("Skill");
        player.SpendMana(player.skillCost);
        player.CastSkill();
    }
}

public class DashState : PlayerState
{
    float timer;
    Vector3 dir;
    public DashState(PlayerController p) : base(p) { }

    public override void Enter()
    {
        timer = player.dashDuration;
        dir = player.LastMoveDir != Vector3.zero ? player.LastMoveDir : player.transform.forward;
        player.ConsumeDash();
        player.Invulnerable = true;
        player.PlayTrigger("Dash");
    }

    public override void Update()
    {
        player.Controller.Move(dir * player.dashSpeed * Time.deltaTime);
        timer -= Time.deltaTime;
        if (timer <= 0) player.ChangeState(new IdleMoveState(player));
    }

    public override void Exit() { player.Invulnerable = false; }
}

public class DeadState : PlayerState
{
    public DeadState(PlayerController p) : base(p) { }
    public override void Enter()
    {
        player.PlayTrigger("Die");
        GameManager.Instance.NotifyPlayerDied();
    }
}
