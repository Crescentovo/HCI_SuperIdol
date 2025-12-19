using UnityEngine;
using System;

public class ControlManager : MonoBehaviour
{
    public static ControlManager Instance;

    public bool IsPaused { get; private set; } = false;

    // UI 可订阅这个事件
    public event Action<bool> OnPauseStateChanged;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        //按键控制
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }


    public void TogglePause()
    {
        if (IsPaused)
            ResumeGame();
        else
            PauseGame();
    }

    public void PauseGame()
    {
        if (IsPaused) return;

        IsPaused = true;
        Time.timeScale = 0f;

        // 通知 UI
        OnPauseStateChanged?.Invoke(true);
    }

    public void ResumeGame()
    {
        if (!IsPaused) return;

        IsPaused = false;
        Time.timeScale = 1f;

        // 通知 UI
        OnPauseStateChanged?.Invoke(false);
    }
}
