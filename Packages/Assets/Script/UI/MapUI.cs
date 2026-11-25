using TMPro;
using UnityEngine;

// หน้าแผนที่: เลือกด่าน, เปิดร้านค้า, กลับเมนู
public class MapUI : MonoBehaviour
{
    [SerializeField] GameObject shopPanel;     // Panel ร้านค้า (ปิดไว้ตอนเริ่ม)
    [SerializeField] TMP_Text coinText;        // แสดงเงินบนแผนที่ (ไม่บังคับ)

    void Start()
    {
        if (shopPanel) shopPanel.SetActive(false);
    }

    void Update()
    {
        if (coinText) coinText.text = $"Coins: {GameManager.Instance.Coins}";
    }

    // ผูกกับปุ่มต่างๆ
    public void OnLevel1Clicked()   => GameManager.Instance.LoadLevel1();
    public void OnBossClicked()     => GameManager.Instance.LoadBoss();
    public void OnShopClicked()     { if (shopPanel) shopPanel.SetActive(true); }
    public void OnCloseShopClicked(){ if (shopPanel) shopPanel.SetActive(false); }
    public void OnBackToMenuClicked() => GameManager.Instance.LoadMainMenu();
}
