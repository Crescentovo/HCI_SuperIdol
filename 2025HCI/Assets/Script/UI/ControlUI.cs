using UnityEngine;
using UnityEngine.UI;

public class ControlUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pausePanel;
    public Button pauseButton;
    public Button resumeButton;

    void Start()
    {
        // °´Å¥°ó¶¨Âß¼­²ã
        pauseButton.onClick.AddListener(ControlManager.Instance.TogglePause);
        resumeButton.onClick.AddListener(ControlManager.Instance.ResumeGame);

        // UI ¶©ÔÄÊÂ¼þ
        ControlManager.Instance.OnPauseStateChanged += HandlePauseChanged;

        pausePanel.SetActive(false);
    }

    void HandlePauseChanged(bool isPaused)
    {
        pausePanel.SetActive(isPaused);
        pauseButton.gameObject.SetActive(!isPaused);

    }
}
