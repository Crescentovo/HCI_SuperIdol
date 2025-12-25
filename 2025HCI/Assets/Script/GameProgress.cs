using Fungus;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityIntegration.Worker.DnnModule;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

// 游戏进度管理器：负责协调事件触发、表情检测和弹幕发送

public class GameProgress : MonoBehaviour
{
    public Flowchart flowchart;


    // 更好的封装性：使用 private + [SerializeField]
    [SerializeField] private string SettlementSceneName = "EndScene";
    [SerializeField]
    private DanmakuSpawner spawner; // 外部只能通过属性或函数访问，但在 Inspector 中可见
    [Header("SC 设置")]
    [SerializeField] private Transform scSpawnRoot;
    [SerializeField] private GameObject scPrefab;

    [Header("配置Warning事件列表")]
    [SerializeField] 
    private List<WarningEvent> warningEvents = new List<WarningEvent>();

    [Header("弹幕事件列表")]
    [Tooltip("配置所有需要触发的弹幕事件和内容")]
    [SerializeField]
    private List<DanmakuEvent> danmakuEvents = new List<DanmakuEvent>();

    [Header("事件 → 表情 → 数值变化配置")]
    [SerializeField]
    private List<EventExpressionEffect> eventExpressionEffects = new List<EventExpressionEffect>();

    [Header("面部表情检测")]
    public EmotionStabilityDetector emotionDetector;
    public FacialExpressionRecognizer facialRecognizer;




    // 面部表情检测状态
    private bool StartFacialDetection = false;
    public bool IsFacialDetectionStarted => StartFacialDetection;
    public enum FacialExpression
    {
        Neutral,
        Happy,
        Surprised,
        Angry,
        None
    }
    [SerializeField]
    private FacialExpression currentFacialExpression = FacialExpression.None;
    public FacialExpression CurrentExpression => currentFacialExpression;

    // 用于外部启用/禁用检测（如果需要）
    private bool isHandlingEvent = false;
    private int pendingEventIndex = -1;

    // Fungus 里约定的变量名（可按需改）
    private const string FungusEventIndexVar = "SendEventIndex"; // Fungus 写入这个 index 发起事件
    private const string FungusStartFacialVar = "StartFacialDetection"; // （可选）通知 Fungus：我们开始检测了
    private const string FungusContinueBlockName = "OnEventFinished"; // 检测结束后脚本会执行这个 Block
    private const string FungusEndGameBoolName = "EndGame";
    //Fungus控制
    private bool FungusEndGameBool; // 设置为 true 则结束游戏（可选）

    // ---------- 公共接口 ----------
    // 如果你想从其他脚本直接触发，也可以调用这个方法
    public void RequestEvent(int index)
    {
        if (index < 0) return;
        if (isHandlingEvent)
        {
            Debug.LogWarning($"已有事件正在处理，忽略新的请求 index={index}");
            return;
        }
        pendingEventIndex = index;
        StartCoroutine(HandleEventCoroutine());
    }

    // 这个方法由 UI 或面部检测系统调用，用来改变当前检测到的表情
    public void ChangeCurrentFacialExpression(FacialExpression e)
    {
        currentFacialExpression = e;
        Debug.Log($"[GameProgress] ChangeCurrentFacialExpression -> {e}");
    }



    // ---------- 内部实现 ----------

    /// <summary>
    /// 【推荐的触发方法】通过索引在 Fungus 或其他脚本中调用。
    /// </summary>
    /// <param name="eventIndex">danmakuEvents 列表中的索引</param>,start with 0

    public void DestroyWarning()
    {
        spawner.DestroyWarning();
        Debug.Log("已销毁warning弹幕");
    }
    public void GoToSettlement()
    {
        GameResultCache.LastResult =
            PlayerStatsManager.Instance.BuildResultData();

        SceneManager.LoadScene(SettlementSceneName);
    }

    private void Start()
    {
        PlayerStatsManager.Instance.SetChapter(1); // 初始化章节为 1
        AudioManager.Instance.PlayBGMByName("BGM_Chapter1"); // 播放主背景音乐
    }


