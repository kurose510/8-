using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class TitleManager : MonoBehaviour
{
    [Header("タイトル画面のUIパネル")]
    public GameObject titlePanel;

    [Header("操作説明・ルール画面のパネル")]
    public GameObject howToPlayPanel;

    [Header("フェード用パネル（黒いImage）")]
    public Image fadePanel;

    [Header("暗転にかける時間（秒）")]
    public float fadeDuration = 0.8f;

    private bool isGameStarted = false;

    void Start()
    {
        isGameStarted = false;

        if (titlePanel != null) titlePanel.SetActive(true);
        if (howToPlayPanel != null) howToPlayPanel.SetActive(false);

        // 開始時はフェード用パネルを透明にしておく
        if (fadePanel != null)
        {
            Color color = fadePanel.color;
            color.a = 0f;
            fadePanel.color = color;
            fadePanel.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // ルール画面が開いている時 ➔ 2キー/Escキーで閉じる
        if (howToPlayPanel != null && howToPlayPanel.activeSelf)
        {
            if (Keyboard.current.digit2Key.wasPressedThisFrame ||
                Keyboard.current.numpad2Key.wasPressedThisFrame ||
                Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                CloseHowToPlay();
            }
            return;
        }

        // タイトル画面の時
        if (!isGameStarted)
        {
            // 1キーが押されたら暗転処理を開始
            if (Keyboard.current.digit1Key.wasPressedThisFrame || Keyboard.current.numpad1Key.wasPressedThisFrame)
            {
                StartCoroutine(FadeAndStartGame());
            }

            // 2キーが押されたらルール画面を表示
            if (Keyboard.current.digit2Key.wasPressedThisFrame || Keyboard.current.numpad2Key.wasPressedThisFrame)
            {
                OpenHowToPlay();
            }
        }
    }

    public void OpenHowToPlay()
    {
        if (howToPlayPanel != null) howToPlayPanel.SetActive(true);
    }

    public void CloseHowToPlay()
    {
        if (howToPlayPanel != null) howToPlayPanel.SetActive(false);
    }

    // ★暗転してそのままゲームシーンへ移動する処理
    private IEnumerator FadeAndStartGame()
    {
        isGameStarted = true;

        // 徐々に暗転させる（Alpha 0 ➔ 1）
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            if (fadePanel != null)
            {
                Color color = fadePanel.color;
                color.a = Mathf.Clamp01(timer / fadeDuration);
                fadePanel.color = color;
            }
            yield return null;
        }

        // 完全に真っ黒になったらそのままゲームシーンへ遷移！
        SceneManager.LoadScene("GameScene");
    }
}