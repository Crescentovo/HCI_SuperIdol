using UnityEngine;
using UnityEngine.UI;

public class EmotionInGameUI : MonoBehaviour
{
    [Header("引用 GameProgress 脚本")]
    [SerializeField] private GameProgress gameProgress;

    [Header("四个表情按钮")]
    [SerializeField] private Button neutralBtn;
    [SerializeField] private Button happyBtn;
    [SerializeField] private Button surprisedBtn;
    [SerializeField] private Button angryBtn;

    [Header("当前表情指示箭头")]
    [SerializeField] private RectTransform pointer;

    [Header("箭头偏移（可选）")]
    [SerializeField] private Vector2 pointerOffset = new Vector2(0, -40);

    private void Start()
    {
        // 注册按钮点击事件
        neutralBtn.onClick.AddListener(() => ConfirmExpression(GameProgress.FacialExpression.Neutral));
        happyBtn.onClick.AddListener(() => ConfirmExpression(GameProgress.FacialExpression.Happy));
        surprisedBtn.onClick.AddListener(() => ConfirmExpression(GameProgress.FacialExpression.Surprised));
        angryBtn.onClick.AddListener(() => ConfirmExpression(GameProgress.FacialExpression.Angry));

        // 初始化箭头位置
        UpdatePointer(GameProgress.FacialExpression.Neutral);
    }

    private void Update()
    {
        // 每帧更新箭头（如果 UI 动态变化可以保持同步）
        //UpdatePointer();
        if (gameProgress.IsFacialDetectionStarted)
        {
            ChangeInteractablity(true);
        }
        else
        {
            ChangeInteractablity(false);
        }
    }

    /// <summary>
    /// 修改当前表情并更新指示箭头
    /// </summary>
    public void SetExpression(GameProgress.FacialExpression expression)
    {
        Debug.Log($"设置表情为 {expression}");
        UpdatePointer(expression);
    }

    public void ConfirmExpression(GameProgress.FacialExpression expression)
    {
        Debug.Log($"确定表情为 {expression}");
        gameProgress.ChangeCurrentFacialExpression(expression);
        UpdatePointer(expression);
    }

    /// <summary>
    /// 更新指向当前的表情按钮（None 则指向 Neutral）
    /// </summary>
    private void UpdatePointer(GameProgress.FacialExpression e)
    {
        //GameProgress.FacialExpression e = GetCurrentExpression();

        RectTransform target = neutralBtn.GetComponent<RectTransform>();

        switch (e)
        {
            case GameProgress.FacialExpression.Neutral:
                target = neutralBtn.GetComponent<RectTransform>();
                break;
            case GameProgress.FacialExpression.Happy:
                target = happyBtn.GetComponent<RectTransform>();
                break;
            case GameProgress.FacialExpression.Surprised:
                target = surprisedBtn.GetComponent<RectTransform>();
                break;
            case GameProgress.FacialExpression.Angry:
                target = angryBtn.GetComponent<RectTransform>();
                break;
        }

        // 把 pointer 移动到目标按钮位置
        pointer.position = target.position + (Vector3)pointerOffset;
    }

    private GameProgress.FacialExpression GetCurrentExpression()
    {
        // 直接从 gameProgress 获取当前表情
        // 你已经有 currentFacialExpression，是 private，
        // 如果需要可提供一个 getter（推荐打开一个只读属性）。

        // 为了兼容你未提供 getter，这里假设你会加一个：
        // public FacialExpression CurrentExpression => currentFacialExpression;
        return gameProgress.CurrentExpression;
    }

    public void ChangeInteractablity(bool interactable)
    {
        neutralBtn.interactable = interactable;
        happyBtn.interactable = interactable;
        surprisedBtn.interactable = interactable;
        angryBtn.interactable = interactable;

        if (interactable)
        {
            neutralBtn.image.color = Color.white;
        }
        else
        {
            neutralBtn.image.color = new Color(202 / 255f, 202 / 255f, 202 / 255f);
        }
    }
}
