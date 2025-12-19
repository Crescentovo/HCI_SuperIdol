using UnityEngine;
using UnityEngine.UI;

public class DanmakuItem : MonoBehaviour
{
    public Text messageText;
    public Image avatarImage;
    CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void Set(string text, Sprite avatar)
    {
        messageText.text = text;

        if (avatar != null)
        {
            avatarImage.sprite = avatar;
            avatarImage.enabled = true;
        }
        else
            avatarImage.enabled = false;
    }

    public void OnSpawn()
    {
        canvasGroup.alpha = 1f;
        gameObject.SetActive(true);
    }

    public void OnDespawn()
    {
        gameObject.SetActive(false);
    }
}
