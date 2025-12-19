using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class warningItem : MonoBehaviour
{
    public string hintWord = "   WARNING ";
    public Text messageText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Set(string text)
    {
        messageText.text = hintWord + text;
    }
}