    // Update is called once per frame
    void Update()
    {
        // 从 Fungus 读取一次性事件 index（Fungus 设置后我们会把它重置为 -1）
        if (!isHandlingEvent)
        {
            int idx = flowchart.GetIntegerVariable(FungusEventIndexVar);
            if (idx >= 0)
            {
                // 立即把 Fungus 的变量重置，避免重复触发
                flowchart.SetIntegerVariable(FungusEventIndexVar, -1);
                pendingEventIndex = idx;
                StartCoroutine(HandleEventCoroutine());
            }
        }

        FungusEndGameBool = flowchart.GetBooleanVariable(FungusEndGameBoolName);
        if (FungusEndGameBool)
        {
            GoToSettlement();
        }

    }


    private IEnumerator HandleEventCoroutine()
    {
        if (isHandlingEvent)
            yield break;

        if (pendingEventIndex < 0)
            yield break;

        isHandlingEvent = true;
        int index = pendingEventIndex;
        pendingEventIndex = -1;

        Debug.Log($"[GameProgress] 开始处理事件 index={index}");

        // 1) 触发 warning
        AudioManager.Instance.PlaySFXByName("Focus");
        //AudioManager.Instance.PlaySFXByName("Warning");
        if (index >= 0 && index < warningEvents.Count)
        {
            var we = warningEvents[index];
            if (!string.IsNullOrEmpty(we.Message))
            {
                spawner.SpawnWarning(we.Message);
                Debug.Log($"[GameProgress] SpawnWarning index={index}, message={we.Message}");
            }
        }
        else
        {
            Debug.Log($"[GameProgress] Warning 不存在或未配置（index={index}）");
        }

        // 可选：通知 Fungus 我们已开始表情检测（如果你在 Flowchart 中想显示等待 UI）
        StartFacialDetection = true;
        flowchart.SetBooleanVariable(FungusStartFacialVar, true);

        // 2) 等待表情识别（脚本负责）—— 可以在这里加入超时逻辑
        // 默认策略：当 currentFacialExpression != None 时视为识别完成
        yield return new WaitUntil(() => currentFacialExpression != FacialExpression.None);

        // 识别到了
        var detected = currentFacialExpression;
        Debug.Log($"[GameProgress] 表情检测完成：{detected}");

        // 2.5) 应用数值变化
        ApplyExpressionEffect(index, detected);

        // 重置检测标记，准备下次检测
        currentFacialExpression = FacialExpression.None;
        StartFacialDetection = false;
        flowchart.SetBooleanVariable(FungusStartFacialVar, false);

        // 3) 根据相同 index 发送弹幕（danmakuEvents 与 warningEvents 使用相同 index 约定）
        //    如果 danmakuEvents 没有该 index，则会输出警告
        if (index >= 0 && index < danmakuEvents.Count)
        {
            TriggerDanmakuEventByIndex(index, detected);
            //人物做表情
            string expression = detected.ToString();
            flowchart.ExecuteBlock(expression);
            //warning取消
            DestroyWarning();

            //-----------等待逻辑----------------
            //bool burstFinished = false;
            // 订阅一次性事件
            //spawner.OnBurstFinished += () => burstFinished = true;
            // 等待直到弹幕发完
            //yield return new WaitUntil(() => burstFinished);

            Debug.Log("全部弹幕发送完毕，继续 Fungus 流程");
        }
        else
        {
            TriggerDebugComment("该事件无弹幕配置");
            // 人物表情
            flowchart.ExecuteBlock(detected.ToString());
            // warning 取消
            DestroyWarning();
            Debug.LogWarning($"[GameProgress] danmakuEvents 没有配置 index={index}");
            //不等待弹幕
        }

        // 4) 通知 Fungus 继续（脚本直接执行指定 Block）
        if (!string.IsNullOrEmpty(FungusContinueBlockName))
        {
            //等待弹幕发送完毕再继续 Fungus 流程
            //格式：“[index][name]”
            string num=index.ToString();
            Debug.Log("执行Fungus继续块：" + num + FungusContinueBlockName);
            flowchart.ExecuteBlock(num + FungusContinueBlockName);
        }

        isHandlingEvent = false;
        yield break;
    }

