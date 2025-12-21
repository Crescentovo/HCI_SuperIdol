using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HotSearchItemView : MonoBehaviour
{
    [SerializeField] private Text rankText;
    //[SerializeField] private Text tagText;
    [SerializeField] private Text contentText;
    [SerializeField] private Image background;
    [SerializeField] private Image hitIcon;
    [SerializeField] private Image rankImage;

    [Header("Rank Images")]
    [SerializeField] private List<Sprite> rankSprites;   

    [Header("bg")]
    [SerializeField] private Sprite highlight;
    [SerializeField] private Sprite normal;


    public void Bind(HotSearchResultEntry data, int rank)
    {
        contentText.text = data.content;

        // 排名数字（1 / 2 / 3）
        //rankText.text = rank.ToString();

        // 排名图标
        // 排名图标
        if (rank - 1 < 0 || rank - 1 >= rankSprites.Count)
        {
            Debug.LogWarning($"没有配置该排名的 image，rank = {rank}");
            rankImage.gameObject.SetActive(false);
        }
        else
        {
            rankImage.gameObject.SetActive(true);
            rankImage.sprite = rankSprites[rank - 1];
        }


        //排名为偶数的高亮背景色
        if (rank % 2 == 0)
        {
            background.sprite = highlight;
        }
        else
        {
            background.sprite = normal;
        }

        //排名前三的有特殊显示
        hitIcon.gameObject.SetActive(rank <= 3);

    }


    public void Init(
        int rank,
        string content,
        bool highlight
        )
    {
        rankText.text = $"#{rank}";
        contentText.text = content;

        //background.color = highlight
        //    ? new Color(1f, 0.95f, 0.8f) // 高亮色
        //    : Color.white;
    }
}
