// UIButtonSound.cs (新規作成)
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    [Tooltip("クリック時に再生する効果音")]
    public AudioClip clickSound;

    void Start()
    {
        // ボタンのOnClickイベントに、SE再生メソッドを自動で登録する
        GetComponent<Button>().onClick.AddListener(PlayClickSound);
    }

    public void PlayClickSound()
    {
        AudioManager.Instance.PlaySE(clickSound);
    }
}