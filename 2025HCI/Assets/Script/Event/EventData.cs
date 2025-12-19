using System;
using System.Collections.Generic;
using UnityEngine;

// 确保可序列化，才能在 Inspector 中显示
[Serializable]
public class DanmakuEvent
{
    // [Header("事件名称")] // 可以在 Inspector 中为每个元素添加标题
    [Tooltip("开启此项可在测试时触发该事件的弹幕")]
    public bool TriggerNow = false;

    [Header("发送参数（可选）")]
    [Tooltip("最短发射间隔")]
    public float MinInterval = 0.6f;
    [Tooltip("最长发射间隔")]
    public float MaxInterval = 1.2f;

    [Tooltip("此事件集中发送的弹幕内容")]
    public List<string> NeutralMessages = new List<string>();
    public List<string> HappyMessages = new List<string>();
    public List<string> SurprisedMessages = new List<string>();
    public List<string> AngryMessages = new List<string>();

    //[HideInInspector]
    //public List<string> CurrentMessages = new List<string>();
    public List<string> GetMessagesByExpression(GameProgress.FacialExpression expression)
    {
        return expression switch
        {
            GameProgress.FacialExpression.Neutral => NeutralMessages,
            GameProgress.FacialExpression.Happy => HappyMessages,
            GameProgress.FacialExpression.Surprised => SurprisedMessages,
            GameProgress.FacialExpression.Angry => AngryMessages,
            GameProgress.FacialExpression.None => NeutralMessages,
            _ => new List<string>(),
        };
    }
}

[Serializable]
public class WarningEvent
{
    [Tooltip("开启此项可在测试时触发该事件的弹幕")]
    public bool TriggerNow = false;
    public bool DestroyNow = false;

    [Tooltip("此事件集中发送的弹幕内容")]
    public string Message;
}

[System.Serializable]
public class EventExpressionEffect
{
    [Header("事件信息")]
    public int index;
    public string eventName;

    [Header("Neutral（中性）")]
    public ExpressionEffect Neutral = new ExpressionEffect(GameProgress.FacialExpression.Neutral);

    [Header("Happy（开心）")]
    public ExpressionEffect Happy = new ExpressionEffect(GameProgress.FacialExpression.Happy);

    [Header("Surprised（惊讶）")]
    public ExpressionEffect Surprised = new ExpressionEffect(GameProgress.FacialExpression.Surprised);

    [Header("Angry（生气）")]
    public ExpressionEffect Angry = new ExpressionEffect(GameProgress.FacialExpression.Angry);

    /// <summary>
    /// 根据表情获取对应配置（给 GameProgress 用）
    /// </summary>
    public ExpressionEffect GetEffect(GameProgress.FacialExpression expression)
    {
        return expression switch
        {
            GameProgress.FacialExpression.Neutral => Neutral,
            GameProgress.FacialExpression.Happy => Happy,
            GameProgress.FacialExpression.Surprised => Surprised,
            GameProgress.FacialExpression.Angry => Angry,
            _ => Neutral
        };
    }
}

[System.Serializable]
public class PersonaScoreChange
{
    public PersonaType persona;
    public int scoreChange;
}

[System.Serializable]
public class SuperChatData
{
    [TextArea]
    public string content;
    public Sprite avatar;
}




[System.Serializable]
public class ExpressionEffect
{
    [HideInInspector]
    public GameProgress.FacialExpression expression;

    [Header("数值变化")]
    public int popularityChange;
    public int cpHeatChange;

    [Header("SC（可选）")]
    public SuperChatData scConfigs;

    [Header("人设加分")]
    public List<PersonaScoreChange> personaChanges = new();

    public ExpressionEffect(GameProgress.FacialExpression exp)
    {
        expression = exp;
        popularityChange = 0;
        cpHeatChange = 0;
        personaChanges = new List<PersonaScoreChange>();
    }
}

