// ManualViewer.cs (新規作成)

using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshProを使用するために必要

public class ManualViewer : MonoBehaviour
{
    [Header("マニュアル設定")]
    [Tooltip("ここにマニュアルの各ページ画像をドラッグ＆ドロップします")]
    public Sprite[] manualPages;

    [Header("UI参照")]
    public Image manualImage; // 画像を表示するUI
    public TextMeshProUGUI pageText; // ページ番号を表示するUI

    private int currentPageIndex = 0;

    // このオブジェクトが有効になった時（表示された時）に呼ばれる
    void OnEnable()
    {
        // 最初のページを表示する
        currentPageIndex = 0;
        ShowPage(currentPageIndex);
    }

    // 次のページへ進むボタン用メソッド
    public void NextPage()
    {
        // 次のページが存在すれば表示
        if (currentPageIndex < manualPages.Length - 1)
        {
            currentPageIndex++;
            ShowPage(currentPageIndex);
        }
    }

    // 前のページへ戻るボタン用メソッド
    public void PrevPage()
    {
        // 前のページが存在すれば表示
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            ShowPage(currentPageIndex);
        }
    }

    // ページを表示するメイン処理
    private void ShowPage(int index)
    {
        // 画像を差し替え
        manualImage.sprite = manualPages[index];

        // ページ番号テキストを更新 (例: "3 / 10")
        if (pageText != null)
        {
            pageText.text = $"{index + 1} / {manualPages.Length}";
        }
    }
}