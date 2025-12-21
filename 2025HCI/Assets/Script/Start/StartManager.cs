using Fungus;
using UnityEngine;
using UnityEngine.SceneManagement; // 必须引用：用于场景切换

public class StartManager : MonoBehaviour
{
    [Header("设置")]
    public string gameSceneName = "Chapter1"; // 目标场景的名称
    public AudioClip bgm; // 在编辑器里拖入音效文件

    void Start()
    {
        AudioManager.Instance.PlayMusic(bgm); // 播放背景音乐
    }

    void Update()
    {
        // 2. 检测逻辑：点击任意键（包括键盘和鼠标点击）
        if (Input.anyKeyDown)
        {
            AudioManager.Instance.StopMusic();
            StartGame();
        }
    }

    void StartGame()
    {
        // 3. 切换场景
        Debug.Log("正在切换至游戏场景...");
        SceneManager.LoadScene(gameSceneName);
    }
}