    // 统一的弹幕发送入口（使用检测到的表情作为消息选择依据）
    private void TriggerDanmakuEventByIndex(int eventIndex, FacialExpression expression)
    {
        if (eventIndex < 0 || eventIndex >= danmakuEvents.Count)
        {
            Debug.LogError($"[GameProgress] TriggerDanmakuEventByIndex 索引越界: {eventIndex}");
            return;
        }

        DanmakuEvent danmaku = danmakuEvents[eventIndex];
        List<string> selectedMessages = danmaku.GetMessagesByExpression(expression);

        // fallback 为 Neutral
        if (selectedMessages == null || selectedMessages.Count == 0)
        {
            Debug.Log($"[GameProgress] 表情 {expression} 的弹幕为空，回退 Neutral");
            selectedMessages = danmaku.NeutralMessages;
            if (selectedMessages == null || selectedMessages.Count == 0)
            {
                Debug.LogWarning($"[GameProgress] Neutral 列表也为空（index={eventIndex}），跳过弹幕发送");
                return;
            }
        }

        spawner.Burst(selectedMessages, selectedMessages.Count, danmaku.MinInterval, danmaku.MaxInterval);
        Debug.Log($"[GameProgress] 触发弹幕 index={eventIndex}, 表情={expression}, count={selectedMessages.Count}");
    }

    private void TriggerDebugComment(string message)
    {
        List<string> messages = new List<string> { message };
        spawner.Burst(messages, 1, 0.5f, 1.0f);
        Debug.Log($"[GameProgress] 触发调试弹幕: {message}");
    }

    //数值处理函数
    private void ApplyExpressionEffect(int eventIndex, FacialExpression expression)
    {
        // 1) 安全检查：确保索引在列表范围内
        if (eventIndex < 0 || eventIndex >= eventExpressionEffects.Count)
        {
            Debug.LogWarning($"[GameProgress] 事件索引 {eventIndex} 越界或未在 eventExpressionEffects 列表中配置。");
            return;
        }

        // 2) 直接通过列表索引获取配置（不再需要查找 index 变量）
        var config = eventExpressionEffects[eventIndex];

        // 3) 根据表情获取具体的数值效果
        var effect = config.GetEffect(expression);
        if (effect == null)
        {
            Debug.Log($"[GameProgress] 事件 index={eventIndex} 未配置表情 {expression} 的数值变化");
            return;
        }

        // ===== 人气 & CP =====
        PlayerStatsManager.Instance.AddPopularity(effect.popularityChange);
        
        if (effect.cpHeatChange != 0)
        {
            PlayerStatsManager.Instance.AddCPHeat(effect.cpHeatChange);
        }

        // ===== 人设加分 =====
        if (effect.personaChanges != null && effect.personaChanges.Count > 0)
        {
            foreach (var change in effect.personaChanges)
            {
                if (change.scoreChange == 0)
                    continue;

                PlayerStatsManager.Instance.AddPersonaScore(
                    change.persona,
                    change.scoreChange
                );
            }
        }

        // ===== SC 生成 =====
        TrySpawnSC(effect);

        // ===== Debug 输出 =====
        string personaLog = "";

        if (effect.personaChanges != null && effect.personaChanges.Count > 0)
        {
            foreach (var change in effect.personaChanges)
            {
                if (change.scoreChange != 0)
                {
                    personaLog += $"{change.persona} {change.scoreChange:+#;-#;0}，";
                }
            }

            if (personaLog.Length > 0)
                personaLog = personaLog.TrimEnd('，');
        }

        Debug.Log(
            $"[GameProgress] 事件 {eventIndex} | 表情 {expression} | " +
            $"人气 {effect.popularityChange:+#;-#;0}, " +
            $"CP {effect.cpHeatChange:+#;-#;0}, " +
            $"人设 [{personaLog}]"
        );
    }

    private void TrySpawnSC(ExpressionEffect effect)
    {
        if (effect.scConfigs == null)
            return; // 本次没有 SC，直接跳过

        var sc = effect.scConfigs;
        if (scPrefab == null)
        {
            Debug.LogError("[GameProgress] SC 预制体未设置，无法生成 SC");
            return;
        }

        GameObject scGO = Instantiate(scPrefab, scSpawnRoot);

        var view = scGO.GetComponent<SCView>();
        if (view != null)
        {
            view.Init(sc.content, sc.avatar);
        }

        Debug.Log($"[SC] 生成 SC：{sc.content}");
    }



}



