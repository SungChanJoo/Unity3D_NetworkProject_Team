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
        Debug.Log($"�÷��̾� �ʱ�ȭ{playerName} : {isFirstPlayer} | {IsDead}");

        Debug.Log($"Player name changed: {oldName} -> {newName}");
    }

    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
            // ���� �÷��̾��� ���, �̸� ����
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
        Debug.Log($"�÷��̾� ����{playerName} : {isFirstPlayer} | {IsDead}");
    }


    /*    public void EnterPlayer()
        {
            GameManager.Instance.AddPlayerOnServer(this);
        }*/

}