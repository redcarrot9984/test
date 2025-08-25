using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("オーディオソース")]
    [Tooltip("BGM再生用のAudioSource")]
    public AudioSource bgmSource;

    [Tooltip("SE再生用のAudioSource")]
    public AudioSource seSource;

    [Header("音量設定")]
    [Tooltip("SEの全体音量")]
    [Range(0f, 1f)]
    public float seVolume = 1.0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null || bgmSource == null) return;

        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }
    
    // ▼▼▼ このメソッドがエラーの原因です。正しく追加してください ▼▼▼
    /// <summary>
    /// BGMの再生を停止する
    /// </summary>
    public void StopBGM()
    {
        if (bgmSource != null)
        {
            bgmSource.Stop();
        }
    }

    public void PlaySE(AudioClip clip)
    {
        if (clip == null || seSource == null) return;
        
        seSource.PlayOneShot(clip, seVolume);
    }
}