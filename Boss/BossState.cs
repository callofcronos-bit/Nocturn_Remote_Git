using System.Collections;

public abstract class BossState
{
    protected BossNocturn boss;
    protected BossState(BossNocturn b) { boss = b; }
    public abstract IEnumerator Execute();
}
