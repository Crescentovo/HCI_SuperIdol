using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WarningMarquee : MonoBehaviour
{
    [Header("裁剪框（通常是 Mask / Viewport）")]
    [SerializeField] private RectTransform clipRoot;

    [Header("UI 引用")]
    [SerializeField] private List<Image> bgParts = new List<Image>();
    [SerializeField] private List<Text> textParts = new List<Text>();

    [Header("滚动参数")]
    public float speed = 100f;

    private float totalTextWidth;
    private float totalBgWidth;

    void OnEnable()
    {
        InitMarquee();
    }

    // --------------------------------------------------
    // 初始化：拼接 + 计算周期宽度
    // --------------------------------------------------
    private void InitMarquee()
    {
        if (clipRoot == null)
        {
            Debug.LogError("WarningMarquee：未指定 clipRoot（裁剪框）");
            return;
        }

        if (bgParts.Count == 0 || textParts.Count == 0)
        {
            Debug.LogError("WarningMarquee：bgParts 或 textParts 为空");
            return;
        }

        // ---------- 文本 ----------
        totalTextWidth = 0f;
        float currentTextX = textParts[0].rectTransform.anchoredPosition.x;

        // 同步文本内容
        foreach (var t in textParts)
            t.text = textParts[0].text;

        for (int i = 0; i < textParts.Count; i++)
        {
            Text part = textParts[i];
            float width = part.preferredWidth;

            part.rectTransform.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal, width);

            if (i > 0)
            {
                currentTextX += textParts[i - 1].preferredWidth;
                part.rectTransform.anchoredPosition =
                    new Vector2(currentTextX, part.rectTransform.anchoredPosition.y);
            }

            totalTextWidth += width;
        }

        // ---------- 背景 ----------
        float bgWidth = bgParts[0].rectTransform.rect.width;
        float currentBgX = bgParts[0].rectTransform.anchoredPosition.x;

        for (int i = 1; i < bgParts.Count; i++)
        {
            currentBgX += bgWidth;
            bgParts[i].rectTransform.anchoredPosition =
                new Vector2(currentBgX, bgParts[i].rectTransform.anchoredPosition.y);
        }

        totalBgWidth = bgWidth * bgParts.Count;
    }

    void Update()
    {
        UpdateBackground();
        UpdateText();
    }

    // --------------------------------------------------
    // 背景滚动（基于裁剪框左边界）
    // --------------------------------------------------
    private void UpdateBackground()
    {
        float clipLeftX = GetClipLeftX();

        float leftMostX = float.MaxValue;
        float rightMostX = float.MinValue;
        Image leftMost = null;

        foreach (var bg in bgParts)
        {
            RectTransform rt = bg.rectTransform;
            rt.anchoredPosition += Vector2.left * speed * Time.deltaTime;

            float x = rt.anchoredPosition.x;
            if (x < leftMostX)
            {
                leftMostX = x;
                leftMost = bg;
            }
            if (x > rightMostX)
            {
                rightMostX = x;
            }
        }

        float bgWidth = bgParts[0].rectTransform.rect.width;
        float rightEdge = leftMostX + bgWidth;

        // 👉 核心条件：最左元素右边界 < 裁剪框左边界
        if (rightEdge < clipLeftX)
        {
            leftMost.rectTransform.anchoredPosition =
                new Vector2(rightMostX + bgWidth, leftMost.rectTransform.anchoredPosition.y);
        }
    }

    // --------------------------------------------------
    // 文本滚动（基于裁剪框左边界）
    // --------------------------------------------------
    private void UpdateText()
    {
        float clipLeftX = GetClipLeftX();

        float leftMostX = float.MaxValue;
        float rightMostX = float.MinValue;
        Text leftMost = null;

        foreach (var t in textParts)
        {
            RectTransform rt = t.rectTransform;
            rt.anchoredPosition += Vector2.left * speed * Time.deltaTime;

            float x = rt.anchoredPosition.x;
            if (x < leftMostX)
            {
                leftMostX = x;
                leftMost = t;
            }
            if (x > rightMostX)
            {
                rightMostX = x;
            }
        }

        float width = leftMost.preferredWidth;
        float rightEdge = leftMostX + width;

        // 👉 核心条件：最左文本右边界 < 裁剪框左边界
        if (rightEdge < clipLeftX)
        {
            leftMost.rectTransform.anchoredPosition =
                new Vector2(rightMostX + width, leftMost.rectTransform.anchoredPosition.y);
        }
    }

    // --------------------------------------------------
    // 获取裁剪框左边界（本地坐标）
    // --------------------------------------------------
    private float GetClipLeftX()
    {
        // clipRoot.rect.xMin 是裁剪框在自身 local space 中的左边界
        // 需要转换到 marquee 父节点的 local space
        Vector3 worldLeft =
            clipRoot.TransformPoint(new Vector3(clipRoot.rect.xMin, 0f, 0f));

        Vector3 localLeft =
            transform.InverseTransformPoint(worldLeft);

        return localLeft.x;
    }
}
