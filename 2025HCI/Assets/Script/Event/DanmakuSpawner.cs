using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DanmakuSpawner : MonoBehaviour
{
    public DanmakuController controller;

    [Header("Auto Spawn")]
    public bool autoStart = true;
    public List<string> autoMessages;
    public float autoMinInterval = 0.8f;
    public float autoMaxInterval = 2.0f;
    public Sprite autoAvatar;

    Coroutine autoRoutine;

    public event System.Action OnBurstFinished; // 添加：通知外部弹幕已全部发完
    private Coroutine currentBurstRoutine;

    void Start()
    {
        if (autoStart)
            StartAuto();
    }

    // ================================
    // 通用生成协程（优先不重复）
    // ================================
    IEnumerator SpawnMessages(
    List<string> messages,
    int count,
    float minInterval,
    float maxInterval,
    Sprite avatar)
    {
        int i = 0;

        // 记录当前这一轮哪些消息已经发过（只存 index）
        List<int> unused = new List<int>();

        // 初始化第一轮
        for (int idx = 0; idx < messages.Count; idx++)
            unused.Add(idx);

        while (count < 0 || i < count)
        {
            // ★ 如果这一轮所有消息都已发过，重新开始一轮
            if (unused.Count == 0)
            {
                for (int idx = 0; idx < messages.Count; idx++)
                    unused.Add(idx);
            }

            // 从“未发送列表”中随机挑一个
            int pickIndex = Random.Range(0, unused.Count);
            int msgIndex = unused[pickIndex];

            // 取到消息
            string msg = messages[msgIndex];

            // 从“未发送列表”中移除它（避免这一轮重复）
            unused.RemoveAt(pickIndex);

            // 发送
            controller.AddMessage(msg, avatar);

            // 随机间隔
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));

            i++;
        }

        // ★★★ 关键：协程结束时触发事件 ★★★
        OnBurstFinished?.Invoke();
        currentBurstRoutine = null;
    }


    // ================================
    // 自然弹幕
    // ================================
    public void StartAuto()
    {
        if (autoRoutine != null)
            StopCoroutine(autoRoutine);

        autoRoutine = StartCoroutine(
            SpawnMessages(autoMessages, -1, autoMinInterval, autoMaxInterval, autoAvatar)
        );
    }

    public void StopAuto()
    {
        if (autoRoutine != null)
            StopCoroutine(autoRoutine);
    }

    // ================================
    // 爆发弹幕（外部调用）
    // ================================
    public void Burst(
        List<string> messages,
        int count,
        float minInterval,
        float maxInterval,
        Sprite avatar = null)
    {
        if (currentBurstRoutine != null)
            StopCoroutine(currentBurstRoutine);

        currentBurstRoutine = StartCoroutine(
            SpawnMessages(messages, count, minInterval, maxInterval, avatar)
        );
    }

    //
    //生成警报
    // ===============================
    public void SpawnWarning(string message)
    {
        controller.AddWarningMessage(message);
    }

    public void DestroyWarning()
    {
        controller.DeleteWarningMessage();
    }


    /*
     //调用：
    public DanmakuSpawner spawner;

    void SomeEvent()
    {
    List<string> list = new List<string>(){ "来了来了！！", "冲冲冲", "谢谢老板！！" };

    spawner.Burst(
        list,
        20,          // 连发 20 条
        0.05f, 0.15f // 超短间隔
    );
    }

     */
}
