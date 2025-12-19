using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class SettlementHotSearchGenerator
{
    private HotSearchConfig config;

    public SettlementHotSearchGenerator(HotSearchConfig config)
    {
        this.config = config;
    }

    public List<HotSearchResultEntry> Generate(
        int popularity,
        int cpHeat)

    {
        List<HotSearchResultEntry> result =
            new List<HotSearchResultEntry>();

        // ===== 分档 =====
        var popTier = config.GetTierByScore(popularity);
        var cpTier = config.GetTierByScore(cpHeat);

        // ===== 固定词条 =====
        var popFixed = GetFixedEntry(
            config.popularityHotSearch, popTier);

        if (!string.IsNullOrEmpty(popFixed))
        {
            result.Add(new HotSearchResultEntry
            {
                content = popFixed,
                source = HotSearchSource.Popularity
            });
        }

        var cpFixed = GetFixedEntry(
            config.cpHotSearch, cpTier);

        if (!string.IsNullOrEmpty(cpFixed))
        {
            result.Add(new HotSearchResultEntry
            {
                content = cpFixed,
                source = HotSearchSource.CP
            });
        }

        // ===== 随机池 =====
        List<string> randomPool = new List<string>();

        CollectRandomEntries(
            config.popularityHotSearch, popTier, randomPool);

        CollectRandomEntries(
            config.cpHotSearch, cpTier, randomPool);

        // 去重 & 排除固定词条
        randomPool = randomPool
            .Distinct()
            .Where(s => !result.Any(r => r.content == s))
            .OrderBy(_ => Random.value)
            .ToList();

        foreach (var entry in randomPool)
        {
            result.Add(new HotSearchResultEntry
            {
                content = entry,
                source = HotSearchSource.Random
            });
        }

        return result;
    }

    private string GetFixedEntry(
        HotSearchCategoryConfig category,
        HotSearchTier tier)
    {
        var cfg = category.tierConfigs
            .FirstOrDefault(t => t.tier == tier);

        return cfg?.fixedEntry?.content;
    }

    private void CollectRandomEntries(
        HotSearchCategoryConfig category,
        HotSearchTier tier,
        List<string> pool)
    {
        var cfg = category.tierConfigs
            .FirstOrDefault(t => t.tier == tier);

        if (cfg?.randomEntries == null)
            return;

        foreach (var e in cfg.randomEntries)
        {
            if (!string.IsNullOrEmpty(e.content))
                pool.Add(e.content);
        }
    }
}
