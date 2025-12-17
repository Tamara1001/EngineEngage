using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Clips")]
    public AudioClip bgmClip;
    public AudioClip coinClip;
    public AudioClip playerDamageClip;
    public AudioClip enemyHitClip;
    public AudioClip areaCompleteClip;

    [Header("Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Settings")]
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeSources()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.volume = musicVolume;
        }
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlayBGM()
    {
        if (bgmClip != null && musicSource != null)
        {
            musicSource.clip = bgmClip;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }
    }

    public void PlayCoin()
    {
        PlaySFX(coinClip);
    }

    public void PlayPlayerDamage()
    {
        PlaySFX(playerDamageClip);
    }

    public void PlayEnemyHit()
    {
        PlaySFX(enemyHitClip);
    }

    public void PlayAreaComplete()
    {
        PlaySFX(areaCompleteClip);
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}
