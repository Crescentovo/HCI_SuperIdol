using UnityEngine;
using UnityEngine.UI;

public class HotSearchItemView : MonoBehaviour
{
    [SerializeField] private Text rankText;
    [SerializeField] private Text tagText;
    [SerializeField] private Text contentText;
    [SerializeField] private Image background;

    public void Init(
        int rank,
        string content,
        bool highlight
        )
    {
        rankText.text = $"#{rank}";
        contentText.text = content;

        //background.color = highlight
        //    ? new Color(1f, 0.95f, 0.8f) // ¸ßÁÁÉ«
        //    : Color.white;
    }
}
