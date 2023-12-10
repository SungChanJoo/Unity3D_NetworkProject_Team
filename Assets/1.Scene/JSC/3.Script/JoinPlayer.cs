using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class JoinPlayer : NetworkBehaviour
{
    [SyncVar]
    public bool isFirstPlayer;
    [SyncVar(hook = nameof(OnPlayerNameChanged))]
    public string playerName;



    private void OnPlayerNameChanged(string oldName, string newName)
    {
        Debug.Log($"Player name changed: {oldName} -> {newName}");
    }

    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
            // 로컬 플레이어인 경우, 이름 설정
            CmdSetPlayer(SQLManager.Instance.Info.User_name, GameManager.Instance.PlayerList.Count == 0);
        }
    }
    public override void OnStopClient()
    {
        base.OnStopClient();
        CmdOutPlayer();
    }
    
    public void CmdOutPlayer()
    {
        OutPlayer();
    }

    public void OutPlayer()
    {
        GameManager.Instance.RemovePlayerOnServer(this);
    }

    [Command(requiresAuthority = false)]
    public void CmdSetPlayer(string newName, bool isFirst)
    {
        playerName = newName;
        isFirstPlayer = isFirst;
        EnterPlayer();
    }
    public void EnterPlayer()
    {
        GameManager.Instance.AddPlayerOnServer(this);
    }

}
