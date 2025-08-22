// AudioManager.cs (新規作成)

using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // シングルトンパターン：どこからでも簡単にアクセスできるようにする
    public static AudioManager Instance { get; private set; }

    [Header("オーディオソース")]
    [Tooltip("BGM再生用のAudioSource")]
    public AudioSource bgmSource;

    [Tooltip("SE再生用のAudioSource")]
    public AudioSource seSource;

    private void Awake()
    {
        // シングルトンの設定
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // シーンをまたいでもAudioManagerが破壊されないようにする
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// BGMを再生する
    /// </summary>
    public void PlayBGM(AudioClip clip)
    {
        if (clip == null || bgmSource == null) return;

        bgmSource.clip = clip;
        bgmSource.loop = true; // BGMはループ再生
        bgmSource.Play();
    }

    /// <summary>
    /// SEを再生する（重複再生可能）
    /// </summary>
    public void PlaySE(AudioClip clip)
    {
        if (clip == null || seSource == null) return;

        // PlayOneShotを使うことで、SEが重なって再生される
        seSource.PlayOneShot(clip);
    }
}