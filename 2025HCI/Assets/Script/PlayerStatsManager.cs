using System.Collections.Generic;
using UnityEngine;
using System.Linq; // 用于排序和比较

/// <summary>
/// 定义四种人设的枚举，便于在其他脚本中清晰调用。
/// </summary>
public enum PersonaType
{
    Sun,          // 小太阳
    Ice,    // 冰山美人
    Ghost,   // 阴湿男鬼
    Straight  // 钢铁直男
}

[System.Serializable]
public class GameResultData
{
    public int chapterIndex;
    public int popularity;
    public int cpHeat;

    // 各人设最终分数（结构化、强类型）
    public int sunScore;
    public int iceScore;
    public int ghostScore;
    public int straightScore;

    // 最终判定人设（核心结论）
    public PersonaType finalPersona;
}


public static class GameResultCache
{
    public static GameResultData LastResult;
}



public class PlayerStatsManager : MonoBehaviour
{
    // 使用 Singleton 模式确保全局唯一且方便访问
    public static PlayerStatsManager Instance { get; private set; }

    [Header("核心指标")]
    [Tooltip("玩家积累的人气值")]
    [SerializeField]
    public int maxPopularity = 10;
    private int popularity = 0;
    public int Popularity => popularity; // 只读属性，外部只能通过函数修改

    [Tooltip("玩家积累的CP热度")]
    [SerializeField]
    public int maxCPHeat = 10;
    private int cpHeat = 0;
    public int CPHeat => cpHeat; // 只读属性

    [Header("人设数值")]
    [Tooltip("小太阳人设分数")]
    [SerializeField]
    private int sunScore = 0;
    [Tooltip("冰山美人人设分数")]
    [SerializeField]
    private int iceScore = 0;
    [Tooltip("阴湿男鬼人设分数")]
    [SerializeField]
    private int ghostScore = 0;
    [Tooltip("钢铁直男人设分数")]
    [SerializeField]
    private int straightScore = 0;

    [Header("可视化UI")]
    //人气值
    public UnityEngine.UI.Text PopularityText;
    public UnityEngine.UI.Image PopularityFiller;
    
    //CP热度
    public UnityEngine.UI.Text CPHeatText;
    public UnityEngine.UI.Image CPHeatFiller;

    //Chapter number    
    public int CurrentChapterIndex { get; private set; }

    public void SetChapter(int chapter)
    {
        CurrentChapterIndex = chapter;
    }



    // ---------------------------------------------------------------------------------

