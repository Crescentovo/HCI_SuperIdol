using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityIntegration.Worker.DnnModule;
using OpenCVForUnityExample;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmotionTestRunner : MonoBehaviour
{
    public FacialExpressionRecognitionExample source;
    public EmotionStabilityDetector detector;

    void Start()
    {
        source.OnEmotionUpdated += detector.FeedEmotion;
    }
}

