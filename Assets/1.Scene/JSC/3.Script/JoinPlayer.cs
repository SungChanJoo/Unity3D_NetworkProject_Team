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
    [SyncVar]
    public bool IsDead;

    private void OnPlayerNameChanged(string oldName, string newName)
    {
        Debug.Log($"Player name changed: {oldName} -> {newName}");
    }

    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
            // 로컬 플레이어인 경우, 이름 설정
            CmdSetPlayer(SQLManager.Instance.Info.User_name, GameManager.Instance.PlayerNum == 1);
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdSetPlayer(string newName, bool isFirst)
    {
        playerName = newName;
        isFirstPlayer = isFirst;
        IsDead = false;
        GameManager.Instance.RPCHandlePlayerList();
    }
    [Command(requiresAuthority = false)]
    public void CmdPlayerDie()
    {
        Debug.Log($"플레이어 뒤짐{playerName} : {isFirstPlayer} | {IsDead}");
        RPCHandlePlayerDie();
    }
    public void PlayerDie()
    {
        IsDead = true;
    }
    [ClientRpc]
    public void RPCHandlePlayerDie()
    {
        PlayerDie();
    }


    /*    public void EnterPlayer()
        {
            GameManager.Instance.AddPlayerOnServer(this);
        }*/

}