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
        if (!isLocalPlayer) return;
        GameManager.Instance.isFirstPlayer = GameManager.Instance.PlayerList[0] == this;
        Debug.Log($"플레이어 이름 {playerName} : {isFirstPlayer} | {IsDead}");
        Debug.Log($"Player name changed: {oldName} -> {newName}");
    }

    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
            // 로컬 플레이어인 경우, 이름 설정
            CmdSetPlayer(SQLManager.Instance.Info.User_name, false);
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdSetPlayer(string newName, bool isFirst)
    {
        playerName = newName;
        isFirstPlayer = isFirst;
        IsDead = false;
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
}