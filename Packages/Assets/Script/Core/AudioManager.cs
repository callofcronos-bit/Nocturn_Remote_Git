using UnityEngine;

// เพลง 3 เพลง: เมนู+แผนที่ / ด่าน 1 / บอส
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioClip menuTheme;     // MainMenu + Map
    [SerializeField] AudioClip level1Theme;   // ด่าน 1
    [SerializeField] AudioClip bossTheme;     // ด่านบอส

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (musicSource == null) musicSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayMenuTheme()   => Play(menuTheme);
    public void PlayLevel1Theme() => Play(level1Theme);
    public void PlayBossTheme()   => Play(bossTheme);

    void Play(AudioClip clip)
    {
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;   // เพลงเดิมเล่นต่อ ไม่ตัด
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }
}
