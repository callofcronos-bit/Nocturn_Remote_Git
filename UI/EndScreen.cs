using UnityEngine;

// วางใน Level1 และ Level2: Panel จะเด้งตอนผู้เล่นตาย หรือผ่านด่าน
public class EndScreen : MonoBehaviour
{
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject clearPanel;      // ผ่านด่าน (ทั้งด่าน 1 และบอส)

    void Start()
    {
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (clearPanel) clearPanel.SetActive(false);
        GameManager.Instance.PlayerDied += ShowGameOver;
        GameManager.Instance.LevelCompleted += ShowClear;
    }

    void OnDestroy()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.PlayerDied -= ShowGameOver;
        GameManager.Instance.LevelCompleted -= ShowClear;
    }

    void ShowGameOver() { if (gameOverPanel) gameOverPanel.SetActive(true); }
    void ShowClear()    { if (clearPanel) clearPanel.SetActive(true); }

    // ผูกกับปุ่มใน Panel
    public void BackToMap() => GameManager.Instance.LoadMap();      // เงินและอัปเกรดไม่หาย
    public void Retry()     => GameManager.Instance.RestartCurrentLevel();
}
