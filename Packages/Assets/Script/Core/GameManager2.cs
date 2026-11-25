using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Singleton + static: เข้าถึงได้ทุกที่ด้วย GameManager.Instance
// เก็บเงิน/อัปเกรดไว้ตลอดที่เกมเปิดอยู่ และรีเซ็ตเมื่อผู้เล่นกดออกเกมเท่านั้น
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public const int KillTarget = 20;
    public const string MenuScene = "MainMenu";
    public const string MapScene = "Map";
    public const string Level1Scene = "Level1_Monsters";
    public const string BossScene = "Level2_Boss";

    public int Coins { get; private set; }
    public int KillCount { get; private set; }
    public PlayerStatsData SavedStats { get; private set; } = new PlayerStatsData();

    public event Action PlayerDied;
    public event Action LevelCompleted;      // ทั้งด่าน 1 และด่านบอส

    bool levelEnding;

    // สร้าง GameManager อัตโนมัติ กด Play จาก Scene ไหนก็ได้
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Bootstrap()
    {
        if (Instance == null) new GameObject("GameManager").AddComponent<GameManager>();
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ---------- เหรียญ ----------
    public void AddCoin(int amount) => Coins += amount;

    public bool SpendCoin(int amount)
    {
        if (Coins < amount) return false;
        Coins -= amount;
        return true;
    }

    // ---------- ด่าน ----------
    public void RegisterKill()
    {
        KillCount++;
        if (KillCount >= KillTarget) CompleteLevel(1.5f);
    }

    // เรียกเมื่อจบด่าน (ด่าน 1 ฆ่าครบ / บอสตาย)
    public void CompleteLevel(float delay = 0.5f)
    {
        if (levelEnding) return;
        levelEnding = true;
        StartCoroutine(CompleteRoutine(delay));
    }

    IEnumerator CompleteRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        LevelCompleted?.Invoke();
    }

    public void NotifyPlayerDied() => PlayerDied?.Invoke();

    void ResetLevelState()
    {
        KillCount = 0;
        levelEnding = false;
    }

    // ---------- โหลด Scene ----------
    public void LoadMap()      { ResetLevelState(); SceneManager.LoadScene(MapScene); }
    public void LoadMainMenu() { ResetLevelState(); SceneManager.LoadScene(MenuScene); }
    public void LoadLevel1()   { ResetLevelState(); SceneManager.LoadScene(Level1Scene); }
    public void LoadBoss()     { ResetLevelState(); SceneManager.LoadScene(BossScene); }

    public void RestartCurrentLevel()
    {
        ResetLevelState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ---------- ออกเกม: รีเซ็ตเงินและอัปเกรดทั้งหมด ----------
    public void QuitGame()
    {
        Coins = 0;
        SavedStats = new PlayerStatsData();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
