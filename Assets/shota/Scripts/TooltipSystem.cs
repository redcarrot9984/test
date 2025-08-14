// TooltipSystem.cs (新規作成)

using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TooltipSystem : MonoBehaviour
{
    public static TooltipSystem Instance { get; private set; }

    [SerializeField] private RectTransform tooltipPanel;
    [SerializeField] private TextMeshProUGUI tooltipText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // 最初は非表示にしておく
            if(tooltipPanel != null) tooltipPanel.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // マウスカーソルの位置に追従させる
        if (tooltipPanel != null && tooltipPanel.gameObject.activeSelf)
        {
            // 新しいInput Systemを使っている場合
            // tooltipPanel.position = Mouse.current.position.ReadValue();
            
            // 古いInput Managerを使っている場合
            tooltipPanel.position = Input.mousePosition;
        }
    }

    /// <summary>
    /// ツールチップを表示する
    /// </summary>
    /// <param name="content">表示したい文字列</param>
    public void Show(string content)
    {
        if (tooltipPanel == null) return;
        
        tooltipText.text = content;
        tooltipPanel.gameObject.SetActive(true);
    }

    /// <summary>
    /// ツールチップを隠す
    /// </summary>
    public void Hide()
    {
        if (tooltipPanel == null) return;
        
        tooltipPanel.gameObject.SetActive(false);
    }
}