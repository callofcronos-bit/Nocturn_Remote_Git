using UnityEngine;

// วางใน Scene ไหนก็ได้ เลือกเพลงที่จะเล่น
public class LevelMusic : MonoBehaviour
{
    public enum Track { Menu, Level1, Boss }
    [SerializeField] Track track = Track.Menu;

    void Start()
    {
        var audio = AudioManager.Instance;
        if (audio == null) return;
        switch (track)
        {
            case Track.Menu:   audio.PlayMenuTheme();   break;
            case Track.Level1: audio.PlayLevel1Theme(); break;
            case Track.Boss:   audio.PlayBossTheme();   break;
        }
    }
}
