using OpenCVForUnityExample;
using Emotion = OpenCVForUnityExample.FacialExpressionRecognitionExample.Emotion;

public static class EmotionAdapter
{
    public static GameProgress.FacialExpression ToGameExpression(Emotion emotion)
    {
        switch (emotion)
        {
            case Emotion.Neutral:
                return GameProgress.FacialExpression.Neutral;

            case Emotion.Happy:
                return GameProgress.FacialExpression.Happy;

            case Emotion.Surprised:
                return GameProgress.FacialExpression.Surprised;

            case Emotion.Angry:
            case Emotion.Disgust:
            case Emotion.Sad:
                return GameProgress.FacialExpression.Angry;

            default:
                return GameProgress.FacialExpression.None;
        }
    }
}
