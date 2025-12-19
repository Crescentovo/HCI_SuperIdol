using UnityEngine;
using DG.Tweening;

public class EnterAnimation : MonoBehaviour
{
    [Header("测试工具")]
    [Tooltip("运行期间点击此勾选框，将以当前位置为终点重新播放动画")]
    public bool testPlay = false;

    [Header("动画配置")]
    public float duration = 1.0f;
    public float moveDistance = 500f;
    public EnterDirection enterDirection = EnterDirection.Left;
    public Ease easeType = Ease.OutBack;
    public float delay = 0f;
    public bool playOnStart = true;

    public enum EnterDirection { Up, Down, Left, Right }

    private Vector3 targetPos; // 动画的终点
    private Tween currentTween;

    void Start()
    {
        // 游戏开始时，以当前 Inspector 摆放的位置为准
        targetPos = transform.position;
        if (playOnStart)
        {
            PlayEnterAnimation();
        }
    }

    void Update()
    {
        // 改进：在 Update 中检测，解决 OnValidate 在某些版本不稳定的问题
        if (testPlay)
        {
            testPlay = false; // 自动复位

            // 重要：测试时，我们假设你现在把物体挪到了理想的“终点”
            // 所以重新刷新目标位置，确保动画轨迹符合你当前的调整
            targetPos = transform.position;

            PlayEnterAnimation();
        }
    }

    public void PlayEnterAnimation()
    {
        // 1. 清理正在进行的动画
        if (currentTween != null) currentTween.Kill();

        // 2. 根据最新参数计算起始偏移
        Vector3 startOffset = Vector3.zero;
        switch (enterDirection)
        {
            case EnterDirection.Up: startOffset = Vector3.up * moveDistance; break;
            case EnterDirection.Down: startOffset = Vector3.down * moveDistance; break;
            case EnterDirection.Left: startOffset = Vector3.left * moveDistance; break;
            case EnterDirection.Right: startOffset = Vector3.right * moveDistance; break;
        }

        // 3. 记录当前的终点坐标（防止在动画过程中再次被修改干扰）
        Vector3 finalPos = targetPos;

        // 4. 立即瞬移到计算出的起点
        transform.position = finalPos + startOffset;

        // 5. 播放动画回到终点
        currentTween = transform.DOMove(finalPos, duration)
                                .SetEase(easeType)
                                .SetDelay(delay)
                                .SetUpdate(true); // 保证不受 TimeScale 影响
    }
}