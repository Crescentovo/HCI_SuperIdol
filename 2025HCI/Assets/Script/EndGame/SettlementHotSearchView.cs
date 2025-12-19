using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettlementHotSearchView : MonoBehaviour
{
    [Header("热搜")]
    [Header("UI")]
    [SerializeField] private Transform contentRoot;
    [SerializeField] private HotSearchItemView itemPrefab;

    [Header("Config")]
    [SerializeField] private HotSearchConfig hotSearchConfig;

    [Header("Debug")]
    [SerializeField] private bool useDebugRandomResult = true;

    [Header("人气值")]
    [SerializeField] private Image popFiller;
    [Header("CP 热度")]
    [SerializeField] private Image cpFiller;
    [Header("结算图")]
    [SerializeField] private Image settlementImage;

    private void OnEnable()
    {
        // ===== Debug 模式下自动生成测试数据 =====
        if (useDebugRandomResult || GameResultCache.LastResult == null)
        {
            Debug_GenerateRandomResult();
        }

        BuildFromCache();

        // 设置 UI
        SetUI(GameResultCache.LastResult);
    }

    /// <summary>
    /// 从 GameResultCache 构建UI
    /// </summary>
    private void BuildFromCache()
    {
        Clear();

        var result = GameResultCache.LastResult;
        if (result == null)
        {
            Debug.LogError("GameResultCache.LastResult 为空，无法生成热搜");
            return;
        }

        //debug
        ShowLog(result);

        var generator = new SettlementHotSearchGenerator(hotSearchConfig);

        List<HotSearchResultEntry> hotSearches =
        generator.Generate(
        result.popularity,
        result.cpHeat
        );


        BuildList(hotSearches);

        // 如果你想要逐条出现动画
        StartCoroutine(ShowSequentially());
    }

    /// <summary>
    /// 随机生成测试用 GameResultCache
    /// 人气 / CP / 人设分值：0–10
    /// </summary>
    [ContextMenu("Debug / Generate Random GameResult")]
    public void Debug_GenerateRandomResult()
    {
        var data = new GameResultData();

        data.chapterIndex = Random.Range(1, 10);
        data.popularity = Random.Range(0, 11);
        data.cpHeat = Random.Range(0, 11);

        data.sunScore = Random.Range(0, 11);
        data.iceScore = Random.Range(0, 11);
        data.ghostScore = Random.Range(0, 11);
        data.straightScore = Random.Range(0, 11);

        // 复用与你正式逻辑一致的判定规则
        data.finalPersona = CalculateFinalPersonaFromScores(data);

        GameResultCache.LastResult = data;

        Debug.Log("【Debug】已生成随机 GameResultCache");
    }

    private PersonaType CalculateFinalPersonaFromScores(GameResultData data)
    {
        int max = Mathf.Max(
            data.sunScore,
            data.iceScore,
            data.ghostScore,
            data.straightScore
        );

        int count = 0;
        PersonaType result = PersonaType.Straight;

        if (data.sunScore == max) { count++; result = PersonaType.Sun; }
        if (data.iceScore == max) { count++; result = PersonaType.Ice; }
        if (data.ghostScore == max) { count++; result = PersonaType.Ghost; }
        if (data.straightScore == max) { count++; result = PersonaType.Straight; }

        // 平分 → 钢铁直男
        return count > 1 ? PersonaType.Straight : result;
    }


    private void ShowLog(GameResultData result)
    {
        Debug.Log($"章节 {result.chapterIndex}");
        Debug.Log($"人气 {result.popularity}");
        Debug.Log($"CP {result.cpHeat}");

        Debug.Log($"小太阳：{result.sunScore}");
        Debug.Log($"冰山美人：{result.iceScore}");
        Debug.Log($"阴湿男鬼：{result.ghostScore}");
        Debug.Log($"钢铁直男：{result.straightScore}");

        Debug.Log($"最终人设：{result.finalPersona}");
    }

    private void SetUI(GameResultData result)
    {
        // 人气值
        popFiller.fillAmount =
            (float)result.popularity /
            10;
        // CP 热度
        cpFiller.fillAmount =
            (float)result.cpHeat /
            10;
        // 结算图
        //settlementImage.sprite =
        //    hotSearchConfig.GetSettlementSpriteForPersona(
        //        result.finalPersona);
    }


    private void BuildList(List<HotSearchResultEntry> entries)
    {
        foreach (var entry in entries)
        {
            var item = Instantiate(itemPrefab, contentRoot);

            item.Init(
                rank: entry.rank,
                //tag: entry.tag,
                content: entry.content,
                highlight: entry.highlight

            );
        }
    }

    private IEnumerator ShowSequentially()
    {
        for (int i = 0; i < contentRoot.childCount; i++)
        {
            contentRoot.GetChild(i).gameObject.SetActive(true);
            yield return new WaitForSeconds(0.15f);
        }
    }

    private void Clear()
    {
        for (int i = contentRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(contentRoot.GetChild(i).gameObject);
        }
    }
}
