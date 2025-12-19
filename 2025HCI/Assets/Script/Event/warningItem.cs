using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class warningItem : MonoBehaviour
{
    public string hintWord = "   WARNING ";
    public Text messageText;

    public void Set(string text)
    {
        messageText.text = hintWord + text;
    }
}
