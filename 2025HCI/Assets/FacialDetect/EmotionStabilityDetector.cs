using System.Reflection;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityIntegration.Worker.DnnModule;
using UnityEngine;
using Emotion = OpenCVForUnityExample.FacialExpressionRecognitionExample.Emotion;
using OpenCVForUnityExample;

public class EmotionStabilityDetector : MonoBehaviour
{
    [Header("Config")]
    [Tooltip("最小置信度，低于这个就忽略")]
    public float confidenceThreshold = 0.6f;

    [Tooltip("切换到新表情前需要确认的时间（秒）")]
    public float switchConfirmTime = 0.3f;

    // ===== 实时输出（每帧）=====
    private Emotion realtimeEmotion;
    private float realtimeConfidence;

    // ===== 去抖后的候选情绪 =====
    private Emotion currentEmotion;
    private Emotion pendingEmotion;
    private float switchTimer = 0f;

    private bool hasCurrentEmotion = false;
    private float lastUpdateTime = -1f;


    public Emotion RealtimeEmotion => realtimeEmotion;
    public float RealtimeConfidence => realtimeConfidence;
    public Emotion CurrentEmotion => currentEmotion;

    [Header("Integration")]
    public bool AutoPollFromExample = true;

    private FacialExpressionRecognitionExample _facialExample;
    private FieldInfo _fiLatestRecognizedFacialExpressions;
    private FieldInfo _fiFacialExpressionRecognizer;

    private void Start()
    {
        _facialExample = FindObjectOfType<FacialExpressionRecognitionExample>();
        if (_facialExample == null)
        {
            Debug.LogWarning("EmotionStabilityDetector: 未找到 FacialExpressionRecognitionExample。");
            return;
        }

        var exType = _facialExample.GetType();
        _fiLatestRecognizedFacialExpressions =
            exType.GetField("_latestRecognizedFacialExpressions", BindingFlags.Instance | BindingFlags.NonPublic);
        _fiFacialExpressionRecognizer =
            exType.GetField("_facialExpressionRecognizer", BindingFlags.Instance | BindingFlags.NonPublic);
    }

    private void Update()
    {
        if (!AutoPollFromExample)
            return;

        PollExampleAndUpdate();
    }

    private void PollExampleAndUpdate()
    {
        if (_facialExample == null ||
            _fiLatestRecognizedFacialExpressions == null ||
            _fiFacialExpressionRecognizer == null)
            return;

        var mat = _fiLatestRecognizedFacialExpressions.GetValue(_facialExample) as Mat;
        var recognizer = _fiFacialExpressionRecognizer.GetValue(_facialExample) as FacialExpressionRecognizer;

        if (mat == null || mat.empty() || recognizer == null)
            return;

        UpdateEmotion(mat, recognizer);
    }

    public void UpdateEmotion(Mat facialExpressionResult, FacialExpressionRecognizer recognizer)
    {
        var best = recognizer.GetBestMatchData(facialExpressionResult, 0);

        // 实时输出（永远更新）
        realtimeEmotion = (Emotion)best.ClassId;
        realtimeConfidence = best.Confidence;

        if (best.Confidence < confidenceThreshold)
            return;

        UpdateEmotionInternal(realtimeEmotion);
    }

    private void UpdateEmotionInternal(Emotion detectedEmotion)
    {
        float now = Time.time;

        if (lastUpdateTime < 0f)
            lastUpdateTime = now;

        float delta = now - lastUpdateTime;
        lastUpdateTime = now;

        if (detectedEmotion == currentEmotion)
        {
            switchTimer = 0f;
            return;
        }

        if (detectedEmotion != pendingEmotion)
        {
            pendingEmotion = detectedEmotion;
            switchTimer = 0f;
        }

        switchTimer += delta;

        if (switchTimer >= switchConfirmTime)
        {
            currentEmotion = pendingEmotion;
            switchTimer = 0f;
        }
    }



    // 用于测试 / Mock
    public void FeedEmotion(Emotion emotion, float confidence)
    {
        realtimeEmotion = emotion;
        realtimeConfidence = confidence;

        if (confidence < confidenceThreshold)
            return;

        UpdateEmotionInternal(emotion);
    }
}
