using TMPro;
using UnityEngine;

// วางใน Scene "Map" (ตัวร้านเป็น Panel ที่เปิด/ปิดด้วย MapUI)
// อัปเกรดได้ทุกอย่างสูงสุด 5 ขั้น ราคาตามขั้นถัดไป
public class ShopManager : MonoBehaviour
{
    [Header("ราคาของขั้น 1-5 (ใช้กับทุกสเตตัส)")]
    [SerializeField] int[] upgradeCosts = { 40, 70, 110, 160, 220 };

    [Header("UI (ไม่ใส่ก็ได้)")]
    [SerializeField] TMP_Text coinText;
    [SerializeField] TMP_Text hpText;
    [SerializeField] TMP_Text atkText;
    [SerializeField] TMP_Text speedText;
    [SerializeField] TMP_Text manaText;
    [SerializeField] TMP_Text potionText;

    PlayerStatsData Stats => GameManager.Instance.SavedStats;

    void OnEnable() => Refresh();     // รีเฟรชทุกครั้งที่เปิดร้าน

    // ผูกกับปุ่มแต่ละปุ่ม
    public void BuyMaxHp()         => Buy(StatType.MaxHp);
    public void BuyAtk()           => Buy(StatType.Attack);
    public void BuyMoveSpeed()     => Buy(StatType.MoveSpeed);
    public void BuyManaRegen()     => Buy(StatType.ManaRegen);
    public void BuyPotionPower()   => Buy(StatType.PotionStrength);

    void Buy(StatType type)
    {
        if (Stats.IsMaxed(type)) return;
        int cost = upgradeCosts[Stats.GetLevel(type)];
        if (GameManager.Instance.SpendCoin(cost)) Stats.Upgrade(type);
        Refresh();
    }

    string Label(string name, StatType type, string valueText)
    {
        if (Stats.IsMaxed(type)) return $"{name}  Lv {PlayerStatsData.MaxLevel}/{PlayerStatsData.MaxLevel} (MAX)\n{valueText}";
        int cost = upgradeCosts[Stats.GetLevel(type)];
        return $"{name}  Lv {Stats.GetLevel(type)}/{PlayerStatsData.MaxLevel}\n{valueText}  |  ราคา {cost}";
    }

    void Refresh()
    {
        if (GameManager.Instance == null) return;
        if (coinText)   coinText.text   = $"Coins: {GameManager.Instance.Coins}";
        if (hpText)     hpText.text     = Label("Max HP",        StatType.MaxHp,          $"{Stats.MaxHp}");
        if (atkText)    atkText.text    = Label("Attack",        StatType.Attack,         $"{Stats.Attack}");
        if (speedText)  speedText.text  = Label("Move Speed",    StatType.MoveSpeed,      $"{Stats.MoveSpeed}");
        if (manaText)   manaText.text   = Label("Mana Regen",    StatType.ManaRegen,      $"{Stats.ManaRegen}/s");
        if (potionText) potionText.text = Label("Potion Power",  StatType.PotionStrength, $"{Stats.PotionHeal}");
    }
}
