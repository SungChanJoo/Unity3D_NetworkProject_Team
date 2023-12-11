using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class KillLogUi : MonoBehaviour
{
    public static KillLogUi instance = null;
    private Coroutine hideCoroutine;
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
        killLogText.text += 
            $"\n<color=#{ColorUtility.ToHtmlStringRGB(Color.red)}>{attackr}</color>이(가) " +
            $"<color=#{ColorUtility.ToHtmlStringRGB(Color.blue)}>{targetPlayer}</color>을(를) 죽였어요!!";

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
