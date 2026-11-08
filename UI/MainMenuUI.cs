using UnityEngine;

// ผูกกับปุ่ม Start / Quit ใน MainMenu
public class MainMenuUI : MonoBehaviour
{
    public void OnStartClicked() => GameManager.Instance.LoadMap();     // ไปหน้าแผนที่ (ไม่รีเซ็ตค่า)
    public void OnQuitClicked()  => GameManager.Instance.QuitGame();    // ออกเกม = รีเซ็ตทุกอย่าง
}
