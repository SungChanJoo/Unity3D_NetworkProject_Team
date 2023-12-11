using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class KillLogUi : MonoBehaviour
{
    public static KillLogUi instance = null;
    private Coroutine hideCoroutine;
    [SerializeField] Image Skull;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 있다면 중복된 것이므로 파괴
        }
    }
    [Header("KillLog")]
    [SerializeField] private Text killLogText;

    public void DisplayKillLog(string attackr, string targetPlayer)
    {
        string SkullImageCode = "☠";
        killLogText.text += 
            $"\n{attackr}<color=#{ColorUtility.ToHtmlStringRGB(Color.red)}>  {SkullImageCode} " +
            $"<color=#{ColorUtility.ToHtmlStringRGB(Color.white)}>{targetPlayer}";

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        // 10초 대기
        hideCoroutine = StartCoroutine(HideKillLog());
    }


    

    private IEnumerator HideKillLog()
    {
        yield return new WaitForSeconds(10f);
        killLogText.text = "";
    }
}
