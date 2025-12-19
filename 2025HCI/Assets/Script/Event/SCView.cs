using UnityEngine;
using UnityEngine.UI;

public class SCView : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image avatarImage;
    [SerializeField] private Text messageText;

    [Header("Life Time")]
    [SerializeField] private float autoDestroyTime = 3f;

    /// <summary>
    /// 初始化 SC 内容（由 GameProgress 调用）
    /// </summary>
    public void Init(string message, Sprite avatar)
    {
        if (messageText != null)
            messageText.text = message;

        if (avatarImage != null)
        {
            avatarImage.gameObject.SetActive(avatar != null);
            avatarImage.sprite = avatar;
        }

        if (autoDestroyTime > 0)
        {
            Destroy(gameObject, autoDestroyTime);
        }
    }
}
