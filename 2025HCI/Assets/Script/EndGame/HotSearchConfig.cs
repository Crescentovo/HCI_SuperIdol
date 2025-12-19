using System.Collections.Generic;
using UnityEngine;

public enum HotSearchTier
{
    C,
    B,
    A,
    S
}

[System.Serializable]
public class HotSearchEntry
{
    [TextArea]
    public string content;
}

[System.Serializable]
public class HotSearchTierConfig
{
    public HotSearchTier tier;

    [Header("该档位的确定词条（只取 1 条）")]
    public HotSearchEntry fixedEntry;

    [Header("该档位的随机词条池")]
    public List<HotSearchEntry> randomEntries;
}

[System.Serializable]
public class HotSearchCategoryConfig
{
    public string categoryName; // 人气 / CP
    public List<HotSearchTierConfig> tierConfigs;
}

public enum HotSearchSource
{
    Popularity,
    CP,
    Random
}

[System.Serializable]
public class HotSearchResultEntry
{
    public int rank;
    //public string tag;     // 人气热搜 / CP 热搜 / 热搜
    public string content;
    public bool highlight;
    public HotSearchSource source;
}




[CreateAssetMenu(menuName = "Game/HotSearchConfig")]
public class HotSearchConfig : ScriptableObject
{
    [Header("分档分数线（统一规则）")]
    [Tooltip("0 ~ tierCMax")]
    public int tierCMax = 2;

    [Tooltip("tierCMax+1 ~ tierBMax")]
    public int tierBMax = 5;

    [Tooltip("tierBMax+1 ~ tierAMax")]
    public int tierAMax = 10;
    // >= tierAMax + 1 为 S

    [Header("人气热搜")]
    public HotSearchCategoryConfig popularityHotSearch;

    [Header("CP 热搜")]
    public HotSearchCategoryConfig cpHotSearch;

    public HotSearchTier GetTierByScore(int score)
    {
        if (score <= tierCMax) return HotSearchTier.C;
        if (score <= tierBMax) return HotSearchTier.B;
        if (score <= tierAMax) return HotSearchTier.A;
        return HotSearchTier.S;
    }

    // ================== 新增：配置校验 ==================

    public bool IsValid(out string error)
    {
        error = "";

        if (!CheckCategory(popularityHotSearch, "人气", ref error))
            return false;

        if (!CheckCategory(cpHotSearch, "CP", ref error))
            return false;

        return true;
    }

    private bool CheckCategory(
        HotSearchCategoryConfig category,
        string name,
        ref string error)
    {
        if (category == null)
        {
            error = $"{name}热搜未配置 Category";
            return false;
        }

        if (category.tierConfigs == null)
        {
            error = $"{name}热搜 tierConfigs 为空";
            return false;
        }

        foreach (HotSearchTier tier in System.Enum.GetValues(typeof(HotSearchTier)))
        {
            if (!category.tierConfigs.Exists(t => t.tier == tier))
            {
                error = $"{name}热搜缺少档位 {tier}";
                return false;
            }
        }

        return true;
    }
}
