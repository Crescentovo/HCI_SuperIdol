using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DanmakuController : MonoBehaviour
{
    [Header("弹幕")]
    [Header("UI")]
    public RectTransform content;
    public GameObject itemPrefab;

    [Header("Logic")]
    public int maxItems = 15;
    public bool forceLayout = true;

    Queue<DanmakuItem> pool = new Queue<DanmakuItem>();
    List<DanmakuItem> active = new List<DanmakuItem>();

    [Header("警告")]
    public GameObject warningObject;



    // 从池中取
    DanmakuItem GetItem()
    {
        if (pool.Count > 0)
        {
            DanmakuItem item = pool.Dequeue();
            item.transform.SetAsLastSibling();
            item.OnSpawn();
            return item;
        }
        else
        {
            GameObject obj = Instantiate(itemPrefab, content);
            DanmakuItem item = obj.GetComponent<DanmakuItem>();
            return item;
        }
    }

    // 回收
    void ReturnItem(DanmakuItem item)
    {
        item.OnDespawn();
        pool.Enqueue(item);
    }

    // 对外接口：添加弹幕
    public void AddMessage(string text, Sprite avatar = null)
    {
        DanmakuItem item = GetItem();
        item.Set(text, avatar);
        active.Add(item);

        if (forceLayout)
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);

        TrimItems();
    }

    // 超过最大数量 → 回收顶部弹幕
    void TrimItems()
    {
        while (active.Count > maxItems)
        {
            DanmakuItem old = active[0];
            active.RemoveAt(0);
            ReturnItem(old);

            if (forceLayout)
                LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        }
    }

    public void AddWarningMessage(string text)
    {
        warningItem item= warningObject.GetComponent<warningItem>();
        item.Set(text);
        warningObject.SetActive(true);
    }

    public void DeleteWarningMessage()
    {
        warningObject.SetActive(false);
    }
}
