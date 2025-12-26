using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    public static PauseController Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject uiBlocker;

    public bool IsPaused { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        pausePanel.SetActive(false);
        uiBlocker.SetActive(false);
    }

    // 点击暂停按钮
    public void Pause()
    {
        if (IsPaused) return;

        IsPaused = true;

        pausePanel.SetActive(true);
        uiBlocker.SetActive(true);

        //DisableFungusInput();
    }

    // 点击继续
    public void Resume()
    {
        if (!IsPaused) return;

        IsPaused = false;

        pausePanel.SetActive(false);
        uiBlocker.SetActive(false);

        //EnableFungusInput();
    }

    // 点击返回菜单
    public void ExitToMenu()
    {
        SceneManager.LoadScene("StartScene"); // 改成你的开始场景名
    }

    private void DisableFungusInput()
    {
        Fungus.Flowchart.BroadcastFungusMessage("DisableInput");
    }

    private void EnableFungusInput()
    {
        Fungus.Flowchart.BroadcastFungusMessage("EnableInput");
    }
}