    private void Awake()
    {
        // 1. 实现单例模式，确保只有一个实例
        if (Instance == null)
        {
            Instance = this;
            // 2. 确保脚本不随场景销毁
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 如果已有实例，则销毁这个新的实例
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 封装函数 1：修改人气值。
    /// </summary>
    /// <param name="changeAmount">人气值变化量 (可正可负)</param>
    public void AddPopularity(int changeAmount)
    {
        popularity += changeAmount;
        // 确保分数不会是负数
        popularity = Mathf.Max(0, popularity);

        //Debug.Log($"人气值变化: {changeAmount}. 当前人气值: {popularity}");
        // 可以在这里触发 UI 更新事件
        UpdateUI();
    }

    /// <summary>
    /// 封装函数 2：修改CP热度。
    /// </summary>
    /// <param name="changeAmount">CP热度变化量 (可正可负)</param>
    public void AddCPHeat(int changeAmount)
    {
        cpHeat += changeAmount;
        // 确保分数不会是负数
        cpHeat = Mathf.Max(0, cpHeat);

        Debug.Log($"CP热度变化: {changeAmount}. 当前CP热度: {cpHeat}");
        // 可以在这里触发 UI 更新事件
        UpdateUI();
    }

    /// <summary>
    /// 封装函数 3：修改指定人设的分数。
    /// </summary>
    /// <param name="personaType">需要积累分数的人设类型</param>
    /// <param name="changeAmount">人设分数变化量 (可正可负)</param>
    public void AddPersonaScore(PersonaType personaType, int changeAmount)
    {
        switch (personaType)
        {
            case PersonaType.Sun:
                sunScore += changeAmount;
                sunScore = Mathf.Max(0, sunScore);
                break;
            case PersonaType.Ice:
                iceScore += changeAmount;
                iceScore = Mathf.Max(0, iceScore);
                break;
            case PersonaType.Ghost:
                ghostScore += changeAmount;
                ghostScore = Mathf.Max(0, ghostScore);
                break;
            case PersonaType.Straight:
                straightScore += changeAmount;
                straightScore = Mathf.Max(0, straightScore);
                break;
        }

        Debug.Log($"人设分数变化: {personaType} 变化 {changeAmount}. 当前分数: {GetPersonaScore(personaType)}");
        // 可以在这里触发 UI 更新事件
        UpdateUI();
    }

    // 辅助函数：用于获取指定人设的当前分数
    public int GetPersonaScore(PersonaType personaType)
    {
        switch (personaType)
        {
            case PersonaType.Sun: return sunScore;
            case PersonaType.Ice: return iceScore;
            case PersonaType.Ghost: return ghostScore;
            case PersonaType.Straight: return straightScore;
            default: return 0;
        }
    }

    // 调用方法示例
    private void SelectOption_Positive()
    {
        // 玩家做出阳光积极的选择

        // 1. 修改人气值
        PlayerStatsManager.Instance.AddPopularity(50);

        // 2. 修改CP热度 (这个选择对CP热度影响较小)
        PlayerStatsManager.Instance.AddCPHeat(5);

        // 3. 修改人设分数 (增加小太阳分数)
        PlayerStatsManager.Instance.AddPersonaScore(PersonaType.Sun, 15);

        Debug.Log("执行了阳光选择。");
    }

    private void UpdateUI()
    {
        // ===== 人气值 =====
        if (PopularityText != null)
            PopularityText.text = $"{popularity} / {maxPopularity}";

        if (PopularityFiller != null)
            PopularityFiller.fillAmount = Mathf.Clamp01((float)popularity / maxPopularity);

        // ===== CP 热度 =====
        if (CPHeatText != null)
            CPHeatText.text = $"{cpHeat} / {maxCPHeat}";

        if (CPHeatFiller != null)
            CPHeatFiller.fillAmount = Mathf.Clamp01((float)cpHeat / maxCPHeat);
    }


    /// <summary>
    /// **游戏结束时调用：** 统计并返回最终的人设。
    /// 规则：分值最高的为人设；如果有平分，则最终人设为钢铁直男。
    /// </summary>
    /// <returns>最终人设的类型</returns>
    public PersonaType CalculateFinalPersona()
    {
        // 1. 将所有人设分数存入字典，便于比较
        Dictionary<PersonaType, int> personaScores = new Dictionary<PersonaType, int>
        {
            { PersonaType.Sun, sunScore },
            { PersonaType.Ice, iceScore },
            { PersonaType.Ghost, ghostScore },
            { PersonaType.Straight, straightScore } // 钢铁直男需特殊处理
        };

        // 2. 找出最高分数
        int maxScore = personaScores.Values.Max();

        // 3. 找出所有获得最高分的人设
        List<PersonaType> topPersonas = personaScores
            .Where(kvp => kvp.Value == maxScore) // 筛选出所有等于最高分数的项
            .Select(kvp => kvp.Key)              // 提取人设类型
            .ToList();

        // 4. 判断平分情况
        if (topPersonas.Count > 1)
        {
            // 如果多于一个类型获得了最高分 (即存在平分情况)
            Debug.Log($"平分情况! 多个指标分数最高: {string.Join(", ", topPersonas)}. 最终人设将是钢铁直男.");
            return PersonaType.Straight; // **平分规则：最终人设为钢铁直男**
        }
        else
        {
            // 只有一个最高分 (正常情况)
            return topPersonas[0];
        }
    }

    // ---------------------------------------------------------------------------------

    /// <summary>
    /// **数据保存函数：** 使用 Unity 的 PlayerPrefs 简易存储数据。
    /// 建议在游戏退出、切场景或 Checkpoint 时调用。
    /// 对于更复杂的数据，建议使用 JSON 或二进制序列化。
    /// </summary>
    public void SaveGameData()
    {
        PlayerPrefs.SetInt("Popularity", popularity);
        PlayerPrefs.SetInt("CPHeat", cpHeat);

        PlayerPrefs.SetInt("SunScore", sunScore);
        PlayerPrefs.SetInt("iceScore", iceScore);
        PlayerPrefs.SetInt("ghostScore", ghostScore);
        PlayerPrefs.SetInt("straightScore", straightScore);

        PlayerPrefs.Save(); // 确保数据写入磁盘
        Debug.Log("Game Data Saved!");
    }

    /// <summary>
    /// **数据加载函数：** 从 PlayerPrefs 加载数据。
    /// 建议在游戏启动或加载存档时调用。
    /// </summary>
    public void LoadGameData()
    {
        // GetInt(key, defaultValue)
        popularity = PlayerPrefs.GetInt("Popularity", 0);
        cpHeat = PlayerPrefs.GetInt("CPHeat", 0);

        sunScore = PlayerPrefs.GetInt("SunScore", 0);
        iceScore = PlayerPrefs.GetInt("iceScore", 0);
        ghostScore = PlayerPrefs.GetInt("ghostScore", 0);
        straightScore = PlayerPrefs.GetInt("straightScore", 0);

        UpdateUI();

        Debug.Log("Game Data Loaded!");
    }

    /// <summary>
    /// 重置所有指标到默认值 (0)
    /// </summary>
    public void ResetStats()
    {
        popularity = 0;
        cpHeat = 0;
        sunScore = 0;
        iceScore = 0;
        ghostScore = 0;
        straightScore = 0;

        UpdateUI();

        // 也可以清空存档
        // PlayerPrefs.DeleteAll(); 
    }


    public GameResultData BuildResultData()
    {
        return new GameResultData
        {
            chapterIndex = CurrentChapterIndex,
            popularity = Popularity,
            cpHeat = CPHeat,

            sunScore = GetPersonaScore(PersonaType.Sun),
            iceScore = GetPersonaScore(PersonaType.Ice),
            ghostScore = GetPersonaScore(PersonaType.Ghost),
            straightScore = GetPersonaScore(PersonaType.Straight),

            finalPersona = CalculateFinalPersona()
        };
    }


    //调用方式

    private void GoToSettlement()
    {
        GameResultCache.LastResult =
            PlayerStatsManager.Instance.BuildResultData();

    }


}