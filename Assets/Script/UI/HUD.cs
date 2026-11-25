using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] Image hpFill, manaFill;
    [SerializeField] TMP_Text potionText, coinText, killText;
    [SerializeField] GameObject[] dashIcons;

    void Start()
    {
        if (player == null) player = FindFirstObjectByType<PlayerController>();
    }

    void Update()
    {
        if (player == null) return;
        if (hpFill) hpFill.fillAmount = player.HpPercent;
        if (manaFill) manaFill.fillAmount = player.ManaPercent;
        if (potionText) potionText.text = $"x{player.PotionCount}";
        if (coinText) coinText.text = $"Coins: {GameManager.Instance.Coins}";
        if (killText) killText.text = $"Kills: {GameManager.Instance.KillCount}/{GameManager.KillTarget}";
        for (int i = 0; i < dashIcons.Length; i++)
            dashIcons[i].SetActive(i < player.DashCharges);
    }
}
