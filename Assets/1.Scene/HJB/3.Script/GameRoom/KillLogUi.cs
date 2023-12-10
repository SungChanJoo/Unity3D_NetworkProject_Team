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
            Destroy(gameObject); // �̹� �ν��Ͻ��� �ִٸ� �ߺ��� ���̹Ƿ� �ı�
        }
    }
    [Header("KillLog")]
    [SerializeField] private Text killLogText;



    public void DisplayKillLog(string attackr, string targetPlayer)
    {
        killLogText.text += $"\n{attackr}�� {targetPlayer}����";

        //StartCoroutine(HideKillLog());
    }




    private IEnumerator HideKillLog()
    {
        yield return new WaitForSeconds(10f);
        killLogText.text = "";
    }
}
