// ชนิดของสเตตัสที่อัปเกรดได้
public enum StatType { MaxHp, Attack, MoveSpeed, ManaRegen, PotionStrength }

// Class Object: เก็บเลเวลอัปเกรดของผู้เล่น (ทุกอย่างอัปได้ 5 ขั้น)
[System.Serializable]
public class PlayerStatsData
{
    public const int MaxLevel = 5;

    int hpLevel, atkLevel, speedLevel, manaLevel, potionLevel;

    // ค่าจริงที่ใช้ในเกม = ค่าพื้นฐาน + เลเวล x ค่าที่เพิ่มต่อขั้น
    public float MaxHp          => 100f + hpLevel * 20f;
    public float Attack         => 10f  + atkLevel * 3f;
    public float MoveSpeed      => 6f   + speedLevel * 0.5f;
    public float ManaRegen      => 5f   + manaLevel * 2f;     // ต่อวินาที
    public float PotionHeal     => 40f  + potionLevel * 10f;

    public int GetLevel(StatType type)
    {
        switch (type)
        {
            case StatType.MaxHp:          return hpLevel;
            case StatType.Attack:         return atkLevel;
            case StatType.MoveSpeed:      return speedLevel;
            case StatType.ManaRegen:      return manaLevel;
            default:                      return potionLevel;
        }
    }

    public bool IsMaxed(StatType type) => GetLevel(type) >= MaxLevel;

    public void Upgrade(StatType type)
    {
        if (IsMaxed(type)) return;
        switch (type)
        {
            case StatType.MaxHp:          hpLevel++;     break;
            case StatType.Attack:         atkLevel++;    break;
            case StatType.MoveSpeed:      speedLevel++;  break;
            case StatType.ManaRegen:      manaLevel++;   break;
            case StatType.PotionStrength: potionLevel++; break;
        }
    }
}
