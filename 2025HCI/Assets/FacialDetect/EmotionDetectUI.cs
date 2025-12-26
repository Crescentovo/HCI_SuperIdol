using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static OpenCVForUnityExample.FacialExpressionRecognitionExample;

public class EmotionDetectUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EmotionStabilityDetector detector;
    [SerializeField] private EmotionInGameUI emotionInGameUI;
    [SerializeField] private GameProgress gameProgress;

    [Header("UI")]
    public Text emotionText;
    public Text realtimeEmotionText;
    public Image stabilityFillImage;
    //0 Neutral, 1 Happy, 2 Surprised, 3 Angry
    public List<Image> EmotionFillers;

    private int EmotionToIndex(Emotion e)
    {
        switch (e)
        {
            //这里负责pointer指向
            case Emotion.Neutral: return 0;
            case Emotion.Happy: return 1;
            case Emotion.Surprised: return 2;
            case Emotion.Disgust:
            case Emotion.Sad:
            case Emotion.Angry: return 3;
            default: return -1;
        }
    }

    [Header("Character Expression")]
    [SerializeField] private Image characterImage;

    // 顺序必须与 Emotion enum 对应
    // 0 Neutral, 1 Happy, 2 Surprised, 3 Angry
    [SerializeField] private List<Sprite> expressionSprites;


    [Header("Visual")]
    public Color normalColor = Color.white;
    public Color lockedColor = Color.green;

    //计时器
    private Emotion displayedEmotion;
    private float displayedEmotionTimer;

    public float requiredHoldTime = 3f;
    private bool isConfirmed;

    private bool StartEmotionDetection = false;


    void Update()
    {
        if (detector == null || !gameProgress.IsFacialDetectionStarted)
            return;

        Emotion targetEmotion = detector.CurrentEmotion;
        int activeIndex = EmotionToIndex(targetEmotion);

        // 表情切换检测
        if (targetEmotion != displayedEmotion)
        {
            displayedEmotion = targetEmotion;
            UpdateCharacterExpression(displayedEmotion);   //切换主角表情
            displayedEmotionTimer = 0f;
            isConfirmed = false;

            var gameExpression = EmotionAdapter.ToGameExpression(displayedEmotion);
            emotionInGameUI.SetExpression(gameExpression);
        }
        else
        {
            displayedEmotionTimer += Time.deltaTime;

            if (!isConfirmed && displayedEmotionTimer >= requiredHoldTime)
            {
                isConfirmed = true;
                OnEmotionConfirmed(displayedEmotion);
            }
        }

        float activeTarget = Mathf.Clamp01(displayedEmotionTimer / requiredHoldTime);

        // ⭐ 核心：统一更新所有 filler
        for (int i = 0; i < EmotionFillers.Count; i++)
        {
            float targetFill =
                (i == activeIndex) ? activeTarget : 0f;

            EmotionFillers[i].fillAmount = Mathf.MoveTowards(
                EmotionFillers[i].fillAmount,
                targetFill,
                Time.deltaTime * 1.5f
            );
        }

        emotionText.text = displayedEmotion.ToString();
    }

    private void OnEmotionConfirmed(Emotion emotion)
    {
        Debug.Log($"🎉 UI确认表情成功：{emotion}");

        // 以后你可以在这里：
        // - 通知 GameProgress
        // - 触发剧情
        // - 播放动画

        //进度归零
        for (int i = 0; i < EmotionFillers.Count; i++)
        {
            EmotionFillers[i].fillAmount = 0;
        }
        var gameExpression = EmotionAdapter.ToGameExpression(emotion);
        emotionInGameUI.ConfirmExpression(gameExpression);

    }

    private void UpdateCharacterExpression(Emotion emotion)
    {
        int id = emotion switch
        {
            Emotion.Neutral => 0,
            Emotion.Happy => 1,
            Emotion.Surprised => 2,
            Emotion.Angry => 3,
            Emotion.Sad => 3,
            Emotion.Disgust => 3,
            _ => -1
        };

        if (id >= 0 && id < expressionSprites.Count)
        {
            characterImage.sprite = expressionSprites[id];
        }
    }


}
