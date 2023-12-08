using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;


public class KillLogManager : NetworkBehaviour
{
    public static KillLogManager instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    [Server]
    public void RpcDisplayKillLog(byte playerNumber)
    {
        RpcUpdateKillLog(playerNumber);
    }

    [ClientRpc]
    void RpcUpdateKillLog(byte playerNumber)
    {
        // 여기서 킬로그를 각 클라이언트의 KillLogUi에 전달
        KillLogUi.instance?.DisplayKillLog(playerNumber);
    }
}


public class KillLogUi : NetworkBehaviour
{
    public static KillLogUi instance;

    [Header("KillLog")]
    [SerializeField] private Text killLogText;


    [SyncVar(hook = nameof(OnPlayerNumberChanged))]
    public byte playerNumber;

    
    public event System.Action<byte> OnPlayerKill;

    public void DisplayKillLog(byte playerNumber)
    {               
        killLogText.text = $"DIE : {playerNumber}";
        
        //StartCoroutine(HideKillLog());
    }
    void OnPlayerNumberChanged(byte _, byte newPlayerNumber)
    {
        // 이벤트 호출
        OnPlayerKill?.Invoke(newPlayerNumber);
    }
    


    private IEnumerator HideKillLog()
    {
        yield return new WaitForSeconds(10f);
        killLogText.text = "";
    }
}
