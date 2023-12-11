using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinnerUI : MonoBehaviour
{
    [SerializeField] private Text killLogText;

    public void WinnerUi(string winner)
    {
        killLogText.text +=
           $"�¸��ڴ� <color=#{ColorUtility.ToHtmlStringRGB(Color.blue)}>{winner}</color>!!";
        
    }
}
