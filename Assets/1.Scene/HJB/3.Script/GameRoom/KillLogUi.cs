using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class KillLogUi : MonoBehaviour
{
    public static KillLogUi instance = null;

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



    public void DisplayKillLog(string player, string attackr)
    {
        killLogText.text = $"{attackr}가 {player}죽임";

        //StartCoroutine(HideKillLog());
    }




    private IEnumerator HideKillLog()
    {
        yield return new WaitForSeconds(10f);
        killLogText.text = "";
    }
}
