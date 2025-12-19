using UnityEngine;

[System.Serializable]
public class DanmakuMessage
{
    public string text;
    public Sprite avatar;

    public DanmakuMessage(string t, Sprite a = null)
    {
        text = t;
        avatar = a;
    }
}
