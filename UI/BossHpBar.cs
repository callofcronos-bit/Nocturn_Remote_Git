using UnityEngine;
using UnityEngine.UI;

public class BossHpBar : MonoBehaviour
{
    [SerializeField] BossNocturn boss;
    [SerializeField] Image fill;

    void Start()
    {
        if (boss == null) boss = FindFirstObjectByType<BossNocturn>();
    }

    void Update()
    {
        if (fill == null) return;
        fill.fillAmount = boss != null ? boss.HpPercent : 0;
    }
}
