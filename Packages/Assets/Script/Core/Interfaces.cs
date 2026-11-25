public interface IDamageable
{
    void TakeDamage(float amount);
    bool IsDead { get; }
}

public interface IDroppable
{
    void DropLoot();
}

public interface IAttacker
{
    void Attack();
}